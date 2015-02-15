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
            forecast = yaworker.GetYandexForecast(sdc.cityID.ToString(), yandexForecast);
            //MessageBox.Show(Convert.ToString(sdc.cityID));
            
            int a = 0;


            return forecast;
        }

        void GridView2 (DateTime Select_day)
        {
            DataAccessLevel.Forecast tag1 = new DataAccessLevel.Forecast();
            tag1 = GetForecat();
            int day = Select_day.Day - tag1.hourlyList[0].periodDate.Day;
            //MessageBox.Show(Convert.ToString(day));
            for (int i = 0; i < 4; i++)
            {
                if (i != 3)
                dataGridView2.Rows.Add();
                dataGridView2.Rows[i].Height = 34;
                dataGridView2.Rows[i].Cells[0].Value = tag1.dailyList[i+day*4].temperature;
                Image myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject(tag1.dailyList[i + day * 4].symbol);
                dataGridView2.Rows[i].Cells[1].Value = myIcon;
                dataGridView2.Rows[i].Cells[2].Value = tag1.dailyList[i + day * 4].windSpeed;
                dataGridView2.Rows[i].Cells[3].Value = tag1.dailyList[i + day * 4].pressure;
                dataGridView2.Rows[i].Cells[4].Value = tag1.dailyList[i + day * 4].hummidity;
            }
            
            int total_height = 34 * 4+62; // высота dataGrubview
            dataGridView2.Height = total_height;
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
            

            DataAccessLevel.Forecast tag1 = new DataAccessLevel.Forecast();
            tag1 = GetForecat();
            
            linkLabel2.Text = DateTime.Parse(Convert.ToString(tag1.hourlyList[0].periodDate)).ToLongDateString();

            for (int i = 0; i < 8; i++)
            {
                int t = 3*i;
                if (i!=7)
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = (t).ToString()+":00";
                dataGridView1.Rows[i].Cells[1].Value = tag1.hourlyList[i].temperature;
                Image myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject(tag1.hourlyList[i].symbol);
                dataGridView1.Rows[i].Cells[2].Value = myIcon;
                dataGridView1.Rows[i].Cells[3].Value = tag1.hourlyList[i].windSpeed;
                dataGridView1.Rows[i].Cells[4].Value = tag1.hourlyList[i].pressure;
                dataGridView1.Rows[i].Cells[5].Value = tag1.hourlyList[i].hummidity;               
            }
            int total_height = 22 * 8 + 47;
            dataGridView1.Height = total_height;
            GridView2(tag1.hourlyList[0].periodDate);
            
            
            /*int day =0;
            for (int i = 0; i < 4; i++)
            {
                if (i != 3)
                    dataGridView2.Rows.Add();
                dataGridView2.Rows[i].Height = 34;
                dataGridView2.Rows[i].Cells[0].Value = tag1.dailyList[i+day*4].temperature;
                Image myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject(tag1.dailyList[i + day * 4].symbol);
                dataGridView2.Rows[i].Cells[1].Value = myIcon;
                dataGridView2.Rows[i].Cells[2].Value = tag1.dailyList[i + day * 4].windSpeed;
                dataGridView2.Rows[i].Cells[3].Value = tag1.dailyList[i + day * 4].pressure;
                dataGridView2.Rows[i].Cells[4].Value = tag1.dailyList[i + day * 4].hummidity;
            }
            total_height = 34 * 4+62; // высота dataGrubview
            dataGridView2.Height = total_height;
            */

            int Kol = 10;
GroupBox[] tb = new GroupBox[Kol];
            PictureBox[] tb1 = new PictureBox[Kol];
            Label[] mor = new Label[Kol*2];
            Label[] tem = new Label[Kol * 2];
            for (int i = 0; i < Kol; i++)
            {
                tb[i] = new System.Windows.Forms.GroupBox();
                mor[i * 2] = new System.Windows.Forms.Label();
                mor[i * 2+1] = new System.Windows.Forms.Label();
                tem[i * 2] = new System.Windows.Forms.Label();
                tem[i * 2+1] = new System.Windows.Forms.Label();
                
                tb[i].Location = new System.Drawing.Point(6 + 85*i, 7);
                tb[i].Name = "groupboxes" + i.ToString();
                tb[i].Size = new System.Drawing.Size(81, 53);
                tb[i].TabIndex = i;
                tb[i].Text = DateTime.Parse(Convert.ToString(tag1.tenDaysList[i*2].periodDate)).ToShortDateString();
                
                mor[i*2].Text = "День";
        //        mor[i*2].Location = new System.Drawing.Point(30 + 85 * i, 16);
                mor[i * 2].Size = new System.Drawing.Size(34, 13);
                
                mor[i * 2+1].Text = "Ночь";
      //          mor[i * 2+1].Location = new System.Drawing.Point(31 + 85 * i, 32);
                mor[i * 2+1].Size = new System.Drawing.Size(32, 13);

                tem[i * 2].Text = tag1.tenDaysList[i].temperature;
//                tem[i * 2].Location = new System.Drawing.Point(64 + 85 * i, 17);
                tem[i * 2].Size = new System.Drawing.Size(30, 13);

                tem[i*2+1].Text = tag1.tenDaysList[i+1].temperature;
  //              tem[i * 2+1].Location = new System.Drawing.Point(63 + 85 * i, 33);
                tem[i * 2+1].Size = new System.Drawing.Size(30, 13);

                tb1[i] = new System.Windows.Forms.PictureBox();
    //            tb1[i].Location = new System.Drawing.Point(2 + 85 * i, 19);
                tb1[i].Name = "pictureboxes" + i.ToString();
                tb1[i].Size = new System.Drawing.Size(28, 28);
                Image myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject("ovc");
                tb1[i].Image = myIcon;

                tabPage3.Controls.Add(tb[i]);
                tb[i].Controls.Add(tem[i * 2]);
                tb[i].Controls.Add(tem[i * 2+1]);
                tb[i].Controls.Add(mor[i * 2]);
                tb[i].Controls.Add(mor[i * 2+1]);
                tb[i].Controls.Add(tb1[i]);
                
            
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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (monthCalendar1.Visible == false)
            {
                monthCalendar1.Visible = true;
                monthCalendar1.MinDate = monthCalendar1.TodayDate;
                monthCalendar1.MaxDate = monthCalendar1.TodayDate.AddDays(10);
            }
            else
            {
                monthCalendar1.Visible = false;
                GridView2(monthCalendar1.SelectionStart);
                //MessageBox.Show(Convert.ToString(monthCalendar1.SelectionStart));
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
