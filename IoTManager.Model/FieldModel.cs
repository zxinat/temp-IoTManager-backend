using System;

namespace IoTManager.Model
{
    public class FieldModel
    {
        public int Id { get; set; }
        public String FieldName { get; set; }
        public String FieldId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}