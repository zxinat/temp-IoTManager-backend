using IoTManager.Model;
using IoTManager;
using System;
using System.Collections.Generic;
using System.Text;
using IoTManager.Model.RequestModel;

namespace IoTManager.Core.Infrastructures
{
    public interface ICustomerBus
    {
        object ListCustomers(string search, int page, string sortColumn, string order);
        string Create(CustomerFormModel customer);
        string Update(int id, CustomerModel customer);
        string Logout(string staffId);
        string Delete(int id);
        string AddAuth(int id, string deviceId);
    }
}
