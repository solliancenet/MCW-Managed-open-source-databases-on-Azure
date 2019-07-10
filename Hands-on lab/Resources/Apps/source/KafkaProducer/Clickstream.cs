using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KafkaProducer
{
    public class Clickstream
    {
        [JsonProperty] public long customer_id { get; set; }

        [JsonProperty] public string event_type { get; set; }

        [JsonProperty] public string country { get; set; }

        [JsonProperty] public string browser { get; set; }

        [JsonProperty] public long device_id { get; set; }

        [JsonProperty] public long session_id { get; set; }

        [JsonIgnore]
        protected string CsvHeader { get; set; }

        [JsonIgnore]
        protected string CsvString { get; set; }

        public string GetData()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Clickstream FromString(string line, string header)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new ArgumentException($"{nameof(line)} cannot be null, empty, or only whitespace");
            }

            var tokens = line.Split(',');
            if (tokens.Length != 6)
            {
                throw new ArgumentException($"Invalid record: {line}");
            }

            var cs = new Clickstream
            {
                CsvString = line,
                CsvHeader = header
            };
            try
            {
                cs.customer_id = long.TryParse(tokens[0], out var lresult) ? lresult : 0;
                cs.event_type = tokens[1];
                cs.country = tokens[2];
                cs.browser = tokens[3];
                cs.device_id = long.TryParse(tokens[4], out lresult) ? lresult : 0;
                cs.session_id = long.TryParse(tokens[5], out lresult) ? lresult : 0;

                return cs;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid record: {line}", ex);
            }
        }
    }
}
