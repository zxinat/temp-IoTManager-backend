using System;
using IoTManager.Core.Infrastructures;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlarmInfoController
    {
        private readonly IAlarmInfoBus _alarmInfoBus;
        private readonly ILogger _logger;

        public AlarmInfoController(IAlarmInfoBus alarmInfoBus, ILogger<AlarmInfoController> logger)
        {
            this._alarmInfoBus = alarmInfoBus;
            this._logger = logger;
        }
        
        //GET api/alarmInfo
        [HttpGet]
        public ResponseSerializer Get(String searchType, String deviceId, int page = 1, String sortColumn = "Id", String order = "asc")
        {
            return new ResponseSerializer(
                200,
                "success",
                _alarmInfoBus.GetAllAlarmInfo(searchType, deviceId, page, sortColumn, order));
        }

        [HttpGet("id/{Id}")]
        public ResponseSerializer GetById(String Id)
        {
            return new ResponseSerializer(
                200,
                "success",
                _alarmInfoBus.GetAlarmInfoById(Id));
        }

        [HttpGet("deviceId/{DeviceId}")]
        public ResponseSerializer GetByDeviceId(String DeviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                _alarmInfoBus.GetAlarmInfoByDeviceId20(DeviceId));
        }

        [HttpGet("indexId/{IndexId}")]
        public ResponseSerializer GetByIndexId(String IndexId)
        {
            return new ResponseSerializer(
                200,
                "success",
                _alarmInfoBus.GetAlarmInfoByIndexId(IndexId));
        }

        [HttpGet("inspect")]
        public ResponseSerializer InspectAlarmInfo()
        {
            return new ResponseSerializer(
                200,
                "success",
                _alarmInfoBus.InspectAlarmInfo());
        }

        [HttpGet("dashboard")]
        public ResponseSerializer GetDashboardAlarmInfo()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._alarmInfoBus.GetFiveInfo());
        }

        [HttpGet("noticeAmount")]
        public ResponseSerializer GetNoticeAlarmInfoAmount()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._alarmInfoBus.GetNoticeAlarmInfoAmount());
        }
        
        [HttpGet("seriousAmount")]
        public ResponseSerializer GetSeriousAlarmInfoAmount()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._alarmInfoBus.GetSeriousAlarmInfoAmount());
        }
        
        [HttpGet("verySeriousAmount")]
        public ResponseSerializer GetVerySeriousAlarmInfoAmount()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._alarmInfoBus.GetVerySeriousAlarmInfoAmount());
        }

        [HttpPost("processed/{id}")]
        public ResponseSerializer UpdateProcessed(String id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._alarmInfoBus.UpdateProcessed(id));
        }

        [HttpGet("number")]
        public ResponseSerializer GetAlarmInfoNumber(String searchType, String deviceId = "all")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._alarmInfoBus.GetAlarmInfoNumber(searchType, deviceId));
        }

        [HttpDelete("{id}")]
        public ResponseSerializer Delete(String id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._alarmInfoBus.DeleteAlarmInfo(id));
        }

        [HttpPost("batchDelete")]
        public ResponseSerializer BatchDelete([FromBody] BatchString bs)
        {
            return new ResponseSerializer(200,
                "success",
                this._alarmInfoBus.BatchDelete(bs.str));
        }
        
    }
}