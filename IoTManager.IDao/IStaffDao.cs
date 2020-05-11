using IoTManager.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.IDao
{
    public interface IStaffDao
    {
        StaffModel GetStaffInfoById(string staffId);
        List<StaffModel> ListStaffs(string searchType, string department = "all", int offset = 0, int limit = 12, string sortColumn = "id", string order = "asc");
        List<StaffModel> ListStaffByDepartment(string departmentName);
        string Create(StaffModel staff);
        string Update(string staffId, StaffModel staff);
        string Delete(string staffId);
        int BatchDelete(List<string> staffIds);
        string AddAuth(StaffAuthModel authModel);
        List<string> GetAuthDevice(string staffId);
        bool Contain(string staffId, string deviceId);
        string DeleteAuth(string staffId, string deviceId);
        int BatchDeleteAuth(string staffId, List<string> deviceIds);
        List<string> ListStaffIdsByDeviceId(string deviceId);
        List<StaffModel> ListStaffsByDeviceId(string deviceId);
        List<object> ListStaffRole();
        string AddStaffRole(string staffRole);
        string UpdateStaffRole(int id, string staffRole);
        string DeleteStaffRole(int id);
    }
}
