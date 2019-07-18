using IoTManager.Core.Infrastructures;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeverityController
    {
        private readonly ISeverityBus _severityBus;
        private readonly ILogger _logger;

        public SeverityController(ISeverityBus severityBus, ILogger<SeverityController> logger)
        {
            this._severityBus = severityBus;
            this._logger = logger;
        }

        [HttpGet]
        public ResponseSerializer Get()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._severityBus.GetAllSeverity());
        }
    }
}