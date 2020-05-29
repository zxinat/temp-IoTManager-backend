using IoTManager.Model;
using StaffManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaffManager.Dao.Infrastructures
{
    public interface IStaffDao
    {
        StaffModel GetStaffInfoById(string staffId);
        List<StaffModel> ListStaffs(string searchType, string department = "all", int offset = 0, int limit = 12, string sortColumn = "id", string order = "asc");
        List<StaffModel> ListStaffByDepartment(string departmentName);
        List<string> ListStaffIdsByDepartmentAndDate(string departmentName, DateTime startTime, DateTime endTime);
        string Create(StaffModel staff);
        string Update(string staffId, StaffModel staff);
        string Delete(string staffId);
        int BatchDelete(List<string> staffIds);
        //List<StaffModel> ListStaffsByDeviceId(string deviceId);
        List<object> ListStaffRole();
        string AddStaffRole(string staffRole);
        string UpdateStaffRole(int id, string staffRole);
        string DeleteStaffRole(int id);
        List<string> ListStaffIds(string search);
    }
}
