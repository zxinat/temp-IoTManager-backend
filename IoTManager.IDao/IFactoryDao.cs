using System;
using System.Collections.Generic;
using IoTManager.Model;
using IoTManager.Utility.Serializers;

namespace IoTManager.IDao
{
    public interface IFactoryDao
    {
        List<FactoryModel> Get(int pageMode = 0, int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc");
        FactoryModel GetById(int id);
        String Create(FactoryModel factoryModel);
        String Update(int id, FactoryModel factoryModel);
        String Delete(int id);
        List<FactoryModel> GetAffiliateFactory(String city);
        List<FactoryModel> GetByFactoryName(String factoryName);
        int GetFactoryAffiliateWorkshop(int id);
        int GetFactoryAffiliateDevice(int id);
        int GetFactoryAffiliateGateway(int id);
        long GetFactoryNumber();
    }
}