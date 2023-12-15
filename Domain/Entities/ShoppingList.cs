namespace Domain.Entities
{
    public class ShoppingList
    {
        public string? BbqId { get; private set; }
        public string? PersonId { get; private set; }
        public int Meat { get; private set; }
        public int Vegetable { get; private set; }

        public void SetShopList(string? bbqId, string? personId, bool isVeg)
        {
            BbqId = bbqId;
            PersonId = personId;
            Vegetable = isVeg ? 600 : 300;
            Meat = isVeg ? 0 : 300;
        }
    }
}
