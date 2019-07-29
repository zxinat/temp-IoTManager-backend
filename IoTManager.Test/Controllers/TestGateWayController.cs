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
    public sealed class TestGateWayController : TestController
    {
        public TestGateWayController(TestServerFixture testServer) : base(testServer)
        {
        }

        [Fact]
        public async Task GetSuccess()
        {
            var response = await this._httpClient.GetAsync("api/gateway");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData(23)]
        public async Task GetByIdSuccess(int id)
        {
            var response = await this._httpClient.GetAsync("api/gateway/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("Test Id", "Test Name", "Shanghai", "Shanghai University 2", "string", "网关类型一", "", "", "")]
        public async Task PostSuccess(String hardwareGatewayId, String gatewayName, String city, String factory, String workshop, String gatewayType, String gatewayState, String imageUrl, String remark)
        {
            var data = new GatewayModel
            {
 
                HardwareGatewayId = hardwareGatewayId,
                GatewayName = gatewayName,
                City = city,
                Factory = factory,
                Workshop = workshop,
                GatewayType = gatewayType,
                GatewayState = gatewayState,
                ImageUrl = imageUrl,
                Remark = remark
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/gateway", content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(31,"Test Id", "Test Name", "Shanghai", "Shanghai University 2", "string", "网关类型一", "", "", "")]
        public async Task PutSuccess(int id,String hardwareGatewayId, String gatewayName, String city, String factory, String workshop, String gatewayType, String gatewayState, String imageUrl, String remark)
        {
            var data = new GatewayModel
            {
                Id=id,
                HardwareGatewayId = hardwareGatewayId,
                GatewayName = gatewayName,
                City = city,
                Factory = factory,
                Workshop = workshop,
                GatewayType = gatewayType,
                GatewayState = gatewayState,
                ImageUrl = imageUrl,
                Remark = remark
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PutAsync("api/gateway/" + id, content); //TODO
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(10, "Test Id1", "Test Name1", "Shanghai", "Shanghai University 2", "string", "网关类型一", "34", "43", "43")]
        [InlineData(12, "Test Id2", "Test Name2", "Shanghai", "Shanghai University 2", "string", "网关类型一", "", "23", "43")]
        [InlineData(13, "Test Id3", "Test Name3", "Shanghai", "Shanghai University 2", "string", "网关类型一", "", "20", "")]
        public async Task PutFailed(int id, String hardwareGatewayId, String gatewayName, String city, String factory, String workshop, String gatewayType, String gatewayState, String imageUrl, String remark)
        {
            var data = new GatewayModel
            {
                Id = id,
                HardwareGatewayId = hardwareGatewayId,
                GatewayName = gatewayName,
                City = city,
                Factory = factory,
                Workshop = workshop,
                GatewayType = gatewayType,
                GatewayState = gatewayState,
                ImageUrl = imageUrl,
                Remark = remark
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PutAsync("api/gateway/" + id, content); //TODO
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        }

    

        [Theory]
        [InlineData(44)]
        public async Task DeleteSuccess(int id)
        {
            var response = await this._httpClient.DeleteAsync("api/gateway/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(25)]
        [InlineData(37)]
        [InlineData(10)]
        public async Task DeleteFailed(int id)
        {
            var response = await this._httpClient.DeleteAsync("api/gateway/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        }


        [Theory]
        [InlineData("Shanghai", "string", "string")]
        public async Task GetGatewayByWorkshopSuccess(String cityName, String factoryName, String workshopName)
        {
            var response = await this._httpClient.GetAsync("api/gateway/workshop/" + cityName+"/"+ factoryName + "/" + workshopName);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData(new int[0] { })]
        [InlineData(new int[2] { 42, 43 })]
        [InlineData(new int[5] { 1, 2, 3, 4, 5 })]
        public async Task BatchDeleteSuccess(int[] Number)
        {
            var data = new BatchNumber
            {
                number = Number
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/gateway/batch/gateways", content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":" + Number.Length, result);
        }
        
        [Theory]
        [InlineData("TestGatewayType")]
        public async Task CreateGatewayTypeSuccess(String gatewayType)
        {
            //var data = gatewayType;
            var content = new StringContent(gatewayType, Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/gateway/type/"+gatewayType, content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

       

        
    }
}
