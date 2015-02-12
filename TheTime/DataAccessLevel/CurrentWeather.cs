using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheTime.DataAccessLevel
{
    class CurrentWeather
    {
        public string symbol { get; set; }
        public string description { get; set; }
        public int temperature { get; set; }
        public int windSpeed { get; set; }
        public string windDirection { get; set; } // в формате С, СЗ, Ю, ....
        public int pressure { get; set; }
        public int hummidity { get; set; }
    }
}
