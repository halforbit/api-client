﻿# Halforbit API Client

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE) &nbsp;[![Build status](https://ci.appveyor.com/api/projects/status/r96ru6eh9rk9s7n4?svg=true)](https://ci.appveyor.com/project/halforbit/api-client) &nbsp;[![Nuget Package](https://img.shields.io/nuget/v/Halforbit.ApiClient.svg)](#nuget-packages)

Easily define and execute web requests with strong request and response types, and a simple fluent interface.

## Features

- Simple, fluent interface
- Natively `async`, easily parallelizable
- Templated routes
- Built-in support for authorization (basic, bearer token, and cookie)
- Built-in support for failure retry
- Dependency injection and unit testing friendly
- Cross platform (.NET Standard 2.0) compatability

## Getting Started

Install the `Halforbit.ApiClient` NuGet package:

```powershell
Install-Package Halforbit.ApiClient
```
   
## Simple Examples

```csharp
// Create a base request
var request = Request.Default.BaseUrl("https://alfa.bravo");

// GET some Users from a JSON array
var response = (await request.GetAsync("users")).Content<IReadOnlyList<User>>();

// POST a new person
var response = await request
    .Body(new
    {
        Name = "John Doe",
        Job = "Farmer"
    })
    .PostAsync("users");

// GET an image
var response = await request.GetAsync("charlie/delta.jpg");
var imageBytes = response.ByteArrayContent(); // image data
var imageType = response.ContentType.MediaType; // e.g. `image/jpeg`
```

## Creating Requests

We provide a chained, fluent interface to construct `Request` objects. Each instance of `Request` is immutable and can be safely reused and built upon. You can create partial requests and compose them together in a way that is thread-safe:

```csharp
// Here both requests will have the base URL and header from baseRequest.

var baseRequest = Request.Default
    .BaseUrl("https://alfa.bravo")
    .Header("x-alfa", "bravo");

var responseA = await baseRequest.GetAsync("people");

var responseB = await baseRequest
    .FormBody(("first_name", "John"), ("last_name", "Doe"))
    .PostAsync("people");
```

### Route Templating

Request routes often have route values to be filled in. Optionally, you can specify named route values with placeholders in the resource path, as well as in the base URL:

```csharp
var request = Request.Create("https://alfa.bravo/{AccountId}");

var response = request
    .RouteValues(new
    {
        AccountId = 1234,
        Category = "vehicles",
        VehicleId = 2345
    })
    .GetAsync("categories/{Category}/images/{VehicleId}");
```

You can also just use literal string values, or use string interpolation:

```csharp
var request = Request.Default.BaseUrl($"https://alfa.bravo/{accountId}");
```

### Route Values, Query Values, and Headers

Route values, query string values, and headers can be specified using a single value, tuple values, or a dictionary of values:

```csharp
// This will have a query string of: 
//   ?alfa=bravo&charlie=delta&echo=foxtrot&golf=hotel
request
    .QueryValue("alfa", "bravo")
    .QueryValues(("charlie", "delta"), ("echo", "foxtrot"))
    .QueryValues(new Dictionary<string, string> { ["golf"] = "hotel" })
```

### Request Bodies

Several methods are provided to easily specify request bodies of various kinds:

```csharp
// Plain text body.
request.TextBody("hello, world!");

// Form body. This can be tuple values or a dictionary of values.
request.FormBody(("first_name", "John"), ("last_name", "Doe"));

// Object body. This can be any serializable object, including classes, 
// anonymous objects, JObject, etc. The default object serialization 
// technique is JSON.
request.Body(new { Name = "John Doe" });

// Byte array body.
request.Body(new byte[] { 1, 2, 3 });

// Stream body. Be sure to close / dispose of your stream properly.
using(var stream = File.OpenRead("body.txt"))
{
    await request
        .Body(stream, contentType: "text/plain; charset: utf-8")
        .PostAsync();
}
```

## Handling Responses

```csharp
var plainText = response.TextContent();

var bytes = response.ByteContent();

var deserializedAsClass = response.Content<Person>();

var deserializedAsJToken = response.Content<JToken>();

// Map a JToken to a type.
var mappedJToken = response.MapContent(c => new Person(c["name"]));

// Map an array of JTokens to an IReadOnlyList<> of your favorite type.
var mappedJArray = response.MapContentArray(e => new Person(e["name"]));
```

## Automatic Retry

You can opt in to automatic retry by specifying the maximum number of times a transient failure should be retried:

```csharp
// Default is 5 retries
request.Retry();

// Specify a retry count
request.Retry(retryCount: 10);
```

The first retry will be immediate, and the interval between subsequent retries is exponential, e.g. 1 sec, 2 sec, 4 sec, 8 sec, etc.

## Dependency Injection and Testing

Behind the scenes, requests use an `IRequestClient` to execute and retrieve a response. When you create a request, you can optionally specify an `IRequestClient`:

```csharp
// Use the IRequestClient instance we're providing
var request = Request.Default
    .RequestClient(requestClient)
    .BaseUrl("https://alfa.bravo");
```

If you do not provide a request client, a static instance, `RequestClient.Instance`, will be used automatically:

```csharp
// Use RequestClient.Instance automatically
var request = Request.Default.BaseUrl("https://alfa.bravo");
```

### Dependency Injection

If you wish to use constructor dependency injection and unit testing, you should register a singleton instance of `RequestClient`:

```csharp
// Using Microsoft.Exensions.DependencyInjection:
services.AddSingleton<IRequestClient, RequestClient>();

// Using Autofac:
builder.RegisterType<RequestClient>().AsImplementedInterfaces().InstancePerLifetimeScope();
```

You can then receive an `IRequestClient` in your constructors and use it when creating requests:

```csharp
class MyClient
{
    readonly Request _request;

    public MyClient(IRequestClient requestClient)
    {
        _request = Request.Default
            .RequestClient(requestClient)
            .BaseUrl("https://alfa.bravo");
    }

    public async Task<string> GetAThing()
    {
        return await _request.GetAsync("things/123").TextContent();
    }
}
```
Here we make a base request and store it in a private field for use by member methods.

### Unit Testing

`IRequestClient` contains only one method, `Execute(Request)`, to mock for a unit test. You can use the mocking framework of your choice to simulate responses from this method and verify the correctness of calls.

## Authorization

Several authorization strategies are supported.

### Basic Authorization

```csharp
request.BasicAuthorization(
    username: "probably_dont",
    password: "hardcode_this");
```

### Bearer Token Authorization

You can specify a lambda for retrieving a bearer token. This lambda should produce an `IAuthorizationToken`:

```csharp
request.BearerTokenAuthorization(
    async () => await _myAuthorizationClient.Authorize());
```

After the bearer token is retrieved, it is cached for subsequent requests to use. If the token expires, or a request returns `401 Unauthorized`, a new bearer token will be retrieved, and the request will be repeated.

How you get a bearer token will vary, but here is an example of how you might do so:

```csharp
public class MyAuthorizationClient
{
    readonly Request _request;

    public MyAuthorizationClient(IRequestClient requestClient)
    {
        _request = Request.Default
            .RequestClient(requestClient)
            .BaseUrl("https://alfa.bravo");
    }

    public async Task<IAuthorizationToken> Authorize()
    {
        return (await _request
            .FormBody(
                ("username", "probably_dont"),
                ("password", "hardcode_this"))
            .PostAsync("token"))
            .MapContent(c => new AuthorizationToken(
                content: (string)c["access_token"],
                expireTime: DateTime.UtcNow.AddSeconds((int)c["expires_in"])));
    }
}
```

If the service you are authorizing against includes the base URL authorized requests should use in its response, you can use `.BearerTokenAuthorizationWithBaseUrl()`:

```csharp
request.BearerTokenAuthorizationWithBaseUrl(async () => 
{
    var authResponse = await _myAuthorizationClient.Authorize();

    return (authResponse.BearerToken, authResponse.BaseUrl);
});
```

### Cookie Authorization

Cookie authorization is similar to bearer token authorization. Just provide a lambda to retrieve the cookie when it is needed:

```csharp
request.CookieAuthorization(
    async () => await _myAuthorizationClient.Authorize());
```

## Roadmap

Some features that are planned for implementation:
- `Content-Encoding`, gzip/deflate support for compressed requests and responses.
- Support multipart requests.
- More robust support for e.g. `Accept`, `Accept-Charset`, `Range`, `206 Partial Content`, `X-Content-Type-Options`.
- Follow redirects, allow distinction of requested vs redirected url

<a name="nuget-packages"></a>
## NuGet Packages

The following NuGet package is provided:

[`Halforbit.ApiClient`](https://www.nuget.org/packages/Halforbit.ApiClient)

## License 

Data Stores is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
