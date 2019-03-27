# C# User Guide

## Table of Contents
* [Introduction](#introduction)
* [Setup](#setup)
    * [Target365Client](#target365client)
* [Text messages](#text-messages)
    * [Send an SMS](#send-an-sms)
    * [Schedule an SMS for later sending](#schedule-an-sms-for-later-sending)
    * [Edit a scheduled SMS](#edit-a-scheduled-sms)
    * [Delete a scheduled SMS](#delete-a-scheduled-sms)
* [Payment transactions](#payment-transactions)
    * [Create a Strex payment transaction](#create-a-strex-payment-transaction)
    * [Create a Strex payment transaction with one-time password](#create-a-strex-payment-transaction-with-one-time-password)
    * [Reverse a Strex payment transaction](#reverse-a-strex-payment-transaction)
* [Lookup](#lookup)
    * [Address lookup for mobile number](#address-lookup-for-mobile-number)
* [Keywords](#keywords)
    * [Create a keyword](#create-a-keyword)
    * [Delete a keyword](#delete-a-keyword)
    * [SMS forward](#sms-forward)
    * [SMS forward using the SDK](#sms-forward-using-the-sdk)
    * [DLR forward](#dlr-forward)
    * [DLR forward using the SDK](#dlr-forward-using-the-sdk)

## Introduction
The Target365 SDK gives you direct access to our online services like sending and receiving SMS, address lookup and Strex payment transactions. The SDK provides an appropriate abstraction level for C# development and is officially support by Target365. The SDK also implements very high security (ECDsaP256 HMAC).

## Setup
### Target365Client
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
## Text messages

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
## Payment transactions

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

### Create a Strex payment transaction with one-time password
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
This example reverses a previously billed Strex payment transaction. The original transaction will not change, but a reversal transaction will be created that counters the previous transaction by a negative Price. The reversal is an asynchronous operation that usually takes a few seconds to finish.
```C#
var reversalTransactionId = await serviceClient.ReverseStrexTransactionAsync(transactionId);
```
## Lookup

### Address lookup for mobile number
This example looks up address information for the mobile number 98079008. Lookup information includes registered name and address.
```C#
var lookup = await serviceClient.LookupAsync("+4798079008");
var firstName = lookup.FirstName;
var lastName = lookup.LastName;
```

## Keywords

### Create a keyword
This example creates a new keyword on short number 2002 that forwards incoming SMS messages to 2002 that starts with "HELLO" to the URL  "https://your-site.net/api/receive-sms".
```C#
var keyword = new Keyword
{
    ShortNumberId = "NO-2002",
    KeywordText = "HELLO",
    Mode = KeywordModes.Text,
    ForwardUrl = "https://your-site.net/api/receive-sms",
    Enabled = true
};

var keywordId = await serviceClient.CreateKeywordAsync(keyword);
```

### Delete a keyword
This example deletes a keyword.
```C#
await serviceClient.DeleteKeywordAsync(keywordId);
```

### SMS forward
This example shows how SMS messages are forwarded to the keywords ForwardUrl. All sms forwards expects a response with status code 200 (OK). If the request times out or response status code differs the forward will be retried several times.
#### Request
```
POST https://your-site.net/api/receive-sms HTTP/1.1
Content-Type: application/json
Host: your-site.net

{
  "transactionId":"00568c6b-7baf-4869-b083-d22afc163059",
  "created":"2019-02-07T21:11:00+00:00",
  "sender":"+4798079008",
  "recipient":"2002",
  "content":"HELLO"
}
```

#### Response
```
HTTP/1.1 200 OK
Date: Thu, 07 Feb 2019 21:13:51 GMT
Content-Length: 0
```

### SMS forward using the SDK
This example shows how to parse an SMS forward request using the SDK.
```C#
[Route("api/receive-sms")]
public async Task<HttpResponseMessage> PostInMessage(HttpRequestMessage request)
{
    var settings = new JsonSerializerSettings
    {
    	Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = false } },
    };
    
    var message = JsonConvert.DeserializeObject<InMessage>(await request.Content.ReadAsStringAsync(), settings);
    return request.CreateResponse(HttpStatusCode.OK);
}
```

### DLR forward
#### Request
```
POST https://your-site.net/api/receive-dlr HTTP/1.1
Content-Type: application/json
Host: your-site.net

{
    "correlationId": null,
    "transactionId": "client-specified-id-5c88e736bb4b8",
    "price": null,
    "sender": "Target365",
    "recipient": "+4798079008",
    "operatorId": "no.telenor",
    "statusCode": "Ok",
    "detailedStatusCode": "Delivered",
    "delivered": true,
    "billed": null,
    "smscTransactionId": "16976c7448d",
    "smscMessageParts": 1
}
```

#### Response
```
HTTP/1.1 200 OK
Date: Thu, 07 Feb 2019 21:13:51 GMT
Content-Length: 0
```

### DLR forward using the SDK
This example shows how to parse an DLR forward request using the SDK.
```C#
[Route("api/receive-dlr")]
public async Task<HttpResponseMessage> PostDeliveryReport(HttpRequestMessage request)
{
    var settings = new JsonSerializerSettings
    {
    	Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = false } },
    };
    
    var message = JsonConvert.DeserializeObject<DeliveryReport>(await request.Content.ReadAsStringAsync(), settings);
    return request.CreateResponse(HttpStatusCode.OK);
}
```
