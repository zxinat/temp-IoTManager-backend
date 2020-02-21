using System;

namespace IoTManager.Model
{
    public class DeviceDailyOnlineTimeModel
    {
        public int Id { get; set; }
        public String DeviceName { get; set; }
        public String HardwareDeviceId { get; set; }
        public Double OnlineTime { get; set; }
        public DateTime Date { get; set; }
        public DateTime Timestamp { get; set; }
    }
}