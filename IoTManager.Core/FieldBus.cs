using System;
using System.Collections.Generic;
using System.Linq;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;

namespace IoTManager.Core
{
    public sealed class FieldBus: IFieldBus
    {
        private readonly IFieldDao _fieldDao;
        private readonly IDeviceDao _deviceDao;
        private readonly ILogger _logger;

        public FieldBus(IFieldDao fieldDao, IDeviceDao deviceDao, ILogger<FieldBus> logger)
        {
            this._fieldDao = fieldDao;
            this._deviceDao = deviceDao;
            this._logger = logger;
        }
        
        public List<FieldSerializer> GetAllFields(int pageMode = 0, int page = 1, String sortColumn = "id", String order = "asc")
        {
            int offset = (page - 1) * 12;
            int limit = 12;
            List<FieldModel> fields = this._fieldDao.Get(pageMode, offset, limit, sortColumn, order);
            List<FieldSerializer> result = new List<FieldSerializer>();

            foreach (FieldModel f in fields)
            {
                result.Add(new FieldSerializer(f));
            }

            return result;
        }

        public String CreateNewField(FieldSerializer fieldSerializer)
        {
            FieldModel fieldModel = new FieldModel();
            fieldModel.FieldName = fieldSerializer.fieldName;
            fieldModel.FieldId = fieldSerializer.fieldId;
            fieldModel.Device = fieldSerializer.device;
            if(!_fieldDao.IsExist(fieldModel))
            {
                return this._fieldDao.Create(fieldModel);
            }
            return "exist";
        }

        public List<FieldSerializer> GetAffiliateFields(String deviceId)
        {
            /*
            List<FieldModel> fields = this._fieldDao.Get();
            List<FieldSerializer> result = new List<FieldSerializer>();
            List<DeviceModel> devices = this._deviceDao.Get("all");
            var selectedDevice = devices.AsQueryable()
                .Where(sd => sd.HardwareDeviceId == deviceId)
                .FirstOrDefault(d => d.Id != 0);
            if (selectedDevice != null)
            {
                var affiliateFields = fields.AsQueryable()
                    .Where(f => f.Device == selectedDevice.DeviceName)
                    .ToList();
                foreach (var f in affiliateFields)
                {
                    result.Add(new FieldSerializer(f));
                }

                return result;
            }
            else
            {
                return new List<FieldSerializer>();
            }
            */
            /*zxin-ÐÞ¸Ä*/
            List<FieldSerializer> result = new List<FieldSerializer>();
            List<FieldModel> fields = this._fieldDao.ListFieldsByDeviceId(deviceId);
            foreach(var f in fields)
            {
                result.Add(new FieldSerializer(f));
            }
            return result;
        }

        public String UpdateField(int id, FieldSerializer fieldSerializer)
        {
            FieldModel fieldModel = new FieldModel();
            fieldModel.Id = fieldSerializer.id;
            fieldModel.FieldId = fieldSerializer.fieldId;
            fieldModel.FieldName = fieldSerializer.fieldName;
            fieldModel.Device = fieldSerializer.device;
            fieldModel.UpdateTime = DateTime.Now;
            return this._fieldDao.Update(id, fieldModel);
        }

        public String DeleteField(int id)
        {
            return this._fieldDao.Delete(id);
        }

        public long GetFieldNumber()
        {
            return this._fieldDao.GetFieldNumber();
        }
    }
}