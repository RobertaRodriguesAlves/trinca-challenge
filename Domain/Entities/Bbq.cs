using Domain.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Domain.Entities
{
    public class Bbq : AggregateRoot
    {
        public string? Reason { get; set; }
        public BbqStatus Status { get; set; }
        public DateTime Date { get; set; }
        public bool IsTrincasPaying { get; set; }
        public List<ShoppingList> Shop { get; set; } = new List<ShoppingList>();

        internal void When(ThereIsSomeoneElseInTheMood @event)
        {
            Id = @event.Id.ToString();
            Date = @event.Date;
            Reason = @event.Reason;
            Status = BbqStatus.New;
        }

        internal void When(BbqStatusUpdated @event)
        {
            if (@event.GonnaHappen)
                Status = BbqStatus.PendingConfirmations;
            else 
                Status = BbqStatus.ItsNotGonnaHappen;

            if (@event.TrincaWillPay)
                IsTrincasPaying = true;
        }

        internal void When(InviteWasAccepted @event)
        {
            var invite = Shop.FirstOrDefault(bbq => bbq.BbqId == @event.InviteId && bbq.PersonId == @event.PersonId);
            if (invite is null)
            {
                var shopList = new ShoppingList();
                shopList.SetShopList(@event!.InviteId, @event!.PersonId, @event.IsVeg);
                Shop.Add(shopList);
            }

            if (Shop.GroupBy(bbq => bbq.BbqId).Count() >= 7 && Status != BbqStatus.Confirmed)
            {
                Status = BbqStatus.Confirmed;
            }
        }

        internal void When(InviteWasDeclined @event)
        {
            var invite = Shop.FirstOrDefault(bbq => bbq.BbqId == @event.InviteId && bbq.PersonId == @event.PersonId);
            if (invite != null)
            {
                Shop.RemoveAll(s => s.PersonId == invite.PersonId);
            }

            if (Shop.GroupBy(bbq => bbq.BbqId).Count() < 7 && Status != BbqStatus.PendingConfirmations)
            {
                Status = BbqStatus.PendingConfirmations;
            }
        }

        public object TakeSnapshot()
        {
            return new
            {
                Id,
                Date,
                IsTrincasPaying,
                Status = Status.ToString(),
                Shop = Shop
                     .GroupBy(bbq => bbq.BbqId)
                     .Select(bbq => new
                     {
                         Vegetables = $"{bbq.Sum(v => v.Vegetable) / 1000} KG",
                         Meat = $"{bbq.Sum(v => v.Meat) / 1000} KG"
                     })
            };
        }
    }
}
