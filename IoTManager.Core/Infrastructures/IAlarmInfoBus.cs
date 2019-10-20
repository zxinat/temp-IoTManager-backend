using System;
using System.Collections.Generic;
using IoTManager.Model;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IAlarmInfoBus
    {
        List<AlarmInfoSerializer> GetAllAlarmInfo(String searchType, String deviceId = "all", int page = 1, String sortColumn = "Id", String order = "asc");
        AlarmInfoSerializer GetAlarmInfoById(String Id);
        List<AlarmInfoSerializer> GetAlarmInfoByDeviceId(String DeviceId);
        List<AlarmInfoSerializer> GetAlarmInfoByIndexId(String IndexId);
        String InspectAlarmInfo();
        List<AlarmInfoSerializer> GetFiveInfo();
        int GetNoticeAlarmInfoAmount();
        int GetSeriousAlarmInfoAmount();
        int GetVerySeriousAlarmInfoAmount();
        String UpdateProcessed(String id);
        long GetAlarmInfoNumber(String searchType, String deviceId = "all");
        String DeleteAlarmInfo(String id);
        int GetDeviceAffiliateAlarmInfo(String deviceId);
        int BatchDelete(List<String> ids);
    }
}