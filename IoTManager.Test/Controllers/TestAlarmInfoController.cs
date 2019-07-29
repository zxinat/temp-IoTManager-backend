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
    public sealed class TestAlarmInfoController : TestController
    {
        public TestAlarmInfoController(TestServerFixture testServer) : base(testServer)
        {
        }
        [Fact]
        public async Task GetSuccess()
        {
            var response = await this._httpClient.GetAsync("api/alarmInfo");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("5d2fef5660028265d793432f")]
        public async Task GetByIdSuccess(string id)
        {
            var response = await this._httpClient.GetAsync("api/alarmInfo/id/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("FTD-01")]
        public async Task GetByDeviceIdSuccess(string deviceId)
        {
            var response = await this._httpClient.GetAsync("api/alarmInfo/deviceId/" + deviceId);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("P-0001")]
        public async Task GetByIndexIdSuccess(string indexId)
        {
            var response = await this._httpClient.GetAsync("api/alarmInfo/indexId/" + indexId);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Fact]
        public async Task InspectSuccess()
        {
           var response = await this._httpClient.GetAsync("api/alarmInfo/inspect");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Fact]
        public async Task GetDashboardSuccess()
        {
            var response = await this._httpClient.GetAsync("api/alarmInfo/dashboard");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Fact]
        public async Task GetNoticeAmountSuccess()
        {
            var response = await this._httpClient.GetAsync("api/alarmInfo/noticeAmount");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Fact]
        public async Task GetSeriousAmountSuccess()
        {
            var response = await this._httpClient.GetAsync("api/alarmInfo/seriousAmount");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }


        [Fact]
        public async Task GetVerySeriousAmountSuccess()
        {
            var response = await this._httpClient.GetAsync("api/alarmInfo/verySeriousAmount");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

    }
}
