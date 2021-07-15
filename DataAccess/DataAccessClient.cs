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

            // add address to customer record
            return await Table.UpdateItem(customerPrimaryKey)

                // only update record if it exists
                .WithCondition(record => DynamoCondition.Exists(record))

                // only allow up to addresses
                .WithCondition(record => DynamoCondition.Exists(record.Addresses[address.Label]) ||(DynamoCondition.Size(record.Addresses) < MAX_CUSTOMER_ADDRESSES))

                // add/update address in record
                .Set(record => record.Addresses[address.Label], address)

                // execute UpdateItem operation
                .ExecuteAsync(cancellationToken);
        }

        public async Task<bool> RemoveCustomerAddressAsync(DynamoPrimaryKey<CustomerRecord> customerPrimaryKey, string addressLabel, CancellationToken cancellationToken = default) {

            // Constraints:
            //  * customer must exist
            //  * address must exist

            // remove address from customer record
            return await Table.UpdateItem(customerPrimaryKey)

                // only update record if it exists
                .WithCondition(record => DynamoCondition.Exists(record))

                // remove entry from Addresses property
                .Remove(record => record.Addresses[addressLabel])

                // execute UpdateItem operation
                .ExecuteAsync(cancellationToken);
        }

        public async Task<bool> CreateOrderRecordAsync(OrderRecord order, CancellationToken cancellationToken = default) {
            const int MAX_PENDING_ORDERS = 10;

            // Constraints:
            //  * set the order total
            //  * only allow 10 pending orders for a customer

            // add order for customer
            return await Table.TransactWriteItems()

                // update PendingOrders counter
                .BeginUpdateItem(DataModel.GetPrimaryKeyForCustomerRecord(order.CustomerId))

                    // customer cannot have too many pending orders
                    .WithCondition(record => record.PendingOrders < MAX_PENDING_ORDERS)

                    // increase pending order count
                    .Set(record => record.PendingOrders, record => record.PendingOrders + 1)
                .End()

                // store new order record
                .BeginPutItem(order.GetPrimaryKey(), order)
                    .WithCondition(record => DynamoCondition.DoesNotExist(record))
                .End()

                // execute TransactWriteItems operation
                .TryExecuteAsync(cancellationToken);
        }

        public async Task<(CustomerRecord Customer, IEnumerable<OrderRecord> Orders)> FindCustomerAndRecentOrderRecordsAsync(DynamoPrimaryKey<CustomerRecord> customerPrimaryKey, int limit, CancellationToken cancellationToken = default) {

            // fetch customer record and most recent orders in a single operation
            var items = await Table.Query(DataModel.SelectCustomerAndOrderRecords(customerPrimaryKey), limit: limit, scanIndexForward: false)
                .ExecuteAsync();

            // parse records into expected types
            return (Customer: items.OfType<CustomerRecord>().Single(), Orders: items.OfType<OrderRecord>());
        }
    }
}
