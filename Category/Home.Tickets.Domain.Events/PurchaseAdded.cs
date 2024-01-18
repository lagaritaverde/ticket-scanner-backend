using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Tickets.Domain.Events {
    public class PurchaseMetadataUpdated : EventBase {
        public string Id { get; set; }
        public string? Owner { get; set; }
        public string? MustPayTo { get; set; }
        public string? WhoPaid { get; set; }
    }

    public class PurchaseCategoryUpdated : EventBase {
        public string Id { get; set; }
        public string? Category { get; set; }
    }

    public class PurchaseDescriptionUpdated : EventBase {
        public string Id { get; set; }
        public string? Description { get; set; }
    }
}
