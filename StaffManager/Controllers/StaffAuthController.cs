using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StaffManager.Core.Infrastructures;
using StaffManager.Models;

namespace StaffManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffAuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IStaffAuthBus _staffAuthBus;

        public StaffAuthController(IStaffAuthBus staffAuthBus,ILogger<StaffAuthController> logger)
        {
            _logger = logger;
            _staffAuthBus = staffAuthBus;
        }
        [HttpPost("{staffId}/add")]
        public ResponseSerializer AddAuth(string staffId, [FromBody] StaffAuthModel staffAuth)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _staffAuthBus.AddAuth(staffAuth);
                if (d == "success")
                {
                    code = 200;
                    msg = d;
                }
                else if (d == "exist")
                {
                    code = StatusCodes.Status409Conflict;
                    msg = d;
                }
                else
                {
                    code = 500;
                    msg = d;
                }
            }
            catch (Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpPost("{staffId}/BatchAddAuth")]
        public ResponseSerializer BatchAddAuth(string staffId, [FromBody] List<string> deviceIds)
        {
            int code;
            string msg;
            object d;
            try
            {
                d = _staffAuthBus.BatchAddAuth(staffId, deviceIds);
                code = 200;
                msg = "success";

            }
            catch (Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpGet("{staffId}/GetAuthDevice")]
        public ResponseSerializer GetAuthDevice(string staffId)
        {
            int code;
            string msg;
            object d;
            try
            {
                d = _staffAuthBus.GetAuthDevice(staffId);
                code = 200;
                msg = "success";

            }
            catch (Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpDelete("{staffId}/Delete/{deviceId}")]
        public ResponseSerializer DeleteAuth(string staffId, string deviceId)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _staffAuthBus.DeleteAuth(staffId, deviceId);
                if (d == "success")
                {
                    code = 200;
                    msg = "success";
                }
                else
                {
                    code = 500;
                    msg = d;
                }

            }
            catch (Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpDelete("{staffId}/BatchDelete")]
        public ResponseSerializer BatchDeleteAuth(string staffId, [FromBody] List<string> deviceIds)
        {
            int code;
            string msg;
            object d;
            try
            {
                d = _staffAuthBus.BatchDeleteAuth(staffId, deviceIds);
                msg = "success";
                code = 200;
            }
            catch (Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpGet("device/{deviceId}/ListStaffIds")]
        public ResponseSerializer ListStaffIdsByDeviceId(string deviceId)
        {
            int code;
            string msg;
            object d;
            try
            {
                d = _staffAuthBus.ListStaffIdsByDeviceId(deviceId);
                msg = "success";
                code = 200;
            }
            catch (Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpPost("device/{deviceId}/AddAuthByDepartment/{departmentName}")]
        public ResponseSerializer AddAuthByDepartment(string deviceId, string departmentName)
        {
            int code;
            string msg;
            int d;
            try
            {
                d = _staffAuthBus.AddAuthByDepartment(deviceId, departmentName);
                if (d != -1)
                {
                    code = 200;
                    msg = "success";
                }
                else
                {
                    code = StatusCodes.Status400BadRequest;
                    msg = "No staff In this Department";
                }
            }
            catch (Exception e)
            {
                code = 500;
                msg = e.Message;
                d = -1;
            }
            return new ResponseSerializer(code, msg, d);
        }
    }
}