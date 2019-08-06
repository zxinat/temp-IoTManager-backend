using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.MonitorService.Models
{
    public sealed class LogEntity : TableEntity
    {
        public string Message { get; set; }
    }
}
