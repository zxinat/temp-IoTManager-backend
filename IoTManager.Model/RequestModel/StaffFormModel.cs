using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model.RequestModel
{
    public class StaffFormModel
    {
        public StaffFormModel()
        {
            gender = null;
            age = 0;
            remark = null;
            email = null;
            status = "在职";
        }
        public string staffId { get; set; }
        public string staffName { get; set; }
        public string staffRole { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public string department { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string remark { get; set; }
        public string status { get; set; }
    }
}
