using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StaffManager.Core.Infrastructures;
using StaffManager.Models.RequestModel;

namespace StaffManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RFIDConfigController : ControllerBase
    {
        private readonly IRFIDTagBus _rFIDTagBus;
        private readonly ILogger _logger;
        public RFIDConfigController(IRFIDTagBus rFIDTagBus,ILogger<RFIDConfigController> logger)
        {
            _rFIDTagBus = rFIDTagBus;
            _logger = logger;
        }
        [HttpGet]
        public ResponseSerializer ListAll(string searchType,int page=1,string sortColumn="id",string order="asc")
        {
            int code;
            string msg;
            object d;
            try
            {
                d = _rFIDTagBus.ListAll(searchType, page, sortColumn, order);
                code = 200;
                msg = "success";
            }
            catch(Exception e)
            {
                code = 500;
                msg = "error";
                d = null; 
            }
            return new ResponseSerializer(code, msg, d);
        }
        [HttpPost("AddRFIDTagConfig")]
        public ResponseSerializer Add([FromBody] RFIDTagFormModel rFIDTagForm)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _rFIDTagBus.Add(rFIDTagForm);
                if(d=="success")
                {
                    code = 200;
                    msg = d;
                }
                else if(d=="error")
                {
                    code = 500;
                    msg = d;
                }
                else
                {
                    code = StatusCodes.Status409Conflict;
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
        [HttpPut("Update/{id}")]
        public ResponseSerializer Update(int id,[FromBody] RFIDTagFormModel rFIDTagForm)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _rFIDTagBus.Update(id, rFIDTagForm);
                if(d=="sucess")
                {
                    code = 200;
                    msg = d;
                }
                else if(d=="No Content")
                {
                    code = StatusCodes.Status404NotFound;
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
        [HttpDelete("Delete/{id}")]
        public ResponseSerializer Delete(int id)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _rFIDTagBus.Delete(id);
                if(d=="success")
                {
                    code = 200;
                    msg = d;
                }
                else if(d=="No Content")
                {
                    code = StatusCodes.Status404NotFound;
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
        [HttpGet("Auth/CheckAuth/{deviceId}/{tagId}")]
        public ResponseSerializer CheckAuth(string deviceId,string tagId)
        {
            int code;
            string msg;
            object d;
            try
            {
                d = _rFIDTagBus.IsAuth(deviceId, tagId);
                code = 200;
                msg = "success";
            }
            catch(Exception e)
            {
                code = 500;
                msg = "error";
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }
    }
}