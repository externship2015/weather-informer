using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheTime
{
    class DayFarecastData
    {
        public string city { get; set; }
        public string region { get; set; }
        public string sourse { get; set; } // источник данных - яндекс или owm
        public DateTime date { get; set; } // дата 
        public string time_of_day { get; set; } // всремя суток в формате утро/день/вечер/ночь
        public string description { get; set; } // описание: мол
        public string temperature { get; set; }
        public string pressure { get; set; } // давление в мм рт ст
        public string wind_direction { get; set; }
        public string wind_speed { get; set; }
        public string hummidity { get; set; } // влажность воздуха, в %
        public string symbol { get; set; } // название картинки

    }
}
