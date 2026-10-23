using Microsoft.EntityFrameworkCore;
using P6.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NexaWorksContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NexaWorks")));

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

app.Run();
