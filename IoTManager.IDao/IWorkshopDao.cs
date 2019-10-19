using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IWorkshopDao
    {
        List<WorkshopModel> Get(int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc");
        WorkshopModel GetById(int id);
        String Create(WorkshopModel workshopModel);
        String Update(int id, WorkshopModel workshopModel);
        String Delete(int id);
        List<WorkshopModel> GetAffiliateWorkshop(String fName);
        List<WorkshopModel> GetByWorkshopName(String workshopName);
        int GetWorkshopAffiliateDevice(int id);
        int GetWorkshopAffiliateGateway(int id);
        long GetWorkshopNumber();
    }
}