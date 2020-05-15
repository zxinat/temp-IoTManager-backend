using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model.RequestModel
{
    public class RFIDTagFormModel
    {
        public string staffId { get; set; }
        public string tagId { get; set; }
        public byte type { get; set; }
    }
}
