using System;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class RoleSerializer
    {
        public RoleSerializer()
        {
            this.id = 0;
            this.roleName = null;
            this.description = null;
            this.timestamp = null;
        }

        public RoleSerializer(RoleModel roleModel)
        {
            this.id = roleModel.Id;
            this.roleName = roleModel.RoleName;
            this.description = roleModel.Description;
            this.timestamp = DateTime.Parse(roleModel.Timestamp.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
        }
        public int id { get; set; }
        public String roleName { get; set; }
        public String description { get; set; }
        public String timestamp { get; set; }
    }
}