using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using IoTManager.API.Formalizers;
using Microsoft.AspNetCore.Mvc;
using IoTManager.DAL.ReturnType;
using IoTManager.DAL.Models;
using IoTManager.DAL.DbContext;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using IoTManager.Core.Infrastructures;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using System.Windows;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceBus _deviceBus;
        private readonly IDeviceDataBus _deviceDataBus;
        private readonly IAlarmInfoBus _alarmInfoBus;
        private readonly IThresholdBus _thresholdBus;

        public DeviceController(IDeviceBus deviceBus, IDeviceDataBus deviceDataBus, IAlarmInfoBus alarmInfoBus, IThresholdBus thresholdBus)
        {
            this._deviceBus = deviceBus;
            this._deviceDataBus = deviceDataBus;
            this._alarmInfoBus = alarmInfoBus;
            this._thresholdBus = thresholdBus;
        }

        // GET api/values
        [HttpGet]
        public ResponseSerializer Get(String searchType, int page, String sortColumn, String order, String city, String factory, String workshop)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetAllDevices(searchType, page, sortColumn, order, city, factory, workshop));
        }

        // GET api/values/{id}
        [HttpGet("{id}")]
        public ResponseSerializer GetById(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDeviceById(id));
        }
        
        // GET api/device/devicename/{deviceName}
        [HttpGet("devicename/{devicename}")]
        public ResponseSerializer GetByDeviceName(String deviceName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDevicesByDeviceName(deviceName));
        }
        
        //GET api/device/deviceid/{deviceId}
        [HttpGet("deviceid/{deviceId}")]
        public ResponseSerializer GetByDeviceId(String deviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDeviceByDeviceId(deviceId));
        }

        [HttpGet("fuzzyDeviceid/{deviceId}")]
        public ResponseSerializer GetByFuzzyDeviceId(String deviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDevicesByFuzzyDeviceId(deviceId)
            );
        }
        // POST api/values
        [HttpPost]
        public ResponseSerializer Post([FromBody] DeviceSerializer deviceSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.CreateNewDevice(deviceSerializer));
        }

        // PUT api/values/{id}
        [HttpPut("{id}")]
        public ResponseSerializer Put(int id, [FromBody] DeviceSerializer deviceSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.UpdateDevice(id, deviceSerializer));
        }

        // DELETE api/values/{id}
        [HttpDelete("{id}")]
        public ResponseSerializer Delete(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.DeleteDevice(id));
        }

        [HttpPost("batch/devices")]
        public ResponseSerializer BatchDelete([FromBody] BatchNumber batchNumber)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.BatchDeleteDevice(batchNumber.number));
        }

        [HttpGet("workshop/{cityName}/{factoryName}/{workshopName}")]
        public ResponseSerializer GetByDeviceWorkshop(String cityName, String factoryName, String workshopName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDeviceByWorkshop(cityName, factoryName, workshopName));
        }

        [HttpGet("amount")]
        public ResponseSerializer GetDeviceAmount()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDeviceAmount());
        }

        [HttpGet("tree/{city}/{factory}")]
        public ResponseSerializer GetDeviceTree(String city, String factory)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDeviceTree(city, factory));
        }

        [HttpPost("type/{deviceType}")]
        public ResponseSerializer CreateDeviceType(String deviceType)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.CreateDeviceType(deviceType));
        }

        [HttpGet("number")]
        public ResponseSerializer GetDeviceNumber(String searchType, String city, String factory, String workshop)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDeviceNumber(searchType, city, factory, workshop));
        }

        [HttpGet("fieldOptions")]
        public ResponseSerializer GetFieldOptions()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetFieldOptions());
        }
        [HttpPost("uploadPicture")]
        public ResponseSerializer UploadPicture([FromBody] PictureUploadSerializer pic)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.UploadPicture(pic)
            );
        }

        [HttpGet("getPicture/{deviceId}")]
        public ResponseSerializer GetPicture(String deviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetPicture(deviceId));
        }

        [HttpGet("getByCity")]
        public ResponseSerializer GetDeviceByCity(String cityName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDeviceByCity(cityName));
        }

        [HttpGet("dashboardStatus")]
        public ResponseSerializer GetDashboardDeviceStatus()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDashboardDeviceStatus());
        }

        [HttpGet("affiliateData/{deviceId}")]
        public ResponseSerializer GetDeviceAffiliateData(String deviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceAffiliateData(deviceId));
        }

        [HttpGet("affiliateAlarmInfo/{deviceId}")]
        public ResponseSerializer GetDeviceAffiliateAlarmInfo(String deviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._alarmInfoBus.GetDeviceAffiliateAlarmInfo(deviceId));
        }

        [HttpGet("affiliateThreshold/{deviceId}")]
        public ResponseSerializer GetDeviceAffiliateThreshold(String deviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._thresholdBus.GetDeviceAffiliateThreshold(deviceId));
        }
        
        [HttpGet("tag/{tag}")]
        public ResponseSerializer GetDeviceByTag(String tag)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDeviceByTag(tag));
        }

        [HttpGet("getalltag")]
        public ResponseSerializer GetAllTag()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetAllTag());
        }

        [HttpPost("deviceTag/{deviceId}")]
        public ResponseSerializer SetDeviceTag(int deviceId, [FromBody] BatchString str)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.SetDeviceTag(deviceId, str.str));
        }

        [HttpGet("deviceTag/{deviceId}")]
        public ResponseSerializer GetDeviceTag(int deviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetDeviceTag(deviceId));
        }

        [HttpPost("tag")]
        public ResponseSerializer AddTag(String tagName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.AddTag(tagName));
        }
    }
}