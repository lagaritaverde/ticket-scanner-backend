using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Home.OutBox.Service;

public class OutboxService {
    private readonly IOutBoxSender outBoxSender;
    private readonly ILogger<OutboxService> logger;
    private const int batchSize = 10;

    public OutboxService(IOutBoxSender outBoxSender, ILogger<OutboxService> logger) {
        this.outBoxSender = outBoxSender;
        this.logger = logger;
    }

    public async Task SendPendings(IOutBoxPendingStorage outBoxStorage, CancellationToken cancelltionToken) {
        var outboxes = await outBoxStorage.Get(batchSize, cancelltionToken);
        var toDeleteList = new List<OutBox>();

        if (outboxes.Any()) {
            try {
                foreach (var outbox in outboxes) {
                    var messageId = Guid.NewGuid().ToString();

                    await outBoxSender.SendAsync(messageId, outbox.Type, outbox.Message, outbox.PropertiesToKeyValue(), cancelltionToken);

                    toDeleteList.Add(outbox);
                }
            }
            catch (Exception ex) {
                logger.LogError(ex, ex.Message);
            }

            await outBoxStorage.Delete(toDeleteList, cancelltionToken);
        }
    }
}
