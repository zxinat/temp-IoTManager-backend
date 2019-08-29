using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IAuthDao
    {
        List<String> GetAuthByUserId(int userId);
        List<String> GetAuthByRoleId(int roleId);
        List<String> GetAllAuth();
    }
}