using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.OleDb;
using System.Windows.Forms;

namespace TheTime.Date_Time
{

    //Вызов класса в Settings
    class GetTime
    {
        public string Yandex_Time()
    {
            
            XmlDocument xDoc = new XmlDocument();
            string pthPic="";
            string id = "27786";
            xDoc.Load("http://export.yandex.ru/weather-ng/forecasts/" + id + ".xml");
            foreach (XmlNode node in xDoc.DocumentElement)
            {
               pthPic += ParseXMLfact(node.OuterXml, "uptime");
               break;
            }
            string DateNow = pthPic.Remove(10);
            string TimeNow = pthPic.Remove(0, 11);
            MessageBox.Show("Дата: "+DateNow+" \n"+"Время: "+TimeNow, "Время с Yandex");
            return TimeNow;
    }
       public string ParseXMLfact(string bigStr, string litStr)
     {
            string ret = "";
            int i = bigStr.IndexOf(litStr) + litStr.Length;
            while (bigStr[i] != '<')
            {
                if (bigStr[i] == '>')
                {
                    while (bigStr[i+1] != '<')
                    {
                        i++;
                        ret += bigStr[i];                        
                    }
                    return ret;
                }
                else
                {
                    i++;
                }
                }
            return ret;  
        }

       
    }
}
