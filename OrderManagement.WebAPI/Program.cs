using OrderManagement.Core.ServiceContracts.OrderItems;
using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.ServiceContracts.Orders;
using OrderManagement.Infrastructure.Repositories;
using OrderManagement.Core.Services.OrderItems;
using OrderManagement.Infrastructure.DataStore;
using OrderManagement.Core.Services.Orders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});


builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<IOrderItemsRepository, OrderItemsRepository>();
builder.Services.AddScoped<IOrdersAdderService, OrdersAdderService>();
builder.Services.AddScoped<IOrdersDeleterService, OrdersDeleterService>();
builder.Services.AddScoped<IOrdersGetterService, OrdersGetterService>();
builder.Services.AddScoped<IOrdersUpdaterService, OrdersUpdaterService>();
builder.Services.AddScoped<IOrderItemsAdderService, OrderItemsAdderService>();
builder.Services.AddScoped<IOrderItemsDeleterService, OrderItemsDeleterService>();
builder.Services.AddScoped<IOrderItemsGetterService, OrderItemsGetterService>();
builder.Services.AddScoped<IOrderItemsUpdaterService, OrderItemsUpdaterService>();


var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseExceptionHandler("api/Errors");
app.UseHsts();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
