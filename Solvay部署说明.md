## Solvay部署说明

#### 数据库更新：

1. 【device】表，base64Image字段类型改成mediumtext，字符集utf8，排序规则utf8_general_ci，

   ```powershell
   mysql>alter table device modify column base64Image mediumtext;
   ```

   

2. 【gateway】表，