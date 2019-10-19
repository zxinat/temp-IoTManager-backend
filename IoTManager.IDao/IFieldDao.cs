using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IFieldDao
    {
        List<FieldModel> Get(int offset = 0, int limit = 12, String sortColumn = "id", String order = "asc");
        String Create(FieldModel field);
        String Update(int id, FieldModel field);
        String Delete(int id);
        long GetFieldNumber();
    }
}