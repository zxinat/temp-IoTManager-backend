using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface ISeverityDao
    {
        List<SeverityModel> Get();
        SeverityModel GetById(int id);
    }
}