using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using IoTManager.Model;
using IoTManager.Model.DataReceiver;

namespace IoTManager.IDao
{
    public interface IWorkshopDao
    {
        List<WorkshopModel> Get(int pageMode = 0, int offset = 0, int limit = 6, String sortColumn = "id", String order = "asc");
        WorkshopModel GetById(int id);
        String Create(WorkshopModel workshopModel);
        String Update(int id, WorkshopModel workshopModel);
        String Delete(int id);
        List<WorkshopModel> GetAffiliateWorkshop(String cName,String fName);
        List<WorkshopTreeModel> ListWorkshopLoaction();
        List<string> ListWorkshopNames(string cName, string fName);
        List<WorkshopModel> GetByWorkshopName(String workshopName);
        int GetWorkshopAffiliateDevice(int id);
        int GetWorkshopAffiliateGateway(int id);
        long GetWorkshopNumber();
    }
}