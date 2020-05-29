using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model.Configuration
{
    public class Parameters
    {
        /// <summary>
        /// 
        /// </summary>
        public string RecurringJobId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CurrentCulture { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CurrentUICulture { get; set; }
    }

    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public string EnqueuedAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Queue { get; set; }
    }

    public class StateHistoryItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public Data Data { get; set; }
    }

    public class JobGraphModel
    {
        public string _id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> _t { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpireAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StateName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InvocationData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Arguments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Parameters Parameters { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<StateHistoryItem> StateHistory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
