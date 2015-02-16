using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace TheTime
{
    static class Program
    {

        public static string DBName = @"Database.db";

        
       // public static DataWorkerDataContext data = new DataWorkerDataContext();
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new TreyTest());
            
            //Application.Run(new OpenWeatherMap.Form1());
            
            //Application.Run(new Date_Time.Test());
            //Application.Run(new TheTime.OpenWeatherMap.Form1());
            Application.Run(new MainForm());
            //Application.Run(new Settings());
            //Application.Run(new TheTime.DataAccessLevel.Form1());
            //Application.Run(new MainForm2());
            //Application.Run(new TheTime.ПапкаАндрея.FormAndrew());
        }
    }
}
