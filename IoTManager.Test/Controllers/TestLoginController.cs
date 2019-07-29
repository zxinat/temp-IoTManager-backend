using IoTManager.Test.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IoTManager.Utility.Serializers;
using IoTManager.Model;
using Xunit;
using System.Net.Http;
using IoTManager.Utility.Extensions;

namespace IoTManager.Test.Controllers
{
    [ExcludeFromCodeCoverage]
    public sealed class TestLoginController : TestController
    {
        public TestLoginController(TestServerFixture testServer) : base(testServer)
        {
        }

        [Theory]
        [InlineData("SampleUser4", "123")]
        public async Task LoginSuccess(String name,String password)
        {
            var data = new LoginModel
            {
                Name = name,
                Password = password
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/login", content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":{\"status\":\"success\"", result);
        }

        [Theory]
        [InlineData("TestUser 1", "123")]
        [InlineData("", "0001")]
        [InlineData("TestUser 3", "")]
        public async Task LoginFailed(String name, String password)
        {
            var data = new LoginModel
            {
                Name = name,
                Password = password
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/login", content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":{\"status\":\"Failed\"", result);
        }
    }
}
