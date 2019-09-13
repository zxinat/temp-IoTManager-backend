using IoTManager.Core.Infrastructures;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly IThemeBus _themeBus;
        private readonly ILogger _logger;

        public ThemeController(IThemeBus themeBus, ILogger<ThemeController> logger)
        {
            this._themeBus = themeBus;
            this._logger = logger;
        }

        [HttpGet]
        public ResponseSerializer Get()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._themeBus.GetAllThemes());
        }

        [HttpGet("{id}")]
        public ResponseSerializer GetById(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._themeBus.GetThemeById(id));
        }

        [HttpPost]
        public ResponseSerializer Post([FromBody] ThemeSerializer themeSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._themeBus.CreateNewTheme(themeSerializer));
        }

        [HttpPut("{id}")]
        public ResponseSerializer Put(int id, [FromBody] ThemeSerializer themeSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._themeBus.UpdateTheme(id, themeSerializer));
        }

        [HttpDelete("{id}")]
        public ResponseSerializer Delete(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._themeBus.DeleteTheme(id));
        }

        [HttpGet("userid/{userId}")]
        public ResponseSerializer GetThemeByUserId(int userId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._themeBus.GetThemeByUserId(userId));
        }
    }
}