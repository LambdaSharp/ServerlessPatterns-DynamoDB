using System;
using System.Collections.Generic;

namespace ServerlessPatterns.DynamoDB.DataAccess.Records {

    public enum CustomerStatus {
        Undefined,
        Active,
        Banned
    }

    public class CustomerRecord {

        //--- Properties ---
        public string CustomerId { get; set; }
        public CustomerStatus Status { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Dictionary<string, CustomerAddress> Addresses { get; set; }
        public int PendingOrders { get; set; }
    }

    public class CustomerAddress {

        //--- Properties ---
        public string Label { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }

    public enum OrderStatus {
        Undefined,
        Pending,
        Shipped,
        Delivered,
        Cancelled,
        Returned
    }

    public class OrderRecord {

        //--- Properties ---
        public string CustomerId { get; set; }
        public string OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal OrderTotal { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem {

        //--- Properties ---
        public string ItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
