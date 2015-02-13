using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheTime.DataAccessLevel
{
    public class RegionsDataContext
    {
        public int regionID { get; set; }
        public string name { get; set; }
    }

    public class CitiesDataContext
    {

        public string name { get; set; }
        public int regionID { get; set; }
        public int yandexID { get; set; }
        public int owmID { get; set; }
    }

    public class RegionCitiesLists
    {
        public List<RegionsDataContext> regionsList = new List<RegionsDataContext>();
        public List<CitiesDataContext> citiesList = new List<CitiesDataContext>();
    }
}
