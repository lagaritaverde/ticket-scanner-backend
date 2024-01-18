namespace Home.Tickets.Domain.Events {
    public class EventBase {
        public DateTime PerformedAt { get; set; }
        public string PerformedByUserId { get; set; }
        public string PerformedByUserName { get; set; }
    }
}
