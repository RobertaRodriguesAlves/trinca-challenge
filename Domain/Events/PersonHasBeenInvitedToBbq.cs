using System;

namespace Domain.Events
{
    public class PersonHasBeenInvitedToBbq : IEvent
    {
        public string? Id { get; }
        public string? BbqId { get; }
        public DateTime Date { get; }
        public string? Reason { get; }

        public PersonHasBeenInvitedToBbq(string? id, string? bbqId, DateTime date, string? reason)
        {
            Id = id;
            BbqId = bbqId;
            Date = date;
            Reason = reason;
        }
    }
}
