using Autofac.Extensions.DependencyInjection;

namespace IoTManager.DI.Infrastructures
{
    public interface IocContainer
    {
        IocContainer Build();
        AutofacServiceProvider FetchServiceProvider();
    }
}
