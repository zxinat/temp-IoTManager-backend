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
    public sealed class TestDeviceDataController : TestController
    {
        public TestDeviceDataController(TestServerFixture testServer) : base(testServer)
        {
        }

        [Fact]
        public async Task GetSuccess()
        {
            var response = await this._httpClient.GetAsync("api/devicedata");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("5d2fef5c55fa84c2b16daca7")]
        public async Task GetByIdSuccess(string id)
        {
            var response = await this._httpClient.GetAsync("api/devicedata/id/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }



        [Theory]
        [InlineData("FTD-01")]
        public async Task GetByDeviceIdSuccess(string deviceId)
        {
            var response = await this._httpClient.GetAsync("api/devicedata/deviceId/" + deviceId);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("FTD-01", "P-0001")]
        public async Task GetLineChartDataSuccess(string deviceId,string indexId)
        {
            var response = await this._httpClient.GetAsync("api/devicedata/" +deviceId+"/"+indexId);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }
        
        [Theory]
        [InlineData(3)]
        public async Task GetDeviceStatusByIdSuccess(int id)    // 测试不通过
        {
            var response = await this._httpClient.GetAsync("api/devicedata/status/" +id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Fact]
        public async Task GetAmount()
        {
            var response = await this._httpClient.GetAsync("api/devicedata/amount");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

    }
}
