using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.OleDb;

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
                    Cities City = new Cities
                    {
                        citName = node2.InnerText,
                        country = ParseXMLString(node2.OuterXml, "country=\""),
                        id = ParseXMLString(node2.OuterXml, "id=\""),
                        part = ParseXMLString(node2.OuterXml, "part=\""),
                        region = ParseXMLString(node2.OuterXml, "region=\"")
                    };
                    if (City.country == "Россия")
                        ListOfCities.Add(City);
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




        //Проверка делать insert или update для яндекса для дневной
        public bool isinsert(string id, string f_date)
        {
            TheTime.ПапкаАндрея.SQLConnection sql = new TheTime.ПапкаАндрея.SQLConnection();
            try
            {
               
         
                OleDbDataReader reader = sql.Query("SELECT * FROM daily_forecast WHERE city='" + id + "' AND f_date = '" + f_date + "'");           
                reader.Read();
                int fieldCount = reader.FieldCount;
               
                if(reader.HasRows)               
                    return false;
                else
                {
                    return true;
                }               
                
            }
            catch
            {
                return true  ;
            }
        }
        //Проверка делать insert или update для яндекса для часовой
        public bool isinserthour(string id, string f_date, string time_of_day)
        {
            TheTime.ПапкаАндрея.SQLConnection sql = new TheTime.ПапкаАндрея.SQLConnection();
            try
            {

                OleDbDataReader reader = sql.Query("SELECT * FROM hourly_forecast WHERE city='" + id + "' AND f_date = '" + f_date + "' AND time_of_day = '" + time_of_day + "'");
                // OleDbDataReader reader = Query("SELECT * FROM open1b");
                reader.Read();
                int fieldCount = reader.FieldCount;
                string count = "";
                while (reader.Read())
                {
                    count = reader[0].ToString();
                }
                if (count == "")
                    return true;
                else
                {
                    return false;
                }

            }
            catch
            {
                return true;
            }
        }


        //Залить все погоду с яндекса в базу
        //Дни
        public void GetFactWeatherDays(string id)
        {
            string city = id;
            string source = "ya";
            string f_date = "";
            string time_of_day = "";
            string region = id;
            string temperature = "";
            string hummidity = "";
            string pressure = "";
            string wind_direction = "";
            string wind_speed = "";
            string symbol = "";
            string description = "";


            TheTime.ПапкаАндрея.SQLConnection sql = new TheTime.ПапкаАндрея.SQLConnection();
            XmlDocument xDoc = new XmlDocument();

            xDoc.Load("http://export.yandex.ru/weather-ng/forecasts/" + id + ".xml");

            foreach (XmlNode node in xDoc.DocumentElement)
            {
                if (node.Name == "day")
                {
                    f_date = ParseXMLString(node.OuterXml, "day date=\"");
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        if (node2.Name == "day_part")
                        {
                            time_of_day = ParseXMLString(node2.OuterXml, "typeid=\"");
                            foreach (XmlNode node3 in node2.ChildNodes)
                            {
                                if (node3.Name == "temperature_from")
                                    temperature += node3.InnerText + "  ";
                                if (node3.Name == "temperature_to")
                                    temperature += node3.InnerText;
                                if (node3.Name == "image-v3")
                                    symbol = node3.InnerText;
                                if (node3.Name == "weather_type")
                                    description = node3.InnerText;
                                if (node3.Name == "wind_direction")
                                    wind_direction = node3.InnerText;
                                if (node3.Name == "wind_speed")
                                    wind_speed = node3.InnerText;
                                if (node3.Name == "humidity")
                                    hummidity = node3.InnerText;
                                if (node3.Name == "pressure")
                                    pressure = node3.InnerText;
                                if (node3.Name == "image-v3")
                                    symbol = node3.InnerText;
                            }
                            if (isinsert(id, f_date))
                            {
                                sql.Query("INSERT INTO daily_forecast (source, region, city, f_date, time_of_day, description, temperature,pressure, wind_direction,wind_speed, hummidity, symbol) Values ('" + source + "','" + region + "', '" + city + "', '" + f_date + "', '" + time_of_day + "', '" + description + "','" + temperature + "', '" + pressure + "', '" + wind_direction + "', '" + wind_speed + "', '" + hummidity + "', '" + symbol + "')");
                            }
                            else
                            {
                                sql.Query("UPDATE daily_forecast SET description='" + description + "', temperature='" + temperature + "',pressure='" + pressure + "', wind_direction='" + wind_direction + "',wind_speed='" + wind_speed + "', hummidity='" + hummidity + "', symbol='" + symbol + "' WHERE source='" + source + "' AND region='" + region + "' AND city='" + city + "' AND f_date='" + f_date + "' AND time_of_day='" + time_of_day + "'");
                            }
                            temperature = "";
                        }
                        if (node2.Name == "hour")
                        {
                            time_of_day = ParseXMLString(node2.OuterXml, "at=\"");
                            foreach (XmlNode node3 in node2.ChildNodes)
                            {
                                if (node3.Name == "temperature")
                                    temperature = node3.InnerText;
                                if (node3.Name == "image-v3")
                                    symbol = node3.InnerText;
                            }
                            if (isinserthour(id, f_date, time_of_day))
                            {
                                sql.Query("INSERT INTO hourly_forecast (source, region, city, f_date, time_of_day, temperature, symbol) Values ('" + source + "','" + region + "', '" + city + "', '" + f_date + "', '" + time_of_day + "', '" + temperature + "',  '" + symbol + "')");
                            }
                            else
                            {
                                sql.Query("UPDATE hourly_forecast SET description='" + description + "', temperature='" + temperature + "', symbol='" + symbol + "' WHERE source='" + source + "' AND region='" + region + "' AND city='" + city + "' AND f_date='" + f_date + "' AND time_of_day='" + time_of_day + "'");
 
                            }
                            temperature = "";


                        }
                    }
                }
            }
            string s = "";     
        }
    }
}
