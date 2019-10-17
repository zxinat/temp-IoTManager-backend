using System;
using System.Collections.Generic;
using System.Text;
using IoTManager.Model;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IGatewayBus
    {
        List<GatewaySerializer> GetAllGateways(String searchType, int page, String sortColumn, String order, String city, String factory, String workshop);
        GatewaySerializer GetGatewayById(int id);
        String CreateNewGateway(GatewaySerializer gatewaySerializer);
        String UpdateGateway(int id, GatewaySerializer gatewaySerializer);
        String DeleteGateway(int id);
        List<GatewaySerializer> GetGatewayByWorkshop(String city, String factory, String workshop);
        int BatchDeleteGateway(int[] ids);
        String CreateGatewayType(String gatewayType);
        long GetGatewayNumber(String searchType, String city="all", String factory="all", String workshop="all");
        int GetAffiliateDeviceNumber(int id);
        int FindGatewayIdExist(String gatewayId);
    }
}
