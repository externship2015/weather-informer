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

        WeatherWorker ww1 = new WeatherWorker();
        List<Cities> listOfCities;
        List<FactWeather> listOfFacts;

        SQLConnection sql = new SQLConnection();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
            //-----Сворачиваем форму при запуске в трей----
            this.WindowState = FormWindowState.Minimized;
            DeactivateForm();
            
            // Устанавливаем соединение с БД
            SQLWorker sqlWorker = new SQLWorker();
            if (sql.checkConnect())
            {
                if (!sqlWorker.FillSettings())
                {
                    MessageBox.Show("Настройки программы заданы по умолчанию");
                }
            }

            //------------ТАЙМЕР------------- 
            //int num = 1; //
            //TimerCallback tm = new TimerCallback(Count);
            //System.Threading.Timer timer = new System.Threading.Timer(tm, num, 0, 2000);
            //-------------------------------
            
            listOfCities = ww1.GetListOfCities(); 


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

            //------Заполнение  GRIDVIEW

            for (int i = 0; i < 8; i++)
            {
                string id = ww1.GetCityIdString(CurCountry, CurRegion, CurSity, listOfCities);
                string ss = "";
                int t = 3*i;
                listOfFacts = ww1.GetFactWeather(id);
                if (i!=7)
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = (t).ToString()+":00";
                dataGridView1.Rows[i].Cells[1].Value = listOfFacts[0].temp;
                Image myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject(listOfFacts[0].pic);
                dataGridView1.Rows[i].Cells[2].Value = myIcon;
                dataGridView1.Rows[i].Cells[3].Value = listOfFacts[0].wind_speed;
                dataGridView1.Rows[i].Cells[4].Value = listOfFacts[0].pressure;
                dataGridView1.Rows[i].Cells[5].Value = listOfFacts[0].humidity;
               
            }
            

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
        /// <summary>
        ////Callback method for timer
        /// </summary>
        /// <param name="obj"></param>
        public static void Count(object obj)
        {
            //Вызывать методы запросов сервисов и записи в БД
           // MessageBox.Show("окно");
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
            string id = ww1.GetCityIdString(CurCountry, CurRegion, CurSity, listOfCities);
            string ss = "";
            listOfFacts = ww1.GetFactWeather(id);

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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }
    }
}
