using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StaffManager.Core.Infrastructures;
using IoTManager.Model;
using StaffManager.Models.RequestModel;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StaffManager.Models;

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
        [HttpPost("CreateData")]
        public ResponseSerializer CreateData([FromBody] StaffDataFormModel staffData)
        {
            int code;
            string msg;
            object d;
            try
            {
                _staffBus.CreateData(staffData);
                msg = "success";
                code = 200;
                d = msg;
            }
            catch(Exception e)
            {
                code = 500;
                msg = e.Message;
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpPost("BatchCreateData")]
        public ResponseSerializer BatchCreateData([FromBody] List<StaffDataFormModel> staffDataForms)
        {
            int code;
            string msg;
            int d;
            try
            {
                d = _staffBus.BatchCreateData(staffDataForms);
                msg = "success";
                code = 200;
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