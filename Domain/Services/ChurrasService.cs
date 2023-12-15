using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal sealed class ChurrasService : IChurrasService
    {
        private readonly IBbqRepository _repository;

        public ChurrasService(IBbqRepository repository)
            =>_repository = repository;

        public async Task<Bbq> CreateAsync(DateTime date, string reason, bool isTrincaPaying)
        {
            var churras = new Bbq();
            churras.Apply(new ThereIsSomeoneElseInTheMood(date, reason, isTrincaPaying));
            await _repository.SaveAsync(churras);

            return churras;
        }

        public async Task<Bbq?> UpdateAsync(string id, bool gonnaHappen, bool trincaWillPay)
        {
            var churras = await _repository.GetAsync(id);
            if (churras is null)
            {
                return null;
            }

            churras!.Apply(new BbqStatusUpdated(gonnaHappen, trincaWillPay));
            await _repository.SaveAsync(churras);

            return churras;
        }

        public async Task<Bbq?> GetShoppingList(string inviteId)
            => await _repository.GetAsync(inviteId);
    }
}
