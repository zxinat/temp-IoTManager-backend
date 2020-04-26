using System;
using System.Collections.Generic;
using IoTManager.Model;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IWorkshopBus
    {
        List<WorkshopSerializer> GetAllWorkshops(int pageMode = 0, int page = 1, String sortColumn = "id", String order = "asc");
        WorkshopSerializer GetWorkshopById(int id);
        String CreateNewWorkshop(WorkshopSerializer workshopSerializer);
        String UpdateWorkshop(int id, WorkshopSerializer workshopSerializer);
        String DeleteWorkshop(int id);
        List<object> GetAffiliateWorkshop(String cName,String fName);
        List<object> ListWorkshopName(string cName, string fName);
        List<WorkshopSerializer> GetByWorkshopName(String workshopName);
        int GetWorkshopAffiliateDevice(int id);
        int GetWorkshopAffiliateGateway(int id);
        long GetWorkshopNumber();
    }
}