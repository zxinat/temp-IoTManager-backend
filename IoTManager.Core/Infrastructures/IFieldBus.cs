using System;
using System.Collections.Generic;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IFieldBus
    {
        List<FieldSerializer> GetAllFields(int page = 1, String sortColumn = "id", String order = "asc");
        String CreateNewField(FieldSerializer fieldSerializer);
        List<FieldSerializer> GetAffiliateFields(String deviceName);
        String UpdateField(int id, FieldSerializer fieldSerializer);
        String DeleteField(int id);
        long GetFieldNumber();
    }
}