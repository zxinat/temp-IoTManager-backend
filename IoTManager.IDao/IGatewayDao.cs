using System;
using System.Collections.Generic;
using System.Text;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IGatewayDao
    {
        List<GatewayModel> Get(String searchType, int offset, int limit, String sortColumn, String order, String city, String factory, String workshop);
        GatewayModel GetById(int id);
        String Create(GatewayModel gatewayModel);
        String Update(int id, GatewayModel gatewayModel);
        String Delete(int id);
        List<GatewayModel> GetByWorkshop(String city, String factory, String workshop);
        int BatchDelete(int[] ids);
        String CreateGatewayType(String gatewayType);
        long GetGatewayNumber(String searchType, String city="all", String factory="all", String workshop="all");
        int GetAffiliateDeviceNumber(int id);
        int FindGatewayIdExist(String gatewayId);
    }
}
