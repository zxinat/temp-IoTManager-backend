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
    public sealed class TestThresholdController : TestController
    {
        public TestThresholdController(TestServerFixture testServer) : base(testServer)
        {
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdSuccess(int id)
        {
            var response = await this._httpClient.GetAsync("api/threshold/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("PRESSURE", "0001", ">",100,null,null,"通知")]    // 暂时无可用测试样例
        public async Task PostSuccess(String IndexId,String DeviceId,String Operator,Double ThresholdValue,
        String RuleName,String Description,String Severity)
        {
            var data = new ThresholdModel
            {
                IndexId = IndexId,
                DeviceId = DeviceId,
                Operator = Operator,
                ThresholdValue = ThresholdValue,
                RuleName = RuleName,
                Description = Description,
                Severity = Severity
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/threshold", content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Fact]
        public async Task GetSuccess()
        {
            var response = await this._httpClient.GetAsync("api/threshold");
            var result = response.Content.ReadAsStringAsync().Result;
            Console.Write(result);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

    }
}
