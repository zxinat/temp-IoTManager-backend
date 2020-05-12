using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model
{
    public class CustomerModel
    {
        public int id { get; set; }
        public string staffId { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string phoneNumber { get; set; }
        public string affiliation { get; set; }     //所属机构
        public string cause { get; set; }           //事由
        public string remark { get; set; }
        public string status { get; set; }          //是否登记离开
        public string lastArea { get; set; }        //最后访问区域
        public DateTime createTime { get; set; }
        public DateTime updateTime { get; set; }
    }
}
