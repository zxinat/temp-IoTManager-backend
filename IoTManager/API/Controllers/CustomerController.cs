using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.API.Formalizers;
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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerBus _customerBus;
        private readonly ILogger _logger;
        public CustomerController(ICustomerBus customerBus,ILogger<CustomerController> logger)
        {
            _customerBus = customerBus;
            _logger = logger;
        }
        //GET api/customer
        [HttpGet]
        public ResponseSerializer List(string searchType, int page = 1, string sortColumn = "id", string order = "asc")
        {
            return new ResponseSerializer(
                200, "success", _customerBus.ListCustomers(searchType, page, sortColumn, order));
        }
        //POST api/customer/create
        [HttpPost("create")]
        public ResponseSerializer Create([FromBody] CustomerFormModel customerForm)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _customerBus.Create(customerForm);
                if(d=="success")
                {
                    code = 200;msg = d;
                }
                else if(d=="error")
                {
                    code = StatusCodes.Status500InternalServerError;
                    msg = StatusCodes.Status500InternalServerError.ToString();
                }
                else
                {
                    code = StatusCodes.Status409Conflict;
                    msg = "conflict";
                }
            }
            catch(Exception e)
            {
                code = 500;
                msg = "error";
                d = null;
            }
            return new ResponseSerializer(code, msg, d);
        }




        [HttpGet("/Logout/{staffId}")]
        public ResponseSerializer Logout(string staffId)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _customerBus.Logout(staffId);
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
                    code = StatusCodes.Status406NotAcceptable;
                    msg = "未注册或禁止注销";
                    
                }
            }
            catch(Exception e)
            {
                code = 500;
                msg = "error";
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
                d = _customerBus.Delete(id);
                if(d=="success")
                {
                    code = 200;
                    msg = d;
                }
                else if(d=="NoContent")
                {
                    code = StatusCodes.Status400BadRequest;
                    msg = $"id = {id} 不存在";
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

        [HttpGet("Auth/AddAuth/{id}/{deviceId}")]
        public ResponseSerializer AddAuth(int id,string deviceId)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _customerBus.AddAuth(id, deviceId);
                if(d=="success")
                {
                    code = 200;
                    msg = d;
                }
                else if(d=="conflict")
                {
                    code = StatusCodes.Status409Conflict;
                    msg = $"{deviceId}:{id} 已授权";
                }
                else if(d=="noContent")
                {
                    code = StatusCodes.Status406NotAcceptable;
                    msg= $"id={id} 的访客不存在";
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
    }
}