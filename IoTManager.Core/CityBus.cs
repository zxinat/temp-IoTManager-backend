using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Http;
using System.Text;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using MongoDB.Bson;
using Newtonsoft.Json;
using IoTManager.Model.DataReceiver;

namespace IoTManager.Core
{
    public sealed class CityBus:ICityBus
    {
        private readonly IFactoryDao _factoryDao;
        private readonly IWorkshopDao _workshopDao;
        private readonly ICityDao _cityDao;
        private readonly IDeviceDao _deviceDao;
        private readonly ILogger _logger;
        public CityBus(ICityDao cityDao,ILogger<CityBus> logger, IFactoryDao factoryDao, IWorkshopDao workshopDao, IDeviceDao deviceDao)
        {
            this._cityDao = cityDao;
            this._logger = logger; 
            this._factoryDao = factoryDao;
            this._workshopDao = workshopDao;
            this._deviceDao = deviceDao;
        }

        public List<CitySerializer> GetAllCities(int pageMode = 0, int page = 1, String sortColumn = "id", String order = "asc")
        {
            int offset = (page - 1) * 6;
            int limit = 6;
            List<CityModel> cities =  this._cityDao.Get(pageMode, offset, limit, sortColumn, order);
            List<CitySerializer> result = new List<CitySerializer>();
            foreach (CityModel city in cities)
            {
                result.Add(new CitySerializer(city));
            }
            return result;
        }

        public CitySerializer GetCityById(int id)
        {
            CityModel city = this._cityDao.GetById(id);
            CitySerializer result = new CitySerializer(city);
            return result;
        }

        public String CreateNewCity(CitySerializer citySerializer)
        {
            CityModel cityModel = new CityModel();
            cityModel.CityName = citySerializer.cityName;
            cityModel.Remark = citySerializer.remark;
            String requestUrl = "https://restapi.amap.com/v3/geocode/geo?address=" + cityModel.CityName +
                                "&key=c6d99b34598e3721a00fb609eb4a4c1b";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = client.GetAsync(requestUrl).Result;
                var responseObj =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<GeoModel>(responseMessage.Content.ReadAsStringAsync().Result);
                var location = responseObj.geocodes[0].location.Split(",");
                cityModel.longitude = double.Parse(location[0]);
                cityModel.latitude = double.Parse(location[1]);
                return this._cityDao.Create(cityModel);
            }
        }

        public String UpdateCity(int id, CitySerializer citySerializer)
        {
            CityModel cityModel = new CityModel();
            cityModel.Id = id;
            cityModel.CityName = citySerializer.cityName;
            cityModel.Remark = citySerializer.remark;
            cityModel.latitude = citySerializer.latitude;
            cityModel.longitude = citySerializer.longitude;
            return this._cityDao.Update(id, cityModel);
        }

        public String DeleteCity(int id)
        {
            return this._cityDao.Delete(id);
        }

        public List<object> GetCityCascaderOptions()
        {
            List<CityModel> cities = this._cityDao.Get();
            List<object> result = new List<object>();
            result.Add(new {value="全部", label="全部"});
            foreach (CityModel c in cities)
            {
                List<object> children = new List<object>();
                List<FactoryModel> factories = this._factoryDao.GetAffiliateFactory(c.CityName);
                children.Add(new {value="全部", label="全部"});
                foreach (FactoryModel f in factories)
                {
                    List<object> subchildren = new List<object>();
                    List<WorkshopModel> workshops = this._workshopDao.GetAffiliateWorkshop(c.CityName,f.FactoryName);
                    subchildren.Add(new {value="全部", label="全部"});
                    foreach(WorkshopModel w in workshops)
                    {
                        subchildren.Add(new{value=w.WorkshopName, label=w.WorkshopName});
                    }
                    children.Add(new{value=f.FactoryName, label=f.FactoryName, children=subchildren});
                }
                result.Add(new {value=c.CityName, label=c.CityName, children=children});
            }

            return result;
        }
        /*  改进GetCityCascaderOptions函数
         *  获取实验室所属地，返回名称树结构
         */
        public List<object> tempName()
        {
            List<object> result = new List<object>();
            result.Add(new { value = "全部", label = "全部" });
            List<string> cityNames = _cityDao.ListCityName();
            List<WorkshopTreeModel> workshopTree = _workshopDao.ListWorkshopLoaction();
            foreach(string cityName in cityNames)
            {
                List<object> children = new List<object>();
                children.Add(new { value = "全部", label = "全部" });
                foreach( var w in workshopTree)
                {
                    List<object> subchildren = new List<object>();
                    subchildren.Add(new { value = "全部", label = "全部" });
                    if (w.cityName==cityName)
                    {
                        subchildren.Add(new { value = w.workshopName, label = w.workshopName });
                        children.Add(new { value = w.factoryName, label = w.factoryName, children = subchildren });
                    }
                    
                }
                result.Add(new { value = cityName, label = cityName, children = children });
            }
            return result;
        }
        public List<object> GetCityOptions()
        {
            List<CityModel> cities = this._cityDao.Get();
            List<object> result = new List<object>();
            foreach(CityModel c in cities)
            {
                result.Add(new{ValueTuple=c.CityName, label=c.CityName});
            }
            return result;
        }
        public List<Object> GetMapInfo()
        {
            List<CityModel> cities = this._cityDao.Get();
            List<DeviceModel> devices = this._deviceDao.Get("all");
            List<object> result = new List<object>();
            foreach (CityModel city in cities)
            {
                int offlineNum = devices.AsQueryable()
                    .Where(d => d.City == city.CityName && d.IsOnline == "no")
                    .ToList().Count;
                int onlineNum = devices.AsQueryable()
                    .Where(d => d.City == city.CityName && d.IsOnline == "yes")
                    .ToList().Count;
                List<object> info = new List<object>();
                info.Add(city.longitude);
                info.Add(city.latitude);
                info.Add("在线: " + onlineNum.ToString() + "; 离线: " + offlineNum.ToString());
                result.Add(new {name=city.CityName, value=info});
            }

            return result;
        }
        /*
        public List<object> GetCityMapInfo(String cityName)
        {
            CityModel city = this._cityDao.GetOneCityByName(cityName);
            List<DeviceModel> devices = this._deviceDao.Get("all");
            var offlineQuery = devices.AsQueryable()
                .Where(d => d.City == cityName && d.IsOnline == "no")
                .ToList().Count;
            var onlineQuery = devices.AsQueryable()
                .Where(d => d.City == cityName && d.IsOnline == "yes")
                .ToList().Count;
            List<object> result = new List<object>();
            List<object> info = new List<object>();
            info.Add(city.longitude);
            info.Add(city.latitude);
            info.Add("在线: " + onlineQuery.ToString() + "; 离线: " + offlineQuery.ToString());
            result.Add(new {name=cityName, value=info});
            return result;
        }
        */
        public List<object> GetCityMapInfo(String cityName)
        {
            CityModel city = this._cityDao.GetOneCityByName(cityName);
            List<DeviceModel> devices = this._deviceDao.ListByCity(cityName);
            var offlineQuery = devices.AsQueryable()
                .Where(d => d.IsOnline == "no")
                .ToList().Count;
            var onlineQuery = devices.AsQueryable()
                .Where(d => d.IsOnline == "yes")
                .ToList().Count;
            List<object> result = new List<object>();
            List<object> info = new List<object>();
            info.Add(city.longitude);
            info.Add(city.latitude);
            info.Add("在线: " + onlineQuery.ToString() + "; 离线: " + offlineQuery.ToString());
            result.Add(new { name = cityName, value = info });
            return result;
        }


        public List<CitySerializer> GetByCityName(String cityName)
        {
            List<CityModel> cities = this._cityDao.GetByCityName(cityName);
            List<CitySerializer> result = new List<CitySerializer>();
            foreach (CityModel c in cities)
            {
                result.Add(new CitySerializer(c));
            }

            return result;
        }

        public object GetThreeLevelMenu()
        {
            return this._cityDao.GetThreeLevelMenu();
        }

        public int GetCityAffiliateFactory(int id)
        {
            return this._cityDao.GetCityAffiliateFactory(id);
        }

        public int GetCityAffiliateDevice(int id)
        {
            return this._cityDao.GetCityAffiliateDevice(id);
        }

        public int GetCityAffiliateGateway(int id)
        {
            return this._cityDao.GetCityAffiliateGateway(id);
        }

        public object GetCityFactoryTree()
        {
            List<CityModel> cities = this._cityDao.Get();
            List<FactoryModel> factories = this._factoryDao.Get();
            List<object> result = new List<object>();
            foreach (CityModel c in cities)
            {
                List<FactoryModel> relatedFactories = factories.AsQueryable()
                    .Where(f => f.City == c.CityName)
                    .ToList();
                List<object> children = new List<object>();
                foreach (FactoryModel f in relatedFactories)
                {
                    children.Add(new {value = f.FactoryName, label = f.FactoryName, id = f.Id, factoryName = f.FactoryName,cityName=c.CityName});
                }
                result.Add(new {value = c.CityName, label = c.CityName, children = children});
            }

            return result;
        }

        public CitySerializer GetOneCityByName(String cityName)
        {
            CityModel city = this._cityDao.GetOneCityByName(cityName);
            CitySerializer result = new CitySerializer(city);
            return result;
        }

        public long GetCityNumber()
        {
            return this._cityDao.GetCityNumber();
        }
    }
}
