# C# User Guide

## Table of Contents
* [Introduction](#introduction)
* [Generate private-public key pair](#generate-private-public-key-pair)
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
* [One-click transactions](#one-click-transactions)
    * [One-time transaction](#one-time-transaction)
    * [Setup subscription transaction](#setup-subscription-transaction)
    * [Recurring transaction](#recurring-transaction)
* [Lookup](#lookup)
    * [Address lookup for mobile number](#address-lookup-for-mobile-number)
* [Keywords](#keywords)
    * [Create a keyword](#create-a-keyword)
    * [Delete a keyword](#delete-a-keyword)
* [Forwards](#forwards)
    * [SMS forward](#sms-forward)
    * [SMS forward using the SDK](#sms-forward-using-the-sdk)
    * [DLR forward](#dlr-forward)
    * [DLR forward using the SDK](#dlr-forward-using-the-sdk)
    * [DLR status codes](#dlr-status-codes)

## Introduction
The Target365 SDK gives you direct access to our online services like sending and receiving SMS, address lookup and Strex payment transactions. The SDK provides an appropriate abstraction level for C# development and is officially support by Target365. The SDK also implements very high security (ECDsaP256 HMAC).

## Generate private-public key pair
```C#
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Target365KeyGen
{
    class Program
    {
        static void Main()
        {
            var keyParams = new CngKeyCreationParameters
            {
                ExportPolicy = CngExportPolicies.AllowPlaintextExport,
                KeyUsage = CngKeyUsages.Decryption | CngKeyUsages.Signing
            };

            using (var cngKey = CngKey.Create(CngAlgorithm.ECDsaP256, null, keyParams))
            using (var cng = new ECDsaCng(cngKey))
            {
                var privateKey = Convert.ToBase64String(cng.Key.Export(CngKeyBlobFormat.EccPrivateBlob));
                var publicKeyBytes = cng.Key.Export(CngKeyBlobFormat.EccPublicBlob);
                var derPublicKey = Convert.ToBase64String(CngEcPublicBlobToDerAns1(publicKeyBytes));

                Console.WriteLine($".NET client private key:");
                Console.WriteLine(privateKey);
                Console.WriteLine();

                Console.WriteLine($"Target365 public key:");
                Console.WriteLine(derPublicKey);
                Console.WriteLine();
            }
        }

        public static byte[] CngEcPublicBlobToDerAns1(byte[] cngEcPublicBlob)
        {
            var secp256r1Prefix = Convert.FromBase64String("MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE");
            var rawKey = cngEcPublicBlob.Skip(8);
            return secp256r1Prefix.Concat(rawKey).ToArray();
        }
    }
}

```

## Setup
### Target365Client
```C#
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Target365.Sdk;

// ONLY FOR .NET FRAMEWORK: Set up Service Point Manager for high performance
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
    SmsConfirmation = true,
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

## One-click transactions

### One-time transaction
This example sets up a simple one-time transaction for one-click. After creation you can redirect the end-user to the one-click landing page by redirecting to http://betal.strex.no/{YOUR-ACCOUNT-ID}/{YOUR-TRANSACTION-ID} for PROD and http://strex-test.target365.io/{YOUR-ACCOUNT-ID}/{YOUR-TRANSACTION-ID} for TEST-environment.
![one-time sequence](https://github.com/Target365/sdk-for-net/raw/master/oneclick-simple-transaction-flow.png "One-time sequence diagram")

```C#
var transaction = new StrexTransaction
{
    TransactionId = transactionId,
    ShortNumber = "2002",
    MerchantId = "YOUR_MERCHANT_ID",
    Price = 1,
    ServiceCode = ServiceCodes.NonCommercialDonation,
    InvoiceText = "Donation test"
};

transaction.Properties["RedirectUrl"] = "https://your-return-url.com?id=" + transactionId;
await serviceClient.CreateStrexTransactionAsync(transaction);

// TODO: Redirect end-user to one-click landing page
```
### Setup subscription transaction
This example sets up a subscription transaction for one-click. After creation you can redirect the end-user to the one-click landing page by redirecting to http://betal.strex.no/{YOUR-ACCOUNT-ID}/{YOUR-TRANSACTION-ID} for PROD and http://strex-test.target365.io/{YOUR-ACCOUNT-ID}/{YOUR-TRANSACTION-ID} for TEST-environment.
![subscription sequence](https://github.com/Target365/sdk-for-net/raw/master/oneclick-subscription-flow.png "Subscription sequence diagram")
```C#
var transaction = new StrexTransaction
{
    TransactionId = transactionId,
    ShortNumber = "2002",
    MerchantId = "YOUR_MERCHANT_ID",
    Price = 1,
    ServiceCode = ServiceCodes.NonCommercialDonation,
    InvoiceText = "Donation test"
};

transaction.Properties["Recurring"] = true;
transaction.Properties["RedirectUrl"] = "https://your-return-url.com?id=" + transactionId;
await serviceClient.CreateStrexTransactionAsync(transaction);

// TODO: Redirect end-user to one-click landing page
```
### Recurring transaction
This example sets up a recurring transaction for one-click. After creation you can immediately get the transaction to get the status code - the server will wait up to 20 seconds for the async transaction to complete.
![Recurring sequence](https://github.com/Target365/sdk-for-net/raw/master/oneclick-recurring-flow.png "Recurring sequence diagram")
```C#
var transaction = new StrexTransaction
{
    TransactionId = transactionId,
    Recipient = "RECIPIENT_FROM_SUBSCRIPTION_TRANSACTION"
    ShortNumber = "2002",
    MerchantId = "YOUR_MERCHANT_ID",
    Price = 1,
    ServiceCode = ServiceCodes.NonCommercialDonation,
    InvoiceText = "Donation test"
};

await serviceClient.CreateStrexTransactionAsync(transaction);
transaction = await serviceClient.GetStrexTransactionAsync(transactionId);

// TODO: Check transaction.StatusCode
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

## Forwards

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
This example shows how delivery reports (DLR) are forwarded to the outmessage DeliveryReportUrl. All DLR forwards expect a response with status code 200 (OK). If the request times out or response status code differs the forward will be retried 10 times with exponentially longer intervals for about 15 hours.
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
### DLR status codes
Delivery reports contains two status codes, one overall called `StatusCode` and one detailed called `DetailedStatusCode`.

#### StatusCode values
|Value|Description|
|:---|:---|
|Queued|Message is queued|
|Sent|Message has been sent|
|Failed|Message has failed|
|Ok|message has been delivered/billed|
|Reversed|Message billing has been reversed|

#### DetailedStatusCode values
|Value|Description|
|:---|:---|
|None|Message has no status|
|Delivered|Message is delivered to destination|
|Expired|Message validity period has expired|
|Undelivered|Message is undeliverable|
|UnknownError|Unknown error|
|Rejected|Message has been rejected|
|UnknownSubscriber|Unknown subscriber|
|SubscriberUnavailable|Subscriber unavailable|
|SubscriberBarred|Subscriber barred|
|InsufficientFunds|Insufficient funds|
|RegistrationRequired|Registration required|
|UnknownAge|Unknown age|
|DuplicateTransaction|Duplicate transaction|
|SubscriberLimitExceeded|Subscriber limit exceeded|
|MaxPinRetry|Max pin retry reached|
|InvalidAmount|Invalid amount|
|OneTimePasswordExpired|One-time password expired|
|OneTimePasswordFailed|One-time password failed|
|SubscriberTooYoung|Subscriber too young|
|TimeoutError|Timeout error|
