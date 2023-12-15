using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public partial class RunAcceptInvite
    {
        private readonly Person _user;
        private readonly IInviteService _service;
        public RunAcceptInvite(IInviteService service, Person user)
        {
            _user = user;
           _service = service;
        }

        [Function(nameof(RunAcceptInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/accept")] HttpRequestData req, string inviteId)
        {
            var answer = await req.Body<InviteAnswer>();
            if (answer is null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required.");
            }

            var person = await _service.AcceptInvitationAsync(_user.Id!, inviteId, answer!.IsVeg);
            if (person is null)
            {
                return req.CreateResponse(HttpStatusCode.NoContent);
            }

            return await req.CreateResponse(HttpStatusCode.OK, person!.TakeSnapshot());
        }
    }
}
