using System;
using IoTManager.Model;
using Microsoft.Azure.Documents.SystemFunctions;

namespace IoTManager.Utility.Serializers
{
    public class ThemeSerializer
    {
        public ThemeSerializer()
        {
            this.id = 0;
            this.name = null;
            this.first = null;
            this.second = null;
            this.third = null;
        }

        public ThemeSerializer(ThemeModel themeModel)
        {
            this.id = themeModel.Id;
            this.name = themeModel.Name;
            this.first = themeModel.First;
            this.second = themeModel.Second;
            this.third = themeModel.Third;
        }
        public int id { get; set; }
        public String name { get; set; }
        public String first { get; set; }
        public String second { get; set; }
        public String third { get; set; }
        
    }
}