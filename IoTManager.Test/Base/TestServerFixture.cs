using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace IoTManager.Test.Base
{
    public class TestServerFixture
    {
        private readonly string _appsettingsFile = "appsettings.json";

        private TestServer _testServer;
        public HttpClient HttpClient { get; set; }

        public TestServerFixture()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            this._testServer = new TestServer(new WebHostBuilder()
                .UseEnvironment("Test")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), this._appsettingsFile))
                .Build())
                .UseStartup<TestStartup>()
                );
            this.HttpClient = this._testServer.CreateClient();
        }
    }
}
