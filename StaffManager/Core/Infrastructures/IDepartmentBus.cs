using IoTManager.Utility.Serializers;
using StaffManager.Models.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaffManager.Core.Infrastructures
{
    public interface IDepartmentBus
    {
        object List();
        object GetByName(string departmentName);
        object GetById(int id);
        string Create(DepartmentSerializer departmentSerializer);
        object Delete(string departmentName);
        string Update(int id, DepartmentSerializer departmentSerializer);
        object ListStaffsByDepartment(string department);
        object GetTotalByDepartment(string department);
        object GetTotalNumber();
    }
}
