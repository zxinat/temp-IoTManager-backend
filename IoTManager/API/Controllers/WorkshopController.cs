using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.API.Formalizers;
using IoTManager.Core.Infrastructures;
using IoTManager.DAL.DbContext;
using IoTManager.DAL.Models;
using IoTManager.DAL.ReturnType;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkshopController : ControllerBase
    {
        private readonly IWorkshopBus _workshopBus;
        private readonly ILogger _logger;

        public WorkshopController(IWorkshopBus workshopBus, ILogger<WorkshopController> logger)
        {
            this._workshopBus = workshopBus;
            this._logger = logger;
        }
        // GET api/values
        [HttpGet]
        public ResponseSerializer Get(int pageMode = 0, int page = 1, String sortColumn = "id", String order = "asc")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.GetAllWorkshops(pageMode, page, sortColumn, order));
        }

        // GET api/values/{id}
        [HttpGet("{id}")]
        public ResponseSerializer Get(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.GetWorkshopById(id));
        }

        // POST api/values
        [HttpPost]
        public ResponseSerializer Post([FromBody] WorkshopSerializer workshopSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.CreateNewWorkshop(workshopSerializer));
        }

        // PUT api/values/{id}
        [HttpPut("{id}")]
        public ResponseSerializer Put(int id, [FromBody] WorkshopSerializer workshopSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.UpdateWorkshop(id, workshopSerializer));
        }

        // DELETE api/values/{id}
        [HttpDelete("{id}")]
        public ResponseSerializer Delete(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.DeleteWorkshop(id));
        }

        [HttpGet("workshopOptions/{cName}/{fName}")]
        public ResponseSerializer GetAffiliateWorkshop(String cName,String fName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.GetAffiliateWorkshop(cName,fName));
        }
        [HttpGet("workshopNameList/{cName}/{fName}")]
        public ResponseSerializer ListWorkshopName(string cName,string fName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.ListWorkshopName(cName, fName));
        }


        [HttpGet("workshopName/{workshopName}")]
        public ResponseSerializer GetByWorkshopName(String workshopName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.GetByWorkshopName(workshopName));
        }

        [HttpGet("affiliateDevice/{id}")]
        public ResponseSerializer GetWorkshopAffiliateDevice(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.GetWorkshopAffiliateDevice(id));
        }

        [HttpGet("affiliateGateway/{id}")]
        public ResponseSerializer GetWorkshopAffiliateGateway(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.GetWorkshopAffiliateGateway(id));
        }

        [HttpGet("number")]
        public ResponseSerializer GetWorkshopNumber()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._workshopBus.GetWorkshopNumber());
        }
    }
}