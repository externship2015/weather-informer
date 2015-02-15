using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Net.Sockets;
using System.Net;

namespace TheTime.OpenWeatherMap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region Проверка городов
            //DataAccessLevel.SQLiteDatabaseWorker SQLworker = new DataAccessLevel.SQLiteDatabaseWorker();
            //SQLworker.SetConnect(Program.DBName);
            //DataAccessLevel.RegionCitiesLists list = SQLworker.GetCitiesList();
            //SQLworker.CloseConnect();

            //APIWorker worker = new APIWorker();

            //List<string> citiesList1 = new List<string>();
            //List<string> citiesList2 = new List<string>();
            //List<string> citiesList3 = new List<string>();
            //List<string> citiesList4 = new List<string>();
            //for (int i = 0; i < list.citiesList.Count(); i++)
            //{
            //    try
            //    {
            //        DataAccessLevel.Forecast forecast = worker.GetWeather(list.citiesList[i].name, "обрывалг", "Россия");
            //    }
            //    catch (System.Xml.XmlException ex)
            //    {
            //        citiesList1.Add(list.citiesList[i].name);
            //    }
            //    catch (System.Net.WebException ex)
            //    {
            //        citiesList2.Add(list.citiesList[i].name);
            //    }
            //    catch (Exception ex)
            //    {
            //        citiesList3.Add(list.citiesList[i].name);                
            //    }
            //}

            //for (int i = 0; i < citiesList1.Count; i++)
            //{
            //    try
            //    {
            //        DataAccessLevel.Forecast forecast = worker.GetWeather(citiesList1[i], "обрывалг", "Россия");
            //    }
            //    catch (System.Xml.XmlException ex)
            //    {
            //        citiesList4.Add(citiesList1[i]);
            //    }
            //    catch (Exception ex)
            //    {
            //        citiesList3.Add(list.citiesList[i].name);
            //    }
            //}           
            #endregion
        }


        #region Запись в базу городов/регионов из файла cities.xml
        public DataAccessLevel.RegionCitiesLists SaveCities(string path)
        {
            string fileName = path;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileName);
            string owmid = "";
            string yaid = "";
            string regionName = "";
            string CityName = "";

            List<Yandex.Cities1> ListOfCities = new List<Yandex.Cities1>();
            DataAccessLevel.RegionCitiesLists DBlist = new DataAccessLevel.RegionCitiesLists();

            foreach (XmlElement node in xDoc.DocumentElement)
            {
                var attr = node.Attributes;
                for (int i = 0; i < attr.Count; i++)
                {
                    CityName = node.InnerText;
                    if (node.HasAttribute("owmid"))
                        owmid = attr.GetNamedItem("owmid").Value;
                    else
                        owmid = "";
                    yaid = attr.GetNamedItem("id").Value;
                    regionName = attr.GetNamedItem("part").Value;
                }

                ListOfCities.Add(new Yandex.Cities1
                {
                    citName = CityName,
                    id = yaid,
                    part = regionName,
                    region = owmid
                });
            }

            var custs = (from customer in ListOfCities
                         select new { customer.part }).Distinct();


            int k = 1;
            foreach (var item in custs)
            {
                DBlist.regionsList.Add(new DataAccessLevel.RegionsDataContext
                {
                    name = item.part,
                    regionID = k
                });
                var custs2 = (from customer in ListOfCities
                              select new { customer.citName, customer.part, customer.id, customer.region }).Where(t => t.part.ToString() == item.part.ToString());
                foreach (var item2 in custs2)
                {
                    DBlist.citiesList.Add(new DataAccessLevel.CitiesDataContext
                    {
                        name = item2.citName,
                        regionID = k,
                        yandexID = int.Parse(item2.id),
                        owmID = item2.region

                    });
                }
                k++;
            }
            return DBlist;

          
        }
        #endregion

        #region Создание БД по дефолту
        private void button2_Click(object sender, EventArgs e)
        {
            CreateDefaultDB(@"D:\Database.db");
        }

        public void CreateDefaultDB(string path)
        {
            // создаем файл бд
            DataAccessLevel.SQLiteDatabaseCreator creator = new DataAccessLevel.SQLiteDatabaseCreator();
            creator.CreateDataBaseFile(path);

            // создаем структуру таблиц
            creator.CreateTables(path);
            
            // вставляем одну стку настек по умолчанию
            DataAccessLevel.SettingsDataContext sdc = new DataAccessLevel.SettingsDataContext();
            sdc.cityID = 27786;
            sdc.sourceID = 1; // по умолчанию - owm
            sdc.saveDate = DateTime.Now;

            DataAccessLevel.SQLiteDatabaseWorker worker = new DataAccessLevel.SQLiteDatabaseWorker();
            worker.SetConnect(path);
            worker.SaveSettings(sdc);
            worker.CloseConnect();

            // считываем города из файла
            DataAccessLevel.RegionCitiesLists DBlist = SaveCities(@"D:\cities.xml");

            // записываем города в базу
            worker.SetConnect(path);
            worker.FillCitiesAndRegionsTables(DBlist);
            worker.CloseConnect();           
        }


        #endregion

        #region Главный метод получения прогноза
        private void button3_Click(object sender, EventArgs e)
        {
            DataAccessLevel.Forecast forecast = GetCurWether(@"D:\Database.db");
        }

        public DataAccessLevel.Forecast GetCurWether(string path)
        {
            DataAccessLevel.SQLiteDatabaseWorker worker = new DataAccessLevel.SQLiteDatabaseWorker();
            DataAccessLevel.Forecast forecast = new DataAccessLevel.Forecast();

            // получаем текущий город из настроек

            worker.SetConnect(path);
            DataAccessLevel.SettingsDataContext sdc = worker.GetSettings(); // настройки
            worker.CloseConnect();

            // sdc.cityID - id выбранного города
            // sdc.ID - id настройки

            try
            {
                HttpWebRequest reqFP = (HttpWebRequest)HttpWebRequest.Create("http://www.google.com");

                HttpWebResponse rspFP = (HttpWebResponse)reqFP.GetResponse();
                if (HttpStatusCode.OK == rspFP.StatusCode)
                {
                    // HTTP = 200 - Интернет безусловно есть! 
                    rspFP.Close();     

                    DataAccessLevel.Forecast yandexForecast = new DataAccessLevel.Forecast();

                    switch (sdc.sourceID)
                    {
                        case 1: // owm 
                            // получаем город по ид яндекса GetCityByYaId
                            worker.SetConnect(path);
                            DataAccessLevel.CitiesDataContext city = worker.GetCityByYaId(sdc.cityID.ToString());
                            worker.CloseConnect();
                            // получаем прогноз owm по названию или owmid
                            OpenWeatherMap.APIWorker owmworker = new APIWorker();
                            DataAccessLevel.Forecast owmForecast = owmworker.GetWeather(city.name, city.owmID);

                            // сохраняем в базу
                            worker.SetConnect(path);
                            worker.SaveForecast(owmForecast);
                            worker.CloseConnect();

                            break;
                        case 2: // яндекс
                            // получаем прогноз с яндекса (по ID города яндекса)                    
                            Yandex.YandexMethods yaworker = new Yandex.YandexMethods();
                            yaworker.GetYandexForecast(sdc.cityID.ToString(), yandexForecast);

                            // сохраняем в базу
                            worker.SetConnect(path);
                            worker.SaveForecast(yandexForecast);
                            worker.CloseConnect();

                            return yandexForecast;
                        default:
                            break;
                    }

                    // получаем текущее время - нужен id текущий города на яндексе
                    Date_Time.GetTime getter = new Date_Time.GetTime();
                    DateTime CurDate = getter.Yandex_Time(sdc.cityID);


                    // получаем прогноз из базы по установленному в настройках серверу

                    worker.SetConnect(path);
                    forecast = worker.GetForecast(CurDate);
                    worker.CloseConnect();


                    int a = 0;

                    return forecast;
                
                }
               
                
                else
                {
                    // сервер вернул отрицательный ответ, возможно что инета нет
                    rspFP.Close();
                    MessageBox.Show("Подключение к интернету ограничено, данные могут быть неточными");

                    // получаем текущее время - нужен id текущий города на яндексе
                    Date_Time.GetTime getter = new Date_Time.GetTime();
                    DateTime CurDate = getter.Yandex_Time(sdc.cityID);


                    // получаем прогноз из базы по установленному в настройках серверу

                    worker.SetConnect(path);
                    forecast = worker.GetForecast(CurDate);
                    worker.CloseConnect();

                    return forecast;

                }               
            }
            catch (WebException)
            {
                // Ошибка, значит интернета у нас нет. Плачем :'(
                MessageBox.Show("Невозможно подключиться к интернету, данные могут быть неточными");

                // получаем прогноз из базы по установленному в настройках серверу

                worker.SetConnect(path);
                forecast = worker.GetForecast(DateTime.Now);
                worker.CloseConnect();

                return forecast;
            }
        }
        #endregion

        #region Проверка интернета
        private void button4_Click(object sender, EventArgs e)
        {
            // проверка соединения с Интернет по средствам запроса к Google
            MessageBox.Show(ConnectionAvailable("http://www.google.com").ToString());
        }

 
        public bool ConnectionAvailable(string strServer)
        {
            try
            {
                HttpWebRequest reqFP = (HttpWebRequest)HttpWebRequest.Create(strServer);
 
                HttpWebResponse rspFP = (HttpWebResponse)reqFP.GetResponse();
                if (HttpStatusCode.OK == rspFP.StatusCode)
                {
                    // HTTP = 200 - Интернет безусловно есть! 
                    rspFP.Close();
                    return true;
                }
                else
                {
                    // сервер вернул отрицательный ответ, возможно что инета нет
                    rspFP.Close();
                    return false;
                }
            }
            catch (WebException)
            {
                // Ошибка, значит интернета у нас нет. Плачем :'(
                return false;
            }
        }
    

        #endregion 
    }
}
