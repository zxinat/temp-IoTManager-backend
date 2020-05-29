using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Models.StatisticModels
{
    public class PersonalRecordsModel
    {
        public string staffId { get; set; }
        public string name { get; set; }
        public string affiliation { get; set; }
        public List<RecordModel> records { get; set; }
    }
}
