using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTManager.Core
{
    public class CustomerBus:ICustomerBus
    {
        private readonly ICustomerDao _customerDao;
        private readonly ILogger _logger;
        private readonly IDeviceDataDao _deviceDataDao;
        private readonly IDeviceDao _deviceDao;
        public CustomerBus(ICustomerDao customerDao,
            ILogger<CustomerBus> logger,
            IDeviceDataDao deviceDataDao,
            IDeviceDao deviceDao)
        {
            _customerDao = customerDao;
            _logger = logger;
            _deviceDataDao = deviceDataDao;
            _deviceDao = deviceDao;
        }
    }
}
