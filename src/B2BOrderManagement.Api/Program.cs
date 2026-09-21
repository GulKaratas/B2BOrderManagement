using B2BOrderManagement.Api.Data;
using B2BOrderManagement.Api.Middleware;
using B2BOrderManagement.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => Results.Ok(new
{
    name = "B2B Integration & Order Management API",
    docs = new
    {
        openapi = "/openapi/v1.json",
        postman = "postman/B2B-Order-Management.postman_collection.json"
    },
    endpoints = new[]
    {
        "/api/suppliers",
        "/api/products",
        "/api/customers",
        "/api/orders",
        "/api/integrations/orders",
        "/api/integrations/logs"
    }
}));

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    await SeedData.EnsureSeededAsync(db);
}

app.Run();
