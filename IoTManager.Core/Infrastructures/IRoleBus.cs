﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Core.Infrastructures
{
    public interface IRoleBus
    {
        String UpdateAuthByRoleId(String roleId, List<String> authId);
        String UpdateAuthByUserId(int userId, List<String> authId);
        String GetRoleByUserId(int userId);
        List<String> GetAllAuth();
    }
}
