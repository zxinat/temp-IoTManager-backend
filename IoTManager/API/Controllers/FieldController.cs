using IoTManager.Core.Infrastructures;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    public class FieldController
    {
        private readonly IFieldBus _fieldBus;
        private readonly ILogger _logger;

        public FieldController(IFieldBus fieldBus, ILogger<FieldController> logger)
        {
            this._fieldBus = fieldBus;
            this._logger = logger;
        }

        [HttpGet]
        public ResponseSerializer GetAllFields()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._fieldBus.GetAllFields());
        }
    }
}