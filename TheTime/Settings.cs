using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheTime.Date_Time;

namespace TheTime
{
    public partial class Settings : Form
    {

        WeatherWorker ww = new WeatherWorker();
        List<Cities> listOfCities;
        GetTime qwe = new GetTime();
        public Settings()
        {
            InitializeComponent();
            listOfCities = ww.GetListOfCities();
            string dt = qwe.Yandex_Time();
             switch (Program.setData.CurService)
             {
                 case "owm":
                     radioButton2.Checked = true;
                     break;
                 case "ya":
                     radioButton1.Checked = true;
                     break;
             }
             comboBox1.Text = Program.setData.CurCountry;
             comboBox2.Text = Program.setData.CurRegion;
             comboBox3.Text = Program.setData.CurCity;           
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            List<string> city = new List<string>();

            var custs = (from customer in listOfCities
                         select new { customer.part }).Distinct();
            custs = custs.OrderBy(customer => customer.part);
            foreach (var item in custs)
            {
                comboBox2.Items.Add(item.part);
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

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            FillComboBox2();
        }

        private void comboBox2_Leave(object sender, EventArgs e)
        {
            FillComboBox3();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Program.setData.CurCity = comboBox3.Text;
                Program.setData.CurRegion = comboBox2.Text;
                Program.setData.CurCountry = "Россия";
               if (radioButton1.Checked)
               {
                   Program.setData.CurService = "ya";
               }
               else
               {
                   Program.setData.CurService = "owm";
               }

                // Записываем в БД
               SQLWorker sqlW= new SQLWorker();
               sqlW.UpdateSettings();

               MessageBox.Show("Изменения сохранены");
               this.Close(); 

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }       
    }
}
