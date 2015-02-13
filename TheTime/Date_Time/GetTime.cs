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
        /// <summary>
        ////Возвращает текущие Дата/Время с яндекса
        /// </summary>
        /// <param name="id">id города</param>
        /// <returns>Дата/Время в формате DateTime</returns>
        public DateTime Yandex_Time(int id)
        {
            
                XmlDocument xDoc = new XmlDocument();
                string pthPic="";
                //string id = "27786";
                xDoc.Load("http://export.yandex.ru/weather-ng/forecasts/" + id.ToString() + ".xml");
                foreach (XmlNode node in xDoc.DocumentElement)
                {
                   pthPic += ParseXMLfact(node.OuterXml, "uptime");
                   break;
                }

                DateTime current = Convert.ToDateTime(pthPic);
                return current;
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
