using Domain.Abstractions;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IInviteService : IAppService
    {
        Task CreateAsync(Bbq? churras);
        Task UpdateAsync(Bbq? churras, bool gonnaHappen);
        Task<IEnumerable<object>> GetChurrasAsync(string userId);
        Task<Person?> AcceptInvitationAsync(string userId, string inviteId, bool isVeg);
        Task<Person?> DeclineInvitationAsync(string userId, string inviteId);
    }
}