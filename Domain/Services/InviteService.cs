using CrossCutting;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    internal sealed class InviteService : IInviteService
    {
        private readonly SnapshotStore _snapshot;
        private readonly IPersonRepository _repository;
        private readonly IBbqRepository _bbqRepository;

        public InviteService(
            SnapshotStore snapshot, 
            IPersonRepository repository,
            IBbqRepository bbqRepository)
        {
            _snapshot = snapshot;
            _repository = repository;
            _bbqRepository = bbqRepository;
        }

        public async Task CreateAsync(Bbq? churras)
        {
            if (churras != null)
            {
                var Lookups = await _snapshot.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();

                foreach (var personId in Lookups.ModeratorIds)
                {
                    var person = await _repository.GetAsync(personId);
                    if (person is null)
                    {
                        continue;
                    }
                    person!.Apply(new PersonHasBeenInvitedToBbq(personId, churras!.Id, churras!.Date, churras!.Reason));
                    await _repository.SaveAsync(person);
                }
            }
        }

        public async Task UpdateAsync(Bbq? churras, bool gonnaHappen)
        {
            if (churras != null)
            {
                var lookups = await _snapshot.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();

                foreach (var personId in lookups.PeopleIds)
                {
                    var person = await _repository.GetAsync(personId);
                    if (person is null)
                    {
                        continue;
                    }

                    if (gonnaHappen && !person!.Invites.Any(p => p.Bbq == churras!.Id))
                    {
                        person.Apply(new PersonHasBeenInvitedToBbq(person.Id, churras!.Id, churras!.Date, churras!.Reason));
                    }
                    else
                    {
                        person!.Apply(new InviteWasDeclined(churras!.Id, person.Id));
                    }

                    await _repository.SaveAsync(person);
                }
            }
        }

        public async Task<IEnumerable<object>> GetChurrasAsync(string userId)
        {
            var moderator = await _repository.GetAsync(userId);
            if (moderator is null)
            {
                return Enumerable.Empty<object>();
            }

            var snapshots = new List<object>();
            foreach (var churrasId in moderator!.Invites.Where(i => i.Date >= DateTime.Now).Select(b => b.Id))
            {
                var churras = await _bbqRepository.GetAsync(churrasId!);
                if (churras is null || churras?.Status == BbqStatus.ItsNotGonnaHappen || churras?.Status == BbqStatus.PendingConfirmations)
                {
                    continue;
                }

                snapshots.Add(churras!.TakeSnapshot());
            }

            return snapshots;
        }

        public async Task<Person?> AcceptInvitationAsync(string userId, string inviteId, bool isVeg)
        {
            var person = await _repository.GetAsync(userId);
            var churras = await _bbqRepository.GetAsync(inviteId);
            if (person is null || churras is null)
            {
                return null;
            }

            person!.Apply(new InviteWasAccepted(person!.Id, inviteId, isVeg));
            await _repository.SaveAsync(person);

            churras!.Apply(new InviteWasAccepted(person!.Id, inviteId, isVeg));
            await _bbqRepository.SaveAsync(churras);

            return person;
        }

        public async Task<Person?> DeclineInvitationAsync(string userId, string inviteId)
        {
            var person = await _repository.GetAsync(userId);
            var churras = await _bbqRepository.GetAsync(inviteId);
            if (person is null || churras is null)
            {
                return null;
            }

            person!.Apply(new InviteWasDeclined(inviteId, person.Id));
            await _repository.SaveAsync(person);

            churras!.Apply(new InviteWasDeclined(inviteId, person.Id));
            await _bbqRepository.SaveAsync(churras);

            return person;
        }
    }
}