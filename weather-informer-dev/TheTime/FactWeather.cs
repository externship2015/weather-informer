using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheTime
{
    public class FactWeather
    {       
            public string temp { get; set; }
            public string desc { get; set; }
            public string humidity { get; set; }
            public string pressure { get; set; }
            public string wind_dir { get; set; } // s,n,w,e,sw,se,nw,ne
            public string wind_speed { get; set; }

            public string pic { get; set; }
       
    }
}
