using System;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class AlarmInfoSerializer
    {
        public AlarmInfoSerializer()
        {
            this.id = null;
            this.alarmInfo = null;
            this.deviceId = null;
            this.indexId = null;
            this.indexName = null;
            this.indexValue = 0.0;
            this.thresholdValue = 0.0;
            this.timestamp = null;
            this.severity = null;
            this.processed = null;
        }

        public AlarmInfoSerializer(AlarmInfoModel alarmInfoModel)
        {
            this.id = alarmInfoModel.Id;
            this.alarmInfo = alarmInfoModel.AlarmInfo;
            this.deviceId = alarmInfoModel.DeviceId;
            this.indexId = alarmInfoModel.IndexId;
            this.indexName = alarmInfoModel.IndexName;
            this.indexValue = alarmInfoModel.IndexValue;
            this.thresholdValue = alarmInfoModel.ThresholdValue;
            this.timestamp = alarmInfoModel.Timestamp
                .ToString(Constant.getDateFormatString());
            this.severity = alarmInfoModel.Severity;
            this.processed = alarmInfoModel.Processed;
        }
        
        public String id { get; set; }
        public String alarmInfo { get; set; }
        public String deviceId { get; set; }
        public String indexId { get; set; }
        public String indexName { get; set; }
        public Double indexValue { get; set; }
        public Double thresholdValue { get; set; }
        public String timestamp { get; set; }
        public String severity { get; set; }
        public String processed { get; set; }
    }
}