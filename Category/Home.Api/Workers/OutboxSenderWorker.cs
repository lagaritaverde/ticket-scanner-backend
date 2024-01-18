

using EventStore.Client;
using Home.OutBox.Service;
using Microsoft.Extensions.Logging;

namespace Home.Api.Workers {
    public class OutboxSenderWorker : BackgroundService {
        private readonly EventStoreClient eventStoreClient;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<OutboxSenderWorker> logger;

        public OutboxSenderWorker(EventStoreClient eventStoreClient, IServiceScopeFactory serviceScopeFactory, ILogger<OutboxSenderWorker> logger) {
            this.eventStoreClient = eventStoreClient;
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    using (var scope = serviceScopeFactory.CreateScope()) {

                        var service = scope.ServiceProvider.GetRequiredService<OutboxService>();

                        var storage = scope.ServiceProvider.GetRequiredService<IOutBoxPendingStorage>();

                        await service.SendPendings(storage, stoppingToken);
                    }
                }
                catch (Exception ex) {
                    logger.LogError(ex, "Error outboxWorker");
                }


                await Task.Delay(1000);
            }
        }
    }
}
