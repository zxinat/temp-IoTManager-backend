using System;

namespace IoTManager.Model
{
    public sealed class ThresholdModel
    {
        public int Id { get; set; }
        public String IndexId { get; set; }
        public String DeviceId { get; set; }
        public String Operator { get; set; }
        public double ThresholdValue { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public String RuleName { get; set; }
        public String Description { get; set; }
        public String Severity { get; set; }
    }
}