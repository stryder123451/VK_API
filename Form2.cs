using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VK_get
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            button1.PerformClick();

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {



            try
            {
                string s = webBrowser1.Url.ToString();
                string l = s.Split('#')[1];
                if (l[0] == 'a')
                {
                    autset.Default.token = l.Split('&')[0].Split('=')[1];
                    autset.Default.id = l.Split('=')[3];
                    autset.Default.auth = true;
                    
                  
                    this.Close();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
   
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = "https://oauth.vk.com/authorize?client_id=" + "5269368" + "&scope=" + "audio,wall" + "&redirect_uri=https://oauth.vk.com/blank.html&display=popup&v=" + "5.44" + "&response_type=" + "token";

            webBrowser1.Navigate("https://oauth.vk.com/authorize?client_id=" + "5269368" + "&scope=" + "audio,wall" + "&redirect_uri=https://oauth.vk.com/blank.html&display=popup&v=" + "5.44" + "&response_type=" + "token");
        }
    
        
    
    }
}
