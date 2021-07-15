using System;
using KSUID;
using ServerlessPatterns.DynamoDB.DataAccess;
using ServerlessPatterns.DynamoDB.DataAccess.Records;

using static ServerlessPatterns.DynamoDB.ConsoleApp.Helper;

// TODO: change table name based on output of the `lash` command!
TableName = "";
if(string.IsNullOrEmpty(TableName)) {
    Console.WriteLine("ERROR: Set TableName in Program.cs");
    return;
}

// Let's get started
Console.WriteLine("DynamoDB for FUN and GLORY!");
Console.WriteLine($"=> Using DynamoDB Table: {TableName}");

// Initialize
await DeleteAllItemsInTableAsync();
var client = CreateDataAccessClient();

// new customer object
var customer = new CustomerRecord {
    CustomerId = Guid.NewGuid().ToString(),
    Status = CustomerStatus.Active,
    Name = "John Doe",
    Email = "john@example.org",
    Addresses = new()
};

//-------------------------
// Task 1: create a new customer record
//-------------------------
await client.CreateCustomerRecordAsync(customer);

//-------------------------
// Task 2: set home address for customer
//-------------------------
await client.SetCustomerAddressAsync(customer.GetPrimaryKey(), new() {
    Label = "Work",
    Street = "101 W. Broadway",
    City = "San Diego",
    State = "CA"
});

//-------------------------
// Task 3: remove home address
//-------------------------
await client.RemoveCustomerAddressAsync(customer.GetPrimaryKey(), "Work");

//-------------------------
// Task 4: add customer order
//-------------------------
var order = new OrderRecord {
    CustomerId = customer.CustomerId,

    // NOTE: we're using KSUID to have a time-ordered GUID
    OrderId = new Ksuid().ToString(),

    Status = OrderStatus.Pending,
    Items = new() {
        new() {
            ItemId = "vendor=123|item=abc",
            Price = 42.13m,
            Quantity = 2
        },
        new() {
            ItemId = "vendor=456|item=def",
            Price = 42.13m,
            Quantity = 2
        }
    }
};
await client.CreateOrderRecordAsync(order);

//-------------------------
// Task 5: fetch customer with most recent orders
//-------------------------
var (foundCustomer, foundRecentOrders) = await client.FindCustomerAndRecentOrderRecordsAsync(customer.GetPrimaryKey(), 10);
if(foundCustomer is not null) {
    Console.WriteLine($"Customer: {ToJson(foundCustomer)}");
    foreach(var foundRecentOrder in foundRecentOrders) {
        Console.WriteLine($"Order: {ToJson(foundRecentOrder)}");
    }
}

// And done!
await PrintAllItemsInTableAsync();
Console.WriteLine($"That's all folks! (⌐■_■)");
