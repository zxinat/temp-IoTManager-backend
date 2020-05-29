using StaffManager.Models;
using StaffManager.Models.StatisticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Dao.Infrastructures
{
    public interface IStaffDataDao
    {
        List<StaffDataModel> ListByDeviceIdAndIndexId(string deviceId, string indexId, DateTime startTime, DateTime endTime, int count = 0);
        List<StaffDataModel> ListByDeviceId(string deviceId, DateTime startTime, DateTime endTime, int count = 0);
        List<StaffDataModel> ListByIndexId(string indexId, DateTime startTime, DateTime endTime, int count = 0);
        List<StaffDataModel> ListByDeviceIdAndValue(string deviceId, int indexValue, int count);
        List<ExitInfoModel> ListExitInfo(string deviceId);
        StaffDataModel GetByIndexIdLatestOne(string indexId);
        StaffDataModel GetLastDataByDeviceId(string deviceId);
        List<string> GetTagIds(DateTime startTime, DateTime endTime);
        List<string> GetTagIds(string deviceId, DateTime startTime, DateTime endTime);
        int GetCount(string deviceId, string tagId, int value, DateTime startTime, DateTime endTime);
        List<RecordModel> GetCount(DateTime startTime, DateTime endTime);
        List<RecordModel> GetByIndexId(string indexId, DateTime startTime, DateTime endTime);
        List<RecordModel> GetCount(string deviceId, DateTime startTime, DateTime endTime);
        void Create(StaffDataModel staffData);
    }
}
