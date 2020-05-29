using System;
using IoTManager.Core.Infrastructures;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Remotion.Linq.Clauses;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    public class FieldController
    {
        private readonly IFieldBus _fieldBus;
        private readonly IDeviceDataBus _deviceDataBus;
        private readonly ILogger _logger;

        public FieldController(IFieldBus fieldBus, IDeviceDataBus deviceDataBus, ILogger<FieldController> logger)
        {
            this._fieldBus = fieldBus;
            this._deviceDataBus = deviceDataBus;
            this._logger = logger;
        }

        [HttpGet]
        public ResponseSerializer GetAllFields(int pageMode = 0,int page = 1, String sortColumn = "id", String order = "asc")
        {
            return new ResponseSerializer(
                200,
                "success",
                this._fieldBus.GetAllFields(pageMode,page, sortColumn, order));
        }

        [HttpPost]
        public ResponseSerializer CreateNewField([FromBody] FieldSerializer fieldSerializer)
        {
            int code;
            string msg;
            string d;
            try
            {
                d = _fieldBus.CreateNewField(fieldSerializer);
                if(d=="success")
                {
                    code = 200;
                    msg = d;
                }
                else if(d=="exist")
                {
                    code = 409;
                    msg = "conflict";
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

        [HttpGet("affiliate/{deviceId}")]
        public ResponseSerializer GetAffiliateField(string deviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._fieldBus.GetAffiliateFields(deviceId));
        }

        [HttpPut("{id}")]
        public ResponseSerializer UpdateField(int id, [FromBody] FieldSerializer fieldSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._fieldBus.UpdateField(id, fieldSerializer));
        }

        [HttpDelete("{id}")]
        public ResponseSerializer DeleteField(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._fieldBus.DeleteField(id));
        }

        [HttpGet("affiliateData/{fieldId}")]
        public ResponseSerializer GetFieldAffiliateData(String fieldId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDataBus.GetFieldAffiliateData(fieldId));
        }

        [HttpGet("number")]
        public ResponseSerializer GetFieldNumber()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._fieldBus.GetFieldNumber());
        }
    }
}