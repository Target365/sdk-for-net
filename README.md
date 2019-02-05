## Target365 SDK for .NET
[![License](https://img.shields.io/github/license/Target365/sdk-for-net.svg?style=flat)](https://opensource.org/licenses/MIT)

### Getting started
To get started please send us an email at <support@target365.no> containing your EC public key in DER(ANS.1) format.
If you want, you can generate your EC public/private key-pair here: <https://crypto-utils.com/>

### NuGet
```
PM> Install-Package Target365.Sdk
```

### .NET CLI
```
> dotnet add package Target365.Sdk
```
[![NuGet](https://buildstats.info/nuget/target365.sdk)](https://www.nuget.org/packages/Target365.Sdk)

### Generate EC public/private key-pair in C#
```C#
using System;
using System.Security.Cryptography;
...
public void Generate()
{
    var keyParams = new CngKeyCreationParameters
    {
        ExportPolicy = CngExportPolicies.AllowPlaintextExport,
        KeyUsage = CngKeyUsages.Decryption | CngKeyUsages.Signing
    };

    using (var cngKey = CngKey.Create(CngAlgorithm.ECDsaP256, null, keyParams))
    using (var cng = new ECDsaCng(cngKey))
    {
        var publicBytes = cng.Key.Export(CngKeyBlobFormat.EccPublicBlob);
        var privateBytes = cng.Key.Export(CngKeyBlobFormat.EccPrivateBlob);
        Console.WriteLine($"Private key: {Convert.ToBase64String(privateBytes)}");
        Console.WriteLine($"Public key: {Convert.ToBase64String(publicBytes)}");
    }
}
```

### Authors and maintainers
Target365 (<support@target365.no>)

### Issues / Bugs / Questions
Please feel free to raise an issue against this repository if you have any questions or problems.

### Contributing
New contributors to this project are welcome. If you are interested in contributing please
send an email to support@target365.no.

### License
This library is released under the MIT license.
