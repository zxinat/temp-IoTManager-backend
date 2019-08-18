using System;
using IoTManager.Core.Infrastructures;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

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
        public ResponseSerializer Get(String searchType, String city, String factory, String workshop, int page = 1, String sortColumn = "Id", String order = "asc")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetAllDeviceData(searchType, city, factory, workshop, page, sortColumn, order));
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

        [HttpGet("number")]
        public ResponseSerializer GetDeviceDataNumber(String searchType, String city = "all", String factory = "all", String workshop = "all")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceDataNumber(searchType, city, factory, workshop));
        }
    }
}