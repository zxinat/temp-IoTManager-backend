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
    public sealed class TestWorkshopController : TestController
    {
        public TestWorkshopController(TestServerFixture testServer) : base(testServer)
        {
        }

        [Fact]
        public async Task GetSuccess()
        {
            var response = await this._httpClient.GetAsync("api/workshop");
            var result = response.Content.ReadAsStringAsync().Result;
            Console.Write(result);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdSuccess(int id)
        {
            var response = await this._httpClient.GetAsync("api/workshop/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("WorkshopTest1","why","12345","12345","Shanghai University")]
        public async Task PostSuccess(String WorkshopName,String remark,String WorkshopPhoneNumber,String WorkshopAddress,
        String Factory)
        {
            var data = new WorkshopModel
            {
                WorkshopName = WorkshopName,
                Remark = remark,
                WorkshopPhoneNumber = WorkshopPhoneNumber,
                WorkshopAddress = WorkshopAddress,
                Factory = Factory
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/workshop", content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        //[Theory]
        //[InlineData("WorkshopTest1", "why", "12345", "12345", "Shanghai University")]
        //public async Task PostError(String WorkshopName, String remark, String WorkshopPhoneNumber, String WorkshopAddress,
        //String Factory)
        //{
        //    var data = new WorkshopModel
        //    {
        //        WorkshopName = WorkshopName,
        //        Remark = remark,
        //        WorkshopPhoneNumber = WorkshopPhoneNumber,
        //        WorkshopAddress = WorkshopAddress,
        //        Factory = Factory
        //    };
        //    var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
        //    var response = await this._httpClient.PostAsync("api/workshop", content);
        //    var result = response.Content.ReadAsStringAsync().Result;
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        //}

        [Theory]
        [InlineData(15,"Workshop Sample124","12345", "12345", "12345", "Shanghai University 1")]
        public async Task PutSuccess(int id,String WorkshopName,String WorkshopPhoneNumber,String WorkshopAddress,
            String Remark,String Factory)
        {
            var data = new WorkshopModel
            {
                Id = id,
                WorkshopName = WorkshopName,
                WorkshopPhoneNumber = WorkshopPhoneNumber,
                WorkshopAddress = WorkshopAddress,
                Remark = Remark,
                Factory = Factory

            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PutAsync("api/workshop/" + id, content); //TODO
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(14, "Workshop Sample124", "12345", "12345", "12345", "Shanghai University 1")]  // 名称重复
        [InlineData(20, "Workshop Sample110", "12345", "12345", "12345", "Shanghai University 1")]  // 不存在的id
        public async Task PutError(int id, String WorkshopName, String WorkshopPhoneNumber, String WorkshopAddress,
            String Remark, String Factory)
        {
            var data = new WorkshopModel
            {
                Id = id,
                WorkshopName = WorkshopName,
                WorkshopPhoneNumber = WorkshopPhoneNumber,
                WorkshopAddress = WorkshopAddress,
                Remark = Remark,
                Factory = Factory
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PutAsync("api/workshop/" + id, content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        }

        [Theory]
        [InlineData(13)]
        public async Task DeleteSuccess(int id)
        {
            var response = await this._httpClient.DeleteAsync("api/workshop/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(3)]
        public async Task DeleteError(int id)
        {
            var response = await this._httpClient.DeleteAsync("api/workshop/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        }

        [Theory]
        [InlineData("Shanghai University")]
        public async Task GetAffiliateWorkshop(String fName)
        {
            var response = await this._httpClient.GetAsync("api/workshop/workshopOptions/" + fName);
            var result = response.Content.ReadAsStringAsync().Result;
            Console.Write(result);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }
    }
}
