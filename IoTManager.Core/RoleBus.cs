using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto.Tls;

namespace IoTManager.Core
{
    public sealed class RoleBus:IRoleBus
    {
        private readonly IRoleDao _roleDao;
        private readonly ILogger _logger;
        private readonly IAuthDao _authDao;
        public RoleBus(IRoleDao roleDao,ILogger<RoleBus> logger, IAuthDao authDao)
        {
            this._roleDao = roleDao;
            this._logger = logger;
            this._authDao = authDao;
        }

        public String UpdateAuthByRoleId(String roleId, List<String> authId)
        {
            if (this._roleDao.DeleteAllAuth(roleId) == "error")
            {
                return "error";
            }
            return this._roleDao.InsertAllAuth(roleId, authId);
        }

        public String UpdateAuthByUserId(int userId, List<String> authId)
        {
            var past = this._authDao.GetAuthByRoleId(1);
            Dictionary<String, int> dic = new Dictionary<string, int>();
            foreach (var aid in authId)
            {
                dic.Add(aid, 1);
            }
            foreach (var p in past)
            {
                if (dic.ContainsKey(p))
                {
                    dic.Remove(p);
                }
                else
                {
                    dic.Add(p, -1);
                }
            }
            
            return this._roleDao.UpdateUserAuth(userId, dic);
        }

        public String GetRoleByUserId(int userId)
        {
            return this._roleDao.GetRoleByUserId(userId);
        }

        public List<String> GetAllAuth()
        {
            return this._authDao.GetAllAuth();
        }
    }
}
