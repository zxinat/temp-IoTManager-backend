using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Models.StatisticModels
{
    public class PersonalAttendenceModel
    {
        public int id { get; set; }
        public string remark { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int missNum { get; set; }
        public int workNum { get; set; }
        public List<string> absenceDates { get; set; }
    }
}
