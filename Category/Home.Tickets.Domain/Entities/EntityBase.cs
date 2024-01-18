using Home.Tickets.Domain.Events;

namespace Home.Tickets.Domain.Entities {
    public class EntityBase {
        private readonly List<EventBase> events = new();

        protected void AddEvent(EventBase eventBase) {
            eventBase.PerformedAt = DateTime.Now;
            events.Add(eventBase);
        }

        public IEnumerable<EventBase> GetEvents() => events;
    }
}
