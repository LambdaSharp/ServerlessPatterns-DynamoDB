using System;
using LambdaSharp.DynamoDB.Native;
using ServerlessPatterns.DynamoDB.DataAccess.Records;

namespace ServerlessPatterns.DynamoDB.DataAccess {

    public static class DataModel {

        //--- Constants ---
        private const string CUSTOMER_RECORD_PK_FORMAT = "CUSTOMER={0}";
        private const string CUSTOMER_RECORD_SK_FORMAT = "INFO";
        private const string ORDER_RECORD_PK_FORMAT = "CUSTOMER={0}";
        private const string ORDER_RECORD_SK_FORMAT = "#ORDER={1}";

        //--- Extension Methods ---
        public static DynamoPrimaryKey<CustomerRecord> GetPrimaryKey(this CustomerRecord record)
            => GetPrimaryKeyForCustomerRecord(record.CustomerId);

        public static DynamoPrimaryKey<OrderRecord> GetPrimaryKey(this OrderRecord record)
            => GetPrimaryKeyForOrderRecord(record.CustomerId, record.OrderId);

        //--- Methods ---
        public static DynamoPrimaryKey<CustomerRecord> GetPrimaryKeyForCustomerRecord(string customerId)
            => new DynamoPrimaryKey<CustomerRecord>(CUSTOMER_RECORD_PK_FORMAT, CUSTOMER_RECORD_SK_FORMAT, customerId);

        public static DynamoPrimaryKey<OrderRecord> GetPrimaryKeyForOrderRecord(string customerId, string orderId)
            => new DynamoPrimaryKey<OrderRecord>(ORDER_RECORD_PK_FORMAT, ORDER_RECORD_SK_FORMAT, customerId, orderId);

        public static IDynamoQueryClause SelectCustomerAndOrderRecords(DynamoPrimaryKey<CustomerRecord> customerPrimaryKey)
            => DynamoQuery.SelectPK(customerPrimaryKey.PKValue)
                .WhereSKMatchesAny()
                .WithTypeFilter<CustomerRecord>()
                .WithTypeFilter<OrderRecord>();
    }
}
