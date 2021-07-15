using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using LambdaSharp.DynamoDB.Native;
using ServerlessPatterns.DynamoDB.DataAccess.Records;

namespace ServerlessPatterns.DynamoDB.DataAccess {

    public class DataAccessClient {

        //--- Constructors ---
        public DataAccessClient(string tableName, IAmazonDynamoDB dynamoClient = null)
            => Table = new DynamoTable(tableName, dynamoClient);

        //--- Properties ---
        private IDynamoTable Table { get; }

        //--- Methods ---
        public Task<bool> CreateCustomerRecordAsync(CustomerRecord customer, CancellationToken cancellationToken = default) {

            // specify DynamoDB PutItem operation
            return Table.PutItem(customer.GetPrimaryKey(), customer)

                // customer record cannot yet exist
                .WithCondition(record => DynamoCondition.DoesNotExist(record))

                // execute PutItem operation
                .ExecuteAsync();
        }

        public async Task<bool> SetCustomerAddressAsync(DynamoPrimaryKey<CustomerRecord> customerPrimaryKey, CustomerAddress address, CancellationToken cancellationToken = default) {
            const int MAX_CUSTOMER_ADDRESSES = 3;

            // Constraints:
            //  * customer must exist
            //  * customer can have at most 3 addresses

            // TODO: add address to customer record
            return false;
        }

        public async Task<bool> RemoveCustomerAddressAsync(DynamoPrimaryKey<CustomerRecord> customerPrimaryKey, string addressLabel, CancellationToken cancellationToken = default) {

            // Constraints:
            //  * customer must exist
            //  * address must exist

            // TODO: remove address from customer record
            return false;
        }

        public async Task<bool> CreateOrderRecordAsync(OrderRecord order, CancellationToken cancellationToken = default) {
            const int MAX_PENDING_ORDERS = 10;

            // Constraints:
            //  * set the order total
            //  * only allow 10 pending orders for a customer

            // add order for customer
            return false;
        }

        public async Task<(CustomerRecord Customer, IEnumerable<OrderRecord> Orders)> FindCustomerAndRecentOrderRecordsAsync(DynamoPrimaryKey<CustomerRecord> customerPrimaryKey, int limit, CancellationToken cancellationToken = default) {

            // TODO: fetch customer record and most recent orders in a single operation
            return (Customer: null, Orders: Enumerable.Empty<OrderRecord>());
        }
    }
}
