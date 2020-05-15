using System;
using System.Collections.Generic;
using System.Text;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class DepartmentSerializer
    {
        public DepartmentSerializer()
        {
            this.id = 0;
            this.departmentName = null;
            this.admin = null;
            this.createTime = null;
            this.updateTime = null;
            this.remark = null;
            
        }
        public DepartmentSerializer(DepartmentModel department)
        {
            this.id = department.id;
            this.departmentName = department.departmentName;
            this.admin = department.admin;
            this.createTime = DateTime.Parse(department.createTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.updateTime = DateTime.Parse(department.updateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.remark = department.remark;
        }

        public int id { get; set; }
        public string departmentName { get; set; }
        public string admin { get; set; }
        public string createTime { get; set; }
        public string updateTime { get; set; }
        public string remark { get; set; }
    }
}
