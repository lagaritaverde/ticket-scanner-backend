using Home.Auth;
using Home.Auth.Database;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var datasource = builder.Configuration.GetConnectionString("database2") ?? "Data Source=users.db";

builder.Services.AddAuthentication("Home")
    .AddScheme<AuthenticationOptions, HomeAuthenticationHandler>(
        "Home",
        opts => { }
    );
// Add services to the container.

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

builder.Services.AddSingleton<SessionStore>();

builder.Services.AddScoped(x => new UserDbContext(datasource));

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
await dbContext.Database.MigrateAsync();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
//}

app.UseForwardedHeaders(new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthorization();

app.MapControllers();

app.Run();
