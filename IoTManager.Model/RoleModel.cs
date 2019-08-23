﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Model
{
    public sealed class RoleModel
    {
        public int Id { get; set; }
        public String RoleName { get; set; }
        public String Description { get; set; }
        public DateTime Timestamp { get; set; }
        
    }
}
