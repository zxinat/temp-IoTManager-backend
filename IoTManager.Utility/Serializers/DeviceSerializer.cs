using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class DeviceSerializer
    {
        public DeviceSerializer()
        {
            this.id = 0;
            this.deviceName = null;
            this.hardwareDeviceID = null;
            this.city = null;
            this.factory = null;
            this.workshop = null;
            this.deviceState = null;
            this.lastConnectiontime = null;
            this.imageUrl = null;
            this.gatewayId = null;
            this.mac = null;
            this.deviceType = null;
            this.createTime = null;
            this.updateTime = null;
            this.remark = null;
            this.pictureRoute = null;
            this.isOnline = null;
            this.base64Image = null;
            this.tags = null;
        }

        public DeviceSerializer(DeviceModel deviceModel)
        {
            this.id = deviceModel.Id;
            this.deviceName = deviceModel.DeviceName;
            this.hardwareDeviceID = deviceModel.HardwareDeviceId;
            this.city = deviceModel.City;
            this.factory = deviceModel.Factory;
            this.workshop = deviceModel.Workshop;
            this.deviceState = deviceModel.DeviceState;
            this.lastConnectiontime = DateTime.Parse(deviceModel.LastConnectionTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.imageUrl = deviceModel.ImageUrl;
            this.gatewayId = deviceModel.GatewayId;
            this.mac = deviceModel.Mac;
            this.deviceType = deviceModel.DeviceType;
            this.createTime = DateTime.Parse(deviceModel.CreateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.updateTime = DateTime.Parse(deviceModel.UpdateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.remark = deviceModel.Remark;
            this.pictureRoute = deviceModel.PictureRoute;
            this.isOnline = deviceModel.IsOnline;
            this.base64Image = deviceModel.Base64Image;
            this.tags = deviceModel.Tags;
        }
        
        public int id { get; set; }
        public String deviceName { get; set; }
        public String hardwareDeviceID { get; set; }
        public String city { get; set; }
        public String factory { get; set; }
        public String workshop { get; set; }
        public String deviceState { get; set; }
        public String lastConnectiontime { get; set; }
        public String imageUrl { get; set; }
        public String gatewayId { get; set; }
        public String mac { get; set; }
        public String deviceType { get; set; }
        public String createTime { get; set; }
        public String updateTime { get; set; }
        public String remark { get; set; }
        public String pictureRoute{ get; set; }
        public String isOnline { get; set; }
        public String base64Image { get; set; }
        public List<String> tags { get; set; }
    }

    public class BatchNumber
    {
        public int[] number { get; set; }
    }
}