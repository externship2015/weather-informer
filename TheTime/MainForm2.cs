using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TheTime
{
    public partial class MainForm2 : Form
    {
        public MainForm2()
        {
            InitializeComponent();
        }

        private void MainForm2_Load(object sender, EventArgs e)
        {
            DataAccessLevel.Forecast forecast = GetForecat();
        }

        public DataAccessLevel.Forecast GetForecat()
        {
            DataAccessLevel.Forecast forecast = new DataAccessLevel.Forecast();

            // получаем текущий город из настроек
            DataAccessLevel.SQLiteDatabaseWorker worker = new DataAccessLevel.SQLiteDatabaseWorker();
            worker.SetConnect(Program.DBName);
            DataAccessLevel.SettingsDataContext sdc = worker.GetSettings(); // настройки
            worker.CloseConnect();

            // sdc.cityID - id выбранного города
            // sdc.ID - id настройки



            // получаем текущее время - нужен id текущий города на яндексе
            Date_Time.GetTime getter = new Date_Time.GetTime();
            DateTime CurDate = getter.Yandex_Time(sdc.cityID);

            // получаем прогноз с яндекса (по ID города)
            DataAccessLevel.Forecast yandexForecast = new DataAccessLevel.Forecast();
            Yandex.YandexMethods yaworker = new Yandex.YandexMethods();
            yaworker.GetYandexForecast(sdc.cityID.ToString(), yandexForecast);


            int a = 0;
            

            return forecast;
        }
    }
}
