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
    public partial class OWM : Form
    {

        WeatherWorker ww = new WeatherWorker();
        List<Cities> listOfCities;

        string CurIcon = "";
        string CurTemp = "";
        public OWM()
        {
            InitializeComponent();
            listOfCities = ww.GetListOfCities();
        }

        private void OWM_Load(object sender, EventArgs e)
        {
            List<string> countries = new List<string>();

            var custs = (from customer in listOfCities
                         select new { customer.country }).Distinct();

            foreach (var item in custs)
            {
                comboBox1.Items.Add(item.country);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboBox2();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboBox3();
        }

        public void FillComboBox2()
        {
            comboBox3.Items.Clear();
            //comboBox3.Visible = false;
            comboBox3.Text = "";
            comboBox2.Items.Clear();
            comboBox2.Text = "";
            var custs = (from customer in listOfCities
                         select new { customer.part, customer.country }).Where(t => t.country.ToString() == comboBox1.Text.ToString()).Distinct();


            foreach (var item in custs.OrderBy(s => s.part))
            {
                if (item.part.ToString() != "")
                    comboBox2.Items.Add(item.part);

            }

            if (comboBox2.Items.Count == 0)
            {
                var custs2 = (from customer in listOfCities
                              select new { customer.citName, customer.country }).Where(t => t.country.ToString() == comboBox1.Text.ToString()).Distinct();


                foreach (var item in custs2.OrderBy(s => s.citName))
                {
                    if (item.citName.ToString() != "")
                        comboBox2.Items.Add(item.citName);
                }

            }


        }

        public void FillComboBox3()
        {
            comboBox3.Items.Clear();
            //comboBox3.Visible = false;
            comboBox3.Text = "";
            List<string> countries = new List<string>();

            var custs = (from customer in listOfCities
                         select new { customer.part, customer.citName }).Where(t => t.part.ToString() == comboBox2.Text.ToString()).Distinct();


            foreach (var item in custs.OrderBy(s => s.citName)) // list2 .Where(t=>t.country.ToString()==comboBox1.Text.ToString()).Distinct())
            {
                if (item.citName.ToString() != "")
                    comboBox3.Items.Add(item.citName);
            }

            if (comboBox3.Items.Count != 0)
            {
                comboBox3.Visible = true;
            }
            else
            {
                comboBox3.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string city = "";
            //string country = comboBox1.Text;
            //if (comboBox3.Visible == true)
            //{
            //    city = comboBox3.Text;
            //}
            //else
            //{
            //    city = comboBox2.Text;
            //}

            //OpenweathermapAPIWorker worker = new OpenweathermapAPIWorker();
            ////List<OpenWeatherForecast>  CurWeather = worker.GetWeather(city, region, country);

            //// CurWeather[0];

            //Image myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject(CurWeather[0].symbol);
            //pictureBox1.Image = myIcon;

            //label1.Text = CurWeather[0].temperature;
            //label2.Text = "Давление: " + CurWeather[0].pressure;
            //label3.Text = "Влажность воздуха: " + CurWeather[0].humidity + "%";

            //CurIcon = CurWeather[0].symbol;
            //CurTemp = CurWeather[0].temperature;



            ////pictureBox1.Image.Dispose();
            //pictureBox1.Image = myIcon;

        }

        private void OWM_Deactivate(object sender, EventArgs e)
        {

            Bitmap bmp = new Bitmap((Image)TheTime.Properties.Resources.ResourceManager.GetObject(CurIcon));

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawString("-11", this.Font, Brushes.Blue, (bmp.Width / 3) * 2, (bmp.Height / 3) * 2);
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

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
            }
        }
    }
}
