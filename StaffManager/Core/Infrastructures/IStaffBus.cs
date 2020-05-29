using IoTManager.Model;
using StaffManager.Models.RequestModel;
using IoTManager.Utility.Serializers;
using StaffManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaffManager.Core.Infrastructures
{
    public interface IStaffBus
    {
        object ListStaffs(string search, int page, string sortColumn, string order, string department);
        object GetByStaffId(string staffId);
        string Create(StaffFormModel staffForm);
        object BatchDelete(List<string> staffIds);
        string Delete(string staffId);
        string Update(string staffId, StaffFormModel staffForm);
        string Logout(string staffId, string status);
        string UpLoadImage(string staffId, string base64Image);
        object ListStaffRole();
        object AddStaffRole(string staffRole);
        object UpdateStaffRole(int id, string staffRole);
        object DeleteStaffRole(int id);
        void CreateData(StaffDataFormModel staffData);
        int BatchCreateData(List<StaffDataFormModel> staffDataForms);
    }
}
