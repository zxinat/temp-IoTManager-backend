using System;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class DeviceDataSerializer
    {
        public DeviceDataSerializer()
        {
            this.id = null;
            this.deviceId = null;
            this.indexName = null;
            this.indexId = null;
            this.indexUnit = null;
            this.indexType = null;
            this.indexValue = 0;
            this.timestamp = null;
            this.gatewayId = null;
            this.deviceType = null;
            this.mark = null;
            this.isCheck = null;
            this.deviceName = null;
        }

        public DeviceDataSerializer(DeviceDataModel deviceDataModel)
        {
            this.id = deviceDataModel.Id;
            this.deviceId = deviceDataModel.DeviceId;
            this.indexName = deviceDataModel.IndexName;
            this.indexId = deviceDataModel.IndexId;
            this.indexUnit = deviceDataModel.IndexUnit;
            this.indexType = deviceDataModel.IndexType;
            this.indexValue = deviceDataModel.IndexValue;
            this.timestamp = DateTime.Parse(deviceDataModel.Timestamp.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.gatewayId = deviceDataModel.GatewayId;
            this.deviceType = deviceDataModel.DeviceType;
            this.mark = deviceDataModel.Mark;
            this.isCheck = deviceDataModel.IsCheck;
            this.deviceName = deviceDataModel.DeviceName;
        }
        
        public String id { get; set; }
        public String deviceId { get; set; }
        public String indexName { get; set; }
        public String indexId { get; set; }
        public String indexUnit { get; set; }
        public String indexType { get; set; }
        public Double indexValue { get; set; }
        public String timestamp { get; set; }
        public String gatewayId { get; set; }
        public String deviceType { get; set; }
        public String mark { get; set; }
        public String isCheck { get; set; }
        public String deviceName { get; set; }
    }
}