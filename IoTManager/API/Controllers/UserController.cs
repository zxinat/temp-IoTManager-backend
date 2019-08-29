using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IoTManager.API.Formalizers;
using IoTManager.API.Services;
using IoTManager.Core.Infrastructures;
using Microsoft.AspNetCore.Mvc;
using IoTManager.DAL.Models;
using IoTManager.DAL.DbContext;
using IoTManager.DAL.ReturnType;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IoTManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBus _userBus;
        private readonly IRoleBus _roleBus;
        private readonly ILogger _logger;

        public UserController(IUserBus userBus, ILogger<UserController> logger, IRoleBus roleBus)
        {
            this._userBus = userBus;
            this._logger = logger;
            this._roleBus = roleBus;
        }

        // GET api/values
        [HttpGet]
        public ResponseSerializer Get()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._userBus.GetAllUsers()
            );
        }

        // GET api/values/{id}
        [HttpGet("{id}")]
        public ResponseSerializer Get(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._userBus.GetUserById(id)
            );
        }

        //GET api/user/username/{userName}
        [HttpGet("username/{userName}")]
        public ResponseSerializer GetByUserName(String userName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._userBus.GetUserByUserName(userName)
            );
        }

        // POST api/values
        [HttpPost]
        public ResponseSerializer Post([FromBody] UserSerializer userSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._userBus.CreateNewUser(userSerializer)
            );
        }

        // PUT api/values/{id}
        [HttpPut("{id}")]
        public ResponseSerializer Put(int id, [FromBody] UserSerializer userSerializer)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._userBus.UpdateUser(id, userSerializer)
            );
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ResponseSerializer Delete(int id)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._userBus.DeleteUser(id)
            );
        }

        [HttpGet("name/{userName}")]
        public ResponseSerializer GetByName(String userName)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._userBus.GetUsersByUserName(userName));
        }

        [HttpPost("password/{userName}")]
        public ResponseSerializer UpdatePassword(String userName, [FromBody] UserModel user)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._userBus.UpdatePassword(userName, user.Password));
        }

        [HttpGet("getAuth/{userId}")]
        public ResponseSerializer GetAuthByUserId(int userId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._userBus.GetAuthByUserId(userId));
        }

        [HttpPost("updateRoleAuth/{roleId}")]
        public ResponseSerializer UpdateAuthByRoleId(String roleId, [FromBody]BatchString batchString)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._roleBus.UpdateAuthByRoleId(roleId, batchString.str));
        }
        
        [HttpPost("updateUserAuth/{userId}")]
        public ResponseSerializer UpdateAuthByUserId(int userId, [FromBody]BatchString batchString)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._roleBus.UpdateAuthByUserId(userId, batchString.str));
        }

        [HttpGet("getRole/{userId}")]
        public ResponseSerializer GetRoleByUserId(int userId)
        {
            return new ResponseSerializer(
                200,
                "success",
                this._roleBus.GetRoleByUserId(userId));
        }

        [HttpGet("getAllAuth")]
        public ResponseSerializer GetAllAuth()
        {
            return new ResponseSerializer(
                200,
                "success",
                this._roleBus.GetAllAuth());
        }
    }
}