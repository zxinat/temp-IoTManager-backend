using System;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class WorkshopSerializer
    {
        public WorkshopSerializer()
        {
            this.id = 0;
            this.workshopName = null;
            this.workshopPhoneNumber = null;
            this.workshopAddress = null;
            this.remark = null;
            this.createTime = null;
            this.updateTime = null;
            this.factory = null;
            this.city = null;
        }

        public WorkshopSerializer(WorkshopModel workshopModel)
        {
            this.id = workshopModel.Id;
            this.workshopName = workshopModel.WorkshopName;
            this.workshopPhoneNumber = workshopModel.WorkshopPhoneNumber;
            this.workshopAddress = workshopModel.WorkshopAddress;
            this.remark = workshopModel.Remark;
            this.createTime = DateTime.Parse(workshopModel.CreateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.updateTime = DateTime.Parse(workshopModel.UpdateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.factory = workshopModel.Factory;
            this.city = workshopModel.City;
        }
        
        public int id { get; set; }
        public String workshopName { get; set; }
        public String workshopPhoneNumber { get; set; }
        public String workshopAddress { get; set; }
        public String remark { get; set; }
        public String createTime { get; set; }
        public String updateTime { get; set; }
        public String factory { get; set; }
        public String city { get; set; }
    }
}