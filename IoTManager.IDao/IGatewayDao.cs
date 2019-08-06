using System;
using System.Collections.Generic;
using System.Text;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IGatewayDao
    {
        List<GatewayModel> Get(int offset, int limit, int id, int createTime, int updateTime);
        GatewayModel GetById(int id);
        String Create(GatewayModel gatewayModel);
        String Update(int id, GatewayModel gatewayModel);
        String Delete(int id);
        List<GatewayModel> GetByWorkshop(String city, String factory, String workshop);
        int BatchDelete(int[] ids);
        String CreateGatewayType(String gatewayType);
    }
}
