using Dapper;
using IoTManager.Utility;
using MySql.Data.MySqlClient;
using StaffManager.Dao.Infrastructures;
using StaffManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace StaffManager.Dao
{
    public  class RFIDTagDao:IRFIDTagDao
    {
        private readonly DatabaseConStr _databaseConStr;
        private readonly string connectionStr;
        public RFIDTagDao(IOptions<DatabaseConStr> databaseConStr)
        {
            _databaseConStr = databaseConStr.Value;
            connectionStr = _databaseConStr.MySQL;
        }
        /*
         *  获取所有的绑定关系
         */
        public List<RFIDTagModel> ListAll(string searchType, int offset = 0,int limit=12,string sortColumn="id",string order="asc")
        {
            string sql = "SELECT * FROM rfidtag";
            if(searchType=="search")
            {
                if (order != "no" && sortColumn != "no")
                {
                    string orderBySubsentence = "order by " + sortColumn + " " + order;
                    sql += orderBySubsentence;
                }
                string limitSubsentence = " limit " + offset.ToString() + "," + limit.ToString();
                sql += limitSubsentence;
            }
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<RFIDTagModel>(sql).ToList();
            }
        }
        /*
         *  添加RFID标签Id和staffId绑定关系
         */
        public string Add(RFIDTagModel rFIDTag)
        {
            string sql = "INSERT INTO rfidtag(staffId,tagId,type,status) VALUES " +
                "(@sid,@tid,@ty,@sta)";
            using (var connection = new MySqlConnection(connectionStr))
            {
                int rows = connection.Execute(sql, new
                {
                    sid = rFIDTag.staffId,
                    tid = rFIDTag.tagId,
                    ty = rFIDTag.type,
                    sta = rFIDTag.status
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*
         *  通过Id获取绑定关系
         */
        public RFIDTagModel GetById(int id)
        {
            string sql = "SELECT * FROME rfidtag WHERE @id=id";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<RFIDTagModel>(sql, new { id = id }).FirstOrDefault();
            }
        }
        /*
         * 更新绑定关系
         */
        public string Update(RFIDTagModel rFIDTag)
        {
            string sql = "UPDATE rfidtag SET staffId=@sid,tagId=@tid,type=@ty,status=@sta,lastTime=@la,updateTime=CURRENT_TIMESTAMP " +
                "WHERE id=@id";
            using (var connection = new MySqlConnection(connectionStr))
            {
                int rows = connection.Execute(sql, new
                {
                    sid = rFIDTag.staffId,
                    tid = rFIDTag.tagId,
                    ty = rFIDTag.type,
                    sta = rFIDTag.status,
                    la = rFIDTag.lastTime,
                    id = rFIDTag.id
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*
         *  删除绑定关系
         */
        public string Delete(int id)
        {
            string sql = "DELETE FROM rfidtag WHERE id=@id";
            using (var connection = new MySqlConnection(connectionStr))
            {
                int rows = connection.Execute(sql, new
                {
                    id = id
                });
                return rows == 1 ? "success" : "error";
            }
        }
        /*
         *  获取绑定关系
         */
        //通过staffId和标签Id查找
        public RFIDTagModel GetByStaffIdAndTagId(string staffId,string tagId)
        {
            string sql = "SELECT * FROM rfidtag WHERE staffId=@sid AND tagId=@tid";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<RFIDTagModel>(sql, new
                {
                    sid = staffId,
                    tid = tagId
                }).FirstOrDefault();
            }
        }
        /*通过标签Id获取最新绑定的staffId*/
        public RFIDTagModel GetByTagId(string tagId)
        {
            string sql = "SELECT * FROM rfidtag WHERE tagId=@tid AND status=1";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<RFIDTagModel>(sql, new { tid = tagId }).FirstOrDefault();
            }
        }
        /*通过staffId获取tagId*/
        public RFIDTagModel GetByStaffId(string staffId)
        {
            string sql = "SELECT * FROM rfidtag WHERE staffId=@sid AND status=1";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<RFIDTagModel>(sql, new { sid = staffId }).FirstOrDefault();
            }
        }
        /*获取staffId的所有绑定记录*/
        public List<RFIDTagModel> ListByStaffId(string staffId)
        {
            string sql = "SELECT * FROM rfidtag WHERE staffId=@sid";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<RFIDTagModel>(sql, new { sid = staffId }).ToList();
            }
        }
        /*获取时间段内staffId绑定的tag*/
        public List<RFIDTagModel> ListByStaffId(string staffId,DateTime startTime,DateTime endTime)
        {
            string date = endTime.ToLocalTime().ToString(Constant.getMysqlDateFormString());
            string sql = "SELECT * FROM rfidtag WHERE staffId=@sid AND createTime<=@ct";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<RFIDTagModel>(sql, new { sid = staffId,ct=date }).ToList();
            }
        }
        /*通过标签Id获取所有记录*/
        public List<RFIDTagModel> ListByTagId(string tagId)
        {
            string sql = "SELECT * FROM rfidtag WHERE tagId=@tid";
            using (var connection = new MySqlConnection(connectionStr))
            {
                return connection.Query<RFIDTagModel>(sql, new { tid = tagId }).ToList();
            }
        }
        /*权限验证*/
        public bool IsAuth(string deviceId,string tagId)
        {
            string sql = "SELECT * FROM staffauth WHERE staffauth.device IN (SELECT id FROM device WHERE  device.hardwareDeviceID=@did) " +
                "AND staffauth.staffId IN (SELECT staffId FROM rfidtag WHERE tagId=@tid AND `status`=1)";
            using (var connection = new MySqlConnection(connectionStr))
            {
                RFIDTagModel rFIDTag = connection.Query<RFIDTagModel>(sql, new { did = deviceId, tid = tagId }).FirstOrDefault();
                return rFIDTag == null ? false : true;
            }
        }
    }
}
