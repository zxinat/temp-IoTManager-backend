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
    public sealed class TestUserController : TestController
    {
        public TestUserController(TestServerFixture testServer) : base(testServer)
        {
        }

        [Fact]
        public async Task GetSuccess()
        {
            var response = await this._httpClient.GetAsync("api/user");
            var result = response.Content.ReadAsStringAsync().Result;
            Console.Write(result);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetByIdSuccess(int id)
        {
            var response = await this._httpClient.GetAsync("api/user/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("SampleUser4")]
        public async Task GetByUserNameSuccess(String UserName)
        {
            var response = await this._httpClient.GetAsync("api/user/username/" + UserName);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("SampleUser187", "Post test data","User187","123", "test@163.com", "13800000000")]
        public async Task PostSuccess(String userName, String remark, String displayName, String password, String email, String phoneNumber)
        {
            var data = new UserModel
            {
                UserName = userName,
                DisplayName = displayName,
                Password = password,
                Email = email,
                PhoneNumber = phoneNumber,
                Remark = remark
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/user", content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(96, "SampleUser12", "TU101","1234", "test@163.com", "13400000000")]
        public async Task PutSuccess(int id, String userName, String displayName,String password, String email, String phoneNumber)
        {
            var data = new UserModel
            {
                Id = id,
                UserName = userName,
                DisplayName = displayName,
                Password = password,
                Email = email,
                PhoneNumber = phoneNumber
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PutAsync("api/user/" + id, content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(3, "SampleUser12", "TU101", "", "123@qq.com", "13918800000")]                   // 不存在的id
        //[InlineData(96, "SampleUser2", "TU101", "", "123@qq.com", "13918800000")]                   // 用户名冲突 会抛异常
        //[InlineData(96, "SampleUser12", "SampleDisplayName2", "", "123@qq.com", "13918800000")]     // 昵称冲突 会抛异常
        public async Task PutError(int id, String userName, String displayName, String password, String email, String phoneNumber)
        {
            var data = new UserModel
            {
                Id = id,
                UserName = userName,
                DisplayName = displayName,
                Password = password,
                Email = email,
                PhoneNumber = phoneNumber
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PutAsync("api/user/" + id, content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        }

        [Theory]
        [InlineData(7)]
        public async Task DeleteSuccess(int id)
        {
            var response = await this._httpClient.DeleteAsync("api/user/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData(3)]
        public async Task DeleteError(int id)
        {
            var response = await this._httpClient.DeleteAsync("api/user/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        }

        [Theory]
        [InlineData("SampleUser")]
        public async Task GetByNameSuccess(String UserName)
        {
            var response = await this._httpClient.GetAsync("api/user/name/" + UserName);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\"", result);
        }

        [Theory]
        [InlineData("SampleUser4", "123")]
        public async Task UpdatePasswordSuccess(String userName, String password)
        {
            var data = new UserModel
            {
                UserName = userName,
                Password = password
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/user/password/" + userName, content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"success\"", result);
        }

        [Theory]
        [InlineData("SampleUser0", "123")]  // 不存在的用户名
        public async Task UpdatePasswordError(String userName, String password)
        {
            var data = new UserModel
            {
                UserName = userName,
                Password = password
            };
            var content = new StringContent(data.ToJson(), Encoding.UTF8, "text/json");
            var response = await this._httpClient.PostAsync("api/user/password/" + userName, content);
            var result = response.Content.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"c\":200,\"m\":\"success\",\"d\":\"error\"", result);
        }
    }
}
