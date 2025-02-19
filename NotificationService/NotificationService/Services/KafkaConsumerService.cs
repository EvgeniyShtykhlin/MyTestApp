using Confluent.Kafka;

namespace NotificationService.Services
{
    public class KafkaConsumerService
    {
        private readonly string _bootstrapServers = "localhost:9092";
        private readonly string _topic = "test-topic";
        private readonly string _groupId = "notification-service-group";

        public void StartConsuming()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            consumer.Subscribe(_topic);
            Console.WriteLine($"✅ Subscribed to topic: {_topic}");

            while (true)
            {
                var consumeResult = consumer.Consume();
                Console.WriteLine($"📥 Received message: {consumeResult.Message.Value}");
                HandleMessage(consumeResult.Message.Value);
            }
        }

        private void HandleMessage(string message)
        {
            Console.WriteLine($"🔔 Notification processed: {message}");
            // Здесь можно добавить сохранение в БД или отправку email
        }
    }
}
