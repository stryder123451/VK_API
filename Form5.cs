using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace VK_get
{
    public partial class Form5 : Form
    {
        List<Playlist> p;
       static Form4 f = new Form4();
        Form1 ff = new Form1();
        static public string Nazvanie;
        static public string Address;
        public string id;
        public string chief_id;
        string owner_id = autset.Default.id;
        string token = autset.Default.token;
        delegate void Potok(int s, int ss);
        static Potok potok;
        delegate void Proizvedenie(string s);
        static Proizvedenie proizv;
        delegate void CloseForm();
        static CloseForm c;  
        Thread t = new Thread(Download);
        public Form5()
        {   
            InitializeComponent();
            potok = new Potok(PotokDownload);
            proizv = new Proizvedenie(NazvaniePesni);
            c = new CloseForm(Zakrytie);
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            WebRequest req = WebRequest.Create("https://api.vk.com/method/audio.search?q=" + textBox1.Text + "&count=100" + "&auto_complete=1" + "&access_token=" + autset.Default.token);
            WebResponse res = req.GetResponse();
            Stream s = res.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string otvet = sr.ReadToEnd();
            otvet = HttpUtility.HtmlDecode(otvet);
         MessageBox.Show(otvet);
            JToken j = JToken.Parse(otvet);
            try
            {
                p = Enumerable.Skip(j["response"].Children(), 1).Select(c => c.ToObject<Playlist>()).ToList();
           
            this.Invoke((MethodInvoker)delegate
            {
                for (int i = 0; i < p.Count; i++)
                {
                    listBox1.Items.Add(p[i].artist + " - " + p[i].title);
                    listBox2.Items.Add(p[i].url);
                    listBox3.Items.Add(p[i].aid);
                    listBox4.Items.Add(p[i].owner_id);
                }
                listBox1.SelectedIndex = 0;
                listBox2.SelectedIndex = 0;
                listBox3.SelectedIndex = 0;
                listBox4.SelectedIndex = 0;
            });
       }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }  }

        class Playlist
        {
            public string artist { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public int aid { get; set; }
            public string owner_id { get; set; }
        }

        private void скачатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f.Show();



            t.Start();
        }

        public static void Download()
        {

            WebClient w = new WebClient();
            w.Proxy = new System.Net.WebProxy();
            w.DownloadProgressChanged += new DownloadProgressChangedEventHandler(w_DownloadProgressChanged);
            w.DownloadFileCompleted += new AsyncCompletedEventHandler(w_DownloadFileCompleted);
            w.DownloadFileAsync(new Uri(Address), Nazvanie + ".mp3");
        }

        private static void w_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {


            f.label1.Invoke(proizv, new object[] { Nazvanie });
            f.progressBar1.Invoke(potok, new object[] { (int)e.TotalBytesToReceive, (int)e.BytesReceived });


        }

        private static void w_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            if (e.Cancelled == true)
            {
                MessageBox.Show("Загрузка отменена!!!");
            }
            else
            {
                MessageBox.Show("Готово!!!");



                f.Invoke(c, new object[] { });
            }
        }

        private void PotokDownload(int s, int ss)
        {

            //   f.label1.Text = Nazvanie;
            f.progressBar1.Maximum = s;
            f.progressBar1.Value = ss;
        }

        private void NazvaniePesni(string s)
        {
            f.label1.Text = s;
        }

        private void Zakrytie()
        {
            f.Visible = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox1.SelectedIndex;
            listBox3.SelectedIndex = listBox1.SelectedIndex;
            listBox4.SelectedIndex = listBox1.SelectedIndex;
            Nazvanie = listBox1.SelectedItem.ToString();
            Address = listBox2.SelectedItem.ToString();
            id = listBox3.SelectedItem.ToString();
            chief_id = listBox4.SelectedItem.ToString();
        }


        private void Form5_Load(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = 0;
            listBox2.SelectedIndex = 0;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            Nazvanie = listBox1.SelectedItem.ToString();
            Address = listBox2.SelectedItem.ToString();
            id = listBox3.SelectedItem.ToString();
        }

        private void добавитьВАудиозаписиToolStripMenuItem_Click(object sender, EventArgs e)

        {
           
            WebRequest req = WebRequest.Create("https://api.vk.com/method/audio.add?audio_id=" + id + "&owner_id=" + chief_id +"&access_token=" + token);
            WebResponse res = req.GetResponse();
            Stream s = res.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string otvet = sr.ReadToEnd();
            otvet = HttpUtility.HtmlDecode(otvet);
            MessageBox.Show("https://api.vk.com/method/audio.add?audio_id=" + id + "&owner_id=" + chief_id +"&access_token=" + token);
            MessageBox.Show(otvet);
            ff.Zagruzit();
            MessageBox.Show("Готово!!!");
        }


    }
}
