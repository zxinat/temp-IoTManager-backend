using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IoTManager.Model
{
    public class DeviceDataModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }
        
        [BsonElement("DeviceId")]
        public String DeviceId { get; set; }
        
        [BsonElement("DeviceName")]
        public String DeviceName { get; set; }
        
        [BsonElement("MonitorName")]
        public String IndexName { get; set; }
        
        [BsonElement("MonitorId")]
        public String IndexId { get; set; }
        
        [BsonElement("Unit")]
        public String IndexUnit { get; set; }
        
        [BsonElement("MonitorType")]
        public String IndexType { get; set; }
        
        [BsonElement("Value")]
        public Double IndexValue { get; set; }
        
        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
        
        [BsonElement("IsScam")]
        public String Inspected { get; set; }
        
        [BsonElement("GatewayId")]
        public String GatewayId { get; set; }
        
        [BsonElement("DeviceType")]
        public String DeviceType { get; set; }
        
        [BsonElement("Mark")]
        public String Mark { get; set; }
        
        [BsonElement("IsCheck")]
        public String IsCheck { get; set; }
    }
}