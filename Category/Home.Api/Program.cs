using EventStore.Client;
using Home.Api;
using Home.Api.EventHandlers;
using Home.Api.Workers;
using Home.OutBox.Service;
using Home.Tickets.Domain;
using Home.Tickets.Domain.Entities;
using Home.Tickets.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;

string[] writeMethod = { "POST", "DELETE", "PATCH", "PUT" };

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var databaseConnectionString = builder.Configuration.GetConnectionString("database");

var optionsBuilder = new DbContextOptionsBuilder<TicketContext>();

optionsBuilder.UseNpgsql(databaseConnectionString, x => {
    x.MigrationsHistoryTable("__EFMigration", TicketContext.schema);
});

builder.Services.AddSingleton(optionsBuilder.Options);

builder.Services.AddScoped<TicketContext>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var eventStoreConnectionString = builder.Configuration.GetConnectionString("eventstore");

var settings = EventStoreClientSettings.Create(eventStoreConnectionString);

builder.Services.AddSingleton(x => new EventStoreClient(settings));

builder.Services.AddScoped<OutboxService>();
builder.Services.AddScoped<MediatorOutbox>();

builder.Services.AddScoped<IOutBoxPendingStorage, EFOutBoxPendingStorage>();
builder.Services.AddScoped<IOutBoxSender, EventStoreOutBoxSenerSender>();

builder.Services.AddHostedService<OutboxSenderWorker>();
builder.Services.AddHostedService<OutboxReciverWorker>();

builder.Services.AddMediatR(x => {
    x.RegisterServicesFromAssemblyContaining<TicketClosedEventHandler>();
});

var app = builder.Build();

await Migrate(app.Services);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) => {
    if (writeMethod.Any(x => x.Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase))) {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        var efContext = context.RequestServices.GetRequiredService<TicketContext>();

        await efContext.BeginTransactionAsync(context.RequestAborted);

        try {
            await next.Invoke();
            await efContext.SaveChangesAsync(context.RequestAborted);
            await efContext.CommitTransactionAsync(context.RequestAborted);
            return;
        }
        catch (Exception ex) {
            logger.LogError(ex, ex.Message);

            await efContext.RollBackTransactionAsync(context.RequestAborted);

            throw;
        }
    }

    await next.Invoke();
});

app.Run();


async Task Migrate(IServiceProvider services) {
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TicketContext>();

    await context.MigrateAsync();

}
