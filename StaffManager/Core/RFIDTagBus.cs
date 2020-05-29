using Microsoft.Extensions.Logging;
using StaffManager.Core.Infrastructures;
using StaffManager.Dao.Infrastructures;
using StaffManager.Models;
using StaffManager.Models.RequestModel;
using StaffManager.Models.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManager.Core
{
    public class RFIDTagBus:IRFIDTagBus
    {
        private readonly IRFIDTagDao _rFIDTagDao;
        private readonly ILogger _logger;
        public RFIDTagBus(IRFIDTagDao rFIDTagDao,ILogger<IRFIDTagBus> logger)
        {
            _rFIDTagDao = rFIDTagDao;
            _logger = logger;
        }
        /*获取所有绑定关系*/
        public object ListAll(string search,int page,string sortColumn,string order)
        {
            int offset = (page - 1) * 12;
            int limit = 12;
            List<RFIDTagModel> rFIDTags = _rFIDTagDao.ListAll(search, offset, limit, sortColumn, order);
            List<RFIDTagSerializer> result = new List<RFIDTagSerializer>();
            foreach(var r in rFIDTags)
            {
                result.Add(new RFIDTagSerializer(r));
            }
            return result;
        }
        /*
         *  添加StaffId和标签Id的绑定关系
         */
        public string Add(RFIDTagFormModel rFIDTagForm)
        {
            var rfidtag = _rFIDTagDao.GetByStaffIdAndTagId(rFIDTagForm.staffId, rFIDTagForm.tagId);
            List<RFIDTagModel> staffIdrFIDTags = _rFIDTagDao.ListByStaffId(rFIDTagForm.staffId);
            List<RFIDTagModel> tagIdrFIDTags = _rFIDTagDao.ListByTagId(rFIDTagForm.tagId);
            RFIDTagModel rFIDTag = new RFIDTagModel(rFIDTagForm);
            if(rfidtag!=null)
            {
                return "exist";
            }
            else if(staffIdrFIDTags.Find(rid=>rid.status==1)!=null)
            {
                return "staffId exists";
            }
            else if(tagIdrFIDTags.Find(tid=>tid.status==1)!=null)
            {
                return "tagId exists";
            }
            else
            {
                return _rFIDTagDao.Add(rFIDTag);
            }
        }
        /*
         *  查看绑定关系
         */
        public object ListAll()
        {
            return null;
        }
        /*
         *  修改绑定信息
         */
        public string Update(int id,RFIDTagFormModel rFIDTagForm)
        {
            RFIDTagModel rFIDTag = _rFIDTagDao.GetById(id);
            if(rFIDTag!=null)
            {
                rFIDTag.staffId = rFIDTagForm.staffId;
                rFIDTag.tagId = rFIDTagForm.tagId;
                rFIDTag.type = rFIDTagForm.type == "员工" ? (Byte)1 : (Byte)0;
                Byte nowStatus = rFIDTag.status;
                rFIDTag.status = rFIDTagForm.status == "已激活" ? (Byte)1 : (Byte)0;
                if (nowStatus == 1 & rFIDTag.status == 0)
                {
                    rFIDTag.lastTime = DateTime.Now;
                }
                else
                {
                    rFIDTag.lastTime = rFIDTag.lastTime <= new DateTime(1970, 1, 1) ? new DateTime(1970, 1, 1) : rFIDTag.lastTime;
                }
                return _rFIDTagDao.Update(rFIDTag);
            }
            else
            {
                return "No Content";
            }
            
        }
        /*删除绑定关系
         */
        public string Delete(int id)
        {
            RFIDTagModel rFIDTag = _rFIDTagDao.GetById(id);
            if(rFIDTag!=null)
            {
                return _rFIDTagDao.Delete(id);
            }
            else
            {
                return "No Content";
            }
        }
        /* 标签权限认证
         */
        public bool IsAuth(string deviceId,string tagId)
        {
            return _rFIDTagDao.IsAuth(deviceId, tagId);
        }
    }
}
