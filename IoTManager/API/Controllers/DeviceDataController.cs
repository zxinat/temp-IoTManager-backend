using System;
using IoTManager.Core.Infrastructures;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceDataController
    {
        private readonly IDeviceDataBus _deviceDataBus;
        private readonly ILogger _logger;

        public DeviceDataController(IDeviceDataBus deviceDataBus, ILogger<DeviceDataController> logger)
        {
            this._deviceDataBus = deviceDataBus;
            this._logger = logger;
        }

        //GET api/deviceData
        [HttpGet]
        public ResponseSerializer Get(String searchType, int page, String sortColumn, String order, String city, String factory, String workshop)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetAllDeviceData(searchType, page, sortColumn, order, city, factory, workshop));
        }

        [HttpGet("id/{Id}")]
        public ResponseSerializer GetById(String Id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceDataById(Id));
        }

        [HttpGet("deviceId/{DeviceId}")]
        public ResponseSerializer GetByDeviceId(String DeviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceDataByDeviceId(DeviceId));
        }

        [HttpGet("{deviceId}/{indexId}")]
        public ResponseSerializer GetLineChartData(String deviceId, String indexId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetLineChartData(deviceId, indexId));
        }

        [HttpGet("amount")]
        public ResponseSerializer GetDeviceDataAmount()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceDataAmount());
        }

        [HttpGet("status/{id}")]
        public ResponseSerializer GetDeviceStatusById(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceStatusById(id));
        }
    }
}