using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TheTime
{
    public partial class TreyTest : Form
    {
        public TreyTest()
        {
            InitializeComponent();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
           

           
        }

        private void TreyTest_Deactivate(object sender, EventArgs e)
        {

            
            DeactivateForm();
        }

        public void DeactivateForm()
        {

            string CurIcon = "_01d";
            string CurTemp = "-10";
            string CurDesc = "солнечно";
            Bitmap bmp = new Bitmap((Image)TheTime.Properties.Resources.ResourceManager.GetObject(CurIcon));
            float size = 15;
            Font f = new Font(this.Font.FontFamily.Name, size, FontStyle.Bold);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawString(CurTemp, f, Brushes.Blue, 10, 15);
            }

            //Icon icon = new Icon(Icon.FromHandle(bmp.GetHicon()), 100, 100);

            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.ShowBalloonTip(500, "Сообщение", "Я свернулась:)", ToolTipIcon.Warning);
                notifyIcon1.Icon = Icon.FromHandle(bmp.GetHicon());
                //notifyIcon1.Icon = icon;
                notifyIcon1.Text = CurTemp + "\r\n" + CurDesc;
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }

           
        }

        private void TreyTest_Load(object sender, EventArgs e)
        {

            string CurIcon = "_01d";
            string CurTemp = "-10";
            Bitmap bmp = new Bitmap((Image)TheTime.Properties.Resources.ResourceManager.GetObject(CurIcon));
            float size = 15;
            Font f = new Font(this.Font.FontFamily.Name, size, FontStyle.Bold);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawString(CurTemp, f, Brushes.Blue, 10, 15);
            }

            pictureBox1.Image = bmp;


            this.WindowState = FormWindowState.Minimized;
            DeactivateForm();  
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.notifyIcon1.Dispose();
            }
            base.Dispose(disposing);
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            //notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            if (e.Button == MouseButtons.Right)
            {
                notifyIcon1.ContextMenuStrip = contextMenuStrip1;                
                notifyIcon1.ContextMenuStrip.Show();
            }
            else
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.ShowInTaskbar = true;
                    notifyIcon1.Visible = false;
                }
            }
        }


        private void TreyTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (e.CloseReason == CloseReason.UserClosing)
            //{
            //    e.Cancel = true;
            //    Hide();
            //    WindowState = FormWindowState.Minimized;
            //    DeactivateForm();
            //}
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
