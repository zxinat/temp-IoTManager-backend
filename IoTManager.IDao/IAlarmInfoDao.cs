using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IAlarmInfoDao
    {
        List<AlarmInfoModel> Get(String searchType, String deviceId = "all", int offset = 0, int limit = 12, String sortColumn = "Id", String order = "asc");
        AlarmInfoModel GetById(String Id);
        List<AlarmInfoModel> GetByDeviceId(String deviceId,int count);
        List<AlarmInfoModel> GetByDeviceId20(String DeviceId);
        List<AlarmInfoModel> GetByIndexId(String IndexId);
        String Create(AlarmInfoModel alarmInfoModel);
        List<AlarmInfoModel> GetFiveInfo();
        int GetNoticeAlarmInfoAmount();
        int GetSeriousAlarmInfoAmount();
        int GetVerySeriousAlarmInfoAmount();
        String UpdateProcessed(String id);
        long GetAlarmInfoNumber(String searchType, String deviceId = "all");
        String Delete(String id);
        int GetDeviceAffiliateAlarmInfoNumber(String deviceId, DateTime startTime, DateTime endTime);
        int GetDeviceAlarmInfoNumberByTime(DateTime startTime, DateTime endTime);
        int BatchDelete(List<String> ids);
    }
}