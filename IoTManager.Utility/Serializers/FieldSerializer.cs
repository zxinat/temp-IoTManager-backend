using System;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class FieldSerializer
    {
        public FieldSerializer()
        {
            this.id = 0;
            this.fieldName = null;
            this.fieldId = null;
            this.createTime = null;
            this.updateTime = null;
            this.device = null;
        }

        public FieldSerializer(FieldModel fieldModel)
        {
            this.id = fieldModel.Id;
            this.fieldName = fieldModel.FieldName;
            this.fieldId = fieldModel.FieldId;
            this.createTime = DateTime.Parse(fieldModel.CreateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.updateTime = DateTime.Parse(fieldModel.UpdateTime.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
            this.device = fieldModel.Device;
        }
        
        public int id { get; set; }
        public String fieldName { get; set; }
        public String fieldId { get; set; }
        public String createTime { get; set; }
        public String updateTime { get; set; }
        public String device { get; set; }
    }
}