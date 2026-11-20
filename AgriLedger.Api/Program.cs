using System.Text.Json.Serialization;
using AgriLedger.Api;
using AgriLedger.Api.Data;
using AgriLedger.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddAgriLedgerDatabase(builder.Configuration);
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IProductionService, ProductionService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddExceptionHandler<LedgerExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgriLedgerDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
app.MapRazorPages();
app.Run();
