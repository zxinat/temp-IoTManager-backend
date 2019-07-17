using System.Collections.Generic;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface ISeverityBus
    {
        List<SeveritySerializer> GetAllSeverity();
    }
}