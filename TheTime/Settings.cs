using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheTime.DataAccessLevel;
using TheTime.Date_Time;
using TheTime.Yandex;

namespace TheTime
{
    public partial class Settings : Form
    {

        SettingsDataContext sdc = new SettingsDataContext();
        YandexMethods ym = new YandexMethods();
        RegionCitiesLists rsl = new RegionCitiesLists();

        int regionid = 0;
        int yacityId = 0;
        int opcityid = 0;
        public Settings()
        {
            InitializeComponent();
            ym.GetRegionCitiesList(rsl);
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
          
        }      

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            regionid = FindRegionId(comboBox1.Text);
            FillComboBox2(regionid);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //FillComboBox3();
        }

        public void FillComboBox2(int regId)
        {
            comboBox2.Items.Clear();         
            foreach (var item in rsl.citiesList.OrderBy(s => s.name).Where(s=>s.regionID==regId))
            {
                comboBox2.Items.Add(item.name);
            }
        }

        public void FillComboBox3()
        {
            regionid = FindRegionId(comboBox1.Text);
           yacityId= FindCityYaId(regionid, comboBox2.Text);            
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            regionid = FindRegionId(comboBox1.Text);
            FillComboBox2(regionid);
        }

        private void comboBox2_Leave(object sender, EventArgs e)
        {

            FillComboBox3();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    Program.setData.CurCity = comboBox3.Text;
            //    Program.setData.CurRegion = comboBox2.Text;
            //    Program.setData.CurCountry = "Россия";
            //   if (radioButton1.Checked)
            //   {
            //       Program.setData.CurService = "ya";
            //   }
            //   else
            //   {
            //       Program.setData.CurService = "owm";
            //   }

            //    // Записываем в БД
            //   SQLWorker sqlW= new SQLWorker();
            //   sqlW.UpdateSettings();

            //   MessageBox.Show("Изменения сохранены");
            //   this.Close(); 

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
              if (radioButton1.Checked == true)
                radioButton2.Checked = false;
              if (radioButton2.Checked == true)
                  radioButton1.Checked = false;
              OnChangedRB();
              if (sdc.sourceID == 2)
                  getYandex();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
              if (radioButton1.Checked == true)
                radioButton2.Checked = false;
              if (radioButton2.Checked == true)
                  radioButton1.Checked = false;
              OnChangedRB();
              if (sdc.sourceID == 2)
                  getYandex();
        }

        public void getYandex()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            
            
            foreach (var item in rsl.regionsList.OrderBy(s => s.name))
            {
                if(item.name!="")
                comboBox1.Items.Add(item.name);                
            }

        }
        public void OnChangedRB()
        {
            if (radioButton1.Checked == true)
                sdc.sourceID = 2;
            if (radioButton2.Checked == true)
                sdc.sourceID = 1;           
        }

        public int FindRegionId(string str)
        {
            int st1=0;
            foreach (var item in rsl.regionsList.Where(s=>s.name==str))
            {
                st1 = item.regionID;
                break;
            }
            return st1; 
        }

        public int FindCityYaId(int regId, string cityName)
        {
            int st1 = 0;
            foreach (var item in rsl.citiesList.Where(s => s.regionID == regId && s.name==cityName))
            {
                st1 = item.yandexID;
                break;
            }
            return st1; 
        }
        
    }
}
