using MassTransit;
using Order_API;
using Order_API.Product_Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddMassTransit(x =>
//{
//    x.AddConsumer<OrderConsumer>();

//    x.UsingRabbitMq((context, cfg) =>
//    {
//        cfg.Host("rabbitmq://localhost", h =>
//        {
//            h.Username("guest");
//            h.Password("guest");
//        });

//        cfg.ReceiveEndpoint("order-queue", e =>
//        {
//            e.ConfigureConsumer<OrderConsumer>(context);
//        });
//    });
//});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductCreatedConsumer>();
    x.AddConsumer<ListProductCreateConsumer>();
    x.AddConsumer<GetOrderConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost"); // RabbitMQ connection

        // Set up retry and DLQ
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5))); // Retry 3 times with 5-second intervals
        cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20), TimeSpan.FromMinutes(1))); // Delayed redelivery
        cfg.UseInMemoryOutbox(); // Prevent duplicate messages during retries
        

        // Configure endpoints automatically
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
