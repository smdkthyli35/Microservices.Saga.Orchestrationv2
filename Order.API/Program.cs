using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Contexts;
using Order.API.Enums;
using Order.API.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, configure) =>
    {
        configure.Host(builder.Configuration["RabbitMQ"]);
    });
});

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MQSSQLServer"));
});

var app = builder.Build();

app.MapPost("/create-order", async (CreateOrderVM model, OrderDbContext context) =>
{
    Order.API.Models.Order order = new()
    {
        BuyerId = model.BuyerId,
        CreatedDate = DateTime.UtcNow,
        OrderStatus = OrderStatus.Suspend,
        TotalPrice = model.OrderItems.Sum(oi => oi.Price * oi.Count),
        OrderItems = model.OrderItems.Select(oi => new Order.API.Models.OrderItem
        {
            Count = oi.Count,
            Price = oi.Price,
            ProductId = oi.ProductId,
        }).ToList()
    };

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
