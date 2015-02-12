using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using System.Xml;

namespace TheTime.OpenWeatherMap
{
    class APIWorker
    {
        System.Xml.XmlNodeList forecastsList;
        public DataAccessLevel.Forecast GetWeather(string city, string region, string country)
        {
            // получаем прогноз на 10 дней вперед с сервера
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("http://api.openweathermap.org/data/2.5/forecast?&q=" + city + "," + country + "&mode=xml&units=metric&cnt=10&lang=ru");

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

            DataAccessLevel.Forecast forecast = new DataAccessLevel.Forecast();
        

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

                DateTime time = DateTime.Parse(forecastsList[i].Attributes.GetNamedItem("from").Value);
             
                var weatherPar = forecastsList[i].ChildNodes;
                for (int k = 0; k < weatherPar.Count; k++)
                {
                    var attr2 = weatherPar[k].Attributes;
                    switch (weatherPar[k].Name)
                    {
                        case "symbol":
                            symbol = "_" + weatherPar[k].Attributes.GetNamedItem("var").Value;
                            description = weatherPar[k].Attributes.GetNamedItem("name").Value;
                            break;
                        case "windDirection":
                            windDirection = DecodeWindDirection(weatherPar[k].Attributes.GetNamedItem("code").Value);
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

                forecast.hourlyList.Add(new DataAccessLevel.HourlyForecastsDataContext
                {
                    description = description,
                    hummidity = humidity,
                    periodDate = time.Date,
                    periodTime = time.Hour,
                    pressure = pressure,
                    symbol = symbol,
                    temperature = temperature,
                    windDirection = windDirection,
                    windSpeed = windSpeed
                });                
            }

            // из почасового делаем прогноз на день
            for (int i = 0; i < forecast.hourlyList.Count; i++)
            {
                switch (forecast.hourlyList[i].periodTime)
                {
                    case 3:
                        forecast.dailyList.Add(new DataAccessLevel.DailyForecastsDataContext {
                            description = forecast.hourlyList[i].description,
                            hummidity = forecast.hourlyList[i].hummidity,
                            periodDate = forecast.hourlyList[i].periodDate,
                            pressure = forecast.hourlyList[i].pressure,
                            symbol = forecast.hourlyList[i].symbol,
                            temperature = forecast.hourlyList[i].temperature,
                            timeOfDay = "ночь",
                            windDirection = forecast.hourlyList[i].windDirection,
                            windSpeed = forecast.hourlyList[i].windSpeed
                        });

                        forecast.tenDaysList.Add(new DataAccessLevel.TenDaysForecastsDataContext {
                            periodDate = forecast.hourlyList[i].periodDate,
                            symbol = forecast.hourlyList[i].symbol,
                            temperature = forecast.hourlyList[i].temperature,
                            timeOfDay = "ночь"
                        });

                        break;
                    case 9:
                        forecast.dailyList.Add(new DataAccessLevel.DailyForecastsDataContext
                        {
                            description = forecast.hourlyList[i].description,
                            hummidity = forecast.hourlyList[i].hummidity,
                            periodDate = forecast.hourlyList[i].periodDate,
                            pressure = forecast.hourlyList[i].pressure,
                            symbol = forecast.hourlyList[i].symbol,
                            temperature = forecast.hourlyList[i].temperature,
                            timeOfDay = "утро",
                            windDirection = forecast.hourlyList[i].windDirection,
                            windSpeed = forecast.hourlyList[i].windSpeed
                        });
                        break;
                    case 15:
                        forecast.dailyList.Add(new DataAccessLevel.DailyForecastsDataContext
                        {
                            description = forecast.hourlyList[i].description,
                            hummidity = forecast.hourlyList[i].hummidity,
                            periodDate = forecast.hourlyList[i].periodDate,
                            pressure = forecast.hourlyList[i].pressure,
                            symbol = forecast.hourlyList[i].symbol,
                            temperature = forecast.hourlyList[i].temperature,
                            timeOfDay = "день",
                            windDirection = forecast.hourlyList[i].windDirection,
                            windSpeed = forecast.hourlyList[i].windSpeed
                        });

                        forecast.tenDaysList.Add(new DataAccessLevel.TenDaysForecastsDataContext
                        {
                            periodDate = forecast.hourlyList[i].periodDate,
                            symbol = forecast.hourlyList[i].symbol,
                            temperature = forecast.hourlyList[i].temperature,
                            timeOfDay = "день"
                        });

                        break;
                    case 21:
                        forecast.dailyList.Add(new DataAccessLevel.DailyForecastsDataContext
                        {
                            description = forecast.hourlyList[i].description,
                            hummidity = forecast.hourlyList[i].hummidity,
                            periodDate = forecast.hourlyList[i].periodDate,
                            pressure = forecast.hourlyList[i].pressure,
                            symbol = forecast.hourlyList[i].symbol,
                            temperature = forecast.hourlyList[i].temperature,
                            timeOfDay = "вечер",
                            windDirection = forecast.hourlyList[i].windDirection,
                            windSpeed = forecast.hourlyList[i].windSpeed
                        });
                        break;
                }
            }            
            return forecast;
        }

        public string DecodeWindDirection(string iwd)
        {
            string wd = "";
            switch (iwd)
            {
                case "W":
                    wd = "З";
                    break;
                case "S":
                    wd = "Ю";
                    break;
                case "N":
                    wd = "С";
                    break;
                case "E":
                    wd = "В";
                    break;
                case "SW":
                    wd = "ЮЗ";
                    break;
                case "NW":
                    wd = "СЗ";
                    break;
                case "SE":
                    wd = "ЮВ";
                    break;
                case "NE":
                    wd = "СВ";
                    break;
                case "NNE":
                    wd = "С, СВ";
                    break;
                case "ENE":
                    wd = "В, СВ";
                    break;
                case "ESE":
                    wd = "В, ЮВ";
                    break;
                case "SSE":
                    wd = "Ю, ЮВ";
                    break;
                case "SSW":
                    wd = "Ю, ЮЗ";
                    break;
                case "WSW":
                    wd = "з, ЮЗ";
                    break;
                case "WNW":
                    wd = "З, СЗ";
                    break;
                case "NNW":
                    wd = "С, СЗ";
                    break;
            }

            return wd;
        }

        
    }
}
