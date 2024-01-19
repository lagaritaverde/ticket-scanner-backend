using Home.Tickets.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Home.Tickets.Domain.Events.TicketClosed;

namespace Home.Tickets.Domain.Entities {
    public class Ticket : EntityBase {
        public string Id { get; private set; }
        public string Shop { get; private set; }
        public DateTime EmitedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }
        public decimal TotalPaid { get; private set; }
        public decimal Total { get; private set; }
        public bool IsOpen { get; private set; }

        public IEnumerable<TicketItem> Items => ticketItems;

        private ICollection<TicketItem> ticketItems;

        private Ticket() {
        }

        public Ticket(string shop, DateTime emitedAt, decimal totalPaid) {
            Shop = shop;
            EmitedAt = emitedAt;
            TotalPaid = totalPaid;
            ticketItems = new List<TicketItem>();
            IsOpen = true;
            Id = $"{shop}_{emitedAt.ToBinary()}";
        }

        public void SetHeader(string shop, DateTime emitedAt, decimal totalPaid) {
            if (!IsOpen) throw new Exception("Already close");

            Shop = shop;
            EmitedAt = emitedAt;
            TotalPaid = totalPaid;

            Id = $"{shop}_{emitedAt.ToBinary()}";
        }

        public void AddItem(string description, decimal totalPrice, int quantity) {
            if (!IsOpen) throw new Exception("Already close");
            ticketItems.Add(new TicketItem(description, totalPrice, quantity));
            RecalculeTotal();
        }

        public void SetItems(IEnumerable<TicketItem> items) {
            if (!IsOpen) throw new Exception("Already close");
            ticketItems.Clear();
            foreach (TicketItem item in items) {
                ticketItems.Add(item);
            }

            RecalculeTotal();
        }

        private void RecalculeTotal() {
            Total = ticketItems.Sum(x => x.TotalPrice);
        }

        public void Close() {
            if (!IsOpen) throw new Exception("Already close");

            IsOpen = false;
            ClosedAt = DateTime.UtcNow;

            var ticketClosed = new TicketClosed() {
                Id = Id,
                Shop = Shop,
                EmitedAt = EmitedAt,
                TotalPaid = TotalPaid,
                Total = Total,
                ClosedAt = ClosedAt.Value,
                Items = ticketItems.Select(x => new TicketItemClosed() {
                    Description = x.Description,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice,
                    UnitPrice = x.UnitPrice,
                }).ToArray(),
            };

            AddEvent(ticketClosed);
        }

        public class TicketItem {
            public string Description { get; }
            public decimal UnitPrice { get; }
            public decimal TotalPrice { get; }
            public int Quantity { get; }

            private TicketItem() {
            }

            public TicketItem(string description, decimal totalPrice, int quantity) {
                Description = description;
                TotalPrice = totalPrice;
                Quantity = quantity;
                UnitPrice = totalPrice / quantity;
            }
        }
    }
}
