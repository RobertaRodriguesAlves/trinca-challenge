using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities
{
    public class Person : AggregateRoot
    {
        public string? Name { get; set; }
        public bool IsCoOwner { get; set; }
        public IEnumerable<Invite> Invites { get; set; } = new List<Invite>();

        internal void When(PersonHasBeenCreated @event)
        {
            Id = @event.Id;
            Name = @event.Name;
            IsCoOwner = @event.IsCoOwner;
        }

        internal void When(PersonHasBeenInvitedToBbq @event)
        {
            Invites = Invites.Append(new Invite
            {
                Id = @event.Id,
                Date = @event.Date,
                Bbq = $"{@event.Date} - {@event.Reason}",
                Status = InviteStatus.Pending
            });
        }

        internal void When(InviteWasAccepted @event)
        {
            var invite = Invites.FirstOrDefault(x => x.Id == @event.InviteId);
            if (invite is null)
            {
                return;
            }

            invite.Status = InviteStatus.Accepted;
        }

        internal void When(InviteWasDeclined @event)
        {
            var invite = Invites.FirstOrDefault(x => x.Id == @event.InviteId);
            
            if (invite is null) 
                return;
            
            invite.Status = InviteStatus.Declined;
        }

        public object? TakeSnapshot()
        {
            return new
            {
                Id,
                Name,
                IsCoOwner,
                Invites = Invites
                             .Where(i => i.Status != InviteStatus.Declined && i.Date > DateTime.Now)
                             .Select(i => new 
                             { 
                                 i.Id, 
                                 i.Bbq, 
                                 Status = i.Status.ToString() 
                             })
            };
        }
    }
}
