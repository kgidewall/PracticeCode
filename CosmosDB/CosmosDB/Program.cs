// Practice Program following https://docs.microsoft.com/en-us/azure/cosmos-db/tutorial-develop-sql-api-dotnet
// I created the DocumentDB resource in the Microsoft subscription
// I have deleted the PrimaryKey variable value for the DocumentDB before committing to the repository. You will need to get it from the Portal and put it into the variable before this will work.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CosmosDB
{
    class Program
    {
        public class DeviceReading
        {
            [JsonProperty("id")]
            public string Id;

            [JsonProperty("deviceId")]
            public string DeviceId;

            [JsonConverter(typeof(IsoDateTimeConverter))]
            [JsonProperty("readingTime")]
            public DateTime ReadingTime;

            [JsonProperty("metricType")]
            public string MetricType;

            [JsonProperty("unit")]
            public string Unit;

            [JsonProperty("metricValue")]
            public double MetricValue;
        }

        private const string EndpointUrl = "https://kentong-cosmosdb-practice.documents.azure.com:443/";
        // get key from here: https://ms.portal.azure.com/#resource/subscriptions/ce619e7d-0542-4f54-8487-9df93a8e2011/resourcegroups/kentong-cosmosdb-practice/providers/Microsoft.DocumentDB/databaseAccounts/kentong-cosmosdb-practice/keys
        private const string PrimaryKey = "<Get key from Portal. See link above>";
        private DocumentClient client;

        static void Main(string[] args)
        {
            RunDocDBFunctionality().Wait();

            Console.WriteLine("Press Enter to exit the program");
            Console.ReadLine();
        }

        static async Task RunDocDBFunctionality()
        { 
            DocumentClient client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            await CreateDatabase(client);

            // Create a document. Here the partition key is extracted 
            // as "XMS-0001" based on the collection definition
            await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri("db", "coll"),
                new DeviceReading
                {
                    Id = "XMS-001-FE24C",
                    DeviceId = "XMS-0001",
                    MetricType = "Temperature",
                    MetricValue = 105.00,
                    Unit = "Fahrenheit",
                    ReadingTime = DateTime.UtcNow
                });

            // Read document. Needs the partition key and the Id to be specified
            Document result = await client.ReadDocumentAsync(
              UriFactory.CreateDocumentUri("db", "coll", "XMS-001-FE24C"),
              new RequestOptions { PartitionKey = new PartitionKey("XMS-0001") });

            DeviceReading reading = (DeviceReading)(dynamic)result;

            // Update the document. Partition key is not required, again extracted from the document
            reading.MetricValue = 104;
            reading.ReadingTime = DateTime.UtcNow;

            await client.ReplaceDocumentAsync(
              UriFactory.CreateDocumentUri("db", "coll", "XMS-001-FE24C"),
              reading);

            // Query using partition key
            IQueryable<DeviceReading> query = client.CreateDocumentQuery<DeviceReading>(
                UriFactory.CreateDocumentCollectionUri("db", "coll"))
                .Where(m => m.MetricType == "Temperature" && m.DeviceId == "XMS-0001");

            foreach (var doc in query)
            {
                Console.WriteLine(doc);
            }

            // Query across partition keys
            IQueryable<DeviceReading> crossPartitionQuery = client.CreateDocumentQuery<DeviceReading>(
                UriFactory.CreateDocumentCollectionUri("db", "coll"),
                new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(m => m.MetricType == "Temperature" && m.MetricValue > 100);

            foreach (var doc in crossPartitionQuery)
            {
                Console.WriteLine(doc);
            }

            /*
            // Cross-partition Order By queries
            IQueryable<DeviceReading> crossPartitionQuery = client.CreateDocumentQuery<DeviceReading>(
                UriFactory.CreateDocumentCollectionUri("db", "coll"),
                new FeedOptions { EnableCrossPartitionQuery = true, MaxDegreeOfParallelism = 10, MaxBufferedItemCount = 100 })
                .Where(m => m.MetricType == "Temperature" && m.MetricValue > 100)
                .OrderBy(m => m.MetricValue);
                */

            // Delete a document. The partition key is required.
            await client.DeleteDocumentAsync(
              UriFactory.CreateDocumentUri("db", "coll", "XMS-001-FE24C"),
              new RequestOptions { PartitionKey = new PartitionKey("XMS-0001") });
        }

        static async Task CreateDatabase(DocumentClient client)
        {
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "db" });

            // Collection for device telemetry. Here the JSON property deviceId is used  
            // as the partition key to spread across partitions. Configured for 2500 RU/s  
            // throughput and an indexing policy that supports sorting against any  
            // number or string property. .
            DocumentCollection myCollection = new DocumentCollection();
            myCollection.Id = "coll";
            myCollection.PartitionKey.Paths.Add("/deviceId");

            await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri("db"),
                myCollection,
                new RequestOptions { OfferThroughput = 400 });
        }
    }
}
