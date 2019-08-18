using System;
using IoTManager.Core.Infrastructures;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThresholdController
    {
        private readonly IThresholdBus _thresholdBus;
        private readonly ILogger _logger;

        public ThresholdController(IThresholdBus thresholdBus, ILogger<ThresholdController> logger)
        {
            this._thresholdBus = thresholdBus;
            this._logger = logger;
        }

        [HttpGet("{id}")]
        public ResponseSerializer GetByDeviceId(String id)
        {
            return new ResponseSerializer(
                200,
                "success",
                _thresholdBus.GetByDeviceId(id));
        }

        [HttpPost]
        public ResponseSerializer Create([FromBody] ThresholdSerializer thresholdSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._thresholdBus.InsertThreshold(thresholdSerializer));
        }

        [HttpGet]
        public ResponseSerializer Get(String searchType, String deviceName = "all", int page = 1, String sortColumn = "id", String order = "asc")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._thresholdBus.GetAllRules(searchType, deviceName, page, sortColumn, order));
        }

        [HttpGet("number")]
        public ResponseSerializer GetThresholdNumber(String searchType, String deviceName = "all")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._thresholdBus.GetThresholdNumber(searchType, deviceName));
        }

        [HttpDelete("{id}")]
        public ResponseSerializer Delete(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._thresholdBus.DeleteThreshold(id));
        }

        [HttpPut("{id}")]
        public ResponseSerializer Put(int id, [FromBody] ThresholdSerializer thresholdSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._thresholdBus.UpdateThreshold(id, thresholdSerializer));
        }

        [HttpPost("batch/thresholds")]
        public ResponseSerializer BatchDelete([FromBody] BatchNumber batchNumber)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._thresholdBus.BatchDeleteThreshold(batchNumber.number));
        }
    }
}