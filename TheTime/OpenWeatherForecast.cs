using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheTime
{
    class OpenWeatherForecast
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string symbol { get; set; } // id картинки с сервера (var) <symbol number="804" name="пасмурно" var="04n"/>

        public string precipitation {get; set;} // осадки
        public string windDirection { get; set;} // направление ветра (code) <windDirection deg="14.5021" code="NNE" name="North-northeast"/>
        public string windSpeed {get;set;} // скорость ветра, mps <windSpeed mps="4.8" name="Gentle Breeze"/>
        public string temperature {get;set;} // <temperature unit="celsius" value="4.8" min="4.8" max="4.82"/>
        public string pressure {get;set;} //<pressure unit="hPa" value="1039.86"/>
        public string humidity {get;set;} // <humidity value="87" unit="%"/>
        public string clouds {get;set;} // <clouds value="overcast clouds" all="88" unit="%"/>
    }
}
