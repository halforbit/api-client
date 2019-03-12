using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Halforbit.ApiClient.Tests
{
    public class ApiClientIntegrationTests
    {
        static readonly Request _request = Request.Create(baseUrl: "https://reqres.in/api");

        [Fact, Trait("Type", "Integration")]
        public async Task ListUsers()
        {
            var response = await _request.Get("users");
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json", response.ContentType.MediaType);

            Assert.Equal("utf-8", response.ContentType.CharSet);

            var result = response.JsonContent<JObject>();

            Assert.Equal(
                @"{""page"":1,""per_page"":3,""total"":12,""total_pages"":4,""data"":[{""id"":1,""first_name"":""George"",""last_name"":""Bluth"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/calebogden/128.jpg""},{""id"":2,""first_name"":""Janet"",""last_name"":""Weaver"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/josephstein/128.jpg""},{""id"":3,""first_name"":""Emma"",""last_name"":""Wong"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/olegpogodaev/128.jpg""}]}",
                JsonConvert.SerializeObject(result));

            Assert.Equal(
                "https://reqres.in/api/users",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task ListUsers_BearerToken()
        {
            var authenticationCalled = false;

            async Task<IAuthenticationToken> Authenticate()
            {
                await Task.Delay(0);

                authenticationCalled = true;

                return new AuthenticationToken("abc123", DateTime.UtcNow.AddDays(1));
            }

            var response = await _request
                .BearerTokenAuthentication(Authenticate)
                .Get("users");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json", response.ContentType.MediaType);

            Assert.Equal("utf-8", response.ContentType.CharSet);

            var result = response.JsonContent<JObject>();

            Assert.Equal(
                @"{""page"":1,""per_page"":3,""total"":12,""total_pages"":4,""data"":[{""id"":1,""first_name"":""George"",""last_name"":""Bluth"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/calebogden/128.jpg""},{""id"":2,""first_name"":""Janet"",""last_name"":""Weaver"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/josephstein/128.jpg""},{""id"":3,""first_name"":""Emma"",""last_name"":""Wong"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/olegpogodaev/128.jpg""}]}",
                JsonConvert.SerializeObject(result));

            Assert.True(authenticationCalled);

            Assert.Equal(
                "Bearer abc123",
                response.Request.Headers["Authentication"]);

            Assert.Equal(
                "https://reqres.in/api/users",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task ListUsers_Cookie()
        {
            var authenticationCalled = false;

            async Task<IAuthenticationToken> Authenticate()
            {
                await Task.Delay(0);

                authenticationCalled = true;

                return new AuthenticationToken("abc123", DateTime.UtcNow.AddDays(1));
            }

            var response = await _request
                .CookieAuthentication(Authenticate)
                .Get("users");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json", response.ContentType.MediaType);

            Assert.Equal("utf-8", response.ContentType.CharSet);

            var result = response.JsonContent<JObject>();

            Assert.Equal(
                @"{""page"":1,""per_page"":3,""total"":12,""total_pages"":4,""data"":[{""id"":1,""first_name"":""George"",""last_name"":""Bluth"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/calebogden/128.jpg""},{""id"":2,""first_name"":""Janet"",""last_name"":""Weaver"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/josephstein/128.jpg""},{""id"":3,""first_name"":""Emma"",""last_name"":""Wong"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/olegpogodaev/128.jpg""}]}",
                JsonConvert.SerializeObject(result));

            Assert.True(authenticationCalled);

            Assert.Equal(
                "abc123",
                response.Request.Headers["Cookie"]);

            Assert.Equal(
                "https://reqres.in/api/users",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task SingleUser()
        {
            var response = await _request
                .RouteValues(new { UserId = 2 })
                .Get("users/{UserId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json", response.ContentType.MediaType);

            Assert.Equal("utf-8", response.ContentType.CharSet);

            var result = response.JsonContent<JObject>();

            Assert.Equal(
                @"{""data"":{""id"":2,""first_name"":""Janet"",""last_name"":""Weaver"",""avatar"":""https://s3.amazonaws.com/uifaces/faces/twitter/josephstein/128.jpg""}}",
                JsonConvert.SerializeObject(result));

            Assert.Equal(
                "https://reqres.in/api/users/2",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task SingleUserNotFound()
        {
            var response = await _request
                .RouteValues(new { UserId = 23 })
                .Get("users/{UserId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            Assert.Equal(
                "https://reqres.in/api/users/23",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task CreateUser()
        {
            var response = await _request
                .JsonBody(new
                {
                    name = "morpheus",

                    job = "leader"
                })
                .Post("users");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            Assert.Equal("application/json", response.ContentType.MediaType);

            Assert.Equal("utf-8", response.ContentType.CharSet);

            var result = response.JsonContent<JObject>();

            Assert.StartsWith(
                @"{""name"":""morpheus"",""job"":""leader"",""id"":""",
                JsonConvert.SerializeObject(result));

            Assert.Equal(
                "https://reqres.in/api/users",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task UpdateUserPut()
        {
            var response = await _request
                .RouteValues(new { UserId = 2 })
                .JsonBody(new
                {
                    name = "morpheus",

                    job = "zion resident"
                })
                .Put("users/{UserId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json", response.ContentType.MediaType);

            Assert.Equal("utf-8", response.ContentType.CharSet);

            var result = response.JsonContent<JObject>();

            Assert.StartsWith(
                @"{""name"":""morpheus"",""job"":""zion resident"",""updatedAt"":""",
                JsonConvert.SerializeObject(result));

            Assert.Equal(
                "https://reqres.in/api/users/2",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task UpdateUserPatch()
        {
            var response = await _request
                .RouteValues(new { UserId = 2 })
                .JsonBody(new
                {
                    name = "morpheus",

                    job = "zion resident"
                })
                .Patch("users/{UserId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("application/json", response.ContentType.MediaType);

            Assert.Equal("utf-8", response.ContentType.CharSet);

            var result = response.JsonContent<JObject>();

            Assert.StartsWith(
                @"{""name"":""morpheus"",""job"":""zion resident"",""updatedAt"":""",
                JsonConvert.SerializeObject(result));

            Assert.Equal(
                "https://reqres.in/api/users/2",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task DeleteUser()
        {
            var response = await _request
                .RouteValues(new { UserId = 2 })
                .Delete("users/{UserId}");

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
            var request = Request.Create("https://s3.amazonaws.com");

            var response = await request.Get("uifaces/faces/twitter/calebogden/128.jpg");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("image/jpeg", response.ContentType.MediaType);

            Assert.Equal(3468, response.Content.Count);

            Assert.Equal(
                "https://s3.amazonaws.com/uifaces/faces/twitter/calebogden/128.jpg",
                response.RequestedUrl);
        }

        [Fact, Trait("Type", "Integration")]
        public async Task GetServerUnreachable()
        {
            var request = Request.Create("https://doesnt.exist");
            
            var response = await request.Get("something/somewhere");

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
            var request = Request.Create("https://doesnt.exist")
                .Timeout(TimeSpan.FromMilliseconds(1));

            var response = await request.Get("something/somewhere");

            Assert.Equal(0, (int)response.StatusCode);

            Assert.Null(response.ContentType);

            Assert.Null(response.Content);

            Assert.False(string.IsNullOrWhiteSpace(response.ErrorMessage));

            Assert.Null(response.Exception);

            Assert.Equal(
                "https://doesnt.exist/something/somewhere",
                response.RequestedUrl);
        }
    }
}
