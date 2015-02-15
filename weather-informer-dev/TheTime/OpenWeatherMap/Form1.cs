using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            APIWorker worker = new APIWorker();
            DataAccessLevel.Forecast forecast = worker.GetWeather("Ульяновск", "Ульяновская область", "Россия");
            DataAccessLevel.SQLiteDatabaseWorker SQLworker = new DataAccessLevel.SQLiteDatabaseWorker();
            SQLworker.SetConnect(Program.DBName);
            SQLworker.SaveForecast(forecast);
            SQLworker.CloseConnect();
        }
    }
}
