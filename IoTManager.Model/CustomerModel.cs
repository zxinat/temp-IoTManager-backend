using IoTManager.Model.RequestModel;
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
        public byte status { get; set; }          //是否登记离开
        public string lastArea { get; set; }        //最后访问区域
        public DateTime createTime { get; set; }
        public DateTime lastTime { get; set; }      //离开时间
        public DateTime updateTime { get; set; }
        public CustomerModel()
        {

        }
        public CustomerModel(CustomerFormModel customerForm)
        {
            staffId = customerForm.staffId;
            name = customerForm.name;
            gender = customerForm.gender;
            phoneNumber = customerForm.phoneNumber;
            affiliation = customerForm.affiliation;
            cause = customerForm.cause;
            remark = customerForm.remark;
            status = 1; //创建时，注册staffId，默认状态置为1，即未注销状态
            lastArea = null;
        }
    }
    
}
