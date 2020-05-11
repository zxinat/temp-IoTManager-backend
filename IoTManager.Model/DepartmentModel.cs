using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model
{
    public sealed class DepartmentModel
    {
        public int id { get; set; }
        public string departmentName { get; set; }
        public string admin { get; set; }
        public DateTime createTime { get; set; }
        public DateTime updateTime { get; set; }
        public string remark { get; set; }
        //public int totalStaff { get; set; }
    }
}
