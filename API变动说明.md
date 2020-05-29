##### API变动说明：

1. GET  /api/Staff/Statistic/GetCurrentStaffOnShop/{deviceId} 变更为 GET   /api/StaffStatistic/GetStatisticData/{deviceId}，返回数据：inside表示当前车间内人数、离开车间的人数、访客人数、今日车间的刷卡总人数
2. GET  /api/Staff/Statistic/GetYesterdayStaffOnShop/{deviceId} 弃用，改用 POST   /api/StaffStatistic/GetAttendenceRecords/{deviceId}，输入：查询时间段，查询车间的设备Id；输出：车间的固定人员数量、打卡总数、缺勤人数
3. POST   /api/StaffStatistic/GetAttendenceRecords，获取所有刷卡数据，统计总的固定人数、打卡人数、和缺勤人数
4. GET    /api/StaffStatistic/{deviceId}/ListLatestEnter10，获取最新入场10人，返回数据：（staffid、name、gender、职工/访客-type、deviceId、deviceName、入场时间）。离场10人的API同上
5. POST    /api/StaffStatistic/{staffId}/GetPersonalAttendenceData  获取个人考勤记录，输入staffId，时间段、按周/月统计；输出：时间段内每周/月缺勤次数、缺勤日期，考勤次数
6. POST    /api/StaffStatistic/GetDepartmentAttendenceData   获取每个部门考勤记录，输入时间段，按周/月统计；输出每个部门这段时间每周/月出勤人数、缺勤人数、出勤率
7. GET    /api/StaffStatistic/GetLatestPersonalData/{staffId}    获取人员最新的刷卡记录，输入staffId；输出：该人员最新的刷卡数据（staffId、name、deviceId、devicename、tagId、进/出、时间）
8. POST    /api/StaffStatistic/GetPersonalRecords/{staffId}   获取人员一段时间的到访记录，输入staffId、时间段；输出：人员这段时间内持有的tagId、访问的deviceId、deviceName、时间、进/出
9. GET    /api/StaffStatistic/GetExitInfo/{deviceId}   获取车间离岗人员，离岗计时