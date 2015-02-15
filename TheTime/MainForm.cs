using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using TheTime.OpenWeatherMap;

namespace TheTime
{
    public partial class MainForm : Form
    {       
        string CurIcon = "";
        string CurTemp = "";

        DataAccessLevel.Forecast forecast = new DataAccessLevel.Forecast();
                
        public MainForm()
        {
            InitializeComponent();
            
        }       

        public DataAccessLevel.Forecast GetForecat(string path)
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

                    // получаем прогноз из базы по установленному в настройках серверу

                    worker.SetConnect(path);
                    forecast = worker.GetForecast(DateTime.Now);
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

        void GridView2 (DateTime Select_day)
        {
            DataAccessLevel.Forecast tag1 = forecast;
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
            //this.WindowState = FormWindowState.Minimized;
            //DeactivateForm();
                        
            
            forecast = GetForecat(Program.DBName);

            //------------ТАЙМЕР------------- 
            //int num = 1; //
            //TimerCallback tm = new TimerCallback(Count);
            //System.Threading.Timer timer = new System.Threading.Timer(tm, num, 0, 2000);                       
            
            //------Заполнение  GRIDVIEW


            Image myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject(forecast.curWeather.symbol);
            pictureBox1.Image = myIcon;

            label2.Text = forecast.curWeather.description;
            label1.Text = forecast.curWeather.temperature;
            label3.Text = "Давление: " + forecast.curWeather.pressure;

            DataAccessLevel.Forecast tag1 = forecast;
            
            linkLabel2.Text = DateTime.Parse(Convert.ToString(tag1.hourlyList[0].periodDate)).ToLongDateString();

            for (int i = 0; i < 8; i++)
            {
                int t = 3*i;
                if (i!=7)
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = (t).ToString()+":00";
                dataGridView1.Rows[i].Cells[1].Value = tag1.hourlyList[i].temperature;
                myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject(tag1.hourlyList[i].symbol);
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
            Label[] mor = new Label[Kol * 2];
            Label[] tem = new Label[Kol * 2];
            for (int i = 0; i < Kol; i++)
            {
                tb[i] = new System.Windows.Forms.GroupBox();
                mor[i * 2] = new System.Windows.Forms.Label();
                mor[i * 2 + 1] = new System.Windows.Forms.Label();
                tem[i * 2] = new System.Windows.Forms.Label();
                tem[i * 2 + 1] = new System.Windows.Forms.Label();
                if (i < 5)
                    tb[i].Location = new System.Drawing.Point(10 + 80 * i, 30);
                else
                    tb[i].Location = new System.Drawing.Point((10 + 80 * i) - 400, 132);
                tb[i].Name = "groupboxes" + i.ToString();
                tb[i].Size = new System.Drawing.Size(75, 77);
                tb[i].TabIndex = i;
                tb[i].Text = DateTime.Parse(Convert.ToString(tag1.tenDaysList[i * 2].periodDate)).ToShortDateString();

                mor[i * 2].Text = "День";
                mor[i * 2].Location = new System.Drawing.Point(30, 16);
                mor[i * 2].Size = new System.Drawing.Size(34, 13);

                mor[i * 2 + 1].Text = "Ночь";
                mor[i * 2 + 1].Location = new System.Drawing.Point(32, 43);
                mor[i * 2 + 1].Size = new System.Drawing.Size(32, 13);

                tem[i * 2].Text = tag1.tenDaysList[i].temperature;
                tem[i * 2].Location = new System.Drawing.Point(36, 29);
                tem[i * 2].Size = new System.Drawing.Size(30, 13);

                tem[i * 2 + 1].Text = tag1.tenDaysList[i + 1].temperature;
                tem[i * 2 + 1].Location = new System.Drawing.Point(36, 59);
                tem[i * 2 + 1].Size = new System.Drawing.Size(30, 13);

                tb1[i] = new System.Windows.Forms.PictureBox();
                tb1[i].Location = new System.Drawing.Point(2, 28);
                tb1[i].Name = "pictureboxes" + i.ToString();
                tb1[i].Size = new System.Drawing.Size(28, 40);
                myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject("ovc");
                tb1[i].Image = myIcon;
                tb1[i].SizeMode = PictureBoxSizeMode.StretchImage;

                tabPage3.Controls.Add(tb[i]);
                tb[i].Controls.Add(tem[i * 2]);
                tb[i].Controls.Add(tem[i * 2 + 1]);
                tb[i].Controls.Add(mor[i * 2]);
                tb[i].Controls.Add(mor[i * 2 + 1]);
                tb[i].Controls.Add(tb1[i]);
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

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            Settings form = new Settings();            
            if (form.ShowDialog() == DialogResult.OK)
            { 
                // вызываем обновление формы
            }
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

      
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (monthCalendar1.Visible == false)
            {

                monthCalendar1.Visible = true;
                monthCalendar1.MinDate = monthCalendar1.TodayDate;
                monthCalendar1.MaxDate = monthCalendar1.TodayDate.AddDays(9);
            }
            else
            {
                linkLabel2.Text = DateTime.Parse(Convert.ToString(monthCalendar1.SelectionStart)).ToLongDateString();
                monthCalendar1.Visible = false;
                GridView2(monthCalendar1.SelectionStart);
                //MessageBox.Show(Convert.ToString(monthCalendar1.SelectionStart));
            }
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            monthCalendar1.Visible = false;
            GridView2(monthCalendar1.SelectionStart);
            linkLabel2.Text = DateTime.Parse(Convert.ToString(monthCalendar1.SelectionStart)).ToLongDateString();

        }

    }
}
