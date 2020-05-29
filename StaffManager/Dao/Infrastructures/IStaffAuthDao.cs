using StaffManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaffManager.Dao.Infrastructures
{
    public interface IStaffAuthDao
    {
        string AddAuth(StaffAuthModel authModel);
        List<string> GetAuthDevice(string staffId);
        bool Contain(string staffId, string deviceId);
        string DeleteAuth(string staffId, string deviceId);
        string DeleteAuthByStaffId(string staffId);
        int BatchDeleteAuth(string staffId, List<string> deviceIds);
        List<string> ListStaffIdsByDeviceId(string deviceId);
        List<string> ListStaffIdsByDeviceId(string deviceId, DateTime startTime, DateTime endTime);
        List<StaffModel> ListStaffsByDeviceId(string deviceId);
    }
}
