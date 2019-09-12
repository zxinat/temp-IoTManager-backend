using System;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class DeviceTypeSerializer
    {
        public DeviceTypeSerializer()
        {
            this.id = 0;
            this.deviceTypeName = null;
            this.offlineTime = 0.0;
        }

        public DeviceTypeSerializer(DeviceTypeModel deviceTypeModel)
        {
            this.id = deviceTypeModel.Id;
            this.deviceTypeName = deviceTypeModel.DeviceTypeName;
            this.offlineTime = deviceTypeModel.OfflineTime;
        }
        
        public int id { get; set; }
        public String deviceTypeName { get; set; }
        public Double offlineTime { get; set; }
    }
}