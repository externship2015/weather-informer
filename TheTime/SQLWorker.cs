using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace TheTime
{
    class SQLWorker
    {
        SQLConnection sql = new SQLConnection();      
        public bool FillSettings()
        {
            OleDbDataReader reader = sql.Query("SELECT * FROM settings_table");
            reader.Read();
           
            int count = 0;
            while (reader.Read())
            {
                Program.setData.CurCity = reader.GetString(3);
                Program.setData.CurCountry = reader.GetString(2);
                Program.setData.CurRegion = reader.GetString(4);
                Program.setData.CurService = reader.GetString(1);
                count++;
            }
            if (count>0)
            {
                // заполняем данными из БД                
                reader.Close();
                return true;
            }
            else
            { 
                // настроек в БД нет, устанавливаем Ульяновск по умолчанию
                // и сообщаем об этом пользователю
                Program.setData.CurCity = "Ульяновск";
                Program.setData.CurCountry = "Россия";
                Program.setData.CurRegion = "Ульяновская область";
                Program.setData.CurService = "ya";

                // и сохраняем их в БД
                SaveSettings();

                return false;
            }                
        }

        public void SaveSettings()
        {
            sql.Query("INSERT INTO settings_table ( service, country, city, region) Values ('" + Program.setData.CurService + "','" + Program.setData.CurCountry + "', '" + Program.setData.CurCity + "', '" + Program.setData.CurRegion +  "')");
                     
        }
        public void UpdateSettings()
        { 
            //обновляем строку в БД
            string query = "UPDATE settings_table SET service='" + Program.setData.CurService + "', country='" + Program.setData.CurCountry + "', city='" + Program.setData.CurCity + "', region='"+Program.setData.CurRegion+"' WHERE Id='4'";
 
            sql.Query(query);
 
        }
    }
}
