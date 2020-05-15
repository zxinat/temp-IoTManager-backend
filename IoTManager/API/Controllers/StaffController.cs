using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.Core.Infrastructures;
using IoTManager.Model;
using IoTManager.Model.RequestModel;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffBus _staffBus;
        private readonly ILogger _logger;

        public StaffController(IStaffBus staffBus, ILogger<StaffController> logger)
        {
            _staffBus = staffBus;
            _logger = logger;
        }

        //GET api/staff
        [HttpGet]
        public ResponseSerializer List(string searchType, string department = "all", int page = 1, string sortColumn = "id", string order = "asc")
        {
            //var result = _staffBus.ListStaffs(searchType, page, sortColumn, order, department);
            return new ResponseSerializer(
                200,
                "success",
                _staffBus.ListStaffs(searchType, page, sortColumn, order, department));
        }
        [HttpGet("{staffId}")]
        public ResponseSerializer GetByStaffId(string staffId)
        {
            int code;
            string msg;
            object d;
            try
            {
                d = _staffBus.GetByStaffId(staffId);
                code = 200;
                msg = "success";
            }
            catch(Exception e)
            {
                code = StatusCodes.Status404NotFound;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpPost("create")]
        public ResponseSerializer Create([FromBody] StaffFormModel staff)
        {
            var result = _staffBus.Create(staff);
            int code;
            if (result == "success")
            {
                code = StatusCodes.Status200OK;
            }
            else if (result == "exist")
            {
                code = StatusCodes.Status409Conflict;
            }
            else
            {
                code = StatusCodes.Status500InternalServerError;
            }

            return new ResponseSerializer(
                code,
                result,
                result);
        }
        [HttpDelete("Delete/{staffId}")]
        public ResponseSerializer Delete(string staffId)
        {
            return new ResponseSerializer(
                200,
                "success",
                _staffBus.Delete(staffId));
        }
        [HttpDelete("BatchDelete")]
        public ResponseSerializer BatchDelete([FromBody] List<string> staffIds)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.BatchDelete(staffIds));
        }
        [HttpPut("Update/{staffId}")]
        public ResponseSerializer Update(string staffId, [FromBody] StaffFormModel staffForm)
        {
            int code;
            string msg;
            string d;
            try
            {
                d=_staffBus.Update(staffId, staffForm);
                if(d=="success")
                {
                    code = 200;
                    msg = d;
                }
                else
                {
                    code = StatusCodes.Status400BadRequest;
                    msg = "BadRequest";
                }

            }
            catch(Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpGet("{staffId}/Logout")]
        public ResponseSerializer Logout(string staffId,string status)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _staffBus.Logout(staffId,status);
                if(d=="success")
                {
                    code = 200;
                    msg = d;
                }
                else
                {
                    code = 500;
                    msg = d;
                }
            }
            catch(Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpPut("UpLoadImage/{staffId}")]
        public ResponseSerializer UpLoadImage(string staffId, string base64Image)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _staffBus.UpLoadImage(staffId, base64Image);
                if(d=="success")
                {
                    code = 200;
                    msg = d;
                }
                else
                {
                    code = StatusCodes.Status400BadRequest;
                    msg = d;
                }
            }
            catch(Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpPost("Auth/{staffId}/add")]
        public ResponseSerializer AddAuth(string staffId, [FromBody] StaffAuthModel staffAuth)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _staffBus.AddAuth(staffAuth);
                if(d=="success")
                {
                    code = 200;
                    msg = d;
                }
                else if(d=="exist")
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
            catch(Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpPost("Auth/{staffId}/BatchAddAuth")]
        public ResponseSerializer BatchAddAuth(string staffId, [FromBody] List<string> deviceIds)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.BatchAddAuth(staffId, deviceIds));
        }
        [HttpGet("Auth/{staffId}/GetAuthDevice")]
        public ResponseSerializer GetAuthDevice(string staffId)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.GetAuthDevice(staffId));
        }
        [HttpDelete("Auth/{staffId}/Delete/{deviceId}")]
        public ResponseSerializer DeleetAuth(string staffId, string deviceId)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.DeleteAuth(staffId, deviceId));
        }
        [HttpDelete("Auth/{staffId}/BatchDelete")]
        public ResponseSerializer BatchDeleteAuth(string staffId, [FromBody] List<string> deviceIds)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.BatchDeleteAuth(staffId, deviceIds));
        }
        [HttpGet("Auth/device/{deviceId}/ListStaffIds")]
        public ResponseSerializer ListStaffIdsByDeviceId(string deviceId)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.ListStaffIdsByDeviceId(deviceId));
        }
        [HttpPost("Auth/device/{deviceId}/AddAuthByDepartment/{departmentName}")]
        public ResponseSerializer AddAuthByDepartment(string deviceId, string departmentName)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.AddAuthByDepartment(deviceId, departmentName));
        }
        [HttpGet("Statistics/GetCurrentStaffOnShop/{deviceId}")]
        public ResponseSerializer GetCurrentStaffOnShop(string deviceId)
        {
            string message;
            int code;
            object result;
            try
            {
                result = _staffBus.GetCurrentStaffOnShop(deviceId);
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
        [HttpGet("Statistics/GetYesterdayStaffOnShop/{deviceId}")]
        public ResponseSerializer GetYesterdayStaffOnShop(string deviceId)
        {
            string message;
            int code;
            object result;
            try
            {
                result = _staffBus.GetYesterdayStaffOnShop(deviceId);
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
        [HttpGet("Statistics/{deviceId}/ListLatestEnter10")]
        public ResponseSerializer ListLatest10Enter(string deviceId)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.ListLatestEnter10(deviceId));
        }
        [HttpGet("Statistics/{deviceId}/ListLatestExit10")]
        public ResponseSerializer ListLatest10Exit(string deviceId)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.ListLatestExit10(deviceId));
        }
        [HttpGet("Statistics/{deviceId}/GetLatestOne")]
        public ResponseSerializer GetLatestOne(string deviceId)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.GetLatestOne(deviceId));
        }
        [HttpGet("Status/{deviceId}/{staffId}")]
        public ResponseSerializer GetStatusByStaffId(string deviceId,string staffId)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.GetCurrentStatusByStaffId(deviceId, staffId));
        }
        [HttpGet("StaffRole/ListStaffRole")]
        public ResponseSerializer ListStaffRole()
        {
            return new ResponseSerializer(
                200, "success", _staffBus.ListStaffRole());
        }
        [HttpPost("StaffRole/AddStaffRole/{staffRole}")]
        public ResponseSerializer AddStaffRole(string staffRole)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.AddStaffRole(staffRole));
        }
        [HttpPut("StaffRole/UpdateStaffRole/{id}/{staffRole}")]
        public ResponseSerializer UpdateStaffRole(int id ,string staffRole)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.UpdateStaffRole(id, staffRole));
        }
        [HttpDelete("StaffRole/Delete/{id}")]
        public ResponseSerializer DeleteStaffRole(int id)
        {
            return new ResponseSerializer(
                200, "success", _staffBus.DeleteStaffRole(id));
        }
    }
}