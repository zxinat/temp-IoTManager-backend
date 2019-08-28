using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.IDao
{
    public interface IRoleDao
    {
        String DeleteAllAuth(String roleId);
        String InsertAllAuth(String roleId, List<String> authId);

        String UpdateUserAuth(int userId, Dictionary<String, int> dic);
    }
}
