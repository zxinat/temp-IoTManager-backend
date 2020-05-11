using IoTManager.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.IDao
{
    public interface IDepartmentDao
    {
        List<DepartmentModel> List();
        DepartmentModel GetByName(string departmentName);
        DepartmentModel GetById(int id);
        string Create(DepartmentModel department);
        string Delete(string departmentName);
        string Update(int id, DepartmentModel department);
        List<StaffModel> ListStaffsBydepartment(string department);
        int GetTotalByDepartment(string department);
    }
}
