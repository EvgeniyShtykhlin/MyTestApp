using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace MyTestApp.Controllers
{
    [ApiController]
    [Route("kafka")]
    public class KafkaController : ControllerBase
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaController(IProducer<Null, string> producer)
        {
            _producer = producer;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromQuery] string message)
        {
            var result = await _producer.ProduceAsync("my-topic", new Message<Null, string> { Value = message });
            return Ok($"Message send: {result.Value}");
        }
    }
}
