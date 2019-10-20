using System;
using System.Collections.Generic;
using System.Text;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Org.BouncyCastle.Crypto.Digests;

namespace IoTManager.Core.Infrastructures
{
    public interface ICityBus
    {
        List<CitySerializer> GetAllCities(int pageMode = 0, int page = 1, String sortColumn = "id", String order = "asc");
        CitySerializer GetCityById(int id);
        String CreateNewCity(CitySerializer citySerializer);
        String UpdateCity(int id, CitySerializer citySerializer);
        String DeleteCity(int id);
        List<object> GetCityCascaderOptions();
        List<object> GetCityOptions();
        List<object> GetMapInfo();
        List<CitySerializer> GetByCityName(String cityName);
        List<object> GetCityMapInfo(String cityName);
        object GetThreeLevelMenu();
        int GetCityAffiliateFactory(int id);
        int GetCityAffiliateDevice(int id);
        int GetCityAffiliateGateway(int id);
        object GetCityFactoryTree();
        CitySerializer GetOneCityByName(String cityName);
        long GetCityNumber();
    }
}
