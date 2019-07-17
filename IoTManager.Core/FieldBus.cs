using System;
using System.Collections.Generic;
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
        private readonly ILogger _logger;

        public FieldBus(IFieldDao fieldDao, ILogger<FieldBus> logger)
        {
            this._fieldDao = fieldDao;
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
            return this._fieldDao.Create(fieldModel);
        }
    }
}