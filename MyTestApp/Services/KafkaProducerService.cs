using Confluent.Kafka;
using System.Text.Json;

namespace MyTestApp.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(string bootstrapServers)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendMessageAsync<T>(string topic, T message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var result = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });

            Console.WriteLine($"Message send in to '{topic}': {result.Value}");
        }
    }
}
