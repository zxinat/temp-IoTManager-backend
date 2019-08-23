using System;
using IoTManager.Model;
using Microsoft.Azure.Documents.SystemFunctions;

namespace IoTManager.Utility.Serializers
{
    public class AuthSerializer
    {
        public AuthSerializer()
        {
            this.id = 0;
            this.authId = null;
            this.description = null;
            this.timestamp = null;
        }

        public AuthSerializer(AuthModel authModel)
        {
            this.id = authModel.Id;
            this.authId = authModel.AuthId;
            this.description = authModel.Description;
            this.timestamp = authModel.Timestamp
                .ToString(Constant.getDateFormatString());
            
        }
        public int id { get; set; }
        public String authId { get; set; }
        public String description { get; set; }
        public String timestamp { get; set; }
    }
}