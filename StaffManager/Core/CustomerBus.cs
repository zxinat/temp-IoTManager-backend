using StaffManager.Core.Infrastructures;
using StaffManager.Dao.Infrastructures;
using IoTManager.Model;
using StaffManager.Models.RequestModel;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using IoTManager.IDao;
using StaffManager.Models;
using StaffManager.Models.Serializers;

namespace StaffManager.Core
{
    public class CustomerBus:ICustomerBus
    {
        private readonly ICustomerDao _customerDao;
        private readonly ILogger _logger;
        private readonly IStaffDataDao _staffDataDao;
        private readonly IDeviceDao _deviceDao;
        public CustomerBus(ICustomerDao customerDao,
            ILogger<CustomerBus> logger,
            IStaffDataDao staffDataDao,
            IDeviceDao deviceDao)
        {
            _customerDao = customerDao;
            _logger = logger;
            _staffDataDao = staffDataDao;
            _deviceDao = deviceDao;
        }
        /*********************/
        /*    访客信息管理    */
        /*********************/

        /* 获取所有员工信息，
         * 输入：
         *  search 搜索类型：search="search"时，开启分页功能，
         *           否则后面的参数都无效，返回所有员工
         *  page 页码
         *  sortColumn 排序字段
         *  order 升序(asc)/降序(dsc)
         *  
         * 输出：
         * 员工列表
         */
        public object ListCustomers(string search, int page, string sortColumn, string order)
        {
            int offset = (page - 1) * 12;
            int limit = 12;
            try
            {
                List<CustomerModel> customers = _customerDao.ListCustomers(search, offset, limit, sortColumn, order);
                List<CustomerSerializer> result = new List<CustomerSerializer>();
                if(customers.Count!=0)
                {
                    foreach(var c in customers)
                    {
                        result.Add(new CustomerSerializer(c));
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        /*创建外来人员*/
        public string Create(CustomerFormModel customerForm)
        {
            CustomerModel customer = new CustomerModel(customerForm);
            List<string> existStaffIds = _customerDao.ListAllStaffIds();
            if(!existStaffIds.Contains(customer.staffId))
            { 
                return _customerDao.Create(customer);
            }
            else
            {
                return "exist";
            }
        }

        /*修改访客信息*/
        public string Update(int id,CustomerFormModel customerForm)
        {
            CustomerModel customer = _customerDao.GetById(id);
            CustomerModel newCustomer = new CustomerModel(customerForm);
            newCustomer.lastArea = customer.lastArea;
            if(customer.status==1&newCustomer.status==0)
            {
                newCustomer.lastTime = DateTime.Now.ToLocalTime();
                StaffDataModel staffData = _staffDataDao.GetByIndexIdLatestOne(customer.staffId);
                if(staffData!=null)
                {
                    newCustomer.lastArea = staffData.DeviceName;
                }
            }
            else
            {
                newCustomer.lastTime = customer.lastTime <= new DateTime(1970, 1, 1) ? 
                    new DateTime(1970, 1, 1) : customer.lastTime;
                newCustomer.lastArea = customer.lastArea;
            }
            return _customerDao.Update(id, newCustomer);
        }
        /*注销访客*/
        public string Logout(string staffId)
        {
            //获取持卡访客信息（未注销）
            CustomerModel customer = _customerDao.GetCurrentByStaffId(staffId);
            if(customer!=null)
            {
                //获取此卡片最后访问的设备名
                StaffDataModel deviceData = _staffDataDao.GetByIndexIdLatestOne(staffId);
                if(deviceData!=null)
                {
                    customer.lastArea = deviceData.DeviceName;
                    
                }
                customer.status = 0;
                customer.lastTime = DateTime.Now.ToLocalTime();
                
                return _customerDao.Update(customer.id, customer);
            }
            else
            {
                return "failed";
            }
        }
        /*删除访客信息，访客的staffId是动态变化的，所以使用Id作为唯一标识*/
        public string Delete(int id)
        {
            return _customerDao.Delete(id);
        }
        
        
        /*********************/
        /*    访客权限管理    */
        /*********************/
        /*访客权限管理*/
        //添加权限
        public string AddAuth(int id,string deviceId )
        {
            
            CustomerModel customer = _customerDao.GetById(id);
            if(customer!=null)
            {
                List<string> deviceIds = _customerDao.ListAuth(id);
                if(!deviceIds.Contains(deviceId))
                {
                    return _customerDao.AddAuth(customer, deviceId);
                }
                else
                {
                    return "conflict";
                }                
            }
            else
            {
                return "noContent";
            }
        }
        /*删除访客权限*/
    }
}
