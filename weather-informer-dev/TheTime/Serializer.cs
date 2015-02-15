using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace TheTime
{
    class Serializer
    {
        XmlSerializer serializer = new XmlSerializer(typeof (Cities));

        public void SaveSerialize(List<Cities> cities)
        {
            FileStream f = new FileStream(Directory.GetCurrentDirectory()+"\\cities.xml", FileMode.OpenOrCreate);
            using (StreamWriter sw = new StreamWriter (f))
            {
                serializer.Serialize(sw, cities);
            }
        }

        public List<Cities> ReadSerialize()
        {
            List<Cities> cities = new List<Cities>();
            FileStream f = new FileStream(Directory.GetCurrentDirectory()+"\\cities.xml", FileMode.Open);
            using (StreamReader sr = new StreamReader (f))
            {                
                cities = serializer.Deserialize(sr) as List<Cities>;
            }
            return cities;
        }

        //Directory.GetCurrentDirectory()
    }
}
