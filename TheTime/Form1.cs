using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace TheTime
{
    public partial class Form1 : Form
    {
        WeatherWorker ww = new WeatherWorker();
        List<Cities> listOfCities;
        List<FactWeather> listOfFacts;

        public Form1()
        {            
            InitializeComponent();
            listOfCities = ww.GetListOfCities();   
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // comboBox3.Visible = false;        
          
            List<string> countries = new List<string>();
            
            var custs = (from customer in listOfCities
                         select new {customer.country}).Distinct();
            
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

        // Иконки в трее
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

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

        private void Form1_Deactivate_1(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.ShowBalloonTip(500, "Сообщение", "Я свернулась:)", ToolTipIcon.Warning);

                notifyIcon1.Text = "+1";
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            string id = ww.GetCityIdString(comboBox1.Text, comboBox2.Text, comboBox3.Text, listOfCities);
            string ss = "";
            listOfFacts = ww.GetFactWeather(id);

            Image myIcon = (Image)TheTime.Properties.Resources.ResourceManager.GetObject(listOfFacts[0].pic);  
            pictureBox1.Image = myIcon;

            label1.Text = listOfFacts[0].desc;
            label2.Text = listOfFacts[0].temp;

            int a = 0;
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            try
            {
                FillComboBox2();
            }

            catch (Exception ex)
            { }
        }

        private void comboBox2_Leave(object sender, EventArgs e)
        {
            try
            {
                FillComboBox3();
            }

            catch (Exception ex)
            { }
        }

    }
}
