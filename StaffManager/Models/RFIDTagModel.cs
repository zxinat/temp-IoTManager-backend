using StaffManager.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaffManager.Models
{
    public class RFIDTagModel
    {
        public int id { get; set; }
        public string staffId { get; set; }
        public string tagId { get; set; }
        public byte type { get; set; }
        public byte status { get; set; }
        public DateTime createTime { get; set; }
        public DateTime updateTime { get; set; }
        public DateTime lastTime { get; set; }
        public RFIDTagModel()
        {

        }
        public RFIDTagModel(RFIDTagFormModel rFIDTagForm)
        {
            staffId = rFIDTagForm.staffId;
            tagId = rFIDTagForm.tagId;
            type = rFIDTagForm.type=="员工"?(Byte)1:(Byte)0;
            status = rFIDTagForm.status == "已激活" ? (Byte)1 : (Byte)0;
        }
    }
}
