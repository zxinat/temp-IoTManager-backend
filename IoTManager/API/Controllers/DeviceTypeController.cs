using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.API.Formalizers;
using IoTManager.DAL.DbContext;
using IoTManager.DAL.Models;
using IoTManager.DAL.ReturnType;
using Microsoft.AspNetCore.Mvc;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceTypeController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public Result Get()
        {
            using (DatabaseContext dbcon = new DatabaseContext())
            {
                /************************************
                 * Find All DeviceType from the Database
                 ************************************/

                var deviceTypes = dbcon.DeviceType
                    .ToList();
                
                
                /************************************
                 * Serialize the Result
                 ************************************/
                
                List<DeviceTypeFormalizer> results = new List<DeviceTypeFormalizer>();

                foreach (DeviceType dt in deviceTypes)
                {
                    results.Add(new DeviceTypeFormalizer(dt));
                }
                
                
                /************************************
                 * Return
                 ************************************/
                
                return new Result(
                    200,
                    "success",
                    results
                );
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}