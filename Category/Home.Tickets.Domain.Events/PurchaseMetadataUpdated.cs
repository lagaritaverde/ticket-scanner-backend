using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Tickets.Domain.Events {
    public class PurchaseAdded : EventBase {
        public string Id { get; set; }
        public string Shop { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string GroupId { get; set; }
        public string Category { get; set; }
        public string ShopItemName { get; set; }
    }
}
