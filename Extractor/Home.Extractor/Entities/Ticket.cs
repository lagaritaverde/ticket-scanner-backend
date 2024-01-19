namespace Home.Extractor.Entities {
    public class Ticket {
        public DateTime EmitedAt { get; set; }
        public string Shop { get; set; }
        public float Total { get; set; }
        public float TotalPaid { get; set; }
        public TicketItem[] Items { get; set; }
        //public PaymentInfo PaymentInfo { get; set; }
    }

    public class TicketItem {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public float TotalPrice { get; set; }
    }

    public class PaymentInfo {
        public string CardNumber { get; set; }
    }
}
