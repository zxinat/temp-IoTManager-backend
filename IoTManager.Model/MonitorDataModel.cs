using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model
{
    public class MonitorDataModel
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        public string GatewayId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string DeviceName { get; set; }
        public string MonitorId { get; set; }
        public string MonitorName { get; set; }
        public string MonitorType { get; set; }
        public string Unit { get; set; }
        public double Value { get; set; }
        public bool IsScam { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
