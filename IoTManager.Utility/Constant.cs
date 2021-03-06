using System;
using System.Runtime.InteropServices;

namespace IoTManager.Utility
{
    public class Constant
    {
        public static String getDatabaseConnectionString()
        {
            // return "Data Source=iotmanagerdbserver.database.chinacloudapi.cn;" +
            //   "User ID=azureuser;" +
            //  "Initial Catalog=iotmanagerdb;" +
            //  "Pwd=123qwe!@#QWE;";
            //return "Data Source=localhost;Database=iotmanager;User ID=root;Password=root;";
            //return "Data Source=iotmanager.mysql.database.chinacloudapi.cn;Database=iotmanager;User ID=SHUIoTDev@iotmanager;Password=Password01!!;";
            return "Data Source=118.31.2.239;Database=iotmanager;User ID=root;Password=root;";
        }

        public static String getMongoDBConnectionString()
        {
            //return
             //   "mongodb://shudev2:Etp13A3NROECpQeJ1GjTbkj7OqHfoukak17BwiMgcjw6g2ap5PPZsfraINEVJ1G34UtR2MHUJCTufvhAz2uwLQ==@shudev2.documents.azure.cn:10255/?ssl=true&replicaSet=globaldb";
            //return
            //"mongodb://localhost:27017";
            return "mongodb://iotmanager:iotmanager-pwd@118.31.2.239:27017/?authSource=iotmanager";
        }

        public static String getDateFormatString()
        {
            return "yyyy-MM-dd\nHH:mm:ss";
        }

        public static String getLineChartDateFormatString()
        {
            return "yy-MM-dd\nhh:mm:ss";
        }

        public static String getStatisticDateFormatString()
        {
            return "yyyy-MM-dd\nhh:mm:ss";
        }
    }
}