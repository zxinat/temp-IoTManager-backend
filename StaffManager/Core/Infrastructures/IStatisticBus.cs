using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTManager.Model;
using StaffManager.Models.StatisticModels;

namespace StaffManager.Core.Infrastructures
{
    public interface IStatisticBus
    {
        object GetYesterdayStaffOnShop(string deviceId);
        object GetStatisticData(string deviceId);
        object GetAttendenceRecords(DataStatisticRequestModel date);
        object GetAttendenceRecords(string deviceId, DataStatisticRequestModel date);
        //object ListLatest10(string deviceId);
        object ListLatestEnter10(string deviceId);
        object ListLatestExit10(string deviceId);
        object GetExitInfo(string deviceId);
        object GetLatestOne(string deviceId);
        PersonalRecordModel GetCurrentData(string staffId);
        PersonalRecordsModel GetPersonalRecord(string staffId, DataStatisticRequestModel date);

        object GetPersonalAttendenceData(string staffId, DataStatisticRequestModel date, string statisticType = "week");
        object GetDepartmentAttendenceData(DataStatisticRequestModel date, string statisticType = "week");
        int GetCurrentStatusByStaffId(string deviceId, string staffId);
        object GetCurrentStaffOnShop(string deviceId);
    }
}
