using System;
using IoTManager.Core.Infrastructures;
using IoTManager.Model;
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
        public ResponseSerializer Get(String searchType, String deviceId = "all", int page = 1, String sortColumn = "Id", String order = "asc")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetAllDeviceData(searchType, deviceId, page, sortColumn, order));
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
                this._deviceDataBus.GetDeviceDataByDeviceId20(DeviceId));
        }

        [HttpGet("{deviceId}/{indexId}")]
        public ResponseSerializer GetLineChartData(String deviceId, String indexId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetLineChartData(deviceId, indexId));
        }

        [HttpPost("multipleLineChart/{deviceId}")]
        public ResponseSerializer GetMultipleLineChartData(String deviceId, [FromBody] BatchString fields)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetMultipleLineChartData(deviceId, fields.str));
        }

        [HttpGet("amount")]
        public ResponseSerializer GetDeviceDataAmount()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceDataAmount());
        }

        [HttpPost("status/{id}")]
        public ResponseSerializer GetDeviceStatusById(int id, [FromBody] DataStatisticRequestModel date)
        {
            System.Console.WriteLine(date.EndTime);
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceStatusById(id, date.StartTime, date.EndTime));
        }

        [HttpGet("number")]
        public ResponseSerializer GetDeviceDataNumber(String searchType, String deviceId = "all")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceDataNumber(searchType, deviceId));
        }

        [HttpDelete("{id}")]
        public ResponseSerializer DeleteDeviceData(String id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.DeleteDeviceData(id));
        }

        [HttpPost("batch/deviceData")]
        public ResponseSerializer BatchDelete([FromBody] BatchString batchString)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.BatchDeleteDeviceData(batchString.str));
        }

        [HttpPut("{id}")]
        public ResponseSerializer Put(String id, [FromBody] DeviceDataSerializer deviceDataSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.UpdateDeviceData(id, deviceDataSerializer));
        }

        [HttpPost("aggregate/day/{deviceId}")]
        public ResponseSerializer GetDayAggregateData(String deviceId, String indexId, String scale, [FromBody] DataStatisticRequestModel request)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDayAggregateData(deviceId, indexId, request.StartTime, request.EndTime, scale));
        }

        [HttpPost("statistic/{deviceId}")]
        public ResponseSerializer GetDataStatistic(String deviceId, [FromBody] DataStatisticRequestModel request)
        {
            return new ResponseSerializer(
                200,
                "success",
                request);
        }

        [HttpGet("dataInDeviceCard")]
        public ResponseSerializer GetDeviceDataInDeviceCardByName(String deviceName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceDataInDeviceCardByName(deviceName)
                );
        }

        [HttpGet("dataInDeviceProperty")]
        public ResponseSerializer GetDeviceDataInDevicePropertyByName(String deviceName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetDeviceDataInDevicePropertyByName(deviceName)
                );
        }

        [HttpGet("dataInAlarmRecord")]
        public ResponseSerializer GetAlarmInfoInAlarmRecord(String deviceName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetAlarmInfoInAlarmRecordByName(deviceName));
        }

        [HttpGet("ruleInDeviceAlarmingRule")]
        public ResponseSerializer GetRuleInDeviceAlarmingRuleByName(String deviceName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetRuleInDeviceAlarmingRuleByName(deviceName)
                );
        }
    }
}