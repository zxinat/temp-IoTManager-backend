using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model.DataReceiver
{
    public class StatisticDataModel
    {
        public string time { get; set; }
        public string index { get; set; }
        public float max { get; set; }
        public float min { get; set; }
        public float avg { get; set; }
        public DateTime date { get; set; }
    }
}
