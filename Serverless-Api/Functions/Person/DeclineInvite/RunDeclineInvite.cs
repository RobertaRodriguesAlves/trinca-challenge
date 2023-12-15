using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public partial class RunDeclineInvite
    {
        private readonly Person _person;
        private readonly IInviteService _service;

        public RunDeclineInvite(
            Person user,
            IInviteService service)
        {
            _person = user;
            _service = service;
        }

        [Function(nameof(RunDeclineInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/decline")] HttpRequestData req, string inviteId)
        {
            var person = await _service.DeclineInvitationAsync(_person.Id, inviteId);
            if (person is null)
            {
                return req.CreateResponse(HttpStatusCode.NoContent);
            }

            return await req.CreateResponse(HttpStatusCode.OK, person!.TakeSnapshot());
        }
    }
}
