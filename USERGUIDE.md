# User Guide for C#

## Getting started
To get started please see the [README.md](README.md).

## Introduction
The Target365 SDK is all about direct API access for handling SMS messaging.
The API is a fairly straight-forward REST API, but it has extremely high security that's hard to implement correctly.
Therefore we highly recommend using the SDK and working on a higher level of abstraction than HTTP.
If none of the SDKs are a good match you can use our REST API directly.
Check out our swagger here: <https://test.target365.io/api/swagger.json>

## Tutorials
### Setting up the Target365Client
```C#
var baseUrl = new Uri("https://shared.target365.io");
var keyName = "YOUR_KEY";
var privateKey = "BASE64_EC_PRIVATE_KEY";
var serviceClient = new Target365Client(baseUrl, keyName, privateKey);
...
serviceClient.Dispose() // Remember to dispose the client or use using clauses :)
```

### Send an SMS
This simple example sends an SMS to 98079008 (+47 for Norway) from "Target365" with the text "Hello world from SMS!".

```C#
var outMessage = new OutMessage
{
    TransactionId = Guid.NewGuid().ToString(),
    Sender = "Target365",
    Recipient = "+4798079008",
    Content = "Hello World from SMS!",
};

await serviceClient.CreateOutMessageAsync(outMessage);
Console.WriteLine($"Out-message created with transaction id {outMessage.TransactionId}");
```
