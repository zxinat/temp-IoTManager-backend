using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Models
{
    public class AttendenceDataModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("StaffId")]
        public string staffId { get; set; }
        [BsonElement("TagId")]
        public string tagId { get; set; }
        [BsonElement("DeviceId")]
        public string deviceId { get; set; }
        [BsonElement("Timestamp")]
        public DateTime timestamp { get; set; }
        [BsonElement("Value")]
        public byte value { get; set; }
    }
}
