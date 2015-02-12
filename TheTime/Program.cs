using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace TheTime
{
    static class Program
    {
        public static SettingsData setData = new SettingsData();
       // public static DataWorkerDataContext data = new DataWorkerDataContext();
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TheTime.DataAccessLevel.Form1());
            //Application.Run(new MainForm());
            //Application.Run(new TheTime.ПапкаАндрея.FormAndrew());
        }
    }
}
