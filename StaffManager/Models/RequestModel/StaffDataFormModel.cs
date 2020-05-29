using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Models.RequestModel
{
    public class StaffDataFormModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string MonitorId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GatewayId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsScam { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsCheck { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DeviceType { get; set; }
        /// <summary>
        /// 维信电子-1号车间
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 西风
        /// </summary>
        public string MonitorName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MonitorType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int value { get; set; }
    }

}
