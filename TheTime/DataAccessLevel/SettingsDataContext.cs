using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheTime.DataAccessLevel
{
    /// <summary>
    /// Представляет контекстный класс для таблицы Settings
    /// </summary>
    class SettingsDataContext
    {
        public int cityID { get; set; }
        public int sourceID { get; set; } // 1 - ya, 2 - owm
        public DateTime saveDate { get; set; }
    }
}
