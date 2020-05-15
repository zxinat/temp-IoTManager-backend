using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model.RequestModel
{
    public class CustomerFormModel
    {
        public CustomerFormModel()
        {
            gender = null;
            remark = null;
            affiliation = null;
        }
        public string staffId { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string phoneNumber { get; set; }
        public string affiliation { get; set; }
        public string cause { get; set; }
        public string remark { get; set; }
    }
}
