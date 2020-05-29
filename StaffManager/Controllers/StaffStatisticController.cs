using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StaffManager.Core.Infrastructures;

namespace StaffManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffStatisticController : ControllerBase
    {
        private readonly IStatisticBus _statisticBus;
        private readonly ILogger _logger;

        public StaffStatisticController(IStatisticBus statisticBus, ILogger<StaffStatisticController> logger)
        {
            _statisticBus = statisticBus;
            _logger = logger;
        }
        [HttpGet("GetCurrentStaffOnShop/{deviceId}")]
        public ResponseSerializer GetCurrentStaffOnShop(string deviceId)
        {
            string message;
            int code;
            object result;
            try
            {
                result = _statisticBus.GetCurrentStaffOnShop(deviceId);
                code = 200;
                message = "success";
            }
            catch (Exception e)
            {
                if (e.Message == "No staff in This Device")
                {
                    code = StatusCodes.Status404NotFound;
                }
                else
                {
                    code = StatusCodes.Status500InternalServerError;
                }
                message = e.Message;
                result = null;
            }
            return new ResponseSerializer(
                code, message, result);
        }
        [HttpGet("GetStatisticData/{deviceId}")]
        public ResponseSerializer GetStatisticData(string deviceId)
        {
            return new ResponseSerializer(
                200, "success", _statisticBus.GetStatisticData(deviceId));
        }
        [HttpPost("GetAttendenceRecords")]
        public ResponseSerializer GetAttendenceRecords([FromBody] DataStatisticRequestModel date)
        {
            return new ResponseSerializer(200, "success", _statisticBus.GetAttendenceRecords(date));
        }
        [HttpPost("GetAttendenceRecords/{deviceId}")]
        public ResponseSerializer GetAttendenceRecords(string deviceId,[FromBody] DataStatisticRequestModel date)
        {
            return new ResponseSerializer(200, "success", _statisticBus.GetAttendenceRecords(deviceId,date));
        }
        [HttpGet("GetYesterdayStaffOnShop/{deviceId}")]
        public ResponseSerializer GetYesterdayStaffOnShop(string deviceId)
        {
            string message;
            int code;
            object result;
            try
            {
                result = _statisticBus.GetYesterdayStaffOnShop(deviceId);
                code = 200;
                message = "success";
            }
            catch (Exception e)
            {
                if (e.Message == "No staff in This Device")
                {
                    code = StatusCodes.Status404NotFound;
                }
                else
                {
                    code = StatusCodes.Status500InternalServerError;
                }
                message = e.Message;
                result = null;
            }
            return new ResponseSerializer(
                code, message, result);
        }
        [HttpGet("{deviceId}/ListLatestEnter10")]
        public ResponseSerializer ListLatest10Enter(string deviceId)
        {
            return new ResponseSerializer(
                200, "success", _statisticBus.ListLatestEnter10(deviceId));
        }
        [HttpGet("{deviceId}/ListLatestExit10")]
        public ResponseSerializer ListLatest10Exit(string deviceId)
        {
            return new ResponseSerializer(
                200, "success", _statisticBus.ListLatestExit10(deviceId));
        }
        [HttpGet("{deviceId}/GetLatestOne")]
        public ResponseSerializer GetLatestOne(string deviceId)
        {
            return new ResponseSerializer(
                200, "success", _statisticBus.GetLatestOne(deviceId));
        }
        [HttpGet("Status/{deviceId}/{staffId}")]
        public ResponseSerializer GetStatusByStaffId(string deviceId, string staffId)
        {
            return new ResponseSerializer(
                200, "success", _statisticBus.GetCurrentStatusByStaffId(deviceId, staffId));
        }
        [HttpPost("{staffId}/GetPersonalAttendenceData")]
        public ResponseSerializer GetPersonalAttendenceData(string staffId, [FromBody] DataStatisticRequestModel date, string statisticType = "week")
        {
            return new ResponseSerializer(
                200, "success", _statisticBus.GetPersonalAttendenceData(staffId, date, statisticType));
        }
        [HttpPost("GetDepartmentAttendenceData")]
        public ResponseSerializer GetDepartmentAttendenceData([FromBody] DataStatisticRequestModel date, string statisticType = "week")
        {
            return new ResponseSerializer(
                200, "success", _statisticBus.GetDepartmentAttendenceData(date, statisticType));
        }
        [HttpGet("GetLatestPersonalData/{staffId}")]
        public ResponseSerializer GetLastPersonalData(string staffId)
        {
            return new ResponseSerializer(
                200, "success", _statisticBus.GetCurrentData(staffId));

        }
        [HttpPost("GetPersonalRecords/{staffId}")]
        public ResponseSerializer GetPersonalRecords(string staffId,[FromBody] DataStatisticRequestModel date)
        {
            return new ResponseSerializer(
                200, "success", _statisticBus.GetPersonalRecord(staffId, date));
        }
        [HttpGet("GetExitInfo/{deviceId}")]
        public ResponseSerializer GetExitInfo(string deviceId)
        {
            return new ResponseSerializer(200,"success",_statisticBus.GetExitInfo(deviceId));
        }
    }
}