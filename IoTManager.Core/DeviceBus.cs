/*
 * DeviceBus.cs
 * 设备管理的业务逻辑层
 */

using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.IHub;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Azure.Devices;
using Microsoft.AspNetCore.Http;
using System.IO;
using DotNetty.Common.Utilities;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using MongoDB.Driver;

namespace IoTManager.Core
{
    public sealed class DeviceBus:IDeviceBus
    {
        private readonly IDeviceDao _deviceDao;
        private readonly IFieldDao _fieldDao;
        private readonly ICityDao _cityDao;
        private readonly IoTHub _iotHub;
        private readonly ILogger _logger;
        
        /*
         * 构造函数
         * 需要注入DeviceDao, FieldDao, Logger
         */
        public DeviceBus(IDeviceDao deviceDao, 
            IFieldDao fieldDao, 
            ICityDao cityDao,
            IoTHub iotHub,
            ILogger<DeviceBus> logger)
        {
            this._deviceDao = deviceDao;
            this._fieldDao = fieldDao;
            this._cityDao = cityDao;
            this._iotHub = iotHub;
            this._logger = logger;
        }

        /*
         * 获取所有设备，带分页功能
         *
         * 输入：
         * 搜索类型
         * 页码
         * 排序列
         * 升序或降序
         * 城市
         * 工厂
         * 车间
         *
         * 输出：
         * 设备列表
         */
        public List<DeviceSerializer> GetAllDevices(String searchType, int page, String sortColumn, String order, String city, String factory, String workshop)
        {
            int offset = (page - 1) * 12;
            int limit = 12;
            List<DeviceModel> devices = this._deviceDao.Get(searchType, offset, limit, sortColumn, order, city, factory, workshop);
            List<DeviceSerializer> result = new List<DeviceSerializer>();
            foreach (DeviceModel device in devices)
            {
                DeviceSerializer d = new DeviceSerializer(device);
                List<String> tags = this._deviceDao.GetDeviceTag(device.Id);
                d.tags = tags;
                result.Add(d);
            }
            return result;
        }

        /*
         * 根据特定ID（数据库ID）获取设备
         *
         * 输入：
         * ID
         *
         * 输出：
         * 该ID对应设备
         */
        public DeviceSerializer GetDeviceById(int id)
        {
            DeviceModel device = this._deviceDao.GetById(id);
            DeviceSerializer result = new DeviceSerializer(device);
            return result;
        }

        /*
         * 根据设备名称获取设备（模糊搜索）
         *
         * 输入：
         * 设备名称
         *
         * 输出：
         * 设备名称含有搜索内容的设备
         */
        public List<DeviceSerializer> GetDevicesByDeviceName(String deviceName)
        {
            List<DeviceModel> devices = this._deviceDao.GetByDeviceName(deviceName);
            List<DeviceSerializer> result = new List<DeviceSerializer>();
            foreach (DeviceModel device in devices)
            {
                result.Add(new DeviceSerializer(device));
            }
            return result;
        }

        /*
         * 根据设备编号获取设备（模糊搜索）
         *
         * 输入：
         * 设备编号
         *
         * 输出：
         * 设备编号含有搜索内容的设备
         */
        public List<DeviceSerializer> GetDevicesByFuzzyDeviceId(String deviceId)
        {
            List<DeviceModel> devices = this._deviceDao.GetByFuzzyDeviceId(deviceId);
            List<DeviceSerializer> result = new List<DeviceSerializer>();
            foreach (DeviceModel device in devices)
            {
                result.Add(new DeviceSerializer(device));
            }
            return result;
        }

        /*
         * 根据设备编号获取设备（精确搜索）
         *
         * 输入：
         * 设备编号
         *
         * 输出：
         * 对应设备编号的设备
         */
        public DeviceSerializer GetDeviceByDeviceId(String deviceId)
        {
            DeviceModel device = this._deviceDao.GetByDeviceId(deviceId);
            return new DeviceSerializer(device);
        }

        /*
         * 创建新设备
         */
        public String CreateNewDevice(DeviceSerializer deviceSerializer)
        {
            DeviceModel deviceModel = new DeviceModel();
            deviceModel.HardwareDeviceId = deviceSerializer.hardwareDeviceID;
            deviceModel.DeviceName = deviceSerializer.deviceName;
            deviceModel.City = deviceSerializer.city;
            deviceModel.Factory = deviceSerializer.factory;
            deviceModel.Workshop = deviceSerializer.workshop;
            deviceModel.DeviceState = deviceSerializer.deviceState;
            deviceModel.ImageUrl = deviceSerializer.imageUrl;
            deviceModel.GatewayId = deviceSerializer.gatewayId;
            deviceModel.Mac = deviceSerializer.mac;
            deviceModel.DeviceType = deviceSerializer.deviceType;
            deviceModel.Remark = deviceSerializer.remark;
            deviceModel.IsOnline = deviceSerializer.isOnline;
            deviceModel.Base64Image = deviceSerializer.base64Image;
            return this._deviceDao.Create(deviceModel);
        }

        /*
         * 更新特定ID（数据库ID）设备
         */
        public String UpdateDevice(int id, DeviceSerializer deviceSerializer)
        {
            DeviceModel deviceModel = new DeviceModel();
            deviceModel.Id = id;
            deviceModel.HardwareDeviceId = deviceSerializer.hardwareDeviceID;
            deviceModel.DeviceName = deviceSerializer.deviceName;
            deviceModel.City = deviceSerializer.city;
            deviceModel.Factory = deviceSerializer.factory;
            deviceModel.Workshop = deviceSerializer.workshop;
            deviceModel.DeviceState = deviceSerializer.deviceState;
            deviceModel.ImageUrl = deviceSerializer.imageUrl;
            deviceModel.GatewayId = deviceSerializer.gatewayId;
            deviceModel.Mac = deviceSerializer.mac;
            deviceModel.DeviceType = deviceSerializer.deviceType;
            deviceModel.Remark = deviceSerializer.remark;
            deviceModel.PictureRoute = deviceSerializer.pictureRoute;
            deviceModel.IsOnline = deviceSerializer.isOnline;
            deviceModel.Base64Image = deviceSerializer.base64Image;
            return this._deviceDao.Update(id, deviceModel);
        }

        /*
         * 删除特定ID（数据库ID）设备
         *
         * 输入：
         * ID
         * 
         * 输出：
         * 无
         */
        public String DeleteDevice(int id)
        {
            return this._deviceDao.Delete(id);
        }

        /*
         * 批量删除
         *
         * 输入：
         * 待删除ID（数据库ID）列表
         *
         * 输出：
         * 无
         */
        public int BatchDeleteDevice(int[] id)
        {
            return this._deviceDao.BatchDelete(id);
        }

        /*
         * 根据城市、工厂、车间搜索设备
         *
         * 输入：
         * 城市
         * 工厂
         * 车间
         *
         * 输出：
         * 对应条件下的设备列表
         */
        public List<DeviceSerializer> GetDeviceByWorkshop(String city, String factory, String workshop)
        {
            List<DeviceModel> devices = _deviceDao.GetByWorkshop(city, factory, workshop);
            List<DeviceSerializer> result = new List<DeviceSerializer>();
            foreach (DeviceModel d in devices)
            {
                result.Add(new DeviceSerializer(d));
            }

            return result;
        }

        /*
         * 获取设备总数
         */
        public int GetDeviceAmount()
        {
            return this._deviceDao.GetDeviceAmount();
        }

        /*
         * 根据城市、工厂获取设备树
         * 此函数主要用于监控配置页面显示左侧设备栏
         *
         * 输入：
         * 城市
         * 工厂
         *
         * 输出：
         * 对应条件下的设备树
         */
        public List<object> GetDeviceTree(String city, String factory)
        {
            return this._deviceDao.GetDeviceTree(city, factory);
        }

        /*
         * 创建设备类型
         */
        public String CreateDeviceType(String deviceType)
        {
            return this._deviceDao.CreateDeviceType(deviceType);
        }

        /*
         * 获取特定分页条件下的设备数量
         *
         * 输入：
         * 搜索类型
         * 城市
         * 工厂
         * 车间
         *
         * 输出：
         * 设备数量
         */
        public long GetDeviceNumber(String searchType, String city="all", String factory="all", String workshop="all")
        {
            return this._deviceDao.GetDeviceNumber(searchType, city, factory, workshop);
        }

        /*
         * 获取设备属性树
         */
        public List<object> GetFieldOptions()
        {
            List<DeviceModel> devices = this._deviceDao.Get("all");
            List<FieldModel> fields = this._fieldDao.Get();
            List<object> result = new List<object>();
            foreach (DeviceModel d in devices)
            {
                var affiliateFields = fields.AsQueryable()
                    .Where(f => f.Device == d.DeviceName)
                    .ToList();
                List<object> children = new List<object>();
                foreach (var f in affiliateFields)
                {
                    children.Add(new {value=f.FieldId, label=f.FieldName});
                }
                result.Add(new {value=d.HardwareDeviceId, label=d.DeviceName, children=children});
            }

            return result;
        }

        /*
         * 上传设备图片
         */
        public String UploadPicture(PictureUploadSerializer data)
        {
            try{
                DeviceSerializer device = this.GetDeviceByDeviceId(data.deviceId);
                device.base64Image = data.picture;
                this.UpdateDevice(device.id, device);
                return "图片上传成功";
            }
            catch(Exception ex){
                return ex.Message;
            }

        }

        /*
         * 获取设备图片
         */
        public String GetPicture(String deviceId)
        {
            try
            {
                DeviceSerializer device = this.GetDeviceByDeviceId(deviceId);
                FileInfo file = new FileInfo(device.pictureRoute);
                var stream = file.OpenRead();
                byte[] buffer = new byte[file.Length];
                stream.Read(buffer, 0, Convert.ToInt32(file.Length));
                stream.Close();
                return Convert.ToBase64String(buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /*
         * 根据城市、工厂、车间搜索设备
         *
         * 输入：
         * 城市
         * 工厂
         * 车间
         *
         * 输出：
         * 对应条件下的设备列表
         */
        public List<DeviceSerializer> GetDeviceByCity(String cityName, String factoryName, String workshopName)
        {
            List<DeviceModel> devices = this._deviceDao.Get("all");
            var query = devices.AsQueryable();
            if (factoryName == "all" && workshopName == "all")
            {
                var r = query.Where(d => d.City == cityName).ToList();
                List<DeviceSerializer> result = new List<DeviceSerializer>();
                foreach (DeviceModel d in r)
                {
                    result.Add(new DeviceSerializer(d));
                }

                return result;
            }
            if (factoryName != "all" && workshopName == "all")
            {
                var r = query.Where(d => d.City == cityName && d.Factory == factoryName).ToList();
                List<DeviceSerializer> result = new List<DeviceSerializer>();
                foreach (DeviceModel d in r)
                {
                    result.Add(new DeviceSerializer(d));
                }

                return result;
            }
            if (factoryName != "all" && workshopName != "all")
            {
                var r = query.Where(d => d.City == cityName && d.Factory == factoryName && d.Workshop == workshopName).ToList();
                List<DeviceSerializer> result = new List<DeviceSerializer>();
                foreach (DeviceModel d in r)
                {
                    result.Add(new DeviceSerializer(d));
                }

                return result;
            }

            return null;
        }
        
        /*
         * 根据标签获取设备
         *
         * 输入：
         * 标签
         *
         * 输出：
         * 对应设备列表
         */
        public List<DeviceSerializer> GetDeviceByTag(String tag)
        {
            List<DeviceModel> deviceModels = this._deviceDao.GetDeviceByTag(tag);
            List<DeviceSerializer> deviceSerializers = new List<DeviceSerializer>();
            foreach (var dm in deviceModels)
            {
                deviceSerializers.Add(new DeviceSerializer(dm));
            }

            return deviceSerializers;
        }

        /*
         * 获取所有标签
         *
         * 输入：
         * 无
         *
         * 输出：
         * 标签列表
         */
        public List<String> GetAllTag()
        {
            return this._deviceDao.GetAllTag();
        }

        /*
         * 设置设备的标签
         *
         * 输入：
         * 设备编号
         * 标签编号列表
         *
         * 输出：
         * 无
         */
        public Object SetDeviceTag(int deviceId, List<String> tagId)
        {
            return this._deviceDao.SetDeviceTag(deviceId, tagId);
        }

        /*
         * 根据设备的ID（数据库ID）获取标签
         *
         * 输入：
         * ID
         *
         * 输出：
         * 标签列表
         */
        public List<String> GetDeviceTag(int id)
        {
            return this._deviceDao.GetDeviceTag(id);
        }

        /*
         * 新增标签
         */
        public String AddTag(String tagName)
        {
            return this._deviceDao.AddTag(tagName);
        }

        /*
         * 删除标签
         */
        public String DeleteTag(String tagName)
        {
            return this._deviceDao.DeleteTag(tagName);
        }

        /*
         * 根据标签获取下属设备数量
         *
         * 输入：
         * 标签
         *
         * 输出：
         * 该标签下设备的数量
         */
        public int FindTagAffiliate(String tagName)
        {
            return this._deviceDao.FindTagAffiliate(tagName);
        }

        /*
         * 查找是否存在相应设备编号的设备
         *
         * 输入：
         * 设备编号
         *
         * 输出：
         * 是否存在此设备编号的设备
         */
        public int FindDeviceIdExist(String deviceId)
        {
            return this._deviceDao.FindDeviceIdExist(deviceId);
        }

        /*
         * 根据设备编号更新最后连接时间
         *
         * 输入：
         * 设备编号
         *
         * 输出：
         * 无
         */
        public int UpdateLastConnectionTimeByDeviceId(String deviceId)
        {
            return this._deviceDao.UpdateLastConnectionTimeByDeviceId(deviceId);
        }

        /*
         * 根据设备名称查询设备（所属的城市的）位置（精确查询）
         *
         * 输入：
         * 设备名称
         *
         * 输出：
         * 设备城市位置
         * 
         */
        public Object GetDeviceLocationByDeviceName(String deviceName)
        {
            DeviceModel device = this._deviceDao.GetByDeviceNamePrecise(deviceName);
            CityModel city = this._cityDao.GetOneCityByName(device.City);
            return new
            {
                longitude = city.longitude,
                latitude = city.latitude
            };
        }
    }
}
