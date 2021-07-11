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
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Threading;
using Un4seen.Bass;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace VK_get
{
    public partial class Form1 : Form
    {
        string before, after, aid,pid;
        int stream;
        List<Playlist> p;
        List<FindSong> fs;
        Request zapros = new Request();
        string nt;
        string uploadstring;
       public Thread t ;
        MouseButtons mb;
       static string  Address;
       string id;
       string chief_id;
    static   Form4 f = new Form4();
  
       
      static  public string Nazvanie;
        delegate void Potok(int s , int ss);
      static  Potok potok;
        delegate void Proizvedenie(string s);
      static  Proizvedenie proizv;
        delegate void CloseForm();
     static CloseForm c;
        delegate void OpenForm();
      static  OpenForm o;
        public Form1()
        {
            InitializeComponent();
            potok = new Potok(PotokDownload);
            proizv = new Proizvedenie(NazvaniePesni);
            c = new CloseForm(Otmena);
            o = new OpenForm(Opening);
           
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            
            pictureBox1.ImageLocation = "1.png";
            pictureBox2.ImageLocation = "5.png";
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();
            timer3.Enabled = true;
            
            Control.CheckForIllegalCrossThreadCalls = false;
            toolStripStatusLabel1.Text = label1.Text;
            toolStripStatusLabel2.Text = label2.Text;
            toolStripStatusLabel3.Text = label3.Text;
                try
                {
                    Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                }
                catch
                {

                }
        }



        public void Abort()
        {
            if (nt == System.Diagnostics.ThreadState.Running.ToString())
            {
               Thread.CurrentThread.Abort();
                
            }
            
        }

        public void Otmena()
        {
            
            f.Visible = false;
            
        }

        public void Zagruzit()
        {
            try
            {
                while (!autset.Default.auth) { Thread.Sleep(1000); }
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                listBox4.Items.Clear();
                listBox5.Items.Clear();
               
               // MessageBox.Show(otvet);
                JToken j = JToken.Parse(zapros.Get("https://api.vk.com/method/audio.get?owner_id" + autset.Default.id + "&access_token=" + autset.Default.token));
                p = Enumerable.Skip(j["response"].Children(), 0).Select(c => c.ToObject<Playlist>()).ToList();

                this.Invoke((MethodInvoker)delegate
                {
                    for (int i = 0; i < p.Count; i++)
                    {
                        listBox1.Items.Add(p[i].artist + " - " + p[i].title);
                        listBox2.Items.Add(p[i].url);
                        listBox5.Items.Add(p[i].aid);
                    }
                     listBox1.SelectedIndex = 0;
                     listBox2.SelectedIndex = listBox1.SelectedIndex;
                     listBox5.SelectedIndex = listBox1.SelectedIndex;
                     аудиозаписиToolStripMenuItem.PerformClick();
                     
                });

            
            }
            catch
            {

            }
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (stream == 0)
            {
                Play();
            }
            else
            {

                if (checkBox2.Checked == true)
                {
                    Bass.BASS_ChannelPause(stream);
                    checkBox2.Checked = false;
                    
                    button1.Text = ">";
                }
                else
                {
                    if (stream != 0)
                    {
                        Bass.BASS_ChannelPlay(stream, false);
                        checkBox2.Checked = true;
                        
                        button1.Text = "||";
                    }
                    else
                    {
                        MessageBox.Show("Песня не запущена!");
                    }
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
         Zagruzit();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox1.SelectedIndex;
         //   listBox3.SelectedIndex = listBox2.SelectedIndex;
         //listBox4.SelectedIndex = listBox2.SelectedIndex;
            Nazvanie = listBox1.SelectedItem.ToString();
            Address = listBox2.SelectedItem.ToString();
            try
            {   
                before = listBox5.Items[listBox1.SelectedIndex + 1].ToString();

                after = listBox5.Items[listBox1.SelectedIndex - 1].ToString();
            }
            catch
            {
 
            }
            

        }
        public void Play()
        {
            try
            {

                Bass.BASS_StreamFree(stream);
                stream = Bass.BASS_StreamCreateURL(listBox2.SelectedItem.ToString(), 0, 0, null, IntPtr.Zero);
                Bass.BASS_ChannelPlay(stream, false);
                progressBar1.Maximum = (int)Bass.BASS_ChannelGetLength(stream);
                progressBar1.Minimum = 0;
                timer1.Enabled = true;
                timer2.Enabled = true;
                checkBox2.Checked = true;
                button1.Text = "||";
                pictureBox2.ImageLocation = listBox2.SelectedItem.ToString();
                toolStripStatusLabel4.Visible = true;
                toolStripStatusLabel4.Text = listBox1.SelectedItem.ToString();
                toolStripStatusLabel5.Visible = true;
            }
            catch
            { }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            button1.Enabled = true;
            Play();
          
        }

        private void progressBar1_MouseDown(object sender, MouseEventArgs e)
        {
            mb = MouseButtons.Left;
            progressBar1.Value = (int)Bass.BASS_ChannelGetPosition(stream, 0) / (progressBar1.Width * e.X);
            Bass.BASS_ChannelSetPosition(stream, Bass.BASS_ChannelGetLength(stream, 0) / progressBar1.Width * e.X, 0);
        }

        private void progressBar1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (mb == MouseButtons.Left)
                {
                    progressBar1.Value = (int)Bass.BASS_ChannelGetPosition(stream, 0) / (progressBar1.Width * e.X);
                    Bass.BASS_ChannelSetPosition(stream, Bass.BASS_ChannelGetLength(stream, 0) / progressBar1.Width * e.X, 0);
                }
            }
            catch
            {
 
            }
        }

        public class Data
        {
            public string online { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string photo_50 { get; set; }
            public string photo_100 { get; set; }
        }



        private void progressBar1_MouseUp(object sender, MouseEventArgs e)
        {
            mb = MouseButtons.Right;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value =(int)Bass.BASS_ChannelGetPosition(stream,0);
           // Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, progressBar2.Value / 100);
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, hScrollBar1.Value / 100F);
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {


            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_STOPPED & checkBox1.Checked == true)
            {
                timer1.Enabled = false;
                listBox2.SelectedIndex = listBox2.SelectedIndex + 0;
                listBox1.SelectedIndex = listBox2.SelectedIndex;
                Play();


            }
            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_STOPPED)
            {
                timer1.Enabled = false;
                listBox2.SelectedIndex = listBox2.SelectedIndex + 1;
                listBox1.SelectedIndex = listBox2.SelectedIndex;
                Play();
            }
            try
            {
               
            }
            catch
            { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bass.BASS_ChannelStop(stream);
            Bass.BASS_StreamFree(stream);
            timer1.Enabled = false;
            timer2.Enabled = false;
            progressBar1.Value = progressBar1.Minimum;
            button1.Enabled = false;
        }

        private void progressBar2_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        private void progressBar2_MouseUp_1(object sender, MouseEventArgs e)
        {
           /* ProgressBar p;
            int RatioG;
            int TotalG;
            int PositionG;

            p = sender as ProgressBar;
            RatioG = e.X / p.Width;
            TotalG = p.Maximum - p.Minimum;
            PositionG = (TotalG * RatioG) + p.Minimum;
            p.Value = PositionG;
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, progressBar2.Value/100); */
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
          //  Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, hScrollBar1.Value / 100);
        }

        private void progressBar2_MouseDown(object sender, MouseEventArgs e)
        {
           
        
        }

        private void hScrollBar1_Scroll_1(object sender, ScrollEventArgs e)
        {
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, hScrollBar1.Value / 100F);
            if (hScrollBar1.Value > 50)
            {
                pictureBox2.ImageLocation = "4.png";
            }
            if (hScrollBar1.Value > 70)
            {
                pictureBox2.ImageLocation = "5.png";
            }
            if (hScrollBar1.Value < 50)
            {
                pictureBox2.ImageLocation = "3.png";
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            listBox2.SelectedIndex = listBox2.SelectedIndex - 1;
            listBox1.SelectedIndex = listBox2.SelectedIndex;
            Play();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            listBox2.SelectedIndex = listBox2.SelectedIndex + 1;
            listBox1.SelectedIndex = listBox2.SelectedIndex;
            Play();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox3_Click(object sender, EventArgs e)
        {
            
            {
                WebRequest req = WebRequest.Create("https://api.vk.com/method/account.setOffline?owner_id" + autset.Default.id + "&access_token=" + autset.Default.token);
                WebResponse res = req.GetResponse();
                Stream s = res.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                string otvet = sr.ReadToEnd();
                otvet = HttpUtility.HtmlDecode(otvet);
                
                string status = otvet.Split(':')[1].Split('}')[0];
                if (status == "1")
                {
                    MessageBox.Show("Вы помечены как : 'Не в сети '");
                }
                else
                {
                    MessageBox.Show("Ошибка!");
                }
                checkBox2.Enabled = false;
                backgroundWorker2.RunWorkerAsync();
               }
            
         }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (!autset.Default.auth) { Thread.Sleep(500); }

               
                //   MessageBox.Show(otvet);
                string o = zapros.Get("https://api.vk.com/method/" + "users.get" + "?" + "user_id=" + autset.Default.id + "&fields=photo_100,online&access_token=" + autset.Default.token).Split('[')[1].Split(']')[0];
                //MessageBox.Show(o);
                var d = JsonConvert.DeserializeObject<Data>(o);



                this.Invoke((MethodInvoker)delegate
                {

                    label1.Text = d.first_name;
                    label2.Text = d.last_name;
                    if (d.online == "1")
                    {
                        label3.Text = "online";
                        checkBox3.Enabled = true;
                        button6.Enabled = true;
                        пометитьОффлайнToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        label3.Text = "offline";
                        checkBox3.Enabled = false;
                        button6.Enabled = false;
                        пометитьОффлайнToolStripMenuItem.Enabled = false;
                    }
                    //pictureBox1.ImageLocation = d.photo_100;

                });

            }
            catch
            {

            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker2.RunWorkerAsync();
                toolStripStatusLabel1.Text = label1.Text;
                toolStripStatusLabel2.Text = label2.Text;
                toolStripStatusLabel3.Text = label3.Text;
                
                if (checkBox1.Checked == true)
                {
                    pictureBox1.ImageLocation = "2.png";
                }
                else
                {
                    pictureBox1.ImageLocation = "1.png";
                }

            }
            catch
            {
 
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
             WebClient w = new WebClient();
            w.DownloadProgressChanged +=new DownloadProgressChangedEventHandler(w_DownloadProgressChanged);
            w.DownloadFileCompleted +=new AsyncCompletedEventHandler(w_DownloadFileCompleted);
            w.DownloadFileAsync(new Uri(listBox2.SelectedItem.ToString()), "C:\\Users\\Александр\\Desktop\\" + listBox1.SelectedItem.ToString() + ".mp3");
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            WebClient w = new WebClient();
            w.DownloadProgressChanged +=new DownloadProgressChangedEventHandler(w_DownloadProgressChanged);
            w.DownloadFileCompleted +=new AsyncCompletedEventHandler(w_DownloadFileCompleted);
            w.DownloadFileAsync(new Uri(listBox2.SelectedItem.ToString()), "C:\\Users\\Александр\\Desktop\\"+listBox1.SelectedItem.ToString()+".mp3");
        }

        private static void w_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
          
            
             
             f.label1.Invoke(proizv, new object[] { Nazvanie });
             f.progressBar1.Invoke(potok, new object[] { (int)e.TotalBytesToReceive,(int)e.BytesReceived });
        
           
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

        private void button6_Click(object sender, EventArgs e)
        {
            {


                string status = zapros.Get("https://api.vk.com/method/account.setOffline?owner_id" + autset.Default.id + "&access_token=" + autset.Default.token).Split(':')[1].Split('}')[0];
                if (status == "1")
                {
                    MessageBox.Show("Вы помечены как : 'Не в сети '");
                }
                else
                {
                    MessageBox.Show("Ошибка!");
                }
                checkBox2.Enabled = false;
                button6.Enabled = false;
                backgroundWorker2.RunWorkerAsync();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void скачатьПеснюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f.Show();

             t = new Thread(Download);
       
            t.Start();
           
           
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
              
            
        }

        private void PotokDownload(int s,int ss)
        {

         //   f.label1.Text = Nazvanie;
            f.progressBar1.Maximum = s;
            f.progressBar1.Value = ss;
        }

        public   void Download()
        {
          
             WebClient w = new WebClient();
             w.Proxy = new System.Net.WebProxy();
             w.DownloadProgressChanged += new DownloadProgressChangedEventHandler(w_DownloadProgressChanged);
             w.DownloadFileCompleted += new AsyncCompletedEventHandler(w_DownloadFileCompleted);
             w.DownloadFileAsync(new Uri(Address), Nazvanie+".mp3");
        }

        private void NazvaniePesni(string s)
        {
            f.label1.Text = s;
        }

        

        public void Opening()
        {
            f.Show(); 
        }

        private void найтиПеснюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 f = new Form5();
            f.Show();
        }

        private void найтиПеснюToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            

            WebRequest req = WebRequest.Create("https://api.vk.com/method/audio.add?audio_id=" + id + "&owner_id=" + chief_id + "&access_token=" + autset.Default.token);
            WebResponse res = req.GetResponse();
            Stream s = res.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string otvet = sr.ReadToEnd();
            otvet = HttpUtility.HtmlDecode(otvet);
            MessageBox.Show("https://api.vk.com/method/audio.add?audio_id=" + id + "&owner_id=" + chief_id + "&access_token=" + autset.Default.token);
            string o =   otvet.Split('{')[1].Split(':')[0];
            if (o=="\"response\"")
            {
                MessageBox.Show(otvet);
                MessageBox.Show(o);


                MessageBox.Show("Готово!!!");
            }
            else { MessageBox.Show("Пожалуйста выберите запись..."); }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            listBox3.Items.Add("хуй");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                listBox1.AllowDrop = true;
                textBox1.Clear();
                Zagruzit();
                
            }
            if (comboBox1.SelectedIndex == 1)
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                listBox4.Items.Clear();
                textBox1.Enabled = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                listBox1.AllowDrop = false;
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                listBox4.Items.Clear();
               
              //  MessageBox.Show(otvet);
                JToken j = JToken.Parse(zapros.Get("https://api.vk.com/method/audio.search?q=" + textBox1.Text + "&count=100" + "&auto_complete=1" + "&access_token=" + autset.Default.token));
                try
                {
                    fs = Enumerable.Skip(j["response"].Children(), 1).Select(c => c.ToObject<FindSong>()).ToList();

                    this.Invoke((MethodInvoker)delegate
                    {
                        if (fs.Count == 0)
                        {
                            listBox1.Items.Add("Ничего не найдено...");
                        }
                        else
                        {
                            for (int i = 0; i < fs.Count; i++)
                            {
                                listBox1.Items.Add(fs[i].artist + " - " + fs[i].title);
                                listBox2.Items.Add(fs[i].url);
                                listBox3.Items.Add(fs[i].aid);
                                listBox4.Items.Add(fs[i].owner_id);
                            }
                            listBox1.SelectedIndex = 0;
                            listBox2.SelectedIndex = 0;
                            listBox3.SelectedIndex = 0;
                            listBox4.SelectedIndex = 0;
                            
                        }
                    });
                }
                catch (Exception ex)
                { MessageBox.Show(ex.Message); }
            }
           else
    {
        MessageBox.Show("Переключите режим!!!");
    }
 }
    
        private void listBox1_Click(object sender, EventArgs e)
        {

            if (listBox3.Items.Count == 0 & listBox4.Items.Count == 0)
            {
              
            }
            else
            {   
                listBox3.SelectedIndex = listBox1.SelectedIndex;
                listBox4.SelectedIndex = listBox1.SelectedIndex;
                listBox5.SelectedIndex = listBox1.SelectedIndex;
                id = listBox3.SelectedItem.ToString();
                chief_id = listBox4.SelectedItem.ToString();
               
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button7.PerformClick();
            }
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            WebRequest req = WebRequest.Create("https://api.vk.com/method/audio.getUploadServer?"+"access_token=" + autset.Default.token);
            WebResponse res = req.GetResponse();
            Stream s = res.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string otvet = sr.ReadToEnd();
            otvet = HttpUtility.HtmlDecode(otvet);
            MessageBox.Show(otvet);
            string o = otvet.Split(':')[3].Split('"')[0];
            MessageBox.Show(o);
          //  StreamWriter sw = new StreamWriter("log.txt");
          //  sw.WriteLine(o);
            string pattern = @"\\";
            MessageBox.Show(pattern);
            string[] ur = { o };
            foreach (string u in ur)
            {

                uploadstring = Regex.Replace(u, pattern, "");
            }
            MessageBox.Show(uploadstring);

            //            UPload up = JsonConvert.DeserializeObject<UPload>(o);
            //        MessageBox.Show(up.upload_uri);

            MessageBox.Show("http:" + uploadstring);
          

           
            

        }
        public void Zapros()
        {
           // StreamReader read = new StreamReader("upload.txt");
         //   MessageBox.Show(read.ReadToEnd());
            WebRequest req = WebRequest.Create("http:" + uploadstring + "&audio="+@"C:\Users\Public\Music\Sample Music\Kalimba.mp3");
            WebResponse res = req.GetResponse();
            Stream s = res.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string otvet = sr.ReadToEnd();
            otvet = HttpUtility.HtmlDecode(otvet);
        
            MessageBox.Show(otvet);

            
            GBS gbs = JsonConvert.DeserializeObject<GBS>(otvet);

          
           this.Invoke((MethodInvoker)delegate 
            {
               
                {
                    MessageBox.Show(gbs.audio);
                    MessageBox.Show(gbs.hash);
                    MessageBox.Show(gbs.server);
                    MessageBox.Show(gbs.redirect);
                }
                
            });

           WebRequest re = WebRequest.Create(gbs.redirect);
           WebResponse rs = re.GetResponse();
           Stream ss = rs.GetResponseStream();
           StreamReader srr = new StreamReader(s);
           string otvet1 = srr.ReadToEnd();
           otvet = HttpUtility.HtmlDecode(otvet1);

           MessageBox.Show(otvet1);

            }
        

        private void button9_Click(object sender, EventArgs e)
        {
            Zapros();
        }

         class GBS
    {   [JsonProperty("server")]
        public string server { get; set; }
        [JsonProperty("audio")]
        public string audio { get; set; }
        [JsonProperty("hash")]
        public string hash { get; set; }
        [JsonProperty("redirect")]
        public string redirect { get; set; } 
    }

         class UploadSong
         {
             public string owner_id { get; set; }
             public string id { get; set; }
             public string artist { get; set; }
             public string  title { get; set; }
             public string url { get; set; }
         }

         private void listBox1_MouseMove(object sender, MouseEventArgs e)
         {
            
             
             if (listBox1.SelectedIndex >= 0)
                 if (e.Button == MouseButtons.Left)
                 {
                     
                    

                 DoDragDrop(listBox1.SelectedIndex, DragDropEffects.Move);

        
                 }
         }

         private void listBox1_DragOver(object sender, DragEventArgs e)
         {
             try
             {
                 var i2 = listBox1.IndexFromPoint(listBox1.PointToClient(MousePosition));
              //   MessageBox.Show(i2.ToString());
                
             //    MessageBox.Show(listBox1.SelectedIndex.ToString());

                 

                 listBox1.SelectedIndex = i2;
                 listBox5.SelectedIndex = i2;
                 
                
                 e.Effect = e.AllowedEffect;
             }
             catch
             {
 
             }
         }

         private void listBox1_DragDrop(object sender, DragEventArgs e)
         {

             try
             {
                 var i1 = (int)e.Data.GetData(typeof(int));

                 var i2 = listBox1.IndexFromPoint(listBox1.PointToClient(MousePosition));
          

                 var obj = listBox1.Items[i1];
                 listBox1.Items.RemoveAt(i1);
                 i2 = i1 > i2 ? i2 : i2 - 1;
                 listBox1.Items.Insert(i2, obj);
                 listBox1.SelectedIndex = i2;
                 listBox5.SelectedIndex = i2;

                 zapros.Get("https://api.vk.com/method/audio.reorder?audio_id=" + aid + "&owner_id=" + autset.Default.id + "&before=" + before + "&after=" + after + "&access_token=" + autset.Default.token);



                 
                 listBox1.Items.Insert(i2, obj);

               
               //   MessageBox.Show(otvet);

                
               //  MessageBox.Show("Песни :" + listBox1.Items[i1].ToString() + " и " + listBox1.Items[i2].ToString() + "поменялись местами");

                 Zagruzit(); 
           
             listBox1.SelectedIndex = i2;
                 
                
               
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message);
             }
         }

         private void listBox1_MouseUp(object sender, MouseEventArgs e)
         {
             if (listBox1.SelectedIndex>=0)
                 if (e.Button == MouseButtons.Left)
                 {
                    


                
                 }
         }

         private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
         {
           
         }

         private void listBox1_KeyUp(object sender, KeyEventArgs e)
         {
            


         }

         private void listBox1_KeyPress(object sender, KeyPressEventArgs e)
         {

         }

         private void listBox1_KeyDown(object sender, KeyEventArgs e)
         {
             if (comboBox1.SelectedIndex == 0)
             {

                 if (e.KeyCode == Keys.Q)
                 { button3.PerformClick(); }
                 if (e.KeyCode == Keys.E)
                 { button4.PerformClick(); }
                 if (e.KeyCode == Keys.F)
                 { button1.PerformClick(); }
                 if (e.KeyCode == Keys.Tab)
                 {
                     if (checkBox1.Checked == true)
                     {
                         checkBox1.Checked = false;
                         pictureBox1.ImageLocation = "1.png";
                     }
                     else
                     {
                         pictureBox1.ImageLocation = "2.png";
                         checkBox1.Checked = true;
                     }
                 }
             }
         }

         private void button10_Click(object sender, EventArgs e)
         {
             

             WebRequest req = WebRequest.Create("https://api.vk.com/method/audio.reorder?audio_id=" + listBox5.Items[6].ToString() + "&owner_id=" + autset.Default.id + "&before=" + listBox5.Items[2].ToString() + "&after=" + listBox5.Items[0].ToString() + "&access_token=" + autset.Default.token);
             WebResponse res = req.GetResponse();
             Stream s = res.GetResponseStream();
             StreamReader sr = new StreamReader(s);
             string otvet = sr.ReadToEnd();
             otvet = HttpUtility.HtmlDecode(otvet);

             MessageBox.Show(otvet);
             //MessageBox.Show("Песни :" + listBox1.Items[listBox1.SelectedIndex].ToString() + " и " + listBox1.Items[i2].ToString() + "поменялись местами");

             Zagruzit();
         }

         private void listBox1_MouseDown(object sender, MouseEventArgs e)
         {
             aid = listBox5.Items[listBox1.SelectedIndex].ToString();
           //  MessageBox.Show(pid);
         }

         private void пометитьОффлайнToolStripMenuItem_Click(object sender, EventArgs e)
         {
             button6.PerformClick();
         }

         private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
         {

         }

         private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
         {
            
         }

         private void checkBox1_CheckedChanged(object sender, EventArgs e)
         {

         }

         private void pictureBox1_Click(object sender, EventArgs e)
         {
             if (checkBox1.Checked == true)
             {
                 checkBox1.Checked = false;
                 pictureBox1.ImageLocation = "1.png";
             }
             else
             {
                 pictureBox1.ImageLocation = "2.png";
                 checkBox1.Checked = true;
             }
         }

         private void textBox1_TextChanged(object sender, EventArgs e)
         {
             if (comboBox1.SelectedIndex == 1)
             {
                 
                 listBox1.AllowDrop = false;
                 listBox1.Items.Clear();
                 listBox2.Items.Clear();
                 listBox3.Items.Clear();
                 listBox4.Items.Clear();
                
                 //  MessageBox.Show(otvet);
                 JToken j = JToken.Parse(zapros.Get("https://api.vk.com/method/audio.search?q=" + textBox1.Text + "&count=100" + "&auto_complete=1" + "&access_token=" + autset.Default.token));
                 try
                 {
                     fs = Enumerable.Skip(j["response"].Children(), 1).Select(c => c.ToObject<FindSong>()).ToList();

                     this.Invoke((MethodInvoker)delegate
                     {
                         if (fs.Count == 0)
                         {
                             listBox1.Items.Add("Ничего не найдено...");
                         }
                         else
                         {
                             for (int i = 0; i < fs.Count; i++)
                             {
                                 listBox1.Items.Add(fs[i].artist + " - " + fs[i].title);
                                 listBox2.Items.Add(fs[i].url);
                                 listBox3.Items.Add(fs[i].aid);
                                 listBox4.Items.Add(fs[i].owner_id);
                             }
                             listBox1.SelectedIndex = 0;
                             listBox2.SelectedIndex = 0;
                             listBox3.SelectedIndex = 0;
                             listBox4.SelectedIndex = 0;

                         }
                     });
                 }
                 catch (Exception ex)
                 { MessageBox.Show(ex.Message); }
             }
             else
             {
                 textBox1.Enabled = false;
                 
             }
         }

         [System.Runtime.InteropServices.DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
         public static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength); 
        
        private static  void сменитьToolStripMenuItem_Click(object sender, EventArgs e)
         {
              
         }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void аудиозаписиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            аудиозаписиToolStripMenuItem.Checked = true;
            поискToolStripMenuItem.Checked = false;
            
        }

        private void поискToolStripMenuItem_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
            аудиозаписиToolStripMenuItem.Checked = false;
            поискToolStripMenuItem.Checked = true;
        }

        private void сменитьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Form2 f = new Form2();
           
          
            
            f.Show();
            f.webBrowser1.Navigate("https://login.vk.com/?act=logout&hash=13957cfe28ae496971&_origin=http://vk.com");
            f.button1.PerformClick();
            /* listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            listBox5.Items.Clear();
            System.Diagnostics.Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 255");
            Form2 f = new Form2();
            f.Show();*/
            
        }

        private void удалитьАудиозаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zapros.Get("https://api.vk.com/method/audio.delete?"+"audio_id="+listBox2.SelectedItem.ToString()+"&owner_id="+autset.Default.id+"&access_token="+autset.Default.id);
            Zagruzit();
        }
       
        


        }
    }

