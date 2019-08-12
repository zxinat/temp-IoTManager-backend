using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.API.Formalizers;
using IoTManager.Core.Infrastructures;
using Microsoft.AspNetCore.Mvc;
using IoTManager.DAL.Models;
using IoTManager.DAL.DbContext;
using IoTManager.DAL.ReturnType;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GatewayController : ControllerBase
    {
        private readonly IGatewayBus _gatewayBus;
        private readonly ILogger _logger;

        public GatewayController(IGatewayBus gatewayBus, ILogger<GatewayController> logger)
        {
            this._gatewayBus = gatewayBus;
            this._logger = logger;
        }
        
        // GET api/values
        [HttpGet]
        public ResponseSerializer Get(String searchType, int page, String sortColumn, String order, String city, String factory, String workshop)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._gatewayBus.GetAllGateways(searchType, page, sortColumn, order, city, factory, workshop));
        }

        // GET api/values/{id}
        [HttpGet("{id}")]
        public ResponseSerializer GetById(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._gatewayBus.GetGatewayById(id));
        }

        // POST api/values
        [HttpPost]
        public ResponseSerializer Post([FromBody] GatewaySerializer gatewaySerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._gatewayBus.CreateNewGateway(gatewaySerializer));
        }

        // PUT api/values/{id}
        [HttpPut("{id}")]
        public ResponseSerializer Put(int id, [FromBody] GatewaySerializer gatewaySerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._gatewayBus.UpdateGateway(id, gatewaySerializer));
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ResponseSerializer Delete(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._gatewayBus.DeleteGateway(id));
        }

        [HttpGet("workshop/{cityName}/{factoryName}/{workshopName}")]
        public ResponseSerializer GetGatewayByWorkshop(String cityName, String factoryName, String workshopName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._gatewayBus.GetGatewayByWorkshop(cityName, factoryName, workshopName));
        }

        [HttpPost("batch/gateways")]
        public ResponseSerializer BatchDelete([FromBody] BatchNumber batchNumber)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._gatewayBus.BatchDeleteGateway(batchNumber.number));
        }

        [HttpPost("type/{gatewayType}")]
        public ResponseSerializer CreateGatewayType(String gatewayType)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._gatewayBus.CreateGatewayType(gatewayType));
        }

        [HttpGet("number")]
        public ResponseSerializer GetGatewayNumber(String searchType, String city="all", String factory="all", String workshop="all")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._gatewayBus.GetGatewayNumber(searchType, city, factory, workshop));
        }
    }
}