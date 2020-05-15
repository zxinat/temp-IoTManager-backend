using IoTManager.Model;
using IoTManager.Model.RequestModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Utility.Serializers
{
    public class CustomerSerializer
    {
        public CustomerSerializer()
        {
            id = 0;
            staffId = null;
            name = null;
            gender = null;
            phoneNumber = null;
            affiliation = null;
            cause = null;
            remark = null;
            status = null;
            lastArea = null;

        }
        public CustomerSerializer(CustomerModel customer)
        {
            id = customer.id;
            staffId = customer.staffId;
            name = customer.name;
            gender = customer.gender;
            phoneNumber = customer.phoneNumber;
            affiliation = customer.affiliation;
            cause = customer.cause;
            remark = customer.remark;
            status = customer.status == 0 ? "已注销" : "未注销";
            lastArea = customer.lastArea;
            createTime = DateTime.Parse(customer.createTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            updateTime = DateTime.Parse(customer.updateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            lastTime = DateTime.Parse(customer.lastTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
        }
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
        public string createTime { get; set; }
        public string lastTime { get; set; }      //离开时间
        public string updateTime { get; set; }
    }
}
