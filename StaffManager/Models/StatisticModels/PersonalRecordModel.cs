using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Models.StatisticModels
{
    public class PersonalRecordModel
    {
        public string staffId { get; set; }
        public string name { get; set; }
        public string deviceId { get; set; }
        public string deviceName { get; set; }
        public string tagId { get; set; }
        public int value { get; set; }
        public DateTime timestamp { get; set; }
    }
}
