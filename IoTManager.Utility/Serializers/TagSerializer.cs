using System;
using IoTManager.Model;
using MongoDB.Driver;

namespace IoTManager.Utility.Serializers
{
    public class TagSerializer
    {
        public TagSerializer()
        {
            this.id = 0;
            this.tagName = null;
            this.timestamp = null;
        }

        public TagSerializer(TagModel tagModel)
        {
            this.id = tagModel.Id;
            this.tagName = tagModel.TagName;
            this.timestamp = DateTime.Parse(tagModel.Timestamp.ToString())
                .ToLocalTime().ToString(Constant.getDateFormatString());
        }
        public int id { get; set; }
        public String tagName { get; set; }
        public String timestamp { get; set; }
    }
}