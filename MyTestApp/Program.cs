using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using MyTestApp.Data;
using MyTestApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Kafka
var kafkaBootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");

builder.Services.AddSingleton(new KafkaProducerService(kafkaBootstrapServers));
builder.Services.AddSingleton<IProducer<Null, string>>(provider =>
{
    var config = new ProducerConfig { BootstrapServers = kafkaBootstrapServers };
    return new ProducerBuilder<Null, string>(config).Build();
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.MapControllers();

// Kafka producer endpoint
app.MapPost("/send", async (KafkaProducerService producer, string message) =>
{
    try
    {
        await producer.SendMessageAsync("test-topic", new { Text = message, Date = DateTime.UtcNow });
        return Results.Ok("Message sent to Kafka");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error sending message: {ex.Message}");
    }
});

app.Run();