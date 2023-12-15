namespace Domain.Events
{
    public class InviteWasAccepted : IEvent
    {
        public InviteWasAccepted(string? personId, string? inviteId, bool isVeg)
        {
            PersonId = personId;
            InviteId = inviteId;
            IsVeg = isVeg;
        }

        public string? PersonId { get; }
        public string? InviteId { get; }
        public bool IsVeg { get; }
    }
}
