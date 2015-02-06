using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TheTime
{
    /// <summary>
    /// Main worker class with yandex.weather.api
    /// </summary>
    class WeatherWorker
    {      
        /// <summary>
        /// Парсит xml с городами, заносит все в список городов
        /// </summary>
        /// <returns>Возвращает список городов</returns>
        public List<Cities> GetListOfCities()
        {
            List<Cities> ListOfCities = new List<Cities>();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("https://pogoda.yandex.ru/static/cities.xml");

            foreach (XmlNode node in xDoc.DocumentElement)
            {
                foreach (XmlNode node2 in node.ChildNodes)
                {           
                    ListOfCities.Add(new Cities { citName = node2.InnerText,
                                                  country = ParseXMLString(node2.OuterXml, "country=\""),
                                                  id = ParseXMLString(node2.OuterXml, "id=\""),
                                                  part = ParseXMLString(node2.OuterXml, "part=\""),
                                                  region = ParseXMLString(node2.OuterXml, "region=\"")
                    });
                }
            }
            return ListOfCities;        
        }

        public List<FactWeather> GetFactWeather(string id)
        {
            List<FactWeather> ListFacts = new List<FactWeather>();
            XmlDocument xDoc = new XmlDocument();

            xDoc.Load("http://export.yandex.ru/weather-ng/forecasts/"+id+".xml");

            foreach (XmlNode node in xDoc.DocumentElement)
            {
                string pthPic = ParseXMLfact(node.OuterXml, "image-v3 type=\"mono\"").Replace("-", "0");
                pthPic = pthPic.Replace("+", "1");

                ListFacts.Add( new FactWeather
                {
                    temp = ParseXMLfact(node.OuterXml, "temperature"),
                    humidity = ParseXMLfact(node.OuterXml, "humidity"),

                    pressure = ParseXMLfact(node.OuterXml, "pressure"),
                    desc = ParseXMLfact(node.OuterXml, "weather_type"),
                    wind_dir = ParseXMLfact(node.OuterXml, "wind_direction"),
                    wind_speed = ParseXMLfact(node.OuterXml, "wind_speed"),

                    pic = pthPic

                });
                if(ListFacts.Count>0)
                return ListFacts;
                
                    //ListOfCities.Add(new Cities
                    //{
                    //    citName = node2.InnerText,
                    //    country = ParseXMLString(node2.OuterXml, "country=\""),
                    //    id = ParseXMLString(node2.OuterXml, "id=\""),
                    //    part = ParseXMLString(node2.OuterXml, "part=\""),
                    //    region = ParseXMLString(node2.OuterXml, "region=\"")
                    //});
                
            }
            return ListFacts;
        }

        /// <summary>
        /// Вспомогательный метод - парсит одну строку xml файла с городами
        /// </summary>
        /// <param name="bigStr"></param>
        /// <param name="litStr"></param>
        /// <returns></returns>
        private string ParseXMLString(string bigStr, string litStr)
        {
            string ret = "";
            int i = bigStr.IndexOf(litStr) + litStr.Length;
            while (bigStr[i] != '\"')
            {
                ret += bigStr[i];
                i++;
            }
            return ret;
        }

        private string ParseXMLfact(string bigStr, string litStr)
        {
            string ret = "";
            int i = bigStr.IndexOf(litStr) + litStr.Length;
            while (bigStr[i] != '<')
            {
                if (bigStr[i] == '>')
                {
                    while (bigStr[i+1] != '<')
                    {
                        i++;
                        ret += bigStr[i];                        
                    }
                    return ret;
                }
                else
                {
                    i++;
                }
               
            }          
            
            return ret;
        }



        public string GetCityIdString(string com1Text, string com2Text, string com3Text, List<Cities> list2)
        {
           
            if (com3Text == "")
            {
                foreach (var item in list2.Where(s=>s.country==com1Text && s.citName == com2Text))
                { 
                    return item.id;
                }
            }
            else
            {
                foreach (var item in list2.Where(s => s.country == com1Text && s.part == com2Text && s.citName == com3Text))
                { 
                    return item.id;
                }
            }
            return "";
        }



        //public List<string> GetListOfCountries(List<Cities> list)
        //{ 

            
        //}
    }
}
