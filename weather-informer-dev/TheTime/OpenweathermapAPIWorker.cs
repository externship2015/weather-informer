using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Web;
using System.Xml.Linq;
using System.Globalization;

namespace TheTime
{
    class OpenweathermapAPIWorker
    {
        System.Xml.XmlNodeList forecastsList;
        public void GetWeather(string city,  string region, string country)
        {
            // получаем прогноз на 10 дней вперед с сервера
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

            List<DayFarecastData> dailyForecast = new List<DayFarecastData>();
            List<HourlyForecastData> hourlyForecast = new List<HourlyForecastData>();

            for (int i = 0; i < forecastsList.Count; i++)
            {                                        
                string symbol = "";
                string description = "";                
                string windDirection = "";
                string windSpeed = "";
                string temperature = "";
                string pressure = "";
                string humidity = "";
                
                var attr = forecastsList[i].Attributes;

                DateTime time1 = DateTime.Parse(forecastsList[i].Attributes.GetNamedItem("from").Value);
                DateTime time2 = DateTime.Parse(forecastsList[i].Attributes.GetNamedItem("from").Value).AddHours(1);
                DateTime time3 = DateTime.Parse(forecastsList[i].Attributes.GetNamedItem("from").Value).AddHours(2);

                var weatherPar = forecastsList[i].ChildNodes;
                for (int k = 0; k < weatherPar.Count; k++)
                {                    
                    var attr2 = weatherPar[k].Attributes;
                    switch (weatherPar[k].Name) 
                    { 
                        case "symbol":
                            symbol = "_"+weatherPar[k].Attributes.GetNamedItem("var").Value;
                            description = weatherPar[k].Attributes.GetNamedItem("name").Value;
                            break;                       
                        case "windDirection":
                            windDirection = DecodeWindDirection( weatherPar[k].Attributes.GetNamedItem("code").Value);
                            break;
                        case "windSpeed":
                            windSpeed = weatherPar[k].Attributes.GetNamedItem("mps").Value;    
                            break;
                        case "temperature":
                            temperature = (weatherPar[k].Attributes.GetNamedItem("value").Value).Remove(weatherPar[k].Attributes.GetNamedItem("value").Value.IndexOf('.'), weatherPar[k].Attributes.GetNamedItem("value").Value.Count() - weatherPar[k].Attributes.GetNamedItem("value").Value.IndexOf('.'));
                            break;
                        case "pressure":                            
                            NumberFormatInfo nfi = new CultureInfo("ru-RU", false).NumberFormat;
                            nfi.NumberDecimalSeparator = ".";
                            double x = Double.Parse(weatherPar[k].Attributes.GetNamedItem("value").Value, nfi) * 0.75;        
                            pressure = ((int)x).ToString();
                            break;
                        case "humidity":
                            humidity = weatherPar[k].Attributes.GetNamedItem("value").Value;
                            break;                        
                        default: break;

                    }
                    
                }

                hourlyForecast.Add(new HourlyForecastData {
                    city = city,
                    region = region,
                    sourse = "owm",
                    date = time1,
                    description = description,
                    temperature = temperature,
                    pressure = pressure,
                    wind_direction = windDirection,
                    wind_speed = windSpeed,
                    hummidity = humidity,
                    symbol = symbol                        
                });

                hourlyForecast.Add(new HourlyForecastData
                {
                    city = city,
                    region = region,
                    sourse = "owm",
                    date = time2,
                    description = description,
                    temperature = temperature,
                    pressure = pressure,
                    wind_direction = windDirection,
                    wind_speed = windSpeed,
                    hummidity = humidity,
                    symbol = symbol
                });

                hourlyForecast.Add(new HourlyForecastData
                {
                    city = city,
                    region = region,
                    sourse = "owm",
                    date = time3,
                    description = description,
                    temperature = temperature,
                    pressure = pressure,
                    wind_direction = windDirection,
                    wind_speed = windSpeed,
                    hummidity = humidity,
                    symbol = symbol
                });

                // из hourlyForecast делаем dailyForecast
            }

            for (int i = 0; i < hourlyForecast.Count; i++)
            {
                switch (hourlyForecast[i].date.Hour)
                { 
                    case 3:
                        dailyForecast.Add(new DayFarecastData
                        {
                            city = hourlyForecast[i].city,
                            region = hourlyForecast[i].region,
                            sourse = hourlyForecast[i].sourse,
                            date = hourlyForecast[i].date.Date,
                            description = hourlyForecast[i].description,
                            temperature = hourlyForecast[i].temperature,
                            pressure = hourlyForecast[i].pressure,
                            wind_direction = hourlyForecast[i].wind_direction,
                            wind_speed = hourlyForecast[i].wind_speed,
                            hummidity = hourlyForecast[i].hummidity,
                            symbol = hourlyForecast[i].symbol,
                            time_of_day = "ночь"
                        });
                        break;
                    case 9:
                        dailyForecast.Add(new DayFarecastData
                        {
                            city = hourlyForecast[i].city,
                            region = hourlyForecast[i].region,
                            sourse = hourlyForecast[i].sourse,
                            date = hourlyForecast[i].date.Date,
                            description = hourlyForecast[i].description,
                            temperature = hourlyForecast[i].temperature,
                            pressure = hourlyForecast[i].pressure,
                            wind_direction = hourlyForecast[i].wind_direction,
                            wind_speed = hourlyForecast[i].wind_speed,
                            hummidity = hourlyForecast[i].hummidity,
                            symbol = hourlyForecast[i].symbol,
                            time_of_day = "утро"
                        }); 
                        break;
                    case 15:
                        dailyForecast.Add(new DayFarecastData
                        {
                            city = hourlyForecast[i].city,
                            region = hourlyForecast[i].region,
                            sourse = hourlyForecast[i].sourse,
                            date = hourlyForecast[i].date.Date,
                            description = hourlyForecast[i].description,
                            temperature = hourlyForecast[i].temperature,
                            pressure = hourlyForecast[i].pressure,
                            wind_direction = hourlyForecast[i].wind_direction,
                            wind_speed = hourlyForecast[i].wind_speed,
                            hummidity = hourlyForecast[i].hummidity,
                            symbol = hourlyForecast[i].symbol,
                            time_of_day = "день"
                        });
                        break;
                    case 21:
                        dailyForecast.Add(new DayFarecastData
                        {
                            city = hourlyForecast[i].city,
                            region = hourlyForecast[i].region,
                            sourse = hourlyForecast[i].sourse,
                            date = hourlyForecast[i].date.Date,
                            description = hourlyForecast[i].description,
                            temperature = hourlyForecast[i].temperature,
                            pressure = hourlyForecast[i].pressure,
                            wind_direction = hourlyForecast[i].wind_direction,
                            wind_speed = hourlyForecast[i].wind_speed,
                            hummidity = hourlyForecast[i].hummidity,
                            symbol = hourlyForecast[i].symbol,
                            time_of_day = "вечер"
                        });
                        break;
                }
            }

            // потом оба списка пишем в базу 
              
        }

        public string DecodeWindDirection(string iwd)
        {
            string wd = "";
            switch (iwd)
            { 
                case "W":
                    wd = "западный";
                    break;
                case "S":
                    wd = "южный";
                    break;
                case "N":
                    wd = "северный";
                    break;
                case "E":
                    wd = "восточный";
                    break;
                case "SW":
                    wd = "юго-западный";
                    break;
                case "NW":
                    wd = "северо-западный";
                    break;
                case "SE":
                    wd = "юго-восточный";
                    break;
                case "NE":
                    wd = "северо-восточный";
                    break;

                case "NNE":
                    wd = "северный, северо-восточный";
                    break;
                case "ENE":
                    wd = "восточный, северо-восточный";
                    break;
                case "ESE":
                    wd = "восточный, юго-восточный";
                    break;
                case "SSE":
                    wd = "южный, юго-восточный";
                    break;
                case "SSW":
                    wd = "южный, юго-западный";
                    break;
                case "WSW":
                    wd = "западный, юго-западный";
                    break;
                case "WNW":
                    wd = "западный, северо-западный";
                    break;
                case "NNW":
                    wd = "северный, северо-западный";
                    break;

            }

            return wd;
        }

        



    }

}
