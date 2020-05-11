using IoTManager.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Utility.Serializers
{
    public class StaffSerializer
    {
        public StaffSerializer()
        {
            this.id = 0;
            this.staffId = null;
            this.staffName = null;
            this.staffRole = null;
            this.gender = null;
            this.age = 0;
            this.department = null;
            this.phoneNumber = null;
            this.email = null;
            this.remark = null;
            this.base64Image = null;
            this.pictureRoute = null;
        }
        public StaffSerializer(StaffModel staffModel)
        {
            this.id = staffModel.id;
            this.staffId = staffModel.staffId;
            this.staffName = staffModel.staffName;
            this.staffRole = staffModel.staffRole;
            this.gender = staffModel.gender;
            this.age = staffModel.age;
            this.department = staffModel.department;
            this.phoneNumber = staffModel.phoneNumber;
            this.email = staffModel.email;
            this.remark = staffModel.remark;
            this.createTime = DateTime.Parse(staffModel.createTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.updateTime = DateTime.Parse(staffModel.updateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.base64Image = staffModel.base64Image;
            this.pictureRoute = staffModel.pictureRoute;

        }
        public int id { get; set; }
        public string staffId { get; set; }
        public string staffName { get; set; }
        public string staffRole { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public string department { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string remark { get; set; }
        public string createTime { get; set; }
        public string updateTime { get; set; }
        public string base64Image { get; set; }
        public string pictureRoute { get; set; }

    }
}
