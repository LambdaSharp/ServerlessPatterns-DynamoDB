using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using LambdaSharp.DynamoDB.Serialization.Utility;
using ServerlessPatterns.DynamoDB.DataAccess;

namespace ServerlessPatterns.DynamoDB.ConsoleApp {

    public static class Helper {

        //--- Class Fields ---
        public static IAmazonDynamoDB DynamoClient = new AmazonDynamoDBClient();
        public static string TableName;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions {
            WriteIndented = true,
            Converters = {
                new DynamoAttributeValueConverter()
            }
        };

        //--- Class Methods ---
        public static DataAccessClient CreateDataAccessClient() => new(TableName, DynamoClient);

        /// <summary>
        /// Delete all items in the DynamoDB table.
        /// </summary>
        public static async Task DeleteAllItemsInTableAsync() {

            // scan for all records
            var scanRequest = new ScanRequest {
                TableName = TableName
            };
            do {
                var scanResponse = await DynamoClient.ScanAsync(scanRequest);

                // delete each returned item
                foreach(var item in scanResponse.Items) {
                    await DynamoClient.DeleteItemAsync(new DeleteItemRequest {
                        TableName = TableName,
                        Key = {
                            ["PK"] = item["PK"],
                            ["SK"] = item["SK"]
                        }
                    });
                }
                scanRequest.ExclusiveStartKey = scanResponse.LastEvaluatedKey;
            } while(scanRequest.ExclusiveStartKey.Any());
        }

        /// <summary>
        /// Fetch all records in the DynamoDB table and print them on the console.
        /// </summary>
        public static async Task PrintAllItemsInTableAsync() {

            // scan for all records
            var scanRequest = new ScanRequest {
                TableName = TableName
            };
            var any = false;
            do {
                var scanResponse = await DynamoClient.ScanAsync(scanRequest);
                foreach(var item in scanResponse.Items) {
                    any = true;
                    Console.WriteLine(ToJson(item));
                }
                scanRequest.ExclusiveStartKey = scanResponse.LastEvaluatedKey;
            } while(scanRequest.ExclusiveStartKey.Any());
            if(!any) {
                Console.WriteLine("No records found. ¯\\_(ツ)_/¯");
            }
        }

        public static string ToJson(object item)
            => (item is null)
                ? "<null>"
                : JsonSerializer.Serialize(item, _jsonSerializerOptions);
    }
}