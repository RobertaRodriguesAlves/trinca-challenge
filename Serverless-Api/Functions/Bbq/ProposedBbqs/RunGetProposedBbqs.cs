using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public partial class RunGetProposedBbqs
    {
        private readonly Person _user;
        private readonly IInviteService _service;

        public RunGetProposedBbqs(IInviteService service, Person user)
        {
            _user = user;
            _service = service;
        }

        [Function(nameof(RunGetProposedBbqs))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras")] HttpRequestData req)
        {
            var snapshots = await _service.GetChurrasAsync(_user.Id);
            return await req.CreateResponse(snapshots.Any() ? HttpStatusCode.Created : HttpStatusCode.BadRequest, snapshots);
        }
    }
}