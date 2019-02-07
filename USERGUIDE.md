# C# User Guide

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
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Target365.Sdk;

// Set up .NET Service Point Manager for high performance
ServicePointManager.CheckCertificateRevocationList = false;
ServicePointManager.DefaultConnectionLimit = 64;
ServicePointManager.Expect100Continue = false;
ServicePointManager.UseNagleAlgorithm = false;

var baseUrl = new Uri("https://shared.target365.io");
var keyName = "YOUR_KEY";
var privateKey = "BASE64_EC_PRIVATE_KEY";
var serviceClient = new Target365Client(baseUrl, keyName, privateKey);
...
serviceClient.Dispose() // Remember to dispose the client or use using clauses :)
```

### Send an SMS
This example sends an SMS to 98079008 (+47 for Norway) from "Target365" with the text "Hello world from SMS!".
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

### Schedule an SMS for later sending
This example sets up a scheduled SMS. Scheduled messages can be updated or deleted before the time of sending.
```C#
var outMessage = new OutMessage
{
    TransactionId = Guid.NewGuid().ToString(),
    Sender = "Target365",
    Recipient = "+4798079008",
    Content = "Hello World from SMS!",
    SendTime = DateTimeOffset.UtcNow.AddHours(2),
};

await serviceClient.CreateOutMessageAsync(outMessage);
Console.WriteLine($"Out-message scheduled to be sent in 2 hours.");
```

### Edit a scheduled SMS
This example updates a previously created scheduled SMS.
```C#
var outMessage = await serviceClient.GetOutMessageAsync(transactionId);
outMessage.SendTime = outMessage.SendTime.AddHours(1);
outMessage.Content += " An hour later! :)";
await serviceClient.UpdateOutMessageAsync(outMessage);
```

### Delete a scheduled SMS
This example deletes a previously created scheduled SMS.
```C#
await serviceClient.DeleteOutMessageAsync(transactionId);
```

### Create a Strex payment transaction
This example creates a 1 NOK Strex payment transaction that the end user will confirm by replying "OK" to an SMS from Strex.
```C#
var transaction = new StrexTransaction
{
    TransactionId = Guid.NewGuid().ToString(),
    ShortNumber = "2002",
    Recipient = "+4798079008",
    MerchantId = "YOUR_MERCHANT_ID",
    Price = 1,
    ServiceCode = ServiceCodes.NonCommercialDonation,
    InvoiceText = "Donation test",
};

await serviceClient.CreateStrexTransactionAsync(transaction);
```

### Create a Strex payment transaction confirmed by one-time password (OTP)
This example creates a Strex one-time password sent to the end user and get completes the payment by using the one-time password.
```C#
transactionId = Guid.NewGuid().ToString();

var oneTimePassword = new OneTimePassword
{
    TransactionId = transactionId,
    MerchantId = "YOUR_MERCHANT_ID",
    Recipient = "+4798079008",
    Recurring = false
};

await serviceClient.CreateOneTimePasswordAsync(oneTimePassword);

// *** Get input from end user (eg. via web site) ***

var transaction = new StrexTransaction
{
    TransactionId = transactionId,
    ShortNumber = "2002",
    Recipient = "+4798079008",
    MerchantId = "YOUR_MERCHANT_ID",
    Price = 1,
    ServiceCode = ServiceCodes.NonCommercialDonation,
    InvoiceText = "Donation test",
    OneTimePassword = "ONE_TIME_PASSWORD_FROM_USER"
};

await serviceClient.CreateStrexTransactionAsync(transaction);
```

### Reverse a Strex payment transaction
This example reverses a previously billed Strex payment transaction. The original transaction will not change, but a reversal transaction will be created that counters the previous transaction by a negative Price. The reversal is an asynchronous operation that usually takes some seconds to finish.
```C#
var reversedTransactionId = await serviceClient.ReverseStrexTransactionAsync(transactionId);
Console.WriteLine($"Reversed transaction id is {reversedTransactionId}");
```

