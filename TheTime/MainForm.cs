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
    public partial class MainForm : Form
    {
        string CurSity = "";
        string CurCountry = "";
        string CurRegion = "";
        int cnt = 1;
        string service = "";
        string CurIcon = "";
        string CurTemp = "";

        WeatherWorker ww = new WeatherWorker();
        List<Cities> listOfCities;
        List<FactWeather> listOfFacts;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            listOfCities = ww.GetListOfCities(); 


            //var sets = from c in Program.data.AppSettings select c;
            //if (sets.Count() > 0)
            //{
            //    CurSity = sets.First().curCity;
            //    CurCountry = sets.First().curCountry;
            //    CurRegion = sets.First().curRegion;
            //    service = sets.First().service;
            //    cnt = (int)sets.First().forecastDaysCount;
            //}
            //else
            //{
            //    Settings form = new Settings();
            //    form.Show();
            //    MessageBox.Show("Заполните форму!");
            //}

            CurSity = "Ульяновск";
            CurCountry = "Россия";
            CurRegion = "Ульяновская область";
            //service = "owm";
            ////    cnt = (int)sets.First().forecastDaysCount;
            service = "ya";

            switch (service)
            {
                case "ya":
                    Yandex();
                    break;
                case "owm":
                    OpenWeather();
                    break;
            }

           

        }

        public void OpenWeather() 
        {
            OpenweathermapAPIWorker worker = new OpenweathermapAPIWorker();
            //List<OpenWeatherForecast> CurWeather = worker.GetWeather(CurSity, CurCountry);
            worker.GetWeather(CurSity, CurRegion, CurCountry);

            //Image myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject(CurWeather[0].symbol);
            //pictureBox1.Image = myIcon;

            //label1.Text = CurWeather[0].temperature;
            //label2.Text = "Давление: " + CurWeather[0].pressure;
            //label3.Text = "Влажность воздуха: " + CurWeather[0].humidity + "%";

            //CurIcon = CurWeather[0].symbol;
            //CurTemp = CurWeather[0].temperature;

            //pictureBox1.Image = myIcon;
        }

        public void Yandex() 
        {
            groupBox1.Visible = true;
            string id = ww.GetCityIdString(CurCountry, CurRegion, CurSity, listOfCities);
            string ss = "";
            listOfFacts = ww.GetFactWeather(id);

            CurIcon = listOfFacts[0].pic;
            CurTemp = listOfFacts[0].temp;
            Image myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject(listOfFacts[0].pic);
            pictureBox1.Image = myIcon;

            label2.Text = listOfFacts[0].desc;
            label1.Text = listOfFacts[0].temp;
            label3.Text = "Давление: " + listOfFacts[0].pressure;

        }


        private void linkLabel1_Click(object sender, EventArgs e)
        {
            Settings form = new Settings();
            form.Show();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
            } 
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            DeactivateForm();
        }

        public void DeactivateForm()
        {
            
            CurIcon = "_01d";
            Bitmap bmp = new Bitmap((Image)TheTime.Properties.Resources.ResourceManager.GetObject(CurIcon));

            float size = 20;
            Font f = new Font(this.Font.FontFamily.Name, size, FontStyle.Bold);


            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawString(CurTemp, f, Brushes.Blue, bmp.Width / 7, bmp.Height / 5);
            }

            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.ShowBalloonTip(500, "Сообщение", "Я свернулась:)", ToolTipIcon.Warning);
                notifyIcon1.Icon = Icon.FromHandle(bmp.GetHicon());
                notifyIcon1.Text = CurTemp;
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }
    }
}
