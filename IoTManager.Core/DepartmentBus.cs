
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Core
{
    public sealed class DepartmentBus:IDepartmentBus
    {
        private readonly IDepartmentDao _departmentDao;
        private readonly ILogger _logger;
        public DepartmentBus(IDepartmentDao departmentDao,ILogger<DepartmentBus> logger)
        {
            _departmentDao = departmentDao;
            _logger = logger;
        }
        /*获取所有部门*/
        public object List()
        {
            try
            {
                List<DepartmentModel> departments= _departmentDao.List();
                List<DepartmentSerializer> result = new List<DepartmentSerializer>();
                foreach(var d in departments)
                {
                    result.Add(new DepartmentSerializer(d));
                }
                return result;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*通过名称获取部门*/
        public object GetByName(string departmentName)
        {
            try
            {
                DepartmentModel department= _departmentDao.GetByName(departmentName);
                return new DepartmentSerializer(department);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*通过id获取部门*/
        public object GetById(int id)
        {
            try
            {
                DepartmentModel department= _departmentDao.GetById(id);
                return new DepartmentSerializer(department);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*创建部门*/
        public string Create(DepartmentSerializer departmentSerializer)
        {
            DepartmentModel department = new DepartmentModel
            {
                departmentName = departmentSerializer.departmentName,
                admin = departmentSerializer.admin,
                remark = departmentSerializer.remark
            };
            try
            {
                DepartmentModel eDepartment = _departmentDao.GetByName(departmentSerializer.departmentName);
                if(eDepartment==null)
                {
                    return _departmentDao.Create(department);
                }
                else
                {
                    return "exist";
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*删除部门*/
        public object Delete(string departmentName)
        {
            try
            {
                return _departmentDao.Delete(departmentName);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*修改部门信息*/
        public string Update(int id,DepartmentSerializer departmentSerializer)
        {
            DepartmentModel department = new DepartmentModel
            {
                departmentName = departmentSerializer.departmentName,
                admin = departmentSerializer.admin,
                remark = departmentSerializer.remark
            };
            try
            {
                return _departmentDao.Update(id, department);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*获取部门内员工列表*/
        public object ListStaffsByDepartment(string department)
        {
            try
            {
                return _departmentDao.ListStaffsBydepartment(department);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*获取部门员工总数*/
        public object GetTotalByDepartment(string department)
        {
            try
            {
                return _departmentDao.GetTotalByDepartment(department);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        /*获取所有部门员工总数*/
        public object GetTotalNumber()
        {
            try
            {
                Dictionary<string, int> result = new Dictionary<string, int>();
                List<DepartmentModel> departments = _departmentDao.List();
                foreach(var d in departments)
                {
                    int total = _departmentDao.GetTotalByDepartment(d.departmentName);
                    result.Add(d.departmentName, total);
                }
                return result;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}

