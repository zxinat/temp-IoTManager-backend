using System;
using IoTManager.Core.Infrastructures;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceDailyOnlineTimeController
    {
        private readonly IDeviceDailyOnlineTimeBus _deviceDailyOnlineTimeBus;
        private readonly ILogger _logger;

        public DeviceDailyOnlineTimeController(IDeviceDailyOnlineTimeBus deviceDailyOnlineTimeBus,
            ILogger<DeviceDailyOnlineTimeController> logger)
        {
            this._deviceDailyOnlineTimeBus = deviceDailyOnlineTimeBus;
            this._logger = logger;
        }

        [HttpGet]
        public ResponseSerializer Get()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDailyOnlineTimeBus.GetAll()
                );
        }

        [HttpGet("averageOnlineTime")]
        public ResponseSerializer GetAverageOnlineTimeByDevice(String deviceId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._deviceDailyOnlineTimeBus.GetAverageOnlineTimeByDevice(deviceId)
                );
        }

        [HttpPost("byTime")]
        public ResponseSerializer GetDeviceOnlineTimeByTime([FromBody] DataStatisticRequestModel date)
        {
            return new ResponseSerializer(
                200, 
                "success",
                this._deviceDailyOnlineTimeBus.GetDeviceOnlineTimeByTime(date.StartTime, date.EndTime));
        }
    }
}