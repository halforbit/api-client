# Halforbit API Client


An easy to use library for making requests over HTTP.

Available on NuGet: [Halforbit.ApiClient](https://www.nuget.org/packages/Halforbit.ApiClient)

[![Build status](https://ci.appveyor.com/api/projects/status/r96ru6eh9rk9s7n4?svg=true)](https://ci.appveyor.com/project/halforbit/api-client)


## Features

- Simple, fluent interface
- Natively `async`, easily parallelizable
- Templated routes
- Built-in support for authentication (basic, bearer token, and cookie)
- Built-in support for failure retry
- Dependency injection and unit testing friendly
- Cross platform (.NET Standard 2.0) compatability

## Simple Examples

```csharp
// Create a base request
var request = Request.Create(baseUrl: "https://alfa.bravo");

// GET some Users from a JSON array
var response = (await request.Get("users")).JsonContent<IReadOnlyList<User>>();

// POST a new person
var response = await request
    .JsonBody(new
    {
        Name = "John Doe",
        Job = "Farmer"
    })
    .Post("users");

// GET an image
var response = await request.Get("charlie/delta.jpg");
var imageBytes = response.ByteArrayContent(); // image data
var imageType = response.ContentType.MediaType; // e.g. `image/jpeg`
```

## Creating Requests

We provide a chained, fluent interface to construct `Request` objects. Each instance of `Request` is immutable and can be safely reused and built upon:

```csharp
// Here both requests will have the base URL and header from baseRequest.

var baseRequest = Request.Create(baseUrl: "https://alfa.bravo")
    .Header("x-alfa", "bravo");

var responseA = await baseRequest.Get("people");

var responseB = await baseRequest
    .FormBody(("first_name", "John"), ("last_name", "Doe"))
    .Post("people");
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
    .Get("categories/{Category}/images/{VehicleId}");
```

You can also just use literal string values, or use string interpolation:

```csharp
var request = Request.Create($"https://alfa.bravo/{accountId}");
```

### Route Values, Query Values, and Headers

Route values, query string values, and headers can be specified using a single value, tuple values, or a dictionary of values:

```csharp
// This will have a query string of:
// ?alfa=bravo&charlie=delta&echo=foxtrot&golf=hotel

var request = Request.Create()
    .QueryValue("alfa", "bravo")
    .QueryValues(("charlie", "delta"), ("echo", "foxtrot"))
    .QueryValues(new Dictionary<string, string> { ["golf"] = "hotel" })
```

### Request Bodies

Several methods are provided to easily specify request bodies of various kinds:

```csharp
// Plain text body.
var request = Request.Create().TextBody("hello, world!");

// JSON body. This can be any JSON-serializable object, including classes, 
// anonymous objects, JObject, etc.
var request = Request.Create().JsonBody(new { Name = "John Doe" });

// Form body. This can be tuple values or a dictionary of values.
var request = Request.Create().FormBody(("first_name", "John"), ("last_name", "Doe"));
```

## Handling Responses

```csharp
var plainText = response.TextContent();

var bytes = response.ByteContent();

var jsonAsClass = response.JsonContent<Person>();

var jsonAsJToken = response.JTokenContent();

// Map a JToken (usually a JObject or JArray) to a type.
var mappedJToken = response.MapJToken(j => new Person(j["name"]));

// Map an array of JTokens to an IReadOnlyList<> of your favorite type.
var mappedJArray = response.MapJArray(j => new Person(j["name"]));
```

## Authentication

Several authentication strategies are supported.

### Basic Authentication

```csharp
var request = Request.Create().BasicAuthentication(
    username: "probably_dont",
    password: "hardcode_this");
```

### Bearer Token Authentication

You can specify a lambda for retrieving a bearer token. This lambda should produce an `IAuthenticationToken`:

```csharp
var request = Request.Create().BearerTokenAuthentication(
    async () => await _myAuthenticationClient.Authenticate());
```

After the bearer token is retrieved, it is cached for subsequent requests to use. If the token expires, or a request returns `401 Unauthorized`, a new bearer token will be retrieved, and the request will be repeated.

How you get a bearer token will vary, but here is an example of how you might do so:

```csharp
public class MyAuthenticationClient
{
    readonly Request _request;

    public MyAuthenticationClient(IRequestClient requestClient)
    {
        _request = Request.Create("https://alfa.bravo", requestClient);
    }

    public async Task<IAuthenticationToken> Authenticate()
    {
        return (await _request
            .FormBody(
                ("username", "probably_dont"),
                ("password", "hardcode_this"))
            .Post("token"))
            .MapJToken(j => new AuthenticationToken(
                content: (string)j["access_token"],
                expireTime: DateTime.UtcNow.AddSeconds((int)j["expires_in"])));
    }
}
```

### Cookie Authentication

Cookie authentication is similar to bearer token authentication. Just provide a lambda to retrieve the cookie when it is needed:

```csharp
var request = Request.Create().CookieAuthentication(
    async () => await _myAuthenticationClient.Authenticate());
```

## Automatic Retry

You can opt in to automatic retry by specifying the maximum number of times a transient failure should be retried:

```csharp
var request = Request.Create().Retry(retryCount: 5);
```

The first retry will be immediate, and the interval between subsequent retries is exponential, e.g. 1 sec, 2 sec, 4 sec, 8 sec, etc.

## Dependency Injection and Testing

Behind the scenes, requests use an `IRequestClient` to execute and retrieve a response. When you `Create` a request, you can optionally specify an `IRequestClient`:

```csharp
// Use the IRequestClient instance we're providing
var request = Request.Create(
    baseUrl: "https://alfa.bravo",
    requestClient: requestClient);
```

The `requestClient` parameter is optional. If you do not provide it then a static instance, `RequestClient.Instance`, will be used automatically:

```csharp
// Use RequestClient.Instance automatically
var request = Request.Create(baseUrl: "https://alfa.bravo"); 
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
        _request = Request.Create(
            baseUrl: "https://alfa.bravo",
            requestClient: requestClient);
    }

    public async Task<string> GetAThing()
    {
        return await _request.Get("things/123").TextContent();
    }
}
```
Here we make a base request and store it in a private field for use by member methods.

### Unit Testing

`IRequestClient` contains only one method, `Execute(Request)`, to mock for a unit test. You can use the mocking framework of your choice to simulate responses from this method and verify the correctness of calls.

## Roadmap

Some features that are planned for implementation:
- `Content-Encoding`, gzip/deflate support for compressed requests and responses.
- Support multipart requests.
- More robust support for e.g. `Accept`, `Accept-Charset`, `Range`, `206 Partial Content`, `X-Content-Type-Options`.
