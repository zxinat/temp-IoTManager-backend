using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IoTManager.DI;
using IoTManager.DI.Infrastructures;
using IoTManager.Model.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using VMD.RESTApiResponseWrapper.Core.Extensions;
using Hangfire.MySql.Core;
using Hangfire.Mongo;
using IoTManager.Utility;
using IoTManager.Core.Jobs;

namespace IoTManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //配置Hangfire服务
            //配置Hangfire连接至MySQL数据库
            /*
            services.AddHangfire(x => x.UseStorage(new MySqlStorage(Constant.getDatabaseConnectionString(),
                new MySqlStorageOptions
                {
                    TransactionIsolationLevel = System.Data.IsolationLevel.ReadCommitted, //默认读取已提交的任务
                    QueuePollInterval = TimeSpan.FromSeconds(15),                         //作业队列轮询间隔，15秒
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),                   //作业到期检查，管理过期记录，间隔1小时
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),                  //聚合计数器间隔
                    PrepareSchemaIfNecessary = true,                                      //true表示创建数据表
                    DashboardJobListLimit = 50000,                                        //仪表板列表限制
                    TransactionTimeout = TimeSpan.FromMinutes(1),                         //交易超时
                    TablePrefix = "HF"                                                    //数据库中数表前缀
                    
                })));
                */
            //配置Hangfire连接至MongoDB
            //var connectionStr = Constant.getMongoDBConnectionString();
            services.AddHangfire(config =>
            {
                config.UseMongoStorage(Constant.getMongoDBConnectionString(), new MongoStorageOptions
                {
                    Prefix = "Hangfire",
                    MigrationOptions = new MongoMigrationOptions
                    {
                        Strategy = MongoMigrationStrategy.Migrate,
                        BackupStrategy = MongoBackupStrategy.Collections
                    }
                });
            });
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder => { builder.WithOrigins(this.Configuration.GetSection("CorsIp")["IpAddress"])
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); });
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //add swagger 
            services.AddSwaggerGen(gen => {
                gen.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
            //add log
            services.AddLogging();
            services.AddTimedJob();
            services.Configure<IoTHubAppSetting>(this.Configuration.GetSection("IoTHubAppSetting"));
            //add dependency injection
            IocContainer autofac = new AutofacContainer(services);
            return autofac.Build().FetchServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //使用hangfire面板
            app.UseHangfireDashboard(); 
            app.UseHangfireServer();    //启动hangfire服务

            app.UseHttpsRedirection();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                    {ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto});
            app.UseAuthentication();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseTimedJob();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            loggerFactory.AddFile("Logs/iotmanager-{Date}.txt");
            //app.UseAPIResponseWrapperMiddleware();
            //添加每日在线时长统计Job，执行时间每日23:59,本地时间
            RecurringJob.AddOrUpdate<ReportJob>(x => x.Run(), Cron.Daily(23, 59), TimeZoneInfo.Local);
            app.UseMvc();
        }
    }
}