using MassTransit;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry;
using Order_API;
using Order_API.Product_Consumer;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddDbContext<OrderDbContext>(context =>
{
    context.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});

builder.Services.AddMassTransit(x =>
{
    //x.AddConsumer<ProductCreatedConsumer>();
    //x.AddConsumer<ListProductCreateConsumer>();
    //x.AddConsumer<GetOrderConsumer>();

    x.AddConsumers(typeof(Program).Assembly);
    
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

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()
    //.ConfigureResource(resource => resource.AddService("CoffeeShopHasan"))
    .WithMetrics(metrics =>
    {
        metrics
        .AddAspNetCoreInstrumentation()               //use for creates spans incoming Http Request 
        .AddHttpClientInstrumentation()
        .AddConsoleExporter();              //capture spans fro outgoing http request 

        //.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyService"));
        //metrics.AddRuntimeInstrumentation()
        //   .AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", "System.Net.Http");

    })
    .WithTracing(tracing =>
    {
        tracing
           .AddAspNetCoreInstrumentation()
           .AddHttpClientInstrumentation()
           .AddSource("Masstransit-queue")
           .AddSource("ProductService", "OrderService")
           //.AddSqlClientInstrumentation(options =>
           // {
           //     options.SetDbStatementForText = true; // Capture the actual SQL queries
           //     options.EnableConnectionLevelAttributes = true; // Capture connection-level information
           // })
           .AddConsoleExporter();

    });

var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
if (useOtlpExporter)
{
    builder.Services.AddOpenTelemetry().UseOtlpExporter();
}

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
