# Epsagon Instrumentation for .NET

This package provides instrumentation for AWS Lambda functions writen in .NET 
for collection of distributed tracing and performence monitoring.

## How to install

Using .NET CLI:

```bash
$ dotnet add package Epsagon.Dotnet.Lambda
```

Using [PackageReference](https://docs.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files) in a `*.csproj` file:

```xml
<PackageReference Include="Epsagon.Dotnet.Lambda" Version="*">
```

## Getting Started

* Set the following environment variables:
    * `EPSAGON_TOKEN` - Epsagon's token, can be found in the [Dashboard](https://dashboard.epsagon.com/)
    * `EPSAGON_APP_NAME` - Name for the application of this function (optional)
* Generate a new AWS Lambda Function project ([For](https://github.com/aws/aws-lambda-dotnet#amazonlambdatools) more info)
* Add `Epsagon.Dotnet.Lambda` package to your project
* Modify your Lambda Function Handler (usually found in `Function.cs`) like so:

```csharp
public class Function : LambdaHandler<string, string>
{
    public override string HandlerFunction(string input, ILambdaContext context)
    {
        return "Hello from Epsagon!";
    }
}
```

* Change the `function-handler` in your project's `aws-lambda-tools-defaults.json` to be `EpsagonEnabledHandler` (see [demo]() for more info)
* And that's it!

## Copyright

Provided under the MIT license. See LICENSE for details.

Copyright 2019, Epsagon
