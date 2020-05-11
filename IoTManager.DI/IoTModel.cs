using Autofac;
using IoTManager.AzureIoTHub;
using IoTManager.Core;
using IoTManager.Core.Infrastructures;
using IoTManager.Dao;
using IoTManager.Dao.MySQL;
using IoTManager.IDao;
using IoTManager.IHub;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.DI
{
    internal sealed class IoTPlatformModel:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //IoTManager.Core
            builder.RegisterType<CityBus>().As<ICityBus>();
            builder.RegisterType<DepartmentBus>().As<IDepartmentBus>();
            builder.RegisterType<DeviceBus>().As<IDeviceBus>();
            builder.RegisterType<GatewayBus>().As<IGatewayBus>();
            builder.RegisterType<RoleBus>().As<IRoleBus>();
            builder.RegisterType<UserBus>().As<IUserBus>();
            builder.RegisterType<FactoryBus>().As<IFactoryBus>();
            builder.RegisterType<WorkshopBus>().As<IWorkshopBus>();
            builder.RegisterType<StateTypeBus>().As<IStateTypeBus>();
            builder.RegisterType<DeviceDataBus>().As<IDeviceDataBus>();
            builder.RegisterType<AlarmInfoBus>().As<IAlarmInfoBus>();
            builder.RegisterType<ThresholdBus>().As<IThresholdBus>();
            builder.RegisterType<LoginBus>().As<ILoginBus>();
            builder.RegisterType<FieldBus>().As<IFieldBus>();
            builder.RegisterType<SeverityBus>().As<ISeverityBus>();
            builder.RegisterType<ThemeBus>().As<IThemeBus>();
            builder.RegisterType<DeviceDailyOnlineTimeBus>().As<IDeviceDailyOnlineTimeBus>();
            builder.RegisterType<StaffBus>().As<IStaffBus>();
            //IoTManager.Dao
            // builder.RegisterType<CityDao>().As<ICityDao>();
            // builder.RegisterType<DepartmentDao>().As<IDepartmentDao>();
            // builder.RegisterType<DeviceDao>().As<IDeviceDao>();
            // builder.RegisterType<GatewayDao>().As<IGatewayDao>();
            // builder.RegisterType<UserDao>().As<IUserDao>();
            // builder.RegisterType<FactoryDao>().As<IFactoryDao>();
            // builder.RegisterType<WorkshopDao>().As<IWorkshopDao>();
            // builder.RegisterType<StateTypeDao>().As<IStateTypeDao>();
            // builder.RegisterType<DeviceDataDao>().As<IDeviceDataDao>();
            // builder.RegisterType<AlarmInfoDao>().As<IAlarmInfoDao>();
            // builder.RegisterType<ThresholdDao>().As<IThresholdDao>();
            // builder.RegisterType<FieldDao>().As<IFieldDao>();
            // builder.RegisterType<SeverityDao>().As<ISeverityDao>();
            //IoTManager.Dao-MySQL
            builder.RegisterType<MySQLCityDao>().As<ICityDao>();
            builder.RegisterType<MySQLDepartmentDao>().As<IDepartmentDao>();
            builder.RegisterType<MySQLDeviceDao>().As<IDeviceDao>();
            builder.RegisterType<MySQLGatewayDao>().As<IGatewayDao>();
            builder.RegisterType<MySQLUserDao>().As<IUserDao>();
            builder.RegisterType<MySQLFactoryDao>().As<IFactoryDao>();
            builder.RegisterType<MySQLWorkshopDao>().As<IWorkshopDao>();
            builder.RegisterType<MySQLStateTypeDao>().As<IStateTypeDao>();
            builder.RegisterType<DeviceDataDao>().As<IDeviceDataDao>();
            builder.RegisterType<AlarmInfoDao>().As<IAlarmInfoDao>();
            builder.RegisterType<MySQLThresholdDao>().As<IThresholdDao>();
            builder.RegisterType<MySQLFieldDao>().As<IFieldDao>();
            builder.RegisterType<MySQLSeverityDao>().As<ISeverityDao>();
            builder.RegisterType<MySQLAuthDao>().As<IAuthDao>();
            builder.RegisterType<MySQLRoleDao>().As<IRoleDao>();
            builder.RegisterType<MySQLThemeDao>().As<IThemeDao>();
            builder.RegisterType<MySQLDeviceDailyOnlineTimeDao>().As<IDeviceDailyOnlineTimeDao>();
            builder.RegisterType<MySQLStaffDao>().As<IStaffDao>();
            //IoTManager.AzureIoTHub
            builder.RegisterType<AzureIoTHub.AzureIoTHub>().As<IoTHub>();
            //base.Load(builder);
        }
    }
}
