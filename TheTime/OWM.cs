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
    }
}
