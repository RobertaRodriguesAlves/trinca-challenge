using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public partial class RunGetInvites
    {
        private readonly Person _user;
        private readonly IPersonRepository _repository;

        public RunGetInvites(Person user, IPersonRepository repository)
        {
            _user = user;
            _repository = repository;
        }

        [Function(nameof(RunGetInvites))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "person/invites")] HttpRequestData req)
        {
            var person = await _repository.GetAsync(_user.Id);
            if (person is null)
            {
                return req.CreateResponse(HttpStatusCode.NoContent);
            }

            return await req.CreateResponse(HttpStatusCode.OK, person!.TakeSnapshot());
        }
    }
}
