using System;
using System.Collections.Generic;
using IoTManager.Core.Infrastructures;
using IoTManager.IDao;
using IoTManager.Model;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core
{
    public sealed class ThemeBus : IThemeBus
    {
        private readonly IThemeDao _themeDao;

        public ThemeBus(IThemeDao themeDao)
        {
            this._themeDao = themeDao;
        }
        public List<ThemeSerializer> GetAllThemes()
        {
            List<ThemeModel> themes = this._themeDao.Get();
            List<ThemeSerializer> result = new List<ThemeSerializer>();
            foreach (var theme in themes)
            {
                result.Add(new ThemeSerializer(theme));
            }

            return result;
        }

        public ThemeSerializer GetThemeById(int id)
        {    
            return new ThemeSerializer(this._themeDao.GetById(id));
        }

        public String CreateNewTheme(ThemeSerializer themeSerializer)
        {
            ThemeModel themeModel = new ThemeModel();
            themeModel.Name = themeSerializer.name;
            themeModel.First = themeSerializer.first;
            themeModel.Second = themeSerializer.second;
            themeModel.Third = themeSerializer.third;
            return this._themeDao.Create(themeModel);
        }

        public String UpdateTheme(int id, ThemeSerializer themeSerializer)
        {
            ThemeModel themeModel = new ThemeModel();
            themeModel.Id = id;
            themeModel.Name = themeSerializer.name;
            themeModel.First = themeSerializer.first;
            themeModel.Second = themeSerializer.second;
            themeModel.Third = themeSerializer.third;
            return this._themeDao.Update(id, themeModel);
        }

        public String DeleteTheme(int id)
        {
            return this._themeDao.Delete(id);
        }

        public ThemeSerializer GetThemeByUserId(int userId)
        {
            return new ThemeSerializer(this._themeDao.GetByUserId(userId));
        }
    }
}