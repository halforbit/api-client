using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Halforbit.ApiClient.Tests
{
    public class C
    {
        public C()
        {
            X = new Random().Next();
        }

        public int X { get; }
    }

    public class ApiClientIntegrationTests
    {
        static readonly Request _request = Request.Create(baseUrl: "https://reqres.in/api");

        //private static readonly string UsersResponse =
        //    @"{""page"":1,""per_page"":6,""total"":12,""total_pages"":2,""data"":[{""id"":1,""email"":""george.bluth@reqres.in"",""first_name"":""George"",""last_name"":""Bluth"",""avatar"":""https://reqres.in/img/faces/1-image.jpg""},{""id"":2,""email"":""janet.weaver@reqres.in"",""first_name"":""Janet"",""last_name"":""Weaver"",""avatar"":""https://reqres.in/img/faces/2-image.jpg""},{""id"":3,""email"":""emma.wong@reqres.in"",""first_name"":""Emma"",""last_name"":""Wong"",""avatar"":""https://reqres.in/img/faces/3-image.jpg""},{""id"":4,""email"":""eve.holt@reqres.in"",""first_name"":""Eve"",""last_name"":""Holt"",""avatar"":""https://reqres.in/img/faces/4-image.jpg""},{""id"":5,""email"":""charles.morris@reqres.in"",""first_name"":""Charles"",""last_name"":""Morris"",""avatar"":""https://reqres.in/img/faces/5-image.jpg""},{""id"":6,""email"":""tracey.ramos@reqres.in"",""first_name"":""Tracey"",""last_name"":""Ramos"",""avatar"":""https://reqres.in/img/faces/6-image.jpg""}],""support"":{""url"":""https://reqres.in/#support-heading"",""text"":""To keep ReqRes free, contributions towards server costs are appreciated!""}}";

        //[Fact, Trait("Type", "Integration")]
        //public async Task ListUsers()
        //{
        //    var response = await _request.GetAsync("users");
            
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    Assert.Equal("application/json", response.ContentType.MediaType);

        //    Assert.Equal("utf-8", response.ContentType.CharSet);

        //    var result = response.Content<JObject>();

        //    Assert.Equal(
        //        UsersResponse,
        //        JsonConvert.SerializeObject(result));

        //    Assert.Equal(
        //        "https://reqres.in/api/users",
        //        response.RequestedUrl);
        //}

        // TODO: Reimplement for System.Text.Json

        //[Fact, Trait("Type", "Integration")]
        //public async Task ListUsers_MapContent()
        //{
        //    var response = await _request.GetAsync("users");

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    Assert.Equal("application/json", response.ContentType.MediaType);

        //    Assert.Equal("utf-8", response.ContentType.CharSet);

        //    var result = response.MapContent(c => c["data"]
        //        .Select(e => new
        //        {
        //            Id = e["id"],

        //            FirstName = e["first_name"],

        //            LastName = e["last_name"],

        //            Avatar = e["avatar"]
        //        })
        //        .ToList());

        //    Assert.Equal(
        //        @"[{""Id"":1,""FirstName"":""George"",""LastName"":""Bluth"",""Avatar"":""https://reqres.in/img/faces/1-image.jpg""},{""Id"":2,""FirstName"":""Janet"",""LastName"":""Weaver"",""Avatar"":""https://reqres.in/img/faces/2-image.jpg""},{""Id"":3,""FirstName"":""Emma"",""LastName"":""Wong"",""Avatar"":""https://reqres.in/img/faces/3-image.jpg""},{""Id"":4,""FirstName"":""Eve"",""LastName"":""Holt"",""Avatar"":""https://reqres.in/img/faces/4-image.jpg""},{""Id"":5,""FirstName"":""Charles"",""LastName"":""Morris"",""Avatar"":""https://reqres.in/img/faces/5-image.jpg""},{""Id"":6,""FirstName"":""Tracey"",""LastName"":""Ramos"",""Avatar"":""https://reqres.in/img/faces/6-image.jpg""}]",
        //        JsonConvert.SerializeObject(result));

        //    Assert.Equal(
        //        "https://reqres.in/api/users",
        //        response.RequestedUrl);
        //}

        //[Fact, Trait("Type", "Integration")]
        //public async Task ListUsers_BearerToken()
        //{
        //    var authorizationCalled = false;

        //    async Task<IAuthorizationToken> Authorize()
        //    {
        //        await Task.Delay(0);

        //        authorizationCalled = true;

        //        return new AuthorizationToken("abc123", DateTime.UtcNow.AddDays(1));
        //    }

        //    var response = await _request
        //        .BearerTokenAuthorization(Authorize)
        //        .GetAsync("users");

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    Assert.Equal("application/json", response.ContentType.MediaType);

        //    Assert.Equal("utf-8", response.ContentType.CharSet);

        //    var result = response.Content<JObject>();

        //    Assert.Equal(
        //        UsersResponse,
        //        JsonConvert.SerializeObject(result));

        //    Assert.True(authorizationCalled);

        //    Assert.Equal(
        //        "Bearer abc123",
        //        response.Request.Headers["Authorization"]);

        //    Assert.Equal(
        //        "https://reqres.in/api/users",
        //        response.RequestedUrl);
        //}

        //[Fact, Trait("Type", "Integration")]
        //public async Task ListUsers_Cookie()
        //{
        //    var authorizationCalled = false;

        //    async Task<IAuthorizationToken> Authorize()
        //    {
        //        await Task.Delay(0);

        //        authorizationCalled = true;

        //        return new AuthorizationToken("abc123", DateTime.UtcNow.AddDays(1));
        //    }

        //    var response = await _request
        //        .CookieAuthorization(Authorize)
        //        .GetAsync("users");

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    Assert.Equal("application/json", response.ContentType.MediaType);

        //    Assert.Equal("utf-8", response.ContentType.CharSet);

        //    var result = response.Content<JObject>();

        //    Assert.Equal(
        //        UsersResponse,
        //        JsonConvert.SerializeObject(result));

        //    Assert.True(authorizationCalled);

        //    Assert.Equal(
        //        "abc123",
        //        response.Request.Headers["Cookie"]);

        //    Assert.Equal(
        //        "https://reqres.in/api/users",
        //        response.RequestedUrl);
        //}

        //[Fact, Trait("Type", "Integration")]
        //public async Task ListUsers_BeforeRequestHandler()
        //{
        //    var handlerCalled = false;

        //    var response = await _request
        //        .BeforeRequest((q, u) =>
        //        {
        //            Assert.Equal("users", q.Resource);

        //            handlerCalled = true;

        //            return Task.FromResult((q, u));
        //        })
        //        .GetAsync("users");

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    Assert.Equal("application/json", response.ContentType.MediaType);

        //    Assert.Equal("utf-8", response.ContentType.CharSet);

        //    var result = response.Content<JObject>();

        //    Assert.Equal(
        //        UsersResponse,
        //        JsonConvert.SerializeObject(result));

        //    Assert.Equal(
        //        "https://reqres.in/api/users",
        //        response.RequestedUrl);

        //    Assert.True(handlerCalled);
        //}

        //[Fact, Trait("Type", "Integration")]
        //public async Task ListUsers_AfterResponseHandler()
        //{
        //    var handlerCalled = false;

        //    var response = await _request
        //        .AfterResponse(r =>
        //        {
        //            Assert.Equal("users", r.Request.Resource);

        //            Assert.Equal(HttpStatusCode.OK, r.StatusCode);

        //            handlerCalled = true;

        //            return Task.FromResult(r);
        //        })
        //        .GetAsync("users");

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    Assert.Equal("application/json", response.ContentType.MediaType);

        //    Assert.Equal("utf-8", response.ContentType.CharSet);

        //    var result = response.Content<JObject>();

        //    Assert.Equal(
        //        UsersResponse,
        //        JsonConvert.SerializeObject(result));

        //    Assert.Equal(
        //        "https://reqres.in/api/users",
        //        response.RequestedUrl);

        //    Assert.True(handlerCalled);
        //}

        //[Fact, Trait("Type", "Integration")]
        //public async Task SingleUser()
        //{
        //    var response = await _request
        //        .RouteValues(new { UserId = 2 })
        //        .GetAsync("users/{UserId}");

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    Assert.Equal("application/json", response.ContentType.MediaType);

        //    Assert.Equal("utf-8", response.ContentType.CharSet);

        //    var result = response.Content<JObject>();

        //    Assert.Equal(
        //        @"{""data"":{""id"":2,""email"":""janet.weaver@reqres.in"",""first_name"":""Janet"",""last_name"":""Weaver"",""avatar"":""https://reqres.in/img/faces/2-image.jpg""},""support"":{""url"":""https://reqres.in/#support-heading"",""text"":""To keep ReqRes free, contributions towards server costs are appreciated!""}}",
        //        JsonConvert.SerializeObject(result));

        //    Assert.Equal(
        //        "https://reqres.in/api/users/2",
        //        response.RequestedUrl);
        //}

        [Fact, Trait("Type", "Integration")]
        public async Task SingleUserNotFound_Exception()
        {
            await Assert.ThrowsAsync<ApiRequestException>(async () => await _request
                .RouteValues(new { UserId = 23 })
                .GetAsync("users/{UserId}"));
        }

        [Fact, Trait("Type", "Integration")]
        public async Task SingleUserNotFound_Expected()
        {
            var response = await _request
                .AllowNotFound()
                .RouteValues(new { UserId = 23 })
                .GetAsync("users/{UserId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            Assert.Equal(
                "https://reqres.in/api/users/23",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task CreateUser()
        {
            var response = await _request
                .Body(new
                {
                    name = "morpheus",

                    job = "leader"
                })
                .PostAsync("users");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            Assert.Equal("application/json", response.ContentType.MediaType);

            Assert.Equal("utf-8", response.ContentType.CharSet);

            var result = response.Content<JsonObject>();

            Assert.StartsWith(
                @"{""name"":""morpheus"",""job"":""leader"",""id"":""",
                System.Text.Json.JsonSerializer.Serialize(result));

            Assert.Equal(
                "https://reqres.in/api/users",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task UpdateUserPut()
        {
            var response = await _request
                .RouteValues(new { UserId = 2 })
                .Body(new
                {
                    name = "morpheus",

                    job = "zion resident"
                })
                .PutAsync("users/{UserId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json", response.ContentType.MediaType);

            Assert.Equal("utf-8", response.ContentType.CharSet);

            var result = response.Content<JsonObject>();

            Assert.StartsWith(
                @"{""name"":""morpheus"",""job"":""zion resident"",""updatedAt"":""",
                System.Text.Json.JsonSerializer.Serialize(result));

            Assert.Equal(
                "https://reqres.in/api/users/2",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task UpdateUserPatch()
        {
            var response = await _request
                .RouteValues(new { UserId = 2 })
                .Body(new
                {
                    name = "morpheus",

                    job = "zion resident"
                })
                .PatchAsync("users/{UserId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json", response.ContentType.MediaType);

            Assert.Equal("utf-8", response.ContentType.CharSet);

            var result = response.Content<JsonObject>();

            Assert.StartsWith(
                @"{""name"":""morpheus"",""job"":""zion resident"",""updatedAt"":""",
                System.Text.Json.JsonSerializer.Serialize(result));

            Assert.Equal(
                "https://reqres.in/api/users/2",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task DeleteUser()
        {
            var response = await _request
                .RouteValues(new { UserId = 2 })
                .DeleteAsync("users/{UserId}");

            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);

            Assert.Null(response.ContentType);

            Assert.Equal(
                "https://reqres.in/api/users/2",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task GetImage()
        {
            var request = Request.Create("https://reqres.in");

            var response = await request
                .GetAsync("img/faces/1-image.jpg");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("image/jpeg", response.ContentType.MediaType);

            Assert.Equal(4324, response.ByteContent().Length);

            Assert.Equal(
                "https://reqres.in/img/faces/1-image.jpg",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task GetImageHead()
        {
            var request = Request.Create("https://reqres.in");

            var response = await request
                .HeadAsync("img/faces/1-image.jpg");
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var date = response.Headers.Date();

            Assert.True(date.HasValue);

            Assert.Equal(
                "https://reqres.in/img/faces/1-image.jpg",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task GetServerUnreachable()
        {
            var request = Request.Create("https://doesnt.exist")
                .AllowAnyStatusCode();
            
            var response = await request.GetAsync("something/somewhere");

            Assert.Equal(0, (int)response.StatusCode);

            Assert.Null(response.ContentType);

            Assert.Null(response.Content);

            Assert.False(string.IsNullOrWhiteSpace(response.ErrorMessage));

            Assert.NotNull(response.Exception);

            Assert.Equal(
                "https://doesnt.exist/something/somewhere",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task GetRequestTimeout()
        {
            var request = Request.Create("https://postman-echo.com")
                .AllowAnyStatusCode()
                .Timeout(TimeSpan.FromMilliseconds(50));

            var response = await request.GetAsync("delay/60");

            Assert.Equal(0, (int)response.StatusCode);

            Assert.Null(response.ContentType);

            Assert.Null(response.Content);

            Assert.False(string.IsNullOrWhiteSpace(response.ErrorMessage));

            Assert.Null(response.Exception);

            Assert.Equal(
                "https://postman-echo.com/delay/60",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task GetRetryTimeout()
        {
            int retryCount = 0;

            var request = Request.Create("https://postman-echo.com")
                .Retry(retryCount: 4, retryOnTimeout: true)
                .BeforeRetry((r, u, s, c) =>
                {
                    retryCount++;

                    return Task.FromResult((r, u, true));
                })
                .Timeout(TimeSpan.FromMilliseconds(50));

            var timer = Stopwatch.StartNew();

            try
            {
                var response = await request.GetAsync("delay/60");
            }
            catch(ApiRequestException)
            {
            }
            finally
            {
                var elapsed = timer.Elapsed;

                Assert.True(elapsed >= TimeSpan.FromSeconds(0 + 1 + 2 + 4));
                
                Assert.True(elapsed < TimeSpan.FromSeconds(0 + 1 + 2 + 4 + 8));

                Assert.Equal(4, retryCount);
            }
        }

        [Fact, Trait("Type", "Integration")]
        public async Task GetRetryInternalServerError()
        {
            int retryCount = 0;

            var request = Request.Create("https://httpstat.us")
                .Retry(retryCount: 4)
                .BeforeRetry((r, u, s, c) =>
                {
                    retryCount++;

                    return Task.FromResult((r, u, true));
                });

            var timer = Stopwatch.StartNew();

            await Assert.ThrowsAsync<ApiRequestException>(async () => await request.GetAsync("500"));

            var elapsed = timer.Elapsed;

            Assert.True(elapsed >= TimeSpan.FromSeconds(0 + 2 + 4 + 8));

            Assert.True(elapsed < TimeSpan.FromSeconds(0 + 2 + 4 + 8 + 16));

            Assert.Equal(4, retryCount);
        }
        
        //[Fact, Trait("Type", "Integration")]
        //public async Task GetRequestCancelled()
        //{
        //    var request = Request.Create("https://postman-echo.com")
        //        .AllowAnyStatusCode();

        //    var timer = Stopwatch.StartNew();
        //    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            
        //    await Assert.ThrowsAsync<OperationCanceledException>(async () => await request.GetAsync("delay/60", cts.Token));
            
        //    timer.Stop();
        //    var elapsed = timer.Elapsed;
            
        //    Assert.True(elapsed >= TimeSpan.FromSeconds(1));

        //    Assert.True(elapsed < TimeSpan.FromSeconds(2));
        //}
    }
}
