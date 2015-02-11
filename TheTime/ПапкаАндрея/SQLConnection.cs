using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace TheTime.ПапкаАндрея
{
    public class SQLConnection
    {
        private System.Data.OleDb.OleDbConnection oleconnectobj;

        public void ConnectDB()
        {

            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Password=;User ID=Admin;Data Source=WeatherBase.mdb;Mode=Share Deny None;";
            // string connectionString = "Data source=mysql.dfx06.myjino.ru;UserId=045468013_cer;Password=goodpass73;database=dfx06_norms";
            if (oleconnectobj == null)
                oleconnectobj = new OleDbConnection(connectionString);

        }

        public bool checkConnect()
        {
            try
            {

                OleDbDataReader reader = Query("SELECT * FROM settings_table");
                // OleDbDataReader reader = Query("SELECT * FROM open1b");
                reader.Read();
                reader.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public OleDbDataReader Query(string query)
        {
            ConnectDB();
            try
            {
                OleDbCommand command = new OleDbCommand();
                command.CommandText = query;
                command.Connection = oleconnectobj;
                if (command.Connection.State != System.Data.ConnectionState.Open)
                    command.Connection.Open();               
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                oleconnectobj.Close();
                return null;
            }
        }

    }
}
