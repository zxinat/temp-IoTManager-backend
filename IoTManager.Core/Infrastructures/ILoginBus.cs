using System;
using IoTManager.Model;

namespace IoTManager.Core.Infrastructures
{
    public interface ILoginBus
    {
        object Login(LoginModel loginModel);
    }
}