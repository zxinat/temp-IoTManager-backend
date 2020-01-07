/*
 * AlarmInfoBus.cs
 * 告警信息管理的业务逻辑层
 */

using System;
using System.Collections.Generic;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;
using Microsoft.Extensions.Logging;

namespace IoTManager.Core
{
    public sealed class AlarmInfoBus: IAlarmInfoBus
    {
        private readonly IDeviceDataDao _deviceDataDao;
        private readonly IAlarmInfoDao _alarmInfoDao;
        private readonly ILogger _logger;

        /*
         * 构造函数
         * 需要注入DeviceDataDao, AlarmInfoDao, Logger
         */
        public AlarmInfoBus(IDeviceDataDao deviceDataDao, IAlarmInfoDao alarmInfoDao, ILogger<AlarmInfoBus> logger)
        {
            this._deviceDataDao = deviceDataDao;
            this._alarmInfoDao = alarmInfoDao;
            this._logger = logger;
        }

        /*
         * 获取所有告警信息，具有分页功能
         * 
         * 输入：
         * 搜索类型
         * 设备编号
         * 页码
         * 排序列
         * 升序或降序
         *
         * 输出：
         * 告警信息的列表
         */
        public List<AlarmInfoSerializer> GetAllAlarmInfo(String searchType, String deviceId = "all", int page = 1, String sortColumn = "Id", String order = "asc")
        {
            int offset = (page - 1) * 12;
            int limit = 12;
            List<AlarmInfoModel> alarmInfos = this._alarmInfoDao.Get(searchType, deviceId, offset, limit, sortColumn, order);
            List<AlarmInfoSerializer> result = new List<AlarmInfoSerializer>();
            foreach (AlarmInfoModel alarmInfo in alarmInfos)
            {
                result.Add(new AlarmInfoSerializer(alarmInfo));
            }
            return result;
        }

        /*
         * 获取某个ID（数据库ID）的告警信息
         * 
         * 输入：
         * ID
         *
         * 输出：
         * 编号为该ID的告警信息
         */
        public AlarmInfoSerializer GetAlarmInfoById(String Id)
        {
            AlarmInfoModel alarmInfo = this._alarmInfoDao.GetById(Id);
            AlarmInfoSerializer result = new AlarmInfoSerializer(alarmInfo);
            return result;
        }

        /*
         * 根据设备编号获取某个设备下的所有告警信息
         *
         * 输入：
         * 设备编号
         *
         * 输出：
         * 该设备下的所有告警信息
         */
        public List<AlarmInfoSerializer> GetAlarmInfoByDeviceId(String DeviceId)
        {
            List<AlarmInfoModel> alarmInfos = this._alarmInfoDao.GetByDeviceId(DeviceId);
            List<AlarmInfoSerializer> result = new List<AlarmInfoSerializer>();
            foreach (AlarmInfoModel alarmInfo in alarmInfos)
            {
                result.Add(new AlarmInfoSerializer(alarmInfo));
            }
            return result;
        }

        /*
         * 根据属性编号获取某个属性下的所有告警信息
         *
         * 输入：
         * 属性编号
         *
         * 输出：
         * 该属性下的所有告警信息
         */
        public List<AlarmInfoSerializer> GetAlarmInfoByIndexId(String IndexId)
        {
            List<AlarmInfoModel> alarmInfos = this._alarmInfoDao.GetByIndexId(IndexId);
            List<AlarmInfoSerializer> result = new List<AlarmInfoSerializer>();
            foreach (AlarmInfoModel alarmInfo in alarmInfos)
            {
                result.Add(new AlarmInfoSerializer(alarmInfo));
            }
            return result;
        }

        /*
         * 该函数已废除
         */
        public String InspectAlarmInfo()
        {
            List<DeviceDataModel> deviceData = _deviceDataDao.GetNotInspected();
            foreach (DeviceDataModel dd in deviceData)
            {
                if (Convert.ToInt32(dd.IndexValue) > 100)
                {
                    AlarmInfoModel alarmInfo = new AlarmInfoModel();
                    alarmInfo.AlarmInfo = dd.Id;
                    alarmInfo.DeviceId = dd.DeviceId;
                    alarmInfo.IndexId = dd.IndexId;
                    alarmInfo.IndexName = dd.IndexName;
                    alarmInfo.IndexValue = dd.IndexValue;
                    alarmInfo.ThresholdValue = 100;
                    alarmInfo.Timestamp = DateTime.Now;

                    _alarmInfoDao.Create(alarmInfo);
                }
            }
            return "success";
        }

        /*
         * 获取最新的五条告警信息
         */
        public List<AlarmInfoSerializer> GetFiveInfo()
        {
            List<AlarmInfoModel> alarmInfos = this._alarmInfoDao.GetFiveInfo();
            List<AlarmInfoSerializer> result = new List<AlarmInfoSerializer>();
            foreach (AlarmInfoModel alarmInfo in alarmInfos)
            {
                result.Add(new AlarmInfoSerializer(alarmInfo));
            }

            return result;
        }

        /*
         * 获取告警等级为Info的告警信息数量
         */
        public int GetNoticeAlarmInfoAmount()
        {
            return this._alarmInfoDao.GetNoticeAlarmInfoAmount();
        }

        /*
         * 获取告警登记为Warning的告警信息数量
         */
        public int GetSeriousAlarmInfoAmount()
        {
            return this._alarmInfoDao.GetSeriousAlarmInfoAmount();
        }
        
        /*
         * 获取告警登记为Critical的告警信息数量
         */
        public int GetVerySeriousAlarmInfoAmount()
        {
            return this._alarmInfoDao.GetVerySeriousAlarmInfoAmount();
        }

        /*
         * 将特定ID的告警信息设置为已处理
         *
         * 输入：
         * 告警信息ID
         *
         * 输出：
         * 无
         */
        public String UpdateProcessed(String id)
        {
            return this._alarmInfoDao.UpdateProcessed(id);
        }

        /*
         * 获取特定分页条件下的告警信息数量
         *
         * 输入：
         * 搜索类型
         * 设备编号
         *
         * 输出：
         * 对应条件下的告警信息数量
         */
        public long GetAlarmInfoNumber(String searchType, String deviceId = "all")
        {
            return this._alarmInfoDao.GetAlarmInfoNumber(searchType, deviceId);
        }

        /*
         * 删除特定ID的告警信息
         *
         * 输入：
         * 告警信息ID
         *
         * 输出：
         * 无
         */
        public String DeleteAlarmInfo(String id)
        {
            return this._alarmInfoDao.Delete(id);
        }

        /*
         * 根据设备编号获取该设备下告警信息数量
         *
         * 输入：
         * 设备编号
         *
         * 输出：
         * 该设备下告警信息的数量
         */
        public int GetDeviceAffiliateAlarmInfo(String deviceId)
        {
            return this._alarmInfoDao.GetDeviceAffiliateAlarmInfo(deviceId);
        }

        /*
         * 批量删除告警信息
         *
         * 输入：
         * 告警信息ID列表
         *
         * 输出：
         * 无
         */
        public int BatchDelete(List<String> ids)
        {
            return this._alarmInfoDao.BatchDelete(ids);
        }
    }
}