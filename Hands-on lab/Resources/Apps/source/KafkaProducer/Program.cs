using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace KafkaProducer
{
    class Program
    {
        // AutoResetEvent to signal when to exit the application.
        private static readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);
        private static IConfigurationRoot _configuration;

        static void Main(string[] args)
        {
            // Setup configuration to either read from the appsettings.json file (if present) or environment variables.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            System.Console.WriteLine("Starting...");
            var conn = _configuration["EVENTHUB_CONNECTIONSTRING"];
            if (string.IsNullOrWhiteSpace(conn))
            {
                System.Console.WriteLine(@"Warning: The EVENTHUB_CONNECTIONSTRING appsettings.json setting or environment variable is not set.");
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
