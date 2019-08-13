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
        
        public List<FieldSerializer> GetAllFields()
        {
            List<FieldModel> fields = this._fieldDao.Get();
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
            return this._fieldDao.Create(fieldModel);
        }

        public List<FieldSerializer> GetAffiliateFields(String deviceName)
        {
            List<FieldModel> fields = this._fieldDao.Get();
            List<FieldSerializer> result = new List<FieldSerializer>();
            List<DeviceModel> devices = this._deviceDao.Get("all");
            var selectedDevice = devices.AsQueryable()
                .Where(sd => sd.HardwareDeviceId == deviceName)
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
        }
    }
}