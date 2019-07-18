using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace IoTManager.Test.Base
{
    public class TestController : IClassFixture<TestServerFixture>
    {
        protected readonly HttpClient _httpClient;

        public TestController(TestServerFixture testServer)
        {
            this._httpClient = testServer.HttpClient;
        }
    }
}
