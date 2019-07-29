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
    public sealed class TestFactoryController : TestController
    {
        public TestFactoryController(TestServerFixture testServer) : base(testServer)
        {
        }

        [Fact]
        public async Task GetSuccess()
        {
            var response = await this._httpClient.GetAsync("api/factory");
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdSuccess(int id)
        {
            var response = await this._httpClient.GetAsync("api/factory/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("Shanghai University 5", "12345", "12345","hjk", "SH")]
        public async Task PostSuccess(string factoryName,string factoryPhoneNumber,string factoryAddress,string remark,string city)
        {
            var data = new FactoryModel
            {
                FactoryName = factoryName,
                FactoryPhoneNumber = factoryPhoneNumber,
                FactoryAddress = factoryAddress,
                Remark = remark,
                City = city
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/factory", content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(1, "Shanghai University", "12345", "12345", "hjk", "SH")]
        public async Task PutSuccess(int id,string factoryName,string factoryPhoneNumber,string factoryAddress,string remark,string city)
        {
            var data = new FactoryModel
            {
                Id = id,
                FactoryName = factoryName,
                FactoryPhoneNumber = factoryPhoneNumber,
                FactoryAddress = factoryAddress,
                Remark = remark,
                City = city
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PutAsync("api/factory/" + id, content); //TODO
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(120, "TestFactory", "12345", "12345", "hjk", "SH")]  // 不存在的id
        public async Task PutError(int id, string factoryName, string factoryPhoneNumber, string factoryAddress, string remark, string city)
        {
            var data = new FactoryModel
            {
                Id = id,
                FactoryName = factoryName,
                FactoryPhoneNumber = factoryPhoneNumber,
                FactoryAddress = factoryAddress,
                Remark = remark,
                City = city
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PutAsync("api/factory/" + id, content); //TODO
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        }

        [Theory]
        [InlineData(3)]
        public async Task DeleteSuccess(int id)
        {
            var response = await this._httpClient.DeleteAsync("api/factory/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(13)]
        public async Task DeleteError(int id)
        {
            var response = await this._httpClient.DeleteAsync("api/factory/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        }
    }



 }
