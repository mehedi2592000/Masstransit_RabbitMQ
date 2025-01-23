using MassTransit;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry;
using Product_API;
using Microsoft.EntityFrameworkCore;
using Product_API.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddMassTransit(x =>
//{
//    x.UsingRabbitMq((context, cfg) =>
//    {
//        cfg.Host("rabbitmq://localhost", h =>
//        {
//            h.Username("guest");
//            h.Password("guest");
//        });
//    });
//});

builder.Services.AddDbContext<ProductDbContext>(context =>
{
    context.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30)));
        cfg.UseInMemoryOutbox();
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
        .AddHttpClientInstrumentation()             //capture spans fro outgoing http request 
        .AddConsoleExporter();

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
           .AddConsoleExporter();

    });

var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
if (useOtlpExporter)
{
    builder.Services.AddOpenTelemetry().UseOtlpExporter();
}


builder.Services.AddScoped<ProductService>();
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
