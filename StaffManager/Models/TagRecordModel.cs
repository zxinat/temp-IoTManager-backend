using StaffManager.Models.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Models
{
    public class TagRecordModel
    {
        public int id { get; set; }
        public string tagId { get; set; }
        public string deviceId { get; set; }
        public int value { get; set; }
        public DateTime timestamp { get; set; }
        public PersonSerializers person { get; set; }
    }
}
