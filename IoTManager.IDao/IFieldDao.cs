using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IFieldDao
    {
        List<FieldModel> Get();
        String Create(FieldModel field);
    }
}