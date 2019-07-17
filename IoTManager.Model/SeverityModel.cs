using System;

namespace IoTManager.Model
{
    public class SeverityModel
    {
        public int Id { get; set; }
        public String SeverityName { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}