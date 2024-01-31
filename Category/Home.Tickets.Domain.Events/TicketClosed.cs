using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Tickets.Domain.Events {
    public class TicketClosed : EventBase {

        public string Id { get; set; }
        public string Shop { get; set; }
        public string AccountingGroup { get; set; }
        public DateTimeOffset EmitedAt { get; set; }
        public DateTimeOffset ClosedAt { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Total { get; set; }
        public TicketItemClosed[] Items { get; set; }

        public class TicketItemClosed {
            public string Description { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
            public int Quantity { get; set; }
        }

    }
}
