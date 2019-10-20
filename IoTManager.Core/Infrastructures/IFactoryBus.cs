using System;
using System.Collections.Generic;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IFactoryBus
    {
        List<FactorySerializer> GetAllFactories(int pageMode = 0, int page = 1, String sortColumn = "id", String order = "asc");
        FactorySerializer GetFactoryById(int id);
        String CreateNewFactory(FactorySerializer factorySerializer);
        String UpdateFactory(int id, FactorySerializer factorySerializer);
        String DeleteFactory(int id);
        List<object> GetAffiliateFactory(String cName);
        List<FactorySerializer> GetByFactoryName(String factoryName);
        int GetFactoryAffiliateWorkshop(int id);
        int GetFactoryAffiliateDevice(int id);
        int GetFactoryAffiliateGateway(int id);
        long GetFactoryNumber();
    }
}