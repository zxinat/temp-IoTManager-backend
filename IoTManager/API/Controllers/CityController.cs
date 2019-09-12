using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.API.Formalizers;
using IoTManager.Core.Infrastructures;
using IoTManager.DAL.DbContext;
using IoTManager.DAL.Models;
using IoTManager.Model;
using IoTManager.Utility;
using IoTManager.Utility.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Common;
using Swashbuckle.AspNetCore.Swagger;
using Result = IoTManager.DAL.ReturnType.Result;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityBus _cityBus;
        private readonly ILogger _logger;
        public CityController(ICityBus cityBus,ILogger<CityController> logger)
        {
            this._cityBus = cityBus;
            this._logger = logger;
        }

        // GET api/values
        [HttpGet]
        public ResponseSerializer Get()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetAllCities()
                );
        }

        // GET api/values/{id}
        [HttpGet("{id}")]
        public ResponseSerializer Get(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetCityById(id)
                );
        }

        // POST api/values
        [HttpPost]
        public ResponseSerializer Post([FromBody] CitySerializer citySerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.CreateNewCity(citySerializer)
                );
        }

        // PUT api/values/{id}
        [HttpPut("{id}")]
        public ResponseSerializer Put(int id, [FromBody] CitySerializer citySerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.UpdateCity(id, citySerializer)
                );
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ResponseSerializer Delete(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.DeleteCity(id)
                );
        }

        [HttpGet("cityOptions")]
        public ResponseSerializer GetCityOptions()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetCityOptions());
        }

        [HttpGet("mapInfo")]
        public ResponseSerializer GetMapInfo()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetMapInfo());
        }

        [HttpGet("oneMapInfo")]
        public ResponseSerializer GetOneMapInfo(String cityName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetCityMapInfo(cityName));
        }

        [HttpGet("cityCascaderOptions")]
        public ResponseSerializer GetCityCascaderOptions()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetCityCascaderOptions()
            );
        }

        [HttpGet("cityName/{cityName}")]
        public ResponseSerializer GetByCityName(String cityName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetByCityName(cityName));
        }

        [HttpGet("threeLevelMenu")]
        public ResponseSerializer GetThreeLevelMenu()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetThreeLevelMenu());
        }

        [HttpGet("affiliateFactory/{id}")]
        public ResponseSerializer GetCityAffiliateFactory(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetCityAffiliateFactory(id));
        }

        [HttpGet("affiliateDevice/{id}")]
        public ResponseSerializer GetCityAffiliateDevice(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetCityAffiliateDevice(id));
        }

        [HttpGet("affiliateGateway/{id}")]
        public ResponseSerializer GetCityAffiliateGateway(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetCityAffiliateGateway(id));
        }

        [HttpGet("cityFactoryTree")]
        public ResponseSerializer GetCityFactoryTree()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetCityFactoryTree());
        }

        [HttpGet("oneCityByName/{cityName}")]
        public ResponseSerializer GetOneCityByName(String cityName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._cityBus.GetOneCityByName(cityName));
        }
    }
}