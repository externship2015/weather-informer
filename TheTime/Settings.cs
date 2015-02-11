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
    public partial class Settings : Form
    {

        WeatherWorker ww = new WeatherWorker();
        List<Cities> listOfCities;

        public Settings()
        {
            InitializeComponent();
            listOfCities = ww.GetListOfCities();

            /*TODO 
             * Доставать настройки из базы и заполнить форму выбранными данными
             */
            try
            {
                var sets = from c in Program.data.AppSettings select c;
                if (sets.Count() > 0)
                {
                    switch (sets.First().service)
                    {
                        case "owm":
                            radioButton2.Checked = true;
                            break;
                        case "ya":
                            radioButton1.Checked = true;
                            break;
                    }
                    switch (sets.First().forecastDaysCount)
                    {
                        case 1:
                            radioButton3.Checked = true;
                            break;
                        case 3:
                            radioButton4.Checked = true;
                            break;
                        case 10:
                            radioButton5.Checked = true;
                            break;
                    }


                    comboBox1.Text = sets.First().curCountry;
                    comboBox2.Text = sets.First().curRegion;
                    comboBox3.Text = sets.First().curCity;

                }
                else
                {
                    MessageBox.Show("Настройка программы не выполнялась. Заполните поля на форме");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        /*private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboBox2();
        }*/

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

                // сохранение настроек в базу 
                string curCity = comboBox3.Text;
                string curRegion = comboBox2.Text;
                string curCountry = comboBox1.Text;
                string service = "";
                int cnt;

                if (radioButton1.Checked)
                {
                    service = "ya";
                }
                else
                {
                    service = "owm";
                }

                if (radioButton3.Checked)
                    cnt = 1;
                else
                    if (radioButton4.Checked)
                        cnt = 3;
                    else cnt = 10;

                // записываем в базу данных
                var sets = from c in Program.data.AppSettings select c;
                if (sets.Count() > 0)
                {
                    // update
                    var AppSettings = (from c in Program.data.AppSettings select c).First();
                    AppSettings.curCity = curCity;
                    AppSettings.curRegion = curRegion;
                    AppSettings.curCountry = curCountry;
                    AppSettings.service = service;
                    AppSettings.forecastDaysCount = cnt;

                    Program.data.GetTable<AppSetting>();
                    Program.data.SubmitChanges();
                }
                else
                { 
                    // new
                    AppSetting set = new AppSetting
                    {
                        curCity = curCity,
                        curCountry = curCountry,
                        curRegion = curRegion,
                        service = service,
                        forecastDaysCount = cnt
                    };
                    Program.data.GetTable<AppSetting>().InsertOnSubmit(set);
                    Program.data.SubmitChanges();
                }
                
               

                MessageBox.Show("Изменения сохранены!");
                this.Close();
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
