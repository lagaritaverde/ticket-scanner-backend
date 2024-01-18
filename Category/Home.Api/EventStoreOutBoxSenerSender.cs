using EventStore.Client;
using Home.Api.EventHandlers;
using Home.OutBox.Service;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Home.Api;

public class EventStoreOutBoxSenerSender : IOutBoxSender {
    private readonly EventStoreClient client;

    public EventStoreOutBoxSenerSender(EventStoreClient client) {
        this.client = client;
    }

    public async Task SendAsync(string id, string type, string data, IReadOnlyList<KeyValuePair<string, string>> customProperties = null, CancellationToken cancellationToken = default) {

        ReadOnlyMemory<byte>? customPropertiesBytes = null;

        if (customProperties != null) {
            customPropertiesBytes = JsonSerializer.SerializeToUtf8Bytes(customProperties);
        }

        var eventData = new EventData(
            Uuid.Parse(id),
            type,
            System.Text.UTF8Encoding.UTF8.GetBytes(data), customPropertiesBytes
        );

        var r = await client.AppendToStreamAsync(
            "home",
            StreamState.Any,
            new[] { eventData },
            cancellationToken: cancellationToken
            );
    }
}
