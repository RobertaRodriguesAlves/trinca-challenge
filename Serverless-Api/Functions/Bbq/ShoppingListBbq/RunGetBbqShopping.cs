using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api.Functions.Bbq.ShoppingListBbq
{
    public partial class RunGetBbqShopping
    {
        private readonly Person _person;
        private readonly IChurrasService _service;

        public RunGetBbqShopping(
            Person person,
            IChurrasService service)
        {
            _person = person;
            _service = service;
        }

        [Function(nameof(RunGetBbqShopping))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{inviteId}/shopping")] HttpRequestData req, string inviteId)
        {
            if (!_person.IsCoOwner)
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var churras = await _service.GetShoppingList(inviteId);
            if (churras is null)
            {
                return req.CreateResponse(HttpStatusCode.NoContent);
            }

            return await req.CreateResponse(HttpStatusCode.OK, churras!.TakeSnapshot());
        }
    }
}