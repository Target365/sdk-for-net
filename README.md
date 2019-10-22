## Target365 SDK for .NET
[![License](https://img.shields.io/github/license/Target365/sdk-for-net.svg?style=flat)](https://opensource.org/licenses/MIT)

### Getting started
To get started please send us an email at <support@target365.no> containing your EC public key in DER(ANS.1) format.
See our [C# User Guide](USERGUIDE.md) on how to generate your private-public key pair in C#.

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
Our production environment is a mix of per-tenant isolated environments and a shared common environment. Contact <support@target365.no> if you're interested in an isolated per-tenant environment.

#### Url: https://shared.target365.io/

### Authors and maintainers
Target365 (<support@target365.no>)

### Issues / Bugs / Questions
Please feel free to raise an issue against this repository if you have any questions or problems.

### Contributing
New contributors to this project are welcome. If you are interested in contributing please
send an email to support@target365.no.

### License
This library is released under the MIT license.
