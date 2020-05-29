## Solvay部署说明

#### 数据库更新：

1. 【device】表，base64Image字段类型改成mediumtext，字符集utf8，排序规则utf8_general_ci，

   ```powershell
   mysql>alter table device modify column base64Image mediumtext;
   ```

   

2. 【gateway】表，



#### staffManager项目卸载说明：

1. IoTManager->Startup.cs文件：修改模块引用、Job任务

   ```C#
   public IServiceProvider ConfigureServices(IServiceCollection services)
   {
       //添加staffManager模块
       var staffManager = Assembly.Load(new AssemblyName("StaffManager"));
       services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
           .AddApplicationPart(staffManager);
   }
   public void Configure(IApplicationBuilder app, 
                         IHostingEnvironment env, ILoggerFactory loggerFactory)
   {
       RecurringJob.AddOrUpdate<IAttendenceRecordJob>(x => x.Run(), 
                                                      Cron.Daily(18, 00), 
                                                      TimeZoneInfo.Local);
   }
   ```

2. 删除依赖注入：IoTModel.cs文件

   ```C#
   protected override void Load(ContainerBuilder builder)
   {
   	builder.RegisterType<StaffBus>().As<IStaffBus>();
       builder.RegisterType<CustomerBus>().As<ICustomerBus>();
       builder.RegisterType<RFIDTagBus>().As<IRFIDTagBus>();
       builder.RegisterType<StaffAuthBus>().As<IStaffAuthBus>();
       builder.RegisterType<StatisticBus>().As<IStatisticBus>();
       builder.RegisterType<StaffDao>().As<IStaffDao>();
       builder.RegisterType<CustomerDao>().As<ICustomerDao>();
       builder.RegisterType<StaffDataDao>().As<IStaffDataDao>();
       builder.RegisterType<RFIDTagDao>().As<IRFIDTagDao>();
       builder.RegisterType<StaffAuthDao>().As<IStaffAuthDao>();
       builder.RegisterType<AttendenceDataDao>().As<IAttendenceDataDao>();
       builder.RegisterType<AttendenceRecordJob>().As<IAttendenceRecordJob>();
   }
   ```

   