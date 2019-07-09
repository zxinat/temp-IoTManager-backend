using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model.Contracts
{
    public class MultipleMonitorDataContract
    {
        public List<DeviceData> Data { get; set; }
        public string GSN { get; set; }
    }
}
