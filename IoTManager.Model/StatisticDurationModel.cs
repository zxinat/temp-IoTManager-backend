using System;

namespace IoTManager.Model
{
    public class StatisticDurationModel
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public String deviceId { get; set;}
    }
}