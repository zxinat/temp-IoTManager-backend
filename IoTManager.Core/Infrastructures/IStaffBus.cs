using IoTManager.Model;
using IoTManager.Utility.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Core.Infrastructures
{
    public interface IStaffBus
    {
        object ListStaffs(string search, int page, string sortColumn, string order, string department);
        object GetByStaffId(string staffId);
        string Create(StaffSerializer staffSerializer);
        object BatchDelete(List<string> staffIds);
        string Delete(string staffId);
        string Update(string staffId, StaffSerializer staffSerializer);
        string UpLoadImage(string staffId, string base64Image);
        string AddAuth(StaffAuthModel staffAuth);
        object BatchAddAuth(string staffId, List<string> deviceIds);
        object GetAuthDevice(string staffId);
        string DeleteAuth(string staffId, string deviceId);
        object BatchDeleteAuth(string staffId, List<string> deviceIds);
        object ListStaffIdsByDeviceId(string deviceId);
        object AddAuthByDepartment(string deviceId, string departmentName);
        object GetCurrentStaffOnShop(string deviceId);
        object GetYesterdayStaffOnShop(string deviceId);
        //object ListLatest10(string deviceId);
        object ListLatestEnter10(string deviceId);
        object ListLatestExit10(string deviceId);
        object GetLatestOne(string deviceId);
        int GetCurrentStatusByStaffId(string deviceId, string staffId);
        object ListStaffRole();
        object AddStaffRole(string staffRole);
        object UpdateStaffRole(int id, string staffRole);
        object DeleteStaffRole(int id);
    }
}
