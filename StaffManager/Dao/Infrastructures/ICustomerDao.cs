using IoTManager.Model;
using System;
using System.Collections.Generic;
using System.Text;
using StaffManager.Models;

namespace StaffManager.Dao.Infrastructures
{
    public interface ICustomerDao
    {
        List<CustomerModel> ListCustomers(string searchType, int offset = 0, int limit = 12, string sortColumn = "id", string order = "asc");
        List<string> ListAllStaffIds();
        string Create(CustomerModel customer);
        string Update(int id, CustomerModel customer);
        CustomerModel GetCurrentByStaffId(string staffId);
        List<CustomerModel> ListBystaffIdAndDate(string staffId, DateTime startTime, DateTime endTime);
        string Delete(int id);
        bool IsExist(int id);
        CustomerModel GetById(int id);
        List<string> ListAuth(int id);
        string AddAuth(CustomerModel customer, string deviceId);
        int BatchAddAuth(CustomerModel customer, List<string> deviceIds);
        string DeleteAuth(CustomerModel customer, string deviceId);
        int BatchDeleteAuth(CustomerModel customer, List<string> deviceIds);
    }
}
