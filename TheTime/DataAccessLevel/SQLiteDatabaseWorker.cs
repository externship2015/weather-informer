﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;
using System.IO;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;

namespace TheTime.DataAccessLevel
{
    /// <summary>
    /// Работает с данными в базе: insert, update, select
    /// </summary>
    class SQLiteDatabaseWorker
    {
      
        #region Done
        
        public SQLiteConnection m_dbConnection;

        /// <summary>
        /// Открывает соединение с базой данных
        /// </summary>
        /// <param name="path">Полный путь до файла БД</param>
        /// <returns></returns>
       
        public void SetConnect(string path)
        {
            m_dbConnection = new SQLiteConnection(@"Data Source=" + path + ";Version=3;datetimeformat=CurrentCulture");
            m_dbConnection.Open();
        }

        /// <summary>
        /// Закрывает соединение с базой данных
        /// </summary>
        /// <returns></returns>
        public void CloseConnect()
        {
            m_dbConnection.Close();
        }

        /// <summary>
        /// Сохраняет строку с настройками в таблице settings
        /// </summary>
        /// <param name="setObj">объект класса SettingsDataContext</param>       
        public void SaveSettings(SettingsDataContext setObj)
        {
            // проверить наличие настройки с параметрами в базе
            string sql = "SELECT * FROM settings WHERE cityID = '" + setObj.cityID + "' AND sourseID = '" + setObj.sourceID +"';";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            // считаем
            int count = 0;
            foreach (DbDataRecord record in reader)
            {
                count++;
            }
            if (count > 0)
            {
                // update
                sql = @"UPDATE settings SET
                        saveDate = '" + DateTime.Now + "' WHERE cityID = '" + setObj.cityID + "' AND sourseID = '" + setObj.sourceID + "';";
                command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            else
            { 
                // insert
                sql = @"INSERT INTO settings
                                (cityID, sourseID, saveDate)
                           VALUES ('" + setObj.cityID.ToString() + "', '" + setObj.sourceID.ToString() + "', '" + DateTime.Now + "')";
                command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }           
        }

        /// <summary>
        /// Получает настройки из таблицы settings
        /// </summary>
        /// <returns>объект класса SettingsDataContext</returns>
        public SettingsDataContext GetSettings()
        {
            SettingsDataContext ret = new SettingsDataContext();

            string sql = "SELECT * FROM 'settings' order by saveDate DESC  Limit 1;";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                ret.ID = int.Parse(record["ID"].ToString());
                ret.cityID = int.Parse(record["cityID"].ToString());
                ret.sourceID = int.Parse(record["sourseID"].ToString());
                ret.saveDate = DateTime.Parse(record["saveDate"].ToString());
            }
            return ret;
        }


        /// <summary>
        /// Сохраняет / обновляет прогнозы в трех таблицах
        /// </summary>
        /// <param name="forecast">объект класса Forecast</param>
        public void SaveForecast(Forecast forecast)
        {
            // получаем текущий SettingID
            SettingsDataContext set = GetSettings();

            // для каждого  public List<HourlyForecastsDataContext> hourlyList { get; set; } - проверяем и обновляем / переписываем
            #region
            for (int i = 0; i < forecast.hourlyList.Count; i++)
            {
                // проверить наличие такой строки в базе               
                string sql = "SELECT * FROM 'hourly_forecasts' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.hourlyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND periodTime = '" + forecast.hourlyList[i].periodTime + "';";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                // считаем
                int count = 0;
                foreach (DbDataRecord record in reader)
                {
                    count++;
                }

                if (count > 0)
                {
                    // делаем update
                    sql = @"UPDATE 'hourly_forecasts' SET
                            description = '" + forecast.hourlyList[i].description + "', temperature = '" + forecast.hourlyList[i].temperature + "', windSpeed = '" + forecast.hourlyList[i].windSpeed + "', windDirection = '" + forecast.hourlyList[i].windDirection + "', pressure = '" + forecast.hourlyList[i].pressure + "', hummidity = '" + forecast.hourlyList[i].hummidity + "', symbol ='" + forecast.hourlyList[i].symbol + "' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.hourlyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND periodTime = '" + forecast.hourlyList[i].periodTime + "';";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    // сохраняем
                    sql = @"INSERT INTO hourly_forecasts
                                (settingID, periodDate, periodTime, description, temperature, windSpeed, windDirection, pressure, hummidity, symbol)
                          VALUES ('" + set.ID + "', date('" + forecast.hourlyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "'), '" + forecast.hourlyList[i].periodTime + "', '" + forecast.hourlyList[i].description + "', '" + forecast.hourlyList[i].temperature + "', '" + forecast.hourlyList[i].windSpeed + "', '" + forecast.hourlyList[i].windDirection + "', '" + forecast.hourlyList[i].pressure + "', '" + forecast.hourlyList[i].hummidity + "', '" + forecast.hourlyList[i].symbol + "')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
            }
            #endregion

            // для каждого public List<DailyForecastsDataContext> dailyList { get; set; } - проверяем и обновляем / переписываем
            #region
            for (int i = 0; i < forecast.dailyList.Count; i++)
            {
                // проверить наличие такой строки в базе               
                string sql = "SELECT * FROM 'daily_forecasts' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.dailyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND timeOfDay = '" + forecast.dailyList[i].timeOfDay + "';";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                // считаем
                int count = 0;
                foreach (DbDataRecord record in reader)
                {
                    count++;
                }

                if (count > 0)
                {
                    // делаем update
                    sql = @"UPDATE 'daily_forecasts' SET
                            description = '" + forecast.dailyList[i].description + "', temperature = '" + forecast.dailyList[i].temperature + "', windSpeed = '" + forecast.dailyList[i].windSpeed + "', windDirection = '" + forecast.dailyList[i].windDirection + "', pressure = '" + forecast.dailyList[i].pressure + "', hummidity = '" + forecast.dailyList[i].hummidity + "', symbol ='" + forecast.dailyList[i].symbol + "' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.dailyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND timeOfDay = '" + forecast.dailyList[i].timeOfDay + "';";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    // сохраняем
                    sql = @"INSERT INTO daily_forecasts
                                (settingID, periodDate, timeOfDay, description, temperature, windSpeed, windDirection, pressure, hummidity, symbol)
                          VALUES ('" + set.ID + "', date('" + forecast.dailyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "'), '" + forecast.dailyList[i].timeOfDay + "', '" + forecast.dailyList[i].description + "', '" + forecast.dailyList[i].temperature + "', '" + forecast.dailyList[i].windSpeed + "', '" + forecast.dailyList[i].windDirection + "', '" + forecast.dailyList[i].pressure + "', '" + forecast.dailyList[i].hummidity + "', '" + forecast.dailyList[i].symbol + "')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
            }
            #endregion

            // для каждого public List<TenDaysForecastsDataContext> tenDaysList { get; set; } - проверяем и обновляем / переписываем
            #region
            for (int i = 0; i < forecast.tenDaysList.Count; i++)
            {
                // проверить наличие такой строки в базе               
                string sql = "SELECT * FROM 'ten_days_forecasts' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.tenDaysList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND timeOfDay = '" + forecast.tenDaysList[i].timeOfDay + "';";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                // считаем
                int count = 0;
                foreach (DbDataRecord record in reader)
                {
                    count++;
                }

                if (count > 0)
                {
                    // делаем update
                    sql = @"UPDATE 'ten_days_forecasts' SET
                            temperature = '" + forecast.tenDaysList[i].temperature + "',  symbol ='" + forecast.tenDaysList[i].symbol + "' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.tenDaysList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND timeOfDay = '" + forecast.tenDaysList[i].timeOfDay + "';";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    // сохраняем
                    sql = @"INSERT INTO ten_days_forecasts
                                (settingID, periodDate, timeOfDay, temperature, symbol)
                          VALUES ('" + set.ID + "', date('" + forecast.tenDaysList[i].periodDate.Date.ToString("yyyy-MM-dd") + "'), '" + forecast.tenDaysList[i].timeOfDay + "', '" + forecast.tenDaysList[i].temperature + "', '" + forecast.tenDaysList[i].symbol + "')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
            }
            #endregion

        }

        /// <summary>
        /// Получаем прогноз из базы данных
        /// </summary>
        /// <param name="Current">Текущие дата/время в формате DateTime</param>
        /// <returns>объект класса Forecast</returns>
        public Forecast GetForecast(DateTime Current)
        {
            SettingsDataContext sdc = GetSettings(); // получили текущие настройки

            List<string> setID = new List<string>();

            // получаем id всех настроек с похожими параметрами
            string sql = "SELECT * FROM 'settings' WHERE cityID = '" + sdc.cityID + "' AND sourseID = '" + sdc.sourceID + "';";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                setID.Add(record["ID"].ToString());
            }


            Forecast f = new Forecast();

            // вычисляем "текущий час"
            int CurHour = Current.Hour;
            switch (Current.Hour)
            {
                case 1:
                case 2:
                    CurHour = 0;
                    break;
                case 4:
                case 5:
                    CurHour = 3;
                    break;
                case 7:
                case 8:
                    CurHour = 6;
                    break;
                case 10:
                case 11:
                    CurHour = 9;
                    break;
                case 13:
                case 14:
                    CurHour = 12;
                    break;
                case 16:
                case 17:
                    CurHour = 15;
                    break;
                case 19:
                case 20:
                    CurHour = 18;
                    break;
                case 22:
                case 23:
                    CurHour = 21;
                    break;
            }

            

            // получаем прогноз на текущий момент
            sql = "SELECT * FROM 'hourly_forecasts' WHERE settingId = '" + sdc.ID + "' AND periodDate = date('" + Current.Date.ToString("yyyy-MM-dd") + "') AND periodTime = '" + CurHour + "' LIMIT 1;";
            command = new SQLiteCommand(sql, m_dbConnection);
            reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                f.curWeather.description = record["description"].ToString();
                f.curWeather.hummidity = record["hummidity"].ToString();
                f.curWeather.pressure = record["pressure"].ToString();
                f.curWeather.symbol = record["symbol"].ToString();
                f.curWeather.temperature = record["temperature"].ToString();
                f.curWeather.windDirection = record["windDirection"].ToString();
                f.curWeather.windSpeed = record["windSpeed"].ToString();
            }

            // получаем прогноз на день
            

            sql = "SELECT * FROM 'daily_forecasts' WHERE settingId = '" + sdc.ID + "' AND periodDate > date ('"+ Current.Date.ToString("yyyy-MM-dd") + "')";
            // должны получить 4 записи
            command = new SQLiteCommand(sql, m_dbConnection);
            reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                DailyForecastsDataContext context = new DailyForecastsDataContext();
                context.description = record["description"].ToString();
                context.hummidity = record["hummidity"].ToString();
                context.periodDate = DateTime.Parse(record["periodDate"].ToString());
                context.pressure = record["pressure"].ToString();
                context.settingID = int.Parse(record["settingID"].ToString());
                context.symbol = record["symbol"].ToString();
                context.temperature = record["temperature"].ToString();
                context.timeOfDay = record["timeOfDay"].ToString();
                context.windDirection = record["windDirection"].ToString();
                context.windSpeed = record["windSpeed"].ToString();
                f.dailyList.Add(context);
            }

            // получаем прогноз на 10 дней -> 10*2 = 20 записей
            sql = "SELECT * FROM 'ten_days_forecasts' WHERE settingId = '" + sdc.ID + "' AND periodDate > date ('"+ Current.Date.ToString("yyyy-MM-dd") + "')";
            command = new SQLiteCommand(sql, m_dbConnection);
            reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
            {
                TenDaysForecastsDataContext context = new TenDaysForecastsDataContext();
                context.periodDate = DateTime.Parse(record["periodDate"].ToString());
                context.settingID = int.Parse(record["settingID"].ToString());
                context.symbol = record["symbol"].ToString();
                context.temperature = record["temperature"].ToString();
                context.timeOfDay = record["timeOfDay"].ToString();
                f.tenDaysList.Add(context);

            }

            // получаем почасовой прогноз на день - должны получить 9 записей
            sql = "SELECT * FROM 'hourly_forecasts' WHERE settingId = '" + sdc.ID + "' AND periodDate = date ('" + Current.Date.ToString("yyyy-MM-dd") + "')";
            command = new SQLiteCommand(sql, m_dbConnection);
            reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                HourlyForecastsDataContext context = new HourlyForecastsDataContext();
                context.description = record["description"].ToString();
                context.hummidity = record["hummidity"].ToString();
                context.periodDate = Current;
                context.pressure = record["pressure"].ToString();
                context.settingID = int.Parse(record["settingID"].ToString());
                context.symbol = record["symbol"].ToString();
                context.temperature = record["temperature"].ToString();
                context.periodTime = int.Parse(record["periodTime"].ToString());
                context.windDirection = record["windDirection"].ToString();
                context.windSpeed = record["windSpeed"].ToString();
                f.hourlyList.Add(context);
            }

            return f;
        }

        /// <summary>
        /// Заполняет таблицы с городами и регионами из xml-ки яндекса
        /// </summary>
        /// <returns></returns>
        public void FillCitiesAndRegionsTables(RegionCitiesLists listRC)
        {
            string sql = "";
            // записываем все регионы в базу
            for (int i = 0; i < listRC.regionsList.Count; i++)
            {
                sql = @"INSERT INTO regions
                                (regionID, name)
                          VALUES ('" + listRC.regionsList[i].regionID + "', '" + listRC.regionsList[i].name + "');";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }

            // записываем все города в базу
            for (int i = 0; i < listRC.citiesList.Count; i++)
            {
                sql = @"INSERT INTO cities
                                (name, regionID, yandexID, owmID)
                          VALUES ('" + listRC.citiesList[i].name + "', '" + listRC.citiesList[i].regionID + "', '" + listRC.citiesList[i].yandexID + "', '" + listRC.citiesList[i].owmID + "');";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
        }    
        #endregion

        #region ToDo

        /// <summary>
        /// Получает список городов из базы
        /// </summary>
        /// <returns>объект класса RegionCitiesLists</returns>
        public RegionCitiesLists GetCitiesList()
        {
            RegionCitiesLists listRC = new RegionCitiesLists();
            string sql = "SELECT * FROM 'cities' ;";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                listRC.citiesList.Add(new CitiesDataContext
                {
                    name = record["name"].ToString(),
                    regionID = int.Parse(record["regionID"].ToString()),
                    yandexID = int.Parse(record["yandexID"].ToString()),
                    owmID = record["owmID"].ToString()
                });
            }

            sql = "SELECT * FROM 'regions';";
            command = new SQLiteCommand(sql, m_dbConnection);
            reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
            {
                listRC.regionsList.Add(new RegionsDataContext
                {
                    regionID = int.Parse(record["regionID"].ToString()),
                    name = record["name"].ToString()
                });
            }

            return listRC;
        }
          

        public CitiesDataContext GetCityByYaId(string yaid)
        {
            CitiesDataContext context = new CitiesDataContext();

            string sql = "select * from cities where yandexID = '"+yaid+"'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
            {
                context.name = record["name"].ToString();
                context.owmID = record["owmID"].ToString();
                context.regionID = int.Parse(record["regionID"].ToString());
                context.yandexID = int.Parse(record["yandexID"].ToString());
                
            }

            return context;
        }
        #endregion


        public void SaveForecast(Forecast forecast, ProgressBar pb, SettingsDataContext set)
        {
            // получаем текущий SettingID
           // SettingsDataContext set = GetSettings();

            // для каждого  public List<HourlyForecastsDataContext> hourlyList { get; set; } - проверяем и обновляем / переписываем
            #region
            for (int i = 0; i < forecast.hourlyList.Count; i++)
            {
                // проверить наличие такой строки в базе               
                string sql = "SELECT * FROM 'hourly_forecasts' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.hourlyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND periodTime = '" + forecast.hourlyList[i].periodTime + "';";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                // считаем
                int count = 0;
                foreach (DbDataRecord record in reader)
                {
                    count++;
                }

                pb.Increment(1);
                if (count > 0)
                {
                    
                    // делаем update
                    sql = @"UPDATE 'hourly_forecasts' SET
                            description = '" + forecast.hourlyList[i].description + "', temperature = '" + forecast.hourlyList[i].temperature + "', windSpeed = '" + forecast.hourlyList[i].windSpeed + "', windDirection = '" + forecast.hourlyList[i].windDirection + "', pressure = '" + forecast.hourlyList[i].pressure + "', hummidity = '" + forecast.hourlyList[i].hummidity + "', symbol ='" + forecast.hourlyList[i].symbol + "' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.hourlyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND periodTime = '" + forecast.hourlyList[i].periodTime + "';";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    // сохраняем
                    sql = @"INSERT INTO hourly_forecasts
                                (settingID, periodDate, periodTime, description, temperature, windSpeed, windDirection, pressure, hummidity, symbol)
                          VALUES ('" + set.ID + "', date('" + forecast.hourlyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "'), '" + forecast.hourlyList[i].periodTime + "', '" + forecast.hourlyList[i].description + "', '" + forecast.hourlyList[i].temperature + "', '" + forecast.hourlyList[i].windSpeed + "', '" + forecast.hourlyList[i].windDirection + "', '" + forecast.hourlyList[i].pressure + "', '" + forecast.hourlyList[i].hummidity + "', '" + forecast.hourlyList[i].symbol + "')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
            }
            #endregion

            // для каждого public List<DailyForecastsDataContext> dailyList { get; set; } - проверяем и обновляем / переписываем
            #region
            for (int i = 0; i < forecast.dailyList.Count; i++)
            {
                // проверить наличие такой строки в базе               
                string sql = "SELECT * FROM 'daily_forecasts' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.dailyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND timeOfDay = '" + forecast.dailyList[i].timeOfDay + "';";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                // считаем
                int count = 0;
                foreach (DbDataRecord record in reader)
                {
                    count++;
                }
                pb.Increment(1);
                if (count > 0)
                {
                    // делаем update
                    sql = @"UPDATE 'daily_forecasts' SET
                            description = '" + forecast.dailyList[i].description + "', temperature = '" + forecast.dailyList[i].temperature + "', windSpeed = '" + forecast.dailyList[i].windSpeed + "', windDirection = '" + forecast.dailyList[i].windDirection + "', pressure = '" + forecast.dailyList[i].pressure + "', hummidity = '" + forecast.dailyList[i].hummidity + "', symbol ='" + forecast.dailyList[i].symbol + "' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.dailyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND timeOfDay = '" + forecast.dailyList[i].timeOfDay + "';";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    // сохраняем
                    sql = @"INSERT INTO daily_forecasts
                                (settingID, periodDate, timeOfDay, description, temperature, windSpeed, windDirection, pressure, hummidity, symbol)
                          VALUES ('" + set.ID + "', date('" + forecast.dailyList[i].periodDate.Date.ToString("yyyy-MM-dd") + "'), '" + forecast.dailyList[i].timeOfDay + "', '" + forecast.dailyList[i].description + "', '" + forecast.dailyList[i].temperature + "', '" + forecast.dailyList[i].windSpeed + "', '" + forecast.dailyList[i].windDirection + "', '" + forecast.dailyList[i].pressure + "', '" + forecast.dailyList[i].hummidity + "', '" + forecast.dailyList[i].symbol + "')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
            }
            #endregion

            // для каждого public List<TenDaysForecastsDataContext> tenDaysList { get; set; } - проверяем и обновляем / переписываем
            #region
            for (int i = 0; i < forecast.tenDaysList.Count; i++)
            {
                // проверить наличие такой строки в базе               
                string sql = "SELECT * FROM 'ten_days_forecasts' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.tenDaysList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND timeOfDay = '" + forecast.tenDaysList[i].timeOfDay + "';";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                // считаем
                int count = 0;
                foreach (DbDataRecord record in reader)
                {
                    count++;
                }
                pb.Increment(1);
                if (count > 0)
                {
                    // делаем update
                    sql = @"UPDATE 'ten_days_forecasts' SET
                            temperature = '" + forecast.tenDaysList[i].temperature + "',  symbol ='" + forecast.tenDaysList[i].symbol + "' WHERE settingId = '" + set.ID + "' AND periodDate = date('" + forecast.tenDaysList[i].periodDate.Date.ToString("yyyy-MM-dd") + "') AND timeOfDay = '" + forecast.tenDaysList[i].timeOfDay + "';";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    // сохраняем
                    sql = @"INSERT INTO ten_days_forecasts
                                (settingID, periodDate, timeOfDay, temperature, symbol)
                          VALUES ('" + set.ID + "', date('" + forecast.tenDaysList[i].periodDate.Date.ToString("yyyy-MM-dd") + "'), '" + forecast.tenDaysList[i].timeOfDay + "', '" + forecast.tenDaysList[i].temperature + "', '" + forecast.tenDaysList[i].symbol + "')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();
                }
            }
            #endregion

        }

        public string GetCurCityName(string id)
        {
            string name = "";
            string sql = "select * from cities WHERE yandexID = '" + id + "'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                name = record["name"].ToString();
            }

            return name;
        }

        public SettingsDataContext GetAltSetStr(SettingsDataContext curSet)
        {
            SettingsDataContext altSet = new SettingsDataContext();

            // пробуем получить альтернативную настройку
            string sql = "SELECT * FROM 'settings' WHERE cityID = '" + curSet.cityID + "' AND sourseID <> '" + curSet.sourceID + "' ";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            int count = 0;
            foreach (DbDataRecord record in reader)
            {
                count++;
                altSet.cityID = int.Parse(record["cityID"].ToString());
                altSet.ID = int.Parse(record["ID"].ToString());
                altSet.saveDate = DateTime.Parse(record["saveDate"].ToString());
                altSet.sourceID = int.Parse(record["sourseID"].ToString());
            }

            if (count > 0)
                return altSet;
            else 
            {
                int sID = 1;
                if (curSet.sourceID == 1)
                    sID = 2;
                else sID = 1;
                // если такой строки настроек нет, то добавляем ее
                sql = @"INSERT INTO settings
                                (cityID, sourseID, saveDate)
                           VALUES ('" + curSet.cityID + "', '" + sID + "', '" + DateTime.Now.AddYears(-1)  + "')";
                command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();

                sql = "SELECT * FROM 'settings' WHERE cityID = '" + curSet.cityID + "' AND sourseID <> '" + curSet.sourceID + "' ";
                command = new SQLiteCommand(sql, m_dbConnection);
                reader = command.ExecuteReader();                
                foreach (DbDataRecord record in reader)
                {
                    altSet.cityID = int.Parse(record["cityID"].ToString());
                    altSet.ID = int.Parse(record["ID"].ToString());
                    altSet.saveDate = DateTime.Parse(record["saveDate"].ToString());
                    altSet.sourceID = int.Parse(record["sourseID"].ToString());
                }
            }
            return altSet;
        }
    }
}
