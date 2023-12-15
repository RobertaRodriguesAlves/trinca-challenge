using System;

namespace Domain.Events
{
    public class ThereIsSomeoneElseInTheMood : IEvent
    {
        public ThereIsSomeoneElseInTheMood(DateTime date, string reason, bool isTrincasPaying)
        {
            Id = Guid.NewGuid();
            Date = date;
            Reason = reason;
            IsTrincasPaying = isTrincasPaying;
        }

        public Guid Id { get; }
        public string Reason { get; }
        public bool IsTrincasPaying { get; }
        public DateTime Date { get; }
    }
}
