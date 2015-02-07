using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using Newtonsoft.Json;
using System.Web;
using System.Xml.Linq;

namespace TheTime
{
    class OpenweathermapAPIWorker
    {
        System.Xml.XmlNodeList forecastsList;
        public void GetWeather(string city, string country)
        {

            //using (WebClient webClient = new System.Net.WebClient())
            //{
            //    WebClient n = new WebClient();
            //    var json = n.DownloadString("http://api.openweathermap.org/data/2.5/forecast?&q=" + city + "," + country + "&mode=json&units=metric&cnt=10&lang=ru");
            //    string valueOriginal = Convert.ToString(json);
            //    Dictionary<string, string> dict = ParseJson(valueOriginal);
            //}

            // получаем прогноз на неделю с сервера
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("http://api.openweathermap.org/data/2.5/forecast?&q="+city+","+country+"&mode=xml&units=metric&cnt=10&lang=ru");

            var Child1 = xDoc.ChildNodes;
            var Child2 = Child1[1].ChildNodes;
            
            for (int i = 0; i < Child2.Count; i++)
            {
                if (Child2[i].Name == "forecast")
                {
                    forecastsList = Child2[i].ChildNodes;
                    break;
                }
            }


            List<OpenWeatherForecast> list = new List<OpenWeatherForecast>();

            for (int i = 0; i < forecastsList.Count; i++)
            {
                DateTime from;
                DateTime to;
                string symbol = "";
                string precipitation = "";
                string windDirection = "";
                string windSpeed = "";
                string temperature = "";
                string pressure = "";
                string humidity = "";
                string clouds = ""; 
                var attr = forecastsList[i].Attributes;

                from = DateTime.Parse(forecastsList[i].Attributes.GetNamedItem("from").Value);
                to = DateTime.Parse(forecastsList[i].Attributes.GetNamedItem("to").Value);
               

                var weatherPar = forecastsList[i].ChildNodes;
                for (int k = 0; k < weatherPar.Count; k++)
                {                    
                    var attr2 = weatherPar[k].Attributes;
                    switch (weatherPar[k].Name) 
                    { 
                        case "symbol":
                            symbol = weatherPar[k].Attributes.GetNamedItem("var").Value;
                            break;
                        case "precipitation":
                            precipitation = "123";
                            break;
                        case "windDirection":
                            windDirection = weatherPar[k].Attributes.GetNamedItem("code").Value;
                            break;
                        case "windSpeed":
                            windSpeed = weatherPar[k].Attributes.GetNamedItem("mps").Value;    
                            break;
                        case "temperature":
                            temperature = weatherPar[k].Attributes.GetNamedItem("value").Value;
                            break;
                        case "pressure":
                            pressure = weatherPar[k].Attributes.GetNamedItem("value").Value;
                            break;
                        case "humidity":
                            humidity = weatherPar[k].Attributes.GetNamedItem("value").Value;
                            break;
                        case "clouds":
                            clouds = weatherPar[k].Attributes.GetNamedItem("all").Value;
                            break;
                        default: break;

                    }
                    
                }
                list.Add(new OpenWeatherForecast
                {
                    from = from,
                    to = to,
                    symbol = symbol,
                    precipitation = precipitation,
                    windDirection = windDirection,
                    windSpeed = windSpeed,
                    temperature = temperature,
                    pressure = pressure,
                    humidity = humidity,
                    clouds = clouds

                });
               

                int a = 0;
            }
            //var users = (from entry in xDoc.ChildNodes("user")
            //             select new
            //             {
            //                 UserName = entry.Attribute("Name").Value
            //             }).ToList();


            int t = 0;
        }



    }

}
