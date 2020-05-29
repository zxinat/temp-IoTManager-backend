using MongoDB.Driver;
using StaffManager.Models;
using StaffManager.Models.StatisticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Dao.Infrastructures
{
    public interface IAttendenceDataDao
    {
        void Add(AttendenceDataModel attendenceData);
        DeleteResult RemoveByStaffId(string staffId);
        Task BatchAddAsync(List<AttendenceDataModel> attendenceDatas);
        Task<BulkWriteResult<AttendenceDataModel>> BatchUpdateAsync(List<AttendenceDataModel> attendenceDatas);
        bool IsContain(AttendenceDataModel ad);
        List<PersonalAttendenceModel> GetPersonalStatisticData(string staffId, DateTime startTime, DateTime endTime, string statisticType = "week");
        List<DepartmentAttendenceModel> GetDepartmentAttendenceData(DateTime startTime, DateTime endTime, string statisticType = "week");
    }
}
