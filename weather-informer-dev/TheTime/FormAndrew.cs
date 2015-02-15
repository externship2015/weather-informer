using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TheTime.ПапкаАндрея
{
    public partial class FormAndrew : Form
    {
        public FormAndrew()
        {
            InitializeComponent();
        }

        private void FormAndrew_Load(object sender, EventArgs e)
        {
            SQLConnection sql = new SQLConnection();
            if (sql.checkConnect())
            {
                WeatherWorker ww = new WeatherWorker();
                ww.GetFactWeatherDays("27786");
            }
        
        }
    }
}
