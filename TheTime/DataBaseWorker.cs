using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;


namespace TheTime
{
    class DataBaseWorker
    {
        public void DB_query()
        {
            OleDbConnection oleCon = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\WeatherBase.mdb");
            OleDbCommand oleCmd = new OleDbCommand();
            oleCmd.Connection = oleCon;
            oleCmd.CommandText = "SELECT * FROM Tab";
            oleCon.Open();
            //DataSet ds; ds = new DataSet();
            OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM Tab", oleCon);
            //da.Fill(ds, "Tab");
            //int i = 0;
            //for (i = 0; i < ds.Tables["Tab"].Rows.Count; i++)
            //{
            //    comboBox1.Items.Add(ds.Tables["Tab"].Rows[i][1].ToString());//выводит все данные из 2 столбца нужно чтоб данные не повторялись
            //}
        }
    }
}
