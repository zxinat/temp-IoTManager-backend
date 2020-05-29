using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Models.StatisticModels
{
    public class DepartStatisticModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int staffNum { get; set; }
        public int missNum { get; set; }
        public double rate { get; set; }
        public List<string> attendenceStaffIds { get; set; }
    }
    public class  DepartmentAttendenceModel
    {
        public int id { get; set; }
        public string remark { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public List<DepartStatisticModel> departStatistics { get; set; }
    }
}
