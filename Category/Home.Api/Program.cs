using EventStore.Client;
using Home.Api;
using Home.Api.EventHandlers;
using Home.Api.Options;
using Home.Api.Workers;
using Home.OutBox.Service;
using Home.Tickets.Domain;
using Home.Tickets.Domain.Entities;
using Home.Tickets.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection.Metadata;

string[] writeMethod = { "POST", "DELETE", "PATCH", "PUT" };

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication("Home")
    .AddScheme<AuthenticationOptions, HomeAuthenticationHandler>(
        "Home",
        opts => { }
    );
//builder.Services.AddAuthorization(options => {
//    options.DefaultPolicy = new AuthorizationPolicyBuilder()
//        .AddAuthenticationSchemes("Home")
//        .RequireAuthenticatedUser()
//        .Build();
//});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => {
    var bearerScheme = new OpenApiSecurityScheme {
        BearerFormat = "Bearer",
        Name = "TOKEN Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Put **_ONLY_** your Bearer token on textbox below!",

        Reference = new OpenApiReference {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    x.AddSecurityDefinition(bearerScheme.Reference.Id, bearerScheme);

    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { bearerScheme, Array.Empty<string>() }
    });
});

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

var authOptions = builder.Configuration.GetSection("Auth").Get<AuthOptions>();

builder.Services.AddSingleton<AuthClient>();

builder.Services.AddHttpClient<AuthClient>(x => {
    x.BaseAddress = authOptions.Url;
});

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

app.UseAuthentication();
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
