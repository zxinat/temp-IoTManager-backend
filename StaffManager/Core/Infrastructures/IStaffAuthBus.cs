using StaffManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Core.Infrastructures
{
    public interface IStaffAuthBus
    {
        string AddAuth(StaffAuthModel staffAuth);
        object BatchAddAuth(string staffId, List<string> deviceIds);
        object GetAuthDevice(string staffId);
        string DeleteAuth(string staffId, string deviceId);
        object BatchDeleteAuth(string staffId, List<string> deviceIds);
        object ListStaffIdsByDeviceId(string deviceId);
        int AddAuthByDepartment(string deviceId, string departmentName);
    }
}
