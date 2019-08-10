using System;
using IoTManager.Model;

namespace IoTManager.Utility.Serializers
{
    public class CitySerializer
    {
        public CitySerializer()
        {
            this.id = 0;
            this.cityName = null;
            this.remark = null;
            this.createTime = null;
            this.updateTime = null;
            this.latitude = -1;
            this.longitude = -1;
        }

        public CitySerializer(CityModel cityModel)
        {
            this.id = cityModel.Id;
            this.cityName = cityModel.CityName;
            this.remark = cityModel.Remark;
            this.createTime = cityModel.CreateTime
                .ToString(Constant.getDateFormatString());
            this.updateTime = cityModel.UpdateTime
                .ToString(Constant.getDateFormatString());
            this.longitude = cityModel.longitude;
            this.latitude = cityModel.latitude;
        }
        
        public int id { get; set; }
        public String cityName { get; set; }
        public String remark { get; set; }
        public String createTime { get; set; }
        public String updateTime { get; set; }
        public int longitude { get; set; }
        public int latitude{ get; set; }
    }
}