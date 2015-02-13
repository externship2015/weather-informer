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
        int tempid = 0;
        public Settings()
        {
            InitializeComponent();
             rsl= ym.GetRegionCitiesList();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;

            SQLiteDatabaseWorker worker = new SQLiteDatabaseWorker();
            worker.SetConnect(Program.DBName);
            SettingsDataContext sdc = worker.GetSettings(); // настройки
            worker.CloseConnect();
            regionid = FindRegionIdByCityId(sdc.cityID);
            yacityId = sdc.cityID;
            getYandex(FindRegionIdByCityId(sdc.cityID));
            tempid = sdc.cityID;
            FillComboBox2(FindRegionIdByCityId(sdc.cityID), tempid);
            if (sdc.sourceID == 2)
                radioButton1.Checked = true;
            if (sdc.sourceID == 1)
                radioButton2.Checked = true;            

           






          
        }      

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            regionid = FindRegionId(comboBox1.Text);
            FillComboBox2(regionid,tempid);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //FillComboBox3();
        }

        public void FillComboBox2(int regId, int cityid)
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "";
            bool check = false;
            foreach (var item in rsl.citiesList.OrderBy(s => s.name).Where(s=>s.regionID==regId))
            {
                
                
                comboBox2.Items.Add(item.name);
                if (!check)
                {
                    comboBox2.Text = item.name;
                    yacityId = item.yandexID;
                    check = true;
                }
                if (item.yandexID == cityid)
                    comboBox2.Text = item.name;
            }
        }

        public void FillComboBox3()
        {
            regionid = FindRegionId(comboBox1.Text);
            yacityId = FindCityYaId(regionid, comboBox2.Text);            
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            regionid = FindRegionId(comboBox1.Text);
            FillComboBox2(regionid,tempid);
        }

        private void comboBox2_Leave(object sender, EventArgs e)
        {

            FillComboBox3();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //FillComboBox2(regionid, tempid);
            sdc.cityID = yacityId;
            if (radioButton1.Checked == true)
                sdc.sourceID = 2;
            else
                sdc.sourceID = 1;
            sdc.saveDate = DateTime.Now.Date;

            string s = "";
            SQLiteDatabaseWorker worker = new SQLiteDatabaseWorker();
            worker.SetConnect(Program.DBName);
            worker.SaveSettings(sdc);
            worker.CloseConnect();
            this.Close();

            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
              if (radioButton1.Checked == true)
                radioButton2.Checked = false;
              if (radioButton2.Checked == true)
                  radioButton1.Checked = false;             
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
              if (radioButton1.Checked == true)
                radioButton2.Checked = false;
              if (radioButton2.Checked == true)
                  radioButton1.Checked = false;
          
        }

        public void getYandex(int regionid)
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            
            
            foreach (var item in rsl.regionsList.OrderBy(s => s.name))
            {
                if(item.name!="")
                comboBox1.Items.Add(item.name);
                if (regionid == item.regionID)
                    comboBox1.Text = item.name;
            }

        }
        //public void OnChangedRB()
        //{
        //    if (radioButton1.Checked == true)
        //        sdc.sourceID = 2;
        //    if (radioButton2.Checked == true)
        //        sdc.sourceID = 1;           
        //}

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

        public int FindRegionIdByCityId(int cityid)
        {
            int st1 = 0;
            foreach (var item in rsl.citiesList.Where(s => s.yandexID == cityid))
            {
                st1 = item.regionID;
                break;
            }
            return st1;

        }
        
    }
}
