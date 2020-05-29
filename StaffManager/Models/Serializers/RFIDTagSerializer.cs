using IoTManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Models.Serializers
{
    public class RFIDTagSerializer
    {
        public int id { get; set; }
        public string staffId { get; set; }
        public string tagID { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string createTime { get; set; }
        public string updateTime { get; set; }
        public string lastTime { get; set; }
        public RFIDTagSerializer()
        {

        }
        public RFIDTagSerializer(RFIDTagModel rFIDTagModel)
        {
            id = rFIDTagModel.id;
            staffId = rFIDTagModel.staffId;
            tagID = rFIDTagModel.tagId;
            type = rFIDTagModel.type == 1 ? "员工" : "访客";
            status = rFIDTagModel.status == 1 ? "已激活" : "未激活";
            createTime = DateTime.Parse(rFIDTagModel.createTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            updateTime= DateTime.Parse(rFIDTagModel.updateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            lastTime= rFIDTagModel.lastTime<=new DateTime(1970,1,1)?"":DateTime.Parse(rFIDTagModel.lastTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
        }
    }
}
