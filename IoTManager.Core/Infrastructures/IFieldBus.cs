using System;
using System.Collections.Generic;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IFieldBus
    {
        List<FieldSerializer> GetAllFields();
        String CreateNewField(FieldSerializer fieldSerializer);
        List<FieldSerializer> GetAffiliateFields(String deviceName);
    }
}