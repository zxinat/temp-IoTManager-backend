using System;
using System.Collections.Generic;
using System.Text;

namespace StaffManager.Models.RequestModel
{
    public class RFIDTagFormModel
    {
        public RFIDTagFormModel()
        {
            status = "已激活";
        }
        public string staffId { get; set; }
        public string tagId { get; set; }
        public string type { get; set; } // 标志访客、员工
        public string status { get; set; }
    }
}
