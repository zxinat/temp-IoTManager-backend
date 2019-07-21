using System;

namespace IoTManager.Utility
{
    public class Constant
    {
        public static String getDatabaseConnectionString()
        {
            //return "Data Source=iotmanagerdbserver.database.chinacloudapi.cn;" +
            //       "User ID=azureuser;" +
            //      "Initial Catalog=iotmanagerdb;" +
            //      "Pwd=123qwe!@#QWE;";
            return "Data Source=localhost;Database=iotmanager;User ID=jackjack59;Password=jackjack123;";          
        }

        public static String getMongoDBConnectionString()
        {
            //return
                //"mongodb://iotmangermongodb:1BiabpxKh7xotBoShVPkp8IvKaNvb2DoIqfZ9SfZ8tg8Cpwj2y5DKgdK8gVDZZelCfPonWfVJge66y3UjWS6KA==@iotmangermongodb.documents.azure.cn:10255/?ssl=true&replicaSet=globaldb";
                return
                    "mongodb://localhost:27017";
        }

        public static String getDateFormatString()
        {
            return "yyyy年MM月dd日 hh:mm:ss";
        }

        public static String getLineChartDateFormatString()
        {
            return "yy-MM-dd\nhh:mm:ss";
        }
    }
}