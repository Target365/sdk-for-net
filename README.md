## Target365 SDK for .NET
[![License](https://img.shields.io/github/license/Target365/sdk-for-node.svg?style=flat)](https://opensource.org/licenses/MIT)

### Getting started
To get started, please click here: https://strex.no/strex-connect#Prispakker and register your organisation. 

After registration you can activate the SDK by logging in to Strex Connect and create a key here:  
https://www.strexconnect.no/admin/public-key 
Select ".NET" as your SDK. Copy and store the "Private key-string" safe and encrypted (we recommend EAS-encryption). We only store the public-key.
Set the expiry date, and optionally add an e-mail address so we can warn you when the key is about to expire.

You can also generate your own public/private key pair using openssl and import this into Strex Connect.
For more details on using the SDK we strongly suggest you check out our [.NET User Guide](USERGUIDE.md).

### NuGet
```
PM> Install-Package Target365.Sdk
```

### .NET CLI
```
> dotnet add package Target365.Sdk
```
[![NuGet](https://badge.fury.io/nu/Target365.Sdk.svg)](https://www.nuget.org/packages/Target365.Sdk)

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
New contributors to this project are welcome. If you are interested in contributing please create a pull request and we will review it.

### License
This library is released under the MIT license.

### About Target365
![Target365](https://github.com/Target365/sdk-for-net/raw/master/target365.png "Target365 AS")

Target365 is a Norwegian CPaaS provider that delivers mobile communication via A2P SMS, RCS and integrated payment solutions at scale.

### About Strex
![Strex](https://github.com/Target365/sdk-for-net/raw/master/strex.png "Strex AS")

Strex AS is a Norwegian payment and SMS gateway provider. Strex withholds an e-money license and processes more than 70 million transactions every year. Strex has more than 4.2 mill customers in Norway and are owned by the Norwegian mobile network operators (Telenor, Telia and Ice). Strex Connect is based on the Target365 marketing and communication platform.

### About Strex Connect
Strex Connect is based on the Target365 CPaaS platform and provides a comprehensive suite of payment and communication services.