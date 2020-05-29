using IoTManager.Model;
using IoTManager;
using System;
using System.Collections.Generic;
using System.Text;
using StaffManager.Models.RequestModel;
using StaffManager.Models;

namespace StaffManager.Core.Infrastructures
{
    public interface ICustomerBus
    {
        object ListCustomers(string search, int page, string sortColumn, string order);
        string Create(CustomerFormModel customer);
        string Update(int id, CustomerFormModel customerForm);
        string Logout(string staffId);
        string Delete(int id);
        string AddAuth(int id, string deviceId);
    }
}
