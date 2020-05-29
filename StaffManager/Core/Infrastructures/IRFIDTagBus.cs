using StaffManager.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Core.Infrastructures
{
    public interface IRFIDTagBus
    {
        object ListAll(string search, int page, string sortColumn, string order);
        string Add(RFIDTagFormModel rFIDTagForm);
        string Update(int id, RFIDTagFormModel rFIDTagForm);
        string Delete(int id);
        bool IsAuth(string deviceId, string tagId);
    }
}
