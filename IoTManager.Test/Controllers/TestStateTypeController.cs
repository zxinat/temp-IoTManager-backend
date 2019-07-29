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
    public sealed class TestStateTypeController : TestController
    {
        public TestStateTypeController(TestServerFixture testServer) : base(testServer)
        {
        }

        [Fact]
        public async Task GetdeviceTypeSuccess()
        {
            var response = await this._httpClient.GetAsync("api/deviceType");
            var result = response.Content.ReadAsStringAsync().Result;
            Console.Write(result);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Fact]
        public async Task GetdeviceStateSuccess()
        {
            var response = await this._httpClient.GetAsync("api/deviceState");
            var result = response.Content.ReadAsStringAsync().Result;
            Console.Write(result);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Fact]
        public async Task GetgatewayTypeSuccess()
        {
            var response = await this._httpClient.GetAsync("api/gatewayType");
            var result = response.Content.ReadAsStringAsync().Result;
            Console.Write(result);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Fact]
        public async Task GetgatewayStateSuccess()
        {
            var response = await this._httpClient.GetAsync("api/gatewayState");
            var result = response.Content.ReadAsStringAsync().Result;
            Console.Write(result);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }
    }
}
