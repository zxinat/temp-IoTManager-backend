using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model
{
    public class StaffAuthModel
    {
        public int id { get; set; }
        public string staffId { get; set; }
        public string deviceId { get; set; }
        public DateTime timestamp { get; set; }
    }
}
