using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TheTime.DataAccessLevel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // создание файла БД
        private void button1_Click(object sender, EventArgs e)
        {
            SQLiteDatabaseCreator creator = new SQLiteDatabaseCreator();
            string databaseName = Program.DBName;
            if (creator.CreateDataBaseFile(databaseName))
                MessageBox.Show("получилось");
            else
                MessageBox.Show("увы");
        }

        // создание таблиц в базе
        private void button2_Click(object sender, EventArgs e)
        {
            SQLiteDatabaseCreator creator = new SQLiteDatabaseCreator();
            if (creator.CreateTables(Program.DBName))
                MessageBox.Show("получилось");
            else
                MessageBox.Show("увы");
        }

        // сохранение настроек
        private void button3_Click(object sender, EventArgs e)
        {
            SettingsDataContext sdc = new SettingsDataContext();
            sdc.cityID = 1234;
            sdc.sourceID = 1;
            sdc.saveDate = DateTime.Now;

            SQLiteDatabaseWorker worker = new SQLiteDatabaseWorker();
            worker.SetConnect(Program.DBName);
            worker.SaveSettings(sdc);
            worker.CloseConnect();
        }

        // Получаем настройки из базы
        private void button4_Click(object sender, EventArgs e)
        {
            SQLiteDatabaseWorker worker = new SQLiteDatabaseWorker();
            worker.SetConnect(Program.DBName);

            SettingsDataContext sdc = worker.GetSettings(); // настройки
            
            worker.CloseConnect();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SQLiteDatabaseWorker worker = new SQLiteDatabaseWorker();
            worker.SetConnect(Program.DBName);

            Forecast f = new Forecast();
            f.hourlyList.Add(new HourlyForecastsDataContext { 
                description = "описание",
                hummidity = "75",
                periodDate = DateTime.Parse("12.02.2015 0:00:00"),
                periodTime = 20,
                pressure = "755",
                settingID = 1,
                symbol = "1d",
                temperature = "-21",
                windDirection = "СЗ",
                windSpeed = "5"              
            });

            f.dailyList.Add(new DailyForecastsDataContext { 
                description = "поиссисми",
                hummidity = "75",
                periodDate = DateTime.Parse("12.02.2015 0:00:00"),
                pressure = "155",
                settingID = 1,
                symbol = "1",
                temperature = "-39",
                windDirection = "CP",
                windSpeed = "5",
                timeOfDay = "ночь"
            });

            f.tenDaysList.Add(new TenDaysForecastsDataContext {
                periodDate = DateTime.Parse("12.02.2015 0:00:00"),
                settingID = 1,
                symbol = "1",
                temperature = "-123",
                timeOfDay = "ночь"

            });

            worker.SaveForecast(f);

            worker.CloseConnect();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SQLiteDatabaseWorker worker = new SQLiteDatabaseWorker();
            worker.SetConnect(@"d:\InformerDataBase2.db");
            Forecast f = worker.GetForecast(DateTime.Now);
            worker.CloseConnect();
            int a = 0;
        }
    }
}
