## ![Strex](https://github.com/Target365/sdk-for-php/raw/master/strex.png "Strex")
Strex AS is a Norwegian payment and SMS gateway (Strex Connect) provider. Strex withholds an e-money license and processes more than 70 million transactions every year. Strex has more than 4.2 mill customers in Norway and are owned by the Norwegian mobile network operators (Telenor, Telia and Ice). Strex Connect is based on the Target365 marketing and communication platform. 

## Target365 SDK for .NET
[![License](https://img.shields.io/github/license/Target365/sdk-for-net.svg?style=flat)](https://opensource.org/licenses/MIT)

### Getting started
To get started, please click here: https://strex.no/strex-connect#Prispakker and register your organisation. 
For the SDK please send us an email at <sdk@strex.no> containing your EC public key in DER(ANS.1) format.

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

### NuGet
```
PM> Install-Package Target365.Sdk
```

### .NET CLI
```
> dotnet add package Target365.Sdk
```
[![NuGet](https://buildstats.info/nuget/target365.sdk)](https://www.nuget.org/packages/Target365.Sdk)

### Test Environment
Our test-environment acts as a sandbox that simulates the real API as closely as possible. This can be used to get familiar with the service before going to production. Please be ware that the simulation isn't perfect and must not be taken to have 100% fidelity.

#### Url: https://test.target365.io/

### Production Environment
Our production environment is a mix of per-tenant isolated environments and a shared common environment. Contact <sdk@strex.no> if you're interested in an isolated per-tenant environment.

#### Url: https://shared.target365.io/

### Authors and maintainers
Target365 (<sdk@strex.no>)

### Issues / Bugs / Questions
Please feel free to raise an issue against this repository if you have any questions or problems.

### Contributing
New contributors to this project are welcome. If you are interested in contributing please
send an email to sdk@strex.no.

### License
This library is released under the MIT license.
