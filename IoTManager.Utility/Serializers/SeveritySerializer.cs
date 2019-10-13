using System;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class SeveritySerializer
    {
        public SeveritySerializer()
        {
            this.id = 0;
            this.severityName = null;
            this.createTime = null;
            this.updateTime = null;
        }

        public SeveritySerializer(SeverityModel severityModel)
        {
            this.id = severityModel.Id;
            this.severityName = severityModel.SeverityName;
            this.createTime = DateTime.Parse(severityModel.CreateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.updateTime = DateTime.Parse(severityModel.UpdateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
        }
        
        public int id { get; set; }
        public String severityName { get; set; }
        public String createTime { get; set; }
        public String updateTime { get; set; }
    }
}