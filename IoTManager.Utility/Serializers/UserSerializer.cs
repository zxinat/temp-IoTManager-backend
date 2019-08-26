using System;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class UserSerializer
    {
        public UserSerializer()
        {
            this.id = 0;
            this.userName = null;
            this.displayName = null;
            this.password = null;
            this.email = null;
            this.phoneNumber = null;
            this.remark = null;
            this.createTime = null;
            this.updateTime = null;
            this.identify = false;
            this.role = 0;
        }

        public UserSerializer(UserModel userModel)
        {
            this.id = userModel.Id;
            this.userName = userModel.UserName;
            this.displayName = userModel.DisplayName;
            this.password = "";
            this.email = userModel.Email;
            this.phoneNumber = userModel.PhoneNumber;
            this.remark = userModel.Remark;
            this.createTime = userModel.CreateTime
                .ToString(Constant.getDateFormatString());
            this.updateTime = userModel.UpdateTime
                .ToString(Constant.getDateFormatString());
            this.identify = userModel.Identify;
            this.role = userModel.Role;

        }
        
        public int id { get; set; }
        public String userName { get; set; }
        public String displayName { get; set; }
        public String password { get; set; }
        public String email { get; set; }
        public String phoneNumber { get; set; }
        public String remark { get; set; }
        public String createTime { get; set; }
        public String updateTime { get; set; }
        public Boolean identify { get; set; }
        public int role { get; set; }
    }
}