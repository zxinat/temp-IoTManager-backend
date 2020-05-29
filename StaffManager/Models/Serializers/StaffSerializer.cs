using IoTManager.Utility;
using StaffManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaffManager.Models.Serializers
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
            this.status = staffModel.status == 1 ? "在职" : "离职";
            this.lastTime = staffModel.lastTime<=new DateTime(1970,1,1)?"": DateTime.Parse(staffModel.lastTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());

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
        public string status { get; set; }
        public string lastTime { get; set; }

    }
}
