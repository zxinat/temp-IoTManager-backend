using System;
using System.Collections.Generic;
using System.Text;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface ICityDao
    {
        String Create(CityModel cityModel);
        List<CityModel> Get(int pageMode = 0, int offset = 0, int limit = 6, String sortColumn = "id", String order = "asc");
        CityModel GetById(int id);
        String Update(int id, CityModel cityModel);
        String Delete(int id);
        List<CityModel> GetByCityName(String cityName);
        CityModel GetOneCityByName(String cityName);
        object GetThreeLevelMenu();
        int GetCityAffiliateFactory(int id);
        int GetCityAffiliateDevice(int id);
        int GetCityAffiliateGateway(int id);
        long GetCityNumber();
    }
}
