using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model
{
    public sealed class StaffModel
    {
        public int id { get; set; }
        public string staffId { get; set; }
        public string staffName { get; set; }
        public string staffRole { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public string department { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string remark { get; set; }
        public DateTime createTime { get; set; }
        public DateTime updateTime { get; set; }
        public string base64Image { get; set; }
        public string pictureRoute { get; set; }
    }
}
