using Home.Tickets.Domain.Events;

namespace Home.Tickets.Domain.Entities {
    public class Purchase : EntityBase {
        public string Id { get; }
        public string Shop { get; }
        public string AccountingGroup { get; }
        public DateTime Date { get; }
        public decimal Price { get; }
        public decimal UnitPrice { get; }
        public int Quantity { get; }
        public string ShopItemName { get; }
        public string? Description { get; private set; }
        public string GroupId { get; }


        public string? Owner { get; private set; }
        public string? MustPayTo { get; private set; }
        public string? Category { get; private set; }
        public string? WhoPaid { get; private set; }

        private Purchase() {
        }

        public Purchase(string id, string shop, string accountingGroup, DateTime date, decimal price, int quantity, string groupId, string shopItemName, string description, string category) {
            Id = id;
            Shop = shop;
            AccountingGroup = accountingGroup;
            Date = date;
            Price = price;
            Quantity = quantity;
            Description = description;
            Category = category;
            GroupId = groupId;
            UnitPrice = price / quantity;
            ShopItemName = shopItemName;

            this.AddEvent(new PurchaseAdded() {
                AccountingGroup = accountingGroup,
                Date = date,
                Price = price,
                Description = description,
                Id = id,
                Quantity = quantity,
                Shop = shop,
                GroupId = groupId,
                UnitPrice = UnitPrice,
                Category = category,
                ShopItemName = shopItemName
            });
        }

        public void SetCategory(string category) {
            Category = category;

            this.AddEvent(new PurchaseCategoryUpdated() {
                Id = Id,
                Category = category
            });
        }

        public void SetDescription(string description) {
            Description = description;
            this.AddEvent(new PurchaseDescriptionUpdated() {
                Id = Id,
                Description = description
            });
        }

        public void SetMetadata(string? owner, string? mustPayTo, string? whoPaid) {
            Owner = owner;
            MustPayTo = mustPayTo;
            WhoPaid = whoPaid;

            this.AddEvent(new PurchaseMetadataUpdated() {
                Id = Id,
                Owner = owner,
                MustPayTo = mustPayTo,
                WhoPaid = whoPaid
            });
        }
    }
}
