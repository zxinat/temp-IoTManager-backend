using System;
using System.Collections.Generic;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;

namespace IoTManager.Core
{
    public sealed class FactoryBus: IFactoryBus
    {
        private readonly IFactoryDao _factoryDao;
        private readonly ILogger _logger;

        public FactoryBus(IFactoryDao factoryDao, ILogger<FactoryBus> logger)
        {
            this._factoryDao = factoryDao;
            this._logger = logger;
        }
        
        public List<FactorySerializer> GetAllFactories()
        {
            List<FactoryModel> factories = this._factoryDao.Get();
            List<FactorySerializer> result = new List<FactorySerializer>();
            foreach (FactoryModel factory in factories)
            {
                result.Add(new FactorySerializer(factory));
            }
            return result;
        }

        public FactorySerializer GetFactoryById(int id)
        {
            FactoryModel factory = this._factoryDao.GetById(id);
            FactorySerializer result = new FactorySerializer(factory);
            return result;
        }

        public String CreateNewFactory(FactorySerializer factorySerializer)
        {
            FactoryModel factoryModel = new FactoryModel();
            factoryModel.FactoryName = factorySerializer.factoryName;
            factoryModel.FactoryPhoneNumber = factorySerializer.factoryPhoneNumber;
            factoryModel.FactoryAddress = factorySerializer.factoryAddress;
            factoryModel.Remark = factorySerializer.remark;
            factoryModel.City = factorySerializer.city;
            return this._factoryDao.Create(factoryModel);
        }

        public String UpdateFactory(int id, FactorySerializer factorySerializer)
        {
            FactoryModel factoryModel = new FactoryModel();
            factoryModel.Id = factorySerializer.id;
            factoryModel.FactoryName = factorySerializer.factoryName;
            factoryModel.FactoryPhoneNumber = factorySerializer.factoryPhoneNumber;
            factoryModel.FactoryAddress = factorySerializer.factoryAddress;
            factoryModel.Remark = factorySerializer.remark;
            factoryModel.City = factorySerializer.city;
            return this._factoryDao.Update(id, factoryModel);
        }

        public String DeleteFactory(int id)
        {
            return this._factoryDao.Delete(id);
        }

        public List<object> GetAffiliateFactory(String cName)
        {
            List<FactoryModel> factories = this._factoryDao.GetAffiliateFactory(cName);
            List<object> result = new List<object>();
            foreach (FactoryModel f in factories)
            {
                result.Add(new {value=f.FactoryName, label=f.FactoryName});
            }

            return result;
        }

        public List<FactorySerializer> GetByFactoryName(String factoryName)
        {
            List<FactoryModel> factories = this._factoryDao.GetByFactoryName(factoryName);
            List<FactorySerializer> result = new List<FactorySerializer>();
            foreach (FactoryModel factory in factories)
            {
                result.Add(new FactorySerializer(factory));
            }
            return result;
        }

        public int GetFactoryAffiliateWorkshop(int id)
        {
            return this._factoryDao.GetFactoryAffiliateWorkshop(id);
        }

        public int GetFactoryAffiliateDevice(int id)
        {
            return this._factoryDao.GetFactoryAffiliateDevice(id);
        }

        public int GetFactoryAffiliateGateway(int id)
        {
            return this._factoryDao.GetFactoryAffiliateGateway(id);
        }
    }
}