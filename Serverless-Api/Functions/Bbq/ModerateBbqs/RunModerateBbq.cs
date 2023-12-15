using Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public partial class RunModerateBbq
    {
        private readonly IChurrasService _bbqService;
        private readonly IInviteService _invateService;

        public RunModerateBbq(
            IChurrasService bbqService,
            IInviteService invateService)
        {
            _bbqService = bbqService;
            _invateService = invateService;
        }

        [Function(nameof(RunModerateBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{id}/moderar")] HttpRequestData req, string id)
        {
            var moderationRequest = await req.Body<ModerateBbqRequest>();
            if (moderationRequest is null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required.");
            }

            var churras = await _bbqService.UpdateAsync(id, moderationRequest.GonnaHappen, moderationRequest.TrincaWillPay);
            await _invateService.UpdateAsync(churras, moderationRequest.GonnaHappen);

            return await req.CreateResponse(churras is null ? HttpStatusCode.BadRequest : HttpStatusCode.Created, churras?.TakeSnapshot());
        }
    }
}
