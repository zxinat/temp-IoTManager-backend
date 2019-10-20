using System;
using System.Collections.Generic;
using IoTManager.Utility.Serializers;

namespace IoTManager.Core.Infrastructures
{
    public interface IThemeBus
    {
        List<ThemeSerializer> GetAllThemes();
        ThemeSerializer GetThemeById(int id);
        String CreateNewTheme(ThemeSerializer themeSerializer);
        String UpdateTheme(int id, ThemeSerializer themeSerializer);
        String DeleteTheme(int id);
        ThemeSerializer GetThemeByUserId(int userId);
        String UpdateAllUserTheme(int themeId);
    }
}