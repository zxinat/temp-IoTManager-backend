/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IoTManager.Core.Infrastructures;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentBus _departmentBus;
        private readonly ILogger _logger;
        public DepartmentController(ILogger<DepartmentController> logger,IDepartmentBus departmentBus)
        {
            _departmentBus = departmentBus;
            _logger = logger;
        }
        //GET api/department
        [HttpGet]
        public ResponseSerializer List()
        {
            return new ResponseSerializer(
                200, "success", _departmentBus.List());
        }
        [HttpGet("GetByName/{departmentName}")]
        public ResponseSerializer GetByName(string departmentName)
        {
            return new ResponseSerializer(
                200, "success", _departmentBus.GetByName(departmentName));
        }
        [HttpGet("GetById/{id}")]
        public ResponseSerializer GetById(int id)
        {
            return new ResponseSerializer(
                200, "success", _departmentBus.GetById(id));
        }
        [HttpPost("create")]
        public ResponseSerializer Create([FromBody] DepartmentSerializer department)
        {
            var result = _departmentBus.Create(department);
            int code;
            if(result=="success")
            {
                code = StatusCodes.Status200OK;
            }
            else if(result=="exist")
            {
                code = StatusCodes.Status409Conflict;
            }
            else
            {
                code = StatusCodes.Status500InternalServerError;
            }
            return new ResponseSerializer(
                code, result, result);
        }
        [HttpDelete("delete/{departmentName}")]
        public ResponseSerializer Delete(string departmentName)
        {
            return new ResponseSerializer(
                200, "success", _departmentBus.Delete(departmentName));
        }
        [HttpPut("update/{id}")]
        public ResponseSerializer Update(int id,[FromBody] DepartmentSerializer department)
        {
            return new ResponseSerializer(
                200, "success", _departmentBus.Update(id,department));
        }
        [HttpGet("{department}/ListStaffs")]
        public ResponseSerializer ListStaffsByDepartment(string department)
        {
            return new ResponseSerializer(
                200, "success", _departmentBus.ListStaffsByDepartment(department));
        }
        [HttpGet("{department}/GetTotal")]
        public ResponseSerializer GetTotalByDepartment(string department)
        {
            return new ResponseSerializer(
                200, "success", _departmentBus.GetTotalByDepartment(department));
        }
        [HttpGet("GetTotalNumber")]
        public ResponseSerializer GetTotalNumber()
        {
            return new ResponseSerializer(
                200, "success", _departmentBus.GetTotalNumber());
        }
    }
}
*/