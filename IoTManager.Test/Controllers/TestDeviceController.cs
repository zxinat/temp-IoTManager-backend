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
    public sealed class TestDeviceController : TestController
    {
        public TestDeviceController(TestServerFixture testServer) : base(testServer)
        {
        }

        [Fact]
        public async Task GetSuccess()
        {
            var response = await this._httpClient.GetAsync("api/device");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData(44)]
        public async Task GetByIdSuccess(int id)
        {
            var response = await this._httpClient.GetAsync("api/device/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("测试190722")]
        public async Task GetByDeviceNameSuccess(string deviceName)
        {
            var response = await this._httpClient.GetAsync("api/device/deviceName/" + deviceName);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("123")]
        public async Task GetByDeviceIdSuccess(string deviceId)
        {
            var response = await this._httpClient.GetAsync("api/device/deviceid/" + deviceId);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("Test10","12308", "SH", "Shanghai University", "Workshop Sample","1","1", "123","123456","测试","hh")]
        public async Task PostSuccess(string deviceName, string hardwareDeviceId, string city, string factory, string workShop,
            string deviceState, string imageUrl, string gatewatId, string mac, string deviceType, string remark)
        {
            var data = new DeviceModel
            {
                DeviceName = deviceName,
                HardwareDeviceId = hardwareDeviceId,
                City = city,
                Factory = factory,
                Workshop = workShop,
                DeviceState= deviceState,
                //LastConnectionTime=new DateTime(2018,1,1,1,1,1),
                ImageUrl = imageUrl,
                GatewayId= gatewatId,
                Mac= mac,
                DeviceType= deviceType,
                Remark= remark
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/device", content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

    
        [Theory]
        [InlineData(59)]
        public async Task DeleteSuccess(int id)
        {
            var response = await this._httpClient.DeleteAsync("api/device/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(3)]
        public async Task DeleteError(int id)
        {
            var response = await this._httpClient.DeleteAsync("api/device/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
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
            var response = await this._httpClient.PostAsync("api/device/batch/devices", content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":" + Number.Length, result);
        }

        [Theory]
        [InlineData("SH", "Shanghai University", "Workshop Sample")]
        public async Task GetDeviceByWorkshopSuccess(string cityName,string factoryName,string workshopName)
        {
            var response = await this._httpClient.GetAsync("api/device/workshop/" + cityName+"/"+factoryName+"/"+workshopName);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("SH", "Shanghai University")]
        public async Task GetDeviceTreeSuccess(string city, string factory)
        {
            var response = await this._httpClient.GetAsync("api/device/tree/"+city+"/"+factory);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("TestType2")]
        public async Task CreateTypeSuccess(string deviceType)
        {
            var content = new StringContent(deviceType.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/device/type/" + deviceType, content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Fact]
        public async Task GetAmountSuccess()
        {
            var response = await this._httpClient.GetAsync("api/device/amount");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData(67,"Test1", "12306", "SH", "Shanghai University", "Workshop Sample", "1", "2", "123", "123456", "测试", "hh")]
        public async Task PutSuccess(int id, string deviceName, string hardwareDeviceId, string city, string factory, string workShop,
            string deviceState, string imageUrl, string gatewatId, string mac, string deviceType, string remark)
        {
            var data = new DeviceModel
            {
                Id=id,
                DeviceName = deviceName,
                HardwareDeviceId = hardwareDeviceId,
                City = city,
                Factory = factory,
                Workshop = workShop,
                DeviceState = deviceState,
                //LastConnectionTime=new DateTime(2018,1,1,1,1,1),
                ImageUrl = imageUrl,
                GatewayId = gatewatId,
                Mac = mac,
                DeviceType = deviceType,
                Remark = remark
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PutAsync("api/device/" + id, content); //TODO
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        //[InlineData(78, "test3", "12309", "SH", "Shanghai University", "Workshop Sample", "1", "1", "123", "123456", "测试", "hh")]  // 名称重复
        [InlineData(100,"Test3", "12308", "SH", "Shanghai University", "Workshop Sample", "1", "1", "123", "123456", "测试", "hh")]  // 不存在的id
        public async Task PutError(int id,string deviceName, string hardwareDeviceId, string city, string factory, string workShop,
            string deviceState, string imageUrl, string gatewatId, string mac, string deviceType, string remark)
        {
            var data = new DeviceModel
            {
                Id=id,
                DeviceName = deviceName,
                HardwareDeviceId = hardwareDeviceId,
                City = city,
                Factory = factory,
                Workshop = workShop,
                DeviceState = deviceState,
                //LastConnectionTime=new DateTime(2018,1,1,1,1,1),
                ImageUrl = imageUrl,
                GatewayId = gatewatId,
                Mac = mac,
                DeviceType = deviceType,
                Remark = remark
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PutAsync("api/device/" + id, content); //TODO
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        }



    }
}
