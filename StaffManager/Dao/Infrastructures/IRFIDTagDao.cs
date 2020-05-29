using StaffManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Dao.Infrastructures
{
    public interface IRFIDTagDao
    {
        List<RFIDTagModel> ListAll(string searchType, int offset = 0, int limit = 12, string sortColumn = "id", string order = "asc");
        string Add(RFIDTagModel rFIDTag);
        RFIDTagModel GetById(int id);
        string Update(RFIDTagModel rFIDTag);
        string Delete(int id);
        RFIDTagModel GetByStaffIdAndTagId(string staffId, string tagId);
        RFIDTagModel GetByTagId(string tagId);
        RFIDTagModel GetByStaffId(string staffId);
        List<RFIDTagModel> ListByStaffId(string staffId);
        List<RFIDTagModel> ListByStaffId(string staffId, DateTime startTime, DateTime endTime);
        List<RFIDTagModel> ListByTagId(string tagId);
        bool IsAuth(string deviceId, string tagId);
    }
}
