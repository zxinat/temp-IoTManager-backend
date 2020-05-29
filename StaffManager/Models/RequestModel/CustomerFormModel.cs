using System;
using System.Collections.Generic;
using System.Text;

namespace StaffManager.Models.RequestModel
{
    public class CustomerFormModel
    {
        public CustomerFormModel()
        {
            gender = null;
            remark = null;
            affiliation = null;
            status = "登入";
        }
        public string staffId { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string phoneNumber { get; set; }
        public string affiliation { get; set; }
        public string cause { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
    }
}
