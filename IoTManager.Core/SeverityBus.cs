using System.Collections.Generic;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;

namespace IoTManager.Core
{
    public sealed class SeverityBus: ISeverityBus
    {
        private readonly ISeverityDao _severityDao;
        private readonly ILogger _logger;

        public SeverityBus(ISeverityDao severityDao, ILogger<SeverityBus> logger)
        {
            this._severityDao = severityDao;
            this._logger = logger;
        }

        public List<SeveritySerializer> GetAllSeverity()
        {
            List<SeverityModel> severities = this._severityDao.Get();
            List<SeveritySerializer> result = new List<SeveritySerializer>();
            foreach (SeverityModel s in severities)
            {
                result.Add(new SeveritySerializer(s));
            }
            return result;
        }
    }
}