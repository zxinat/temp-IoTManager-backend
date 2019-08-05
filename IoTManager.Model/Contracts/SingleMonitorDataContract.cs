using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model.Contracts
{
    public class SingleMonitorDataContract
    {
        public DeviceData Data { get; set; }
        public string GSN { get; set; }
    }

    public class DeviceData
    {
        public string DName { get; set; }
        public string DId { get; set; }
        public List<MonitorData> IData { get; set; }
    }

    public class MonitorData
    {
        public string IName { get; set; }
        public string IId { get; set; }
        public string IUnit { get; set; }
        public string IType { get; set; }
        public double IValue { get; set; }
        public string ITs { get; set; }
    }
}
