using Confluent.Kafka;

namespace Infrastructure.Kafka
{
        public class KafkaConfig
        {
            public string BootstrapServers { get; set; }
            public string ConsumerGroupId { get; set; }
            
            public KafkaProducerConfig ProducerConfig { get; set; }
            public KafkaConsumerConfig ConsumerConfig { get; set; }
            public static Acks ParseAcks(string acksValue)
            {
                // Converter string para enum Acks
                return acksValue.ToLower() switch
                {
                    "all" => Acks.All,
                    "none" => Acks.None,
                    "leader" => Acks.Leader,
                    _ => throw new ArgumentException($"Valor inválido para Acks: {acksValue}")
                };
            }
        }

        public class KafkaProducerConfig
        {
            public string Acks { get; set; }
            public bool EnableIdempotence { get; set; }
            public int LingerMs { get; set; }
            public int BatchNumMessages { get; set; }
            public string Topic { get; set; }
        }

        public class KafkaConsumerConfig
        {
            public string GroupId { get; set; }
            public string AutoOffsetReset { get; set; }
            public bool EnableAutoCommit { get; set; }
            public string Topic { get; set; }
        }



}
