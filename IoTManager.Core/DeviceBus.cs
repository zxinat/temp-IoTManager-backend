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
        private readonly IoTHub _iotHub;
        private readonly ILogger _logger;
        public DeviceBus(IDeviceDao deviceDao, IFieldDao fieldDao, IoTHub iotHub,ILogger<DeviceBus> logger)
        {
            this._deviceDao = deviceDao;
            this._fieldDao = fieldDao;
            this._iotHub = iotHub;
            this._logger = logger;
        }

        public List<DeviceSerializer> GetAllDevices(String searchType, int page, String sortColumn, String order, String city, String factory, String workshop)
        {
            int offset = (page - 1) * 12;
            int limit = 12;
            List<DeviceModel> devices = this._deviceDao.Get(searchType, offset, limit, sortColumn, order, city, factory, workshop);
            List<DeviceSerializer> result = new List<DeviceSerializer>();
            foreach (DeviceModel device in devices)
            {
                result.Add(new DeviceSerializer(device));
            }
            return result;
        }

        public DeviceSerializer GetDeviceById(int id)
        {
            DeviceModel device = this._deviceDao.GetById(id);
            DeviceSerializer result = new DeviceSerializer(device);
            return result;
        }

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

        public DeviceSerializer GetDeviceByDeviceId(String deviceId)
        {
            DeviceModel device = this._deviceDao.GetByDeviceId(deviceId);
            return new DeviceSerializer(device);
        }

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
            return this._deviceDao.Create(deviceModel);
        }

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
            return this._deviceDao.Update(id, deviceModel);
        }

        public String DeleteDevice(int id)
        {
            return this._deviceDao.Delete(id);
        }

        public int BatchDeleteDevice(int[] id)
        {
            return this._deviceDao.BatchDelete(id);
        }

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

        public int GetDeviceAmount()
        {
            return this._deviceDao.GetDeviceAmount();
        }

        public List<object> GetDeviceTree(String city, String factory)
        {
            return this._deviceDao.GetDeviceTree(city, factory);
        }

        public String CreateDeviceType(String deviceType)
        {
            return this._deviceDao.CreateDeviceType(deviceType);
        }

        public long GetDeviceNumber(String searchType, String city="all", String factory="all", String workshop="all")
        {
            return this._deviceDao.GetDeviceNumber(searchType, city, factory, workshop);
        }

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

        public String UploadPicture(IFormCollection data)
        {
            try{
                IFormFileCollection files = data.Files;
                IFormFile picture = files.GetFile("picture");
                Console.WriteLine(picture.Length);
                String today = DateTime.Now.ToString("yyyyMMdd");
                var filePath = "D:/IoTManager/" + today + "-" + System.Guid.NewGuid().ToString() + picture.FileName;
                var stream = new FileStream(filePath, FileMode.Create);
                picture.CopyToAsync(stream);
                stream.Close();
                DeviceSerializer device = this.GetDeviceByDeviceId(data["deviceId"]);
                device.pictureRoute = filePath;
                this.UpdateDevice(device.id, device);
                return "图片上传成功";
            }
            catch(Exception ex){
                return ex.Message;
            }

        }

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
    }
}
