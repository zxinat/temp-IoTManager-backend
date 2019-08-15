using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.AspNetCore.Http;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceBus _deviceBus;

        public DeviceController(IDeviceBus deviceBus)
        {
            this._deviceBus = deviceBus;
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
        public ResponseSerializer uploadPicture([FromForm]IFormCollection data)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.UploadPicture(data)
            );
        }

    }
}