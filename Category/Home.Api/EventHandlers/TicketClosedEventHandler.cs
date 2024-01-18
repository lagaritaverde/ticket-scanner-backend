using Home.Tickets.Domain;
using Home.Tickets.Domain.Entities;
using Home.Tickets.Domain.Events;
using Home.Tickets.Infrastructure.Database;
using MediatR;

namespace Home.Api.EventHandlers {

    public abstract class EventContext {
        public IReadOnlyList<KeyValuePair<string, string>> Properties { get; set; }
    }

    public class EventContext<T> : EventContext, INotification where T : class {
        public EventContext(T @event) {
            Event = @event;
        }
        public T Event { get; }
    }

    public class TicketClosedEventHandler : MediatR.INotificationHandler<EventContext<TicketClosed>> {
        private readonly IRepository<Purchase> repository;
        private readonly TicketContext ticketContext;

        public TicketClosedEventHandler(Tickets.Domain.IRepository<Purchase> repository, TicketContext ticketContext) {
            this.repository = repository;
            this.ticketContext = ticketContext;
        }

        public async Task Handle(EventContext<TicketClosed> notification, CancellationToken cancellationToken) {

            var purchases = notification.Event.Items.Select(x => new Purchase(Guid.NewGuid().ToString(),
                notification.Event.Shop,
                notification.Event.EmitedAt.UtcDateTime,
                x.TotalPrice,
                x.Quantity,
                notification.Event.Id,
                x.Description,
                "Unknow",
                "Unknow"
                ));

            await ticketContext.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try {
                await repository.AddRange(purchases);
                await ticketContext.SaveChangesAsync(cancellationToken);
                await ticketContext.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception) {
                await ticketContext.RollBackTransactionAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }
    }
}
