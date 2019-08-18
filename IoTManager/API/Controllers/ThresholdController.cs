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
        public ResponseSerializer Get(String searchType, String city, String factory, String workshop, int page = 1, String sortColumn = "Id", String order = "asc")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._thresholdBus.GetAllRules(searchType, city, factory, workshop, page, sortColumn, order));
        }

        [HttpGet("number")]
        public ResponseSerializer GetThresholdNumber(String searchType, String city, String factory, String workshop)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._thresholdBus.GetThresholdNumber(searchType, city, factory, workshop));
        }
    }
}