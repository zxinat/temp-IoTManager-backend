using System;
using IoTManager.Model;
using MongoDB.Driver.Core.Servers;

namespace IoTManager.Utility.Serializers
{
    public class ThresholdSerializer
    {
        public ThresholdSerializer()
        {
            this.id = 0;
            this.field = null;
            this.deviceGroup = null;
            this.Operator = null;
            this.value = null;
            this.createTime = null;
            this.updateTime = null;
            this.name = null;
            this.description = null;
        }

        public ThresholdSerializer(ThresholdModel thresholdModel)
        {
            this.id = thresholdModel.Id;
            this.field = thresholdModel.IndexId;
            this.deviceGroup = thresholdModel.DeviceId;
            this.Operator = thresholdModel.Operator;
            this.value = thresholdModel.ThresholdValue.ToString();
            this.createTime = thresholdModel.CreateTime
                .ToString(Constant.getDateFormatString());
            this.updateTime = thresholdModel.UpdateTime
                .ToString(Constant.getDateFormatString());
            this.name = thresholdModel.RuleName;
            this.description = thresholdModel.Description;
        }
        
        public int id { get; set; }
        public String field { get; set; }
        public String deviceGroup { get; set; }
        public String Operator { get; set; }
        public String value { get; set; }
        public String createTime { get; set; }
        public String updateTime { get; set; }
        public String name { get; set; }
        public String description { get; set; }
    }
}