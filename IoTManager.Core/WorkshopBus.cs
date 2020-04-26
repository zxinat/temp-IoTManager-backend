using System;
using System.Collections.Generic;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;

namespace IoTManager.Core
{
    public class WorkshopBus: IWorkshopBus
    {
        private readonly IWorkshopDao _workshopDao;
        private readonly ILogger _logger;

        public WorkshopBus(IWorkshopDao workshopDao, ILogger<WorkshopBus> logger)
        {
            this._workshopDao = workshopDao;
            this._logger = logger;
        }
        
        public List<WorkshopSerializer> GetAllWorkshops(int pageMode = 0, int page = 1, String sortColumn = "id", String order = "asc")
        {
            int offset = (page - 1) * 6;
            int limit = 6;
            List<WorkshopModel> workshops = this._workshopDao.Get(pageMode, offset, limit, sortColumn, order);
            List<WorkshopSerializer> result = new List<WorkshopSerializer>();
            foreach (WorkshopModel workshop in workshops)
            {
                result.Add(new WorkshopSerializer(workshop));
            }
            return result;
        }

        public WorkshopSerializer GetWorkshopById(int id)
        {
            WorkshopModel workshop = this._workshopDao.GetById(id);
            WorkshopSerializer result = new WorkshopSerializer(workshop);
            return result;
        }

        public String CreateNewWorkshop(WorkshopSerializer workshopSerializer)
        {
            WorkshopModel workshopModel = new WorkshopModel();
            workshopModel.WorkshopName = workshopSerializer.workshopName;
            workshopModel.WorkshopPhoneNumber = workshopSerializer.workshopPhoneNumber;
            workshopModel.WorkshopAddress = workshopSerializer.workshopAddress;
            workshopModel.Remark = workshopSerializer.remark;
            workshopModel.Factory = workshopSerializer.factory;
            workshopModel.City = workshopSerializer.city;
            return this._workshopDao.Create(workshopModel);
        }

        public String UpdateWorkshop(int id, WorkshopSerializer workshopSerializer)
        {
            WorkshopModel workshopModel = new WorkshopModel();
            workshopModel.Id = workshopSerializer.id;
            workshopModel.WorkshopName = workshopSerializer.workshopName;
            workshopModel.WorkshopPhoneNumber = workshopSerializer.workshopPhoneNumber;
            workshopModel.WorkshopAddress = workshopSerializer.workshopAddress;
            workshopModel.Remark = workshopSerializer.remark;
            workshopModel.Factory = workshopSerializer.factory;
            return this._workshopDao.Update(id, workshopModel);
        }

        public String DeleteWorkshop(int id)
        {
            return this._workshopDao.Delete(id);
        }

        public List<object> GetAffiliateWorkshop(String cName,String fName)
        {
            List<WorkshopModel> workshops = this._workshopDao.GetAffiliateWorkshop(cName,fName);
            List<object> result = new List<object>();
            foreach (WorkshopModel w in workshops)
            {
                result.Add(new {value=w.WorkshopName, label=w.WorkshopName});
            }

            return result;
        }
        /* 获取实验室列表*/
        public List<object> ListWorkshopName(string cName,string fName)
        {
            List<string> names= this._workshopDao.ListWorkshopNames(cName, fName);
            List<object> result = new List<object>();
            if(names.Count!=0)
            {
                foreach (string name in names)
                {
                    result.Add(new
                    {
                        label = name,
                        value = name
                    });
                }
            }
            else
            {
                result = null;
            }
            return result;
        }

        public List<WorkshopSerializer> GetByWorkshopName(String workshopName)
        {
            List<WorkshopModel> workshops = this._workshopDao.GetByWorkshopName(workshopName);
            List<WorkshopSerializer> result = new List<WorkshopSerializer>();
            foreach (WorkshopModel workshop in workshops)
            {
                result.Add(new WorkshopSerializer(workshop));
            }
            return result;
        }

        public int GetWorkshopAffiliateDevice(int id)
        {
            return this._workshopDao.GetWorkshopAffiliateDevice(id);
        }

        public int GetWorkshopAffiliateGateway(int id)
        {
            return this._workshopDao.GetWorkshopAffiliateGateway(id);
        }

        public long GetWorkshopNumber()
        {
            return this._workshopDao.GetWorkshopNumber();
        }
    }
}