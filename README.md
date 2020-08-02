<p align="center">
  <a href="https://epsagon.com" target="_blank" align="center">
    <img src="https://cdn2.hubspot.net/hubfs/4636301/Positive%20RGB_Logo%20Horizontal%20-01.svg" width="300">
  </a>
  <br />
</p>

[![NuGet](https://img.shields.io/nuget/v/Epsagon.Dotnet.Core.svg)](https://www.nuget.org/packages/Epsagon.Dotnet.Core)

# Epsagon Tracing for .NET

This package provides tracing to .NET applications for the collection of distributed tracing and performance metrics in [Epsagon](https://app.epsagon.com/?utm_source=github).


## Contents

- [Installation](#installation)
- [Frameworks](#frameworks)
- [Integrations](#integrations)
- [Configuration](#configuration)
- [Getting Help](#getting-help)
- [Opening Issues](#opening-issues)
- [License](#license)


## Installation

To install Epsagon, simply run:
```sh
dotnet add package Epsagon.Dotnet.Lambda
```

Or, using [PackageReference](https://docs.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files) in a `*.csproj` file, follow instructions [here](https://www.nuget.org/packages/Epsagon.Dotnet.Lambda/).

## Frameworks

The following frameworks are supported by Epsagon:

|Framework                               |Supported Version          |
|----------------------------------------|---------------------------|
|[ASP.NET MVC](#aspnet-mvc)              |`>=2.1`                    |
|[AWS Lambda](#aws-lambda)               |All                        |


### AWS Lambda

Tracing Lambda functions can be done in three methods:
1. Inherit from Epsagon's LambdaHandler Base Class.
2. Passing a callback.

- Set the following environment variables:
  - `EPSAGON_TOKEN` - Epsagon's token, can be found in the [Dashboard](https://app.epsagon.com/settings).
  - `EPSAGON_APP_NAME` - Name for the application of this function (optional).
- Generate a new AWS Lambda Function project ([For more info](https://github.com/aws/aws-lambda-dotnet#amazonlambdatools)).
- Add `Epsagon.Dotnet.Lambda` package to your project.


#### To Inherit from Epsagon's LambdaHandler Base Class (example for S3 trigger)

```csharp
public class Function : LambdaHandler<S3Event, string> // LambdaHandler<TEvent, TRes>
{
    public override string HandlerFunction(S3Event input, ILambdaContext context)
    {
        return "Hello from Epsagon!";
    }
}
```

Change the function-handler in your project's aws-lambda-tools-defaults.json to be EpsagonEnabledHandler.

#### Passing a callback

* Add a call to EpsagonBootstrap.Bootstrap() in the constructor of your Lambda.
* Invoke EpsagonHandler.Handle to instrument your function.

```csharp
public class FunctionClass {
    public FunctionClass() {
        EpsagonBootstrap.Bootstrap();
    }

    public string MyHandler(S3Event input, ILambdaContext context) {
        return EpsagonHandler.Handle(input, context, () => {
            // your code is here...
        });
    }

    // Can be async as well
    public Task<string> MyAsyncHandler(S3Event input, ILambdaContext context) {
        return EpsagonHandler.Handle(input, context, async () => {
            // your async code is here
        });
    }
}
```
### ASP.NET MVC

To trace ASP.NET MVC, a few simple actions should be taken:
- Add `Epsagon.Dotnet.Mvc` package to your project.
- Add a call to `EpsagonBootstrap.Bootstrap()` in the constructor of your startup class. for example:
```csharp
public Startup(IConfiguration configuration)
{
    EpsagonBootstrap.Bootstrap();
    Configuration = configuration;
}
```
- Add EpsagonMiddleware to your MVC project. for example:
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseEpsagon();
}
```
- Set the following environment variables:
  - `EPSAGON_TOKEN` - Epsagon's token, can be found in the [Dashboard](https://dashboard.epsagon.com/settings).
  - `EPSAGON_APP_NAME` - Name for the application of this function (optional).
  
## Integrations

Epsagon provides out-of-the-box instrumentation (tracing) for many popular frameworks and libraries.

|Library             |Supported Version          |
|--------------------|---------------------------|
|Elasticsearch       |`>=5.6`                    |
|MongoDB.Driver      |`>=2.4`                    |
|AWSSDK              |`>=3.0`                    |
|ASP.NET MVC         |`>=2.1`                    |

## Configuration

Advanced options can be configured as a parameter to the `Config` struct to the `WrapLambdaHandler` or as environment variables.

|Parameter             |Environment Variable          |Type   |Default      |Description                                                                        |
|----------------------|------------------------------|-------|-------------|-----------------------------------------------------------------------------------|
|Token                 |EPSAGON_TOKEN                 |String |-            |Epsagon account token                                                              |
|ApplicationName       |EPSAGON_APP_NAME              |String |-            |Application name that will be set for traces                                       |
|MetadataOnly          |EPSAGON_METADATA              |Boolean|`true`       |Whether to send only the metadata (`true`) or also the payloads (`false`)          |
|TraceCollectorURL     |-                             |String |-            |The address of the trace collector to send trace to                                |
|UseSSL                |                              |Boolean|`true`       |Whether to send the traces over HTTPS SSL or not                                   |
|IsEpsagonDisabled     |DISABLE_EPSAGON               |Boolean|`false`      |A flag to completely disable Epsagon (can be used for tests or locally)            |
|_                     |EPSAGON_DEBUG                 |Boolean|`false`      |Enable debug prints for troubleshooting                                            |


## Getting Help

If you have any issue around using the library or the product, please don't hesitate to:

* Use the [documentation](https://docs.epsagon.com).
* Use the help widget inside the product.
* Open an issue in GitHub.


## Opening Issues

If you encounter a bug with the Epsagon library for .NET, we want to hear about it.

When opening a new issue, please provide as much information about the environment:
* Library version, .NET runtime version, dependencies, etc.
* Snippet of the usage.
* A reproducible example can really help.

The GitHub issues are intended for bug reports and feature requests.
For help and questions about Epsagon, use the help widget inside the product.

## License

Provided under the MIT license. See LICENSE for details.

Copyright 2020, Epsagon
