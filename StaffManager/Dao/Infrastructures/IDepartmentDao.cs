using StaffManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaffManager.Dao.Infrastructures
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
        List<DepartmentModel> ListByDate(DateTime startTime, DateTime endTime);
    }
}
