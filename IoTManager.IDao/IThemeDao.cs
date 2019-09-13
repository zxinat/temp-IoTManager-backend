using System;
using System.Collections.Generic;
using IoTManager.Model;

namespace IoTManager.IDao
{
    public interface IThemeDao
    {
        List<ThemeModel> Get();
        ThemeModel GetById(int id);
        String Create(ThemeModel themeModel);
        String Update(int id, ThemeModel themeModel);
        String Delete(int id);
        ThemeModel GetByUserId(int userId);
    }
}