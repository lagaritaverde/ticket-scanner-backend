using Home.Api.EventHandlers;
using Home.OutBox.Service;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Home.Api;

public class MediatorOutbox {
    private readonly IMediator mediator;

    public MediatorOutbox(IMediator mediator) {
        this.mediator = mediator;
    }

    public async Task SendAsync(string id, string type, string data, IReadOnlyList<KeyValuePair<string, string>> customProperties = null, CancellationToken cancellationToken = default) {
        var objectType = Type.GetType(type);

        var @event = System.Text.Json.JsonSerializer.Deserialize(data, objectType);

        var eventWrapper = (EventContext)Activator.CreateInstance(typeof(EventContext<>).MakeGenericType(objectType), @event);

        eventWrapper.Properties = customProperties;

        await mediator.Publish(eventWrapper, cancellationToken);
    }
}
