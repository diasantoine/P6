using Microsoft.EntityFrameworkCore;
using P6.Data;
using P6.Dtos;
using P6.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NexaWorksContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NexaWorks")));
builder.Services.AddScoped<TicketQueryService>();

var app = builder.Build();

// Database ready on first launch, no command to type:
//  1. pending migrations are applied (database creation included);
//  2. test data is inserted, only if the database is empty.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NexaWorksContext>();
    await db.Database.MigrateAsync();
    await SeedData.SeedAsync(db);
}

app.MapGet("/", () => "Hello World!");

// e.g. /tickets?status=1&product=Trader en Herbe&version=1.3&from=2024-01-01&to=2024-12-31&keywords=euro
app.MapGet("/tickets", async ([AsParameters] TicketFilter filter, TicketQueryService service) =>
    await service.GetTicketsAsync(filter));

app.Run();
