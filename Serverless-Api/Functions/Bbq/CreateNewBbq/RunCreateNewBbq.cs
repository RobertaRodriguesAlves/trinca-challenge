using Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public partial class RunCreateNewBbq
    {
        private readonly IChurrasService _churrasService;
        private readonly IInviteService _inviteService;

        public RunCreateNewBbq(
            IChurrasService bbqService, 
            IInviteService invateService)
        {
            _churrasService = bbqService;
            _inviteService = invateService;
        }

        [Function(nameof(RunCreateNewBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
        {
            var input = await req.Body<NewBbqRequest>();
            if (input is null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required.");
            }

            var churras = await _churrasService.CreateAsync(input.Date, input.Reason, input.IsTrincasPaying);
            await _inviteService.CreateAsync(churras);

            return await req.CreateResponse(churras is null ? HttpStatusCode.BadRequest : HttpStatusCode.Created, churras?.TakeSnapshot());
        }
    }
}
