# ServerlessPatterns - DynamoDB for Fun and Glory!

## Getting Started

Deploy the LambdaSharp module to create a DynamoDB table with both a local secondary index and global secondary index.

```bash
lash deploy
```

The module is deployed using CloudFormation and could take a few minutes. At the end, it will show the DynamoDB table name in its output under `DataTableName`.

Now open your favorite editor and check out the 2 included projects:
* `ConsoleApp` is a console app that we can run to interact and experiment with our DynamoDB table
* `DataAccess` is the library with our data access business logic

## Run ConsoleApp

Switch to the `ConsoleApp` folder and run the project:
```bash
dotnet run
```

The output will look something like this:
```
DynamoDB for FUN and GLORY!
=> Using DynamoDB Table: ServerlessPatterns-DynamoDB-DataTable-11MP0C498UN54
{
  "_t": {
    "S": "ServerlessPatterns.DynamoDB.DataAccess.Records.CustomerRecord"
  },
  "OpenOrders": {
    "N": "0"
  },
  "SK": {
    "S": "INFO"
  },
  "Status": {
    "S": "Active"
  },
  "PK": {
    "S": "CustomerId=eee1ce47-43d7-40ab-8f0e-d30f437798aa"
  },
  "Addresses": {
    "M": {}
  },
  "CustomerId": {
    "S": "eee1ce47-43d7-40ab-8f0e-d30f437798aa"
  },
  "Email": {
    "S": "john@example.org"
  },
  "_m": {
    "N": "1626309153780"
  },
  "Name": {
    "S": "John Doe"
  }
}
That's all folks! (⌐■_■)
```
