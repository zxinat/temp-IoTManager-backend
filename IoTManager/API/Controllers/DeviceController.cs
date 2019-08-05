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
using IoTManager.Utility.Serializers;

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
        public ResponseSerializer Get(int page)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceBus.GetAllDevices(page));
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
                this._deviceBus.GetDevicesByDeviceId(deviceId));
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
    }
}