using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projectIP
{
    public partial class Form1 : Form
    {
        string pattern = @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$";
        public Form1()
        {
            InitializeComponent();
            label5.Text = "";
            label6.Text = "";
            label7.Text = "";
            label8.Text = "";
        }      

        private void button1_Click(object sender, EventArgs e)
        {            
            if (IsInFormat())
            {            
                string line = "";

                using (WebClient wc = new WebClient())
                    line = wc.DownloadString($"https://ipwhois.app/xml/{textBox1.Text}");

                Match match = Regex.Match(line, "<country>(.*?)</country>(.*?)" +
                    "<region>(.*?)</region>(.*?)" +
                    "<city>(.*?)</city>(.*?)" +
                    "<latitude>(.*?)</latitude>(.*?)" +
                    "<longitude>(.*?)</longitude>(.*?)" +
                    "<isp>(.*?)</isp>");

                label5.Text =  match.Groups[1].Value;
                label6.Text = match.Groups[3].Value;
                label7.Text = match.Groups[5].Value;
                label8.Text = match.Groups[11].Value;

                string latitude = match.Groups[7].Value;
                string longitude = match.Groups[9].Value;

                webBrowser1.Navigate($"https://www.google.com/maps/dir/{latitude},{longitude}/@{latitude},{longitude}");
            }
            else 
                MessageBox.Show("Неверный формат!", Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains(','))
            {
                textBox1.Text = textBox1.Text.Replace(',', '.');
                textBox1.SelectionStart = textBox1.TextLength;
            }
            if (Regex.IsMatch(textBox1.Text, "[^0-9-.]"))
            {                
                MessageBox.Show("IP содержит только цифры", Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                textBox1.Text = textBox1.Text.Remove(textBox1.TextLength - 1);
                textBox1.SelectionStart = textBox1.TextLength;
            }            
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Введите IP")
            {                
                textBox1.Text = string.Empty;
                textBox1.ForeColor = Color.Black;
            }                
        }      

        private bool IsInFormat()
        {           

            if (Regex.IsMatch(textBox1.Text, pattern))
            {
                string input = textBox1.Text;

                ushort[] numbers = Array.ConvertAll(input.Split('.'), Convert.ToUInt16);

                foreach (var number in numbers)
                {
                    if (number > 255) return false;
                }

                return true;
            }

            return false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) button1.PerformClick();
        }

    }
}
