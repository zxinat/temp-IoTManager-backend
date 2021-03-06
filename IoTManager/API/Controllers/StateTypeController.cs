using System;
using IoTManager.Core.Infrastructures;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace IoTManager.API.Controllers
{
    [Route("/api")]
    [ApiController]
    public class StateTypeController: ControllerBase
    {
        private readonly IStateTypeBus _stateTypeBus;
        private readonly ILogger _logger;

        public StateTypeController(IStateTypeBus stateTypeBus, ILogger<StateTypeController> logger)
        {
            this._stateTypeBus = stateTypeBus;
            this._logger = logger;
        }
        
        [HttpGet("deviceType")]
        public ResponseSerializer GetAllDeviceTypes()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._stateTypeBus.GetAllDeviceTypes());
        }

        [HttpGet("deviceState")]
        public ResponseSerializer GetAllDeviceStates()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._stateTypeBus.GetAllDeviceStates());
        }

        [HttpGet("gatewayType")]
        public ResponseSerializer GetAllGatewayTypes()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._stateTypeBus.GetAllGatewayTypes());
        }

        [HttpGet("gatewayState")]
        public ResponseSerializer GetAllGatewayStates()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._stateTypeBus.GetAllGatewayStates());
        }

        [HttpGet("detailedDeviceType")]
        public ResponseSerializer GetDetailedDeviceTypes(int pageMode = 0,int page = 1, String sortColumn = "id", String order = "asc")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._stateTypeBus.GetDetailedDeviceTypes(pageMode, page, sortColumn, order));
        }

        [HttpPost("deviceType")]
        public ResponseSerializer AddDeviceType([FromBody] DeviceTypeSerializer dts)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._stateTypeBus.AddDeviceType(dts));
        }

        [HttpPut("deviceType/{id}")]
        public ResponseSerializer UpdateDeviceType(int id, [FromBody] DeviceTypeSerializer dts)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._stateTypeBus.UpdateDeviceType(id, dts));
        }

        [HttpDelete("deviceType/{id}")]
        public ResponseSerializer DeleteDeviceType(int id)
        {
            return new ResponseSerializer(
                200, 
                "success",
                this._stateTypeBus.DeleteDeviceType(id));
        }

        [HttpGet("deviceType/affiliate/{id}")]
        public ResponseSerializer GetDeviceTypeAffiliateDevice(int id)
        {
            return new  ResponseSerializer(
                200,
                "success",
                this._stateTypeBus.GetDeviceTypeAffiliateDevice(id));
        }

        [HttpGet("detailedDeviceType/number")]
        public ResponseSerializer GetDetailedDeviceTypeNumber()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._stateTypeBus.GetDetailedDeviceTypeNumber());
        }

        [HttpGet("deviceType/byName/{name}")]
        public ResponseSerializer GetDeviceTypeByName(String name)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._stateTypeBus.GetDeviceTypeByName(name));
        }
    }
}