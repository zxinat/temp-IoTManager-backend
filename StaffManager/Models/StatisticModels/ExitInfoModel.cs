using IoTManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Models.StatisticModels
{
    public class ExitInfoModel
    {
        public ExitInfoModel()
        {

        }
        public string tagId { get; set; }
        public string staffId { get; set; }
        public string name { get; set; }
        public DateTime timestamp { get; set; }
    }
    public class ExitInfoSerializer
    {
        public string tagId { get; set; }
        public string staffId { get; set; }
        public string name { get; set; }
        public string timestamp { get; set; }
        public string timespan { get; set; }
        public ExitInfoSerializer(ExitInfoModel exitInfo)
        {
            tagId = exitInfo.tagId;
            staffId = exitInfo.staffId;
            name = exitInfo.name;
            timestamp = exitInfo.timestamp.ToLocalTime().ToString(Constant.getMysqlDateFormString());
            timespan = (DateTime.Now - exitInfo.timestamp.ToLocalTime()).ToString();
        }
    }
}
