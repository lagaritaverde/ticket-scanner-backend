

using EventStore.Client;
using Home.OutBox.Service;
using MediatR;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Home.Api.Workers {
    public class OutboxReciverWorker : BackgroundService {
        private readonly EventStoreClient eventStoreClient;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<OutboxSenderWorker> logger;

        public OutboxReciverWorker(EventStoreClient eventStoreClient, IServiceScopeFactory serviceScopeFactory, ILogger<OutboxSenderWorker> logger) {
            this.eventStoreClient = eventStoreClient;
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await eventStoreClient.SubscribeToStreamAsync("home", FromStream.Start, this.OnEvent);
            logger.LogInformation("Subscription to stream created");

            while (!stoppingToken.IsCancellationRequested) {
                await Task.Delay(1000);
            }
        }

        private async Task OnEvent(StreamSubscription subscription, ResolvedEvent @event, CancellationToken token) {
            try {
                var json = System.Text.UTF8Encoding.UTF8.GetString(@event.Event.Data.Span);
                IReadOnlyList<KeyValuePair<string, string>> customProperties = null;

                if (!@event.Event.Metadata.IsEmpty) {
                    customProperties = JsonSerializer.Deserialize<IReadOnlyList<KeyValuePair<string, string>>>(@event.Event.Metadata.Span);
                }

                using (var scope = serviceScopeFactory.CreateScope()) {
                    var mediatorOutbox = scope.ServiceProvider.GetRequiredService<MediatorOutbox>();

                    await mediatorOutbox.SendAsync(@event.Event.EventId.ToString(), @event.Event.EventType, json, customProperties, token);
                }
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error OnEvent");
                throw;
            }

        }
    }
}
