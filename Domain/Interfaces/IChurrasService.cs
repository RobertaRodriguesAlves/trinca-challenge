using Domain.Abstractions;
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IChurrasService : IAppService
    {
        Task<Bbq> CreateAsync(DateTime date, string reason, bool isTrincaPaying);
        Task<Bbq?> UpdateAsync(string id, bool gonnaHappen, bool trincaWillPay);
        Task<object?> GetShoppingList(string inviteId);
    }
}
