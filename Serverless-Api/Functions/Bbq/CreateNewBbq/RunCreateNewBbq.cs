using Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public partial class RunCreateNewBbq
    {
        private readonly IChurrasService _bbqService;
        private readonly IInviteService _invateService;

        public RunCreateNewBbq(
            IChurrasService bbqService, 
            IInviteService invateService)
        {
            _bbqService = bbqService;
            _invateService = invateService;
        }

        [Function(nameof(RunCreateNewBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
        {
            var input = await req.Body<NewBbqRequest>();
            if (input is null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required.");
            }

            var churras = await _bbqService.CreateAsync(input.Date, input.Reason, input.IsTrincasPaying);
            await _invateService.CreateAsync(churras);

            return await req.CreateResponse(churras is null ? HttpStatusCode.BadRequest : HttpStatusCode.Created, churras?.TakeSnapshot());
        }
    }
}
