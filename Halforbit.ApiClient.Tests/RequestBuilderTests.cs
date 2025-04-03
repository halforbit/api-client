using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Halforbit.ApiClient.Tests
{
    public class RequestBuilderTests
    {
        static readonly Encoding _utf8Encoding = new UTF8Encoding(false);

        // Properties /////////////////////////////////////////////////////////

        [Fact, Trait("Type", "Unit")]
        public void RequestName()
        {
            var request = default(Request).Name("Alfa");

            Assert.Equal("Alfa", request.Name());
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestBaseUrl()
        {
            var request = default(Request).BaseUrl("//alfa//");

            Assert.Equal("alfa", request.BaseUrl);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestBaseUrl_Segments()
        {
            var request = default(Request).BaseUrl(
                "//http://alfa.bravo//", 
                "//charlie//", 
                "//delta//");

            Assert.Equal(
                "http://alfa.bravo/charlie/delta", 
                request.BaseUrl);
        }

        // Methods ////////////////////////////////////////////////////////////

        [Fact, Trait("Type", "Unit")]
        public void RequestGet()
        {
            var request = default(Request).Method("GET").Resource("alfa");

            Assert.Equal("GET", request.Method);

            Assert.Equal("alfa", request.Resource);
        }
        
        [Fact, Trait("Type", "Unit")]
        public void RequestPost()
        {
            var request = default(Request).Method("POST").Resource("alfa");

            Assert.Equal("POST", request.Method);

            Assert.Equal("alfa", request.Resource);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestPut()
        {
            var request = default(Request).Method("PUT").Resource("alfa");

            Assert.Equal("PUT", request.Method);

            Assert.Equal("alfa", request.Resource);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestDelete()
        {
            var request = default(Request).Method("DELETE").Resource("alfa");

            Assert.Equal("DELETE", request.Method);

            Assert.Equal("alfa", request.Resource);
        }

        // Strategy ///////////////////////////////////////////////////////////

        [Fact, Trait("Type", "Unit")]
        public void RequestBasicAuthorization()
        {
            var request = default(Request).BasicAuthorization(
                username: "username",
                password: "password");

            Assert.True(request.Services.AuthorizationStrategy is BasicAuthorizationStrategy);
        }
        
        [Fact, Trait("Type", "Unit")]
        public void RequestBearerTokenAuthorization()
        {
            var request = default(Request).BearerTokenAuthorization(() => null);

            Assert.True(request.Services.AuthorizationStrategy is BearerTokenAuthorizationStrategy);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestBearerTokenAuthorizationWithBaseUrl()
        {
            var request = default(Request).BearerTokenAuthorizationWithBaseUrl(() => null);

            Assert.True(request.Services.AuthorizationStrategy is BearerTokenAuthorizationWithBaseUrlStrategy);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestCookieAuthorization()
        {
            var request = default(Request).CookieAuthorization(() => null);

            Assert.True(request.Services.AuthorizationStrategy is CookieAuthorizationStrategy);
        }

        // Routes /////////////////////////////////////////////////////////////

        [Fact, Trait("Type", "Unit")]
        public void RequestWithRouteValue()
        {
            var request = default(Request).RouteValue("alfa", "bravo");

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo"
                },
                request.RouteValues);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestWithObjectRouteValues()
        {
            var request = default(Request)
                .RouteValues(new
                {
                    alfa = "bravo",

                    charlie = "delta"
                });

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo",

                    ["charlie"] = "delta"
                },
                request.RouteValues);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestWithTupleRouteValues()
        {
            var request = default(Request).RouteValues(
                ("alfa", "bravo"),
                ("charlie", "delta"));

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo",

                    ["charlie"] = "delta"
                },
                request.RouteValues);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestWithDictionaryRouteValues()
        {
            var request = default(Request)
                .RouteValues(new Dictionary<string, string>
                {
                    ["alfa"] = "bravo",

                    ["charlie"] = "delta"
                });

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo",

                    ["charlie"] = "delta"
                },
                request.RouteValues);
        }

        // Query //////////////////////////////////////////////////////////////

        [Fact, Trait("Type", "Unit")]
        public void RequestWithQueryValue()
        {
            var request = default(Request).QueryValue("alfa", "bravo");

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo"
                },
                request.QueryValues);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestWithObjectQueryValues()
        {
            var request = default(Request).QueryValues(new
            {
                alfa = "bravo",

                charlie = "delta"
            });

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo",

                    ["charlie"] = "delta"
                },
                request.QueryValues);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestWithTupleQueryValues()
        {
            var request = default(Request).QueryValues(
                ("alfa", "bravo"),
                ("charlie", "delta"));

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo",

                    ["charlie"] = "delta"
                },
                request.QueryValues);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestWithDictionaryQueryValues()
        {
            var request = default(Request).QueryValues(new Dictionary<string, string>
            {
                ["alfa"] = "bravo",

                ["charlie"] = "delta"
            });

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo",

                    ["charlie"] = "delta"
                },
                request.QueryValues);
        }

        // Headers ////////////////////////////////////////////////////////////

        [Fact, Trait("Type", "Unit")]
        public void RequestWithHeader()
        {
            var request = default(Request).Header("alfa", "bravo");

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo"
                },
                request.Headers);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestWithObjectHeaders()
        {
            var request = default(Request).Headers(new
            {
                alfa = "bravo",

                charlie = "delta"
            });

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo",

                    ["charlie"] = "delta"
                },
                request.Headers);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestWithTupleHeaders()
        {
            var request = default(Request).Headers(
                ("alfa", "bravo"),
                ("charlie", "delta"));

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo",

                    ["charlie"] = "delta"
                },
                request.Headers);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestWithDictionaryHeaders()
        {
            var request = default(Request).Headers(new Dictionary<string, string>
            {
                ["alfa"] = "bravo",

                ["charlie"] = "delta"
            });

            Assert.Equal(
                new Dictionary<string, string>
                {
                    ["alfa"] = "bravo",

                    ["charlie"] = "delta"
                },
                request.Headers);
        }

        // Body ///////////////////////////////////////////////////////////////

        [Fact, Trait("Type", "Unit")]
        public void RequestWithTextBody()
        {
            var request = default(Request).TextBody(
                "hello, world!",
                "text/special");

            Assert.Equal(
                new UTF8Encoding(false).GetBytes("hello, world!"),
                ReadFully(request.Content.GetStream()));

            Assert.Equal(
                "text/special; charset=utf-8",
                request.ContentType.Value);
        }

        [Fact, Trait("Type", "Unit")]
        public void RequestWithJsonBody()
        {
            var body = new
            {
                alfa = "bravo",

                charlie = 123
            };

            var request = default(Request).Body(body);

            Assert.Equal(
                "application/json; charset=utf-8",
                request.ContentType.Value);

            Assert.Equal(
                _utf8Encoding.GetBytes(System.Text.Json.JsonSerializer.Serialize(body)),
                ReadFully(request.Content.GetStream()));
        }

        static byte[] ReadFully(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);

                return ms.ToArray();
            }
        }
    }
}
