using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaProducer
{
    class Program
    {
        // AutoResetEvent to signal when to exit the application.
        private static readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            System.Console.WriteLine("Starting...");
            var conn = Environment.GetEnvironmentVariable("EVENTHUB_CONNECTIONSTRING");
            if (string.IsNullOrWhiteSpace(conn))
            {
                System.Console.WriteLine(@"Warning: The EVENTHUB_CONNECTIONSTRING environment variable is not set.");
            }
            var fqdn = ParseConnectionString(conn);
            var caCertLocation = @".\cacert.pem";

            var tokenSource = new CancellationTokenSource();
            // Fire and forget
            Task.Run(() => { TelemetryGenerator.Init(tokenSource.Token, fqdn, conn, "clickstream", caCertLocation).Wait(); }, tokenSource.Token);

            // Handle Control+C or Control+Break
            System.Console.CancelKeyPress += (o, e) =>
            {
                System.Console.WriteLine("Stopping...");
                tokenSource.Cancel();

                // Allow the main thread to continue and exit...
                WaitHandle.Set();
            };

            // Wait
            WaitHandle.WaitOne();
        }

        /// <summary>
        /// Extracts the FQDN value from the Event Hub connection string.
        /// </summary>
        /// <param name="connectionString">Event Hub connection string.</param>
        /// <returns></returns>
        private static string ParseConnectionString(string connectionString)
        {
            var fqdn = "";

            if (string.IsNullOrWhiteSpace(connectionString)) return fqdn;
            connectionString = connectionString.Replace("Endpoint=sb://", "");
            fqdn = connectionString.Substring(0, connectionString.IndexOf("/", StringComparison.InvariantCultureIgnoreCase));

            return $"{fqdn}:9093";
        }
    }
}
