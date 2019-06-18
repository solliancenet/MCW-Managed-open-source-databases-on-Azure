using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Transactions;
using Newtonsoft.Json;
using Confluent.Kafka;

namespace KafkaProducer
{
    public static class TelemetryGenerator
    {
        private static readonly Random Random = new Random();
        private static CancellationToken _token;

        public static async Task Init(CancellationToken token, string fqdn, string conn,
            string topic, string caCertLocation)
        {
            TelemetryGenerator._token = token;
            var clickstreamRecords = GetClickstreamData(Clickstream.FromString);
            await GenerateMessage(clickstreamRecords, fqdn, conn, topic, caCertLocation);
        }

        // Extract payment transaction data from the sample CSV file, serialize, and return the collection.
        private static List<Clickstream> GetClickstreamData(Func<string, string, Clickstream> factory)
        {
            var clickstreamData = new List<Clickstream>();

            Console.WriteLine("Retrieving sample clickstream data...");

            // Loop through all 10 CSV files:
            for (var x = 1; x < 11; x++)
            {
                using (var reader = new StreamReader(File.OpenRead($@"{x}.csv")))
                {
                    var header = reader.ReadLines()
                        .First();
                    var lines = reader.ReadLines()
                        .Skip(1);

                    // Instantiate a Clickstream object from the CSV line and header data, using the passed in factory:
                    clickstreamData.AddRange(lines.Select(line => factory(line, header)));
                }
            }

            Console.WriteLine($"Sample clickstream data retrieved. {clickstreamData.Count} records found.");

            return clickstreamData;
        }

        private static async Task GenerateMessage(List<Clickstream> clickstreamData, string brokerList,
            string connStr, string topic, string caCertLocation)
        {
            System.Console.WriteLine("Starting clickstream generator...");

            try
            {
                var conf = new ProducerConfig
                {
                    BootstrapServers = brokerList,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = "$ConnectionString",
                    SaslPassword = connStr,
                    SslCaLocation = caCertLocation,
                    LingerMs = 5
                };

                // If serializers are not specified, default serializers from
                // `Confluent.Kafka.Serializers` will be automatically used where
                // available. Note: by default strings are encoded as UTF8.
                //using (var p = new ProducerBuilder<Null, string>(conf).Build())
                //{
                //    foreach (var clickstream in clickstreamData)
                //    {
                //        if (_token.IsCancellationRequested)
                //        {
                //            return;
                //        }

                //        try
                //        {
                //            var serializedString = JsonConvert.SerializeObject(clickstream);
                //            var dr = await p.ProduceAsync(topic, new Message<Null, string> { Value = serializedString });
                //            Console.WriteLine($"{DateTime.Now} > Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                //        }
                //        catch (ProduceException<Null, string> e)
                //        {
                //            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                //        }
                //    }
                //}

                Action<DeliveryReport<Null, string>> handler = r =>
                    Console.WriteLine(!r.Error.IsError
                        ? $"Delivered message to {r.TopicPartitionOffset}"
                        : $"Delivery Error: {r.Error.Reason}");

                using (var p = new ProducerBuilder<Null, string>(conf).Build())
                {
                    int sent = 0;
                    foreach (var clickstream in clickstreamData)
                    {
                        if (_token.IsCancellationRequested)
                        {
                            return;
                        }

                        var serializedString = JsonConvert.SerializeObject(clickstream);
                        p.Produce(topic, new Message<Null, string> { Value = serializedString }, handler);
                        sent++;
                        System.Console.WriteLine($"{DateTime.Now} > Sent #{sent}: {serializedString}");

                        await Task.Delay(20);
                    }

                    // Wait for up to 10 seconds for any inflight messages to be delivered.
                    p.Flush(TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception Occurred - {e.Message}");
                throw;
            }
        }

    }
}
