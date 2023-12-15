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
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var churras = await _bbqService.UpdateAsync(id, moderationRequest.GonnaHappen, moderationRequest.TrincaWillPay);
            if (churras is null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            await _invateService.UpdateAsync(churras, moderationRequest.GonnaHappen);

            return await req.CreateResponse(HttpStatusCode.Created, churras!.TakeSnapshot());
        }
    }
}
