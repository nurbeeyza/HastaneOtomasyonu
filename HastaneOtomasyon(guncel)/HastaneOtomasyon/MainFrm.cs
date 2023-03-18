using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SelectPdf;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.IO;
using System.Diagnostics;

namespace HastaneOtomasyon
{
    public partial class MainFrm : Form
    {
        public int mode=0;
        public int personelno = 0;
        DBManager dbAdapter = new DBManager();
       
        
        tablo[] bt,dt;

        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            //önce ne kadar bölüm varsa onların hepsinin id
            //ve adlarını getiriyoruz

            bt = dbAdapter.bolumgetir();
            dt = dbAdapter.doktorgetir();
            //Bölüm,Ünvan ve personel bilgisini ilgili combobox'lara getiriyoruz
            dbAdapter.bolumgetir(comboBox6);
            dbAdapter.personelgetir(comboBox8);
            dbAdapter.unvangetir(comboBox5);
            string personelad = "Yönetici";
            //Aşağıda giriş yapan kullanıcının yetkisine göre
            //hangi modüller gizelenecek onları ayarlıyoruz
            //3:admin 2:sekreter 1:doktorların hepsi
            switch (mode)
            {
                case 1:
                    personelad = dbAdapter.personeladigetir(personelno);
                    
                    tabControl1.TabPages.Remove(tabPage1);
                    tabControl1.TabPages.Remove(tabPage5);
                    tabControl1.TabPages.Remove(tabPage6);
                    break;
                case 2:
                    personelad = dbAdapter.personeladigetir(personelno);
                    tabControl1.TabPages.Remove(tabPage3);
                    tabControl1.TabPages.Remove(tabPage6);
                    break;
                default:
                    this.Text = "Hastane Otomasyonu";
                    break;
                    }
            this.Text = String.Format("Hoşgeldiniz: {0}", personelad);
        }

        private void MainFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        public bool alankontrol()
        {
            //Önemli alanların doldurulmamasını engellemek için TCNo,Ad,Yaş,
            //e-mail adresini check ediyoruz hatalıysa return true
            //değeri döndürüyor yoksa false döndürüyor
            if (!IsNumeric(TCNo.Text))
                return true;
            if (!IsNumeric(yastxt.Text))
                return true;
            if (!email.Text.Contains('@'))
                return true;
            if (adi.Text.Length < 3)
                return true;
            return false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //Alan kontrol kodumuz çalışıyor sıkıntı varsa true değeri
            //döndürüyor mesaj verdikten sonra return ile fonksiyon sonlanıyor
            if (alankontrol())
            {
                dbAdapter.Mesaj("Lütfen alanların doğru doldurduğunuzdan emin olunuz.", "Hata!");
                return;
            }
            long hastano = Convert.ToInt64(TCNo.Text);
            if (dbAdapter.HastaNoSorgula(hastano))
            {
                MessageBox.Show("Aynı TCNo/YbNo ait hasta kaydı mevcut", "hata");
                return;
            }
            byte yas= Convert.ToByte(yastxt.Text);
            
            dbAdapter.HastaEkle(hastano, adi.Text, soyadi.Text, yas,
                    cinsiyet.Text, richTextBox7.Text, uyruk.Text, email.Text, richTextBox1.Text, richTextBox2.Text, telno.Text, ceptelno.Text);
        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (alankontrol())
            {
                dbAdapter.Mesaj("Lütfen alanların doğru doldurduğunuzdan emin olunuz.", "Hata!");
                return;
            }
            dbAdapter.HastaGuncelle(Convert.ToInt64(TCNo.Text), adi.Text, soyadi.Text, Convert.ToByte(yastxt.Text),
                  cinsiyet.Text,richTextBox7.Text, uyruk.Text, email.Text, richTextBox1.Text, richTextBox2.Text, telno.Text, ceptelno.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dbAdapter.HastaSil(Convert.ToInt64(TCNo.Text));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            long hstno = Convert.ToInt64(TCNo.Text);
            string[] s;
            if (TCNo.Text == "")
            {
                button20.Enabled = false;
                return;
            }
            s=dbAdapter.HastaGetir(hstno);
            adi.Text=s[0];
            soyadi.Text=s[1];
            yastxt.Text=s[2];
            cinsiyet.Text = s[3];
            uyruk.Text=s[4];
            richTextBox7.Text = s[5];
            richTextBox1.Text=s[9];
            richTextBox2.Text = s[10];
            telno.Text=s[6];    
            ceptelno.Text=s[7];
            email.Text = s[8];
            if (adi.Text.Length<1)
            {
                button20.Enabled = false;
                return;
            }
            button20.Enabled = true;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (TCNo2.Text == "")
            {
                button16.Enabled = false;
                button15.Enabled = false;
                button14.Enabled = false;
                return;
            }
            long hstno = Convert.ToInt64(TCNo2.Text);
            AdiSoyadi.Text = dbAdapter.HastaAdiGetir(hstno);
            if (AdiSoyadi.Text.Length<1)
            {
                button16.Enabled = false;
                button15.Enabled = false;
                button14.Enabled = false;
                return;
            }
            dbAdapter.RandevuBilgisiGetir(hstno,comboBox9);
            button16.Enabled = true;
            button15.Enabled = true;
            button14.Enabled = true;
        }
        public void GrafikAlaniHazirla(Chart ct,string grafikadi)
        {
            ct.Series.Clear();
            ct.Legends.Clear();
            ct.Legends.Add(grafikadi);
            ct.Legends[0].LegendStyle = LegendStyle.Table;
            ct.Legends[0].Docking = Docking.Bottom;
            ct.Legends[0].Alignment = StringAlignment.Center;
            ct.Legends[0].Title = grafikadi;
            ct.Legends[0].BorderColor = Color.Black;            
        }
        public void GrafikBolumSeriDoldur(Chart ct)
        {
          
            //Grafiğimizin türünü bar yapıyoruz
            long hastasayısı;
            //Aşağıdaki döngü herbir bölüme göre teker teker
            //hasta sayısını hesaplar ve grafiğe verileri ekler
            for (int i = 0; i < bt.Length; i++)
            {
                hastasayısı = dbAdapter.bolumdoluluk((byte)bt[i].id);
                //Grafiğimize yeni seri ekliyoruz
                ct.Series.Add(bt[i].adi);
                ct.Series[bt[i].adi].ChartType = SeriesChartType.Column;
               ct.Series[bt[i].adi].Points.AddXY(bt[i].adi, hastasayısı);
            }
        }
        public void GrafikDrSeriDoldur(Chart ct)
        {

            //Grafiğimizin türünü bar yapıyoruz
            long hastasayısı;
            //Aşağıdaki döngü herbir doktora göre teker teker
            //hasta sayısını hesaplar ve grafiğe verileri ekler
            string seriadi;
            for (int i = 0; i < dt.Length; i++)
            {
                hastasayısı = dbAdapter.doktordoluluk(dt[i].id);
                seriadi = (i+1).ToString() + ": " + dt[i].adi;
                ct.Series.Add(seriadi);
                ct.Series[seriadi].ChartType = SeriesChartType.Column;
                ct.Series[seriadi].Points.AddXY(dt[i].adi, hastasayısı);
            }
        }
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPage2)
            {
                dbAdapter.bolumgetir(comboBox2);
            }
            else if (e.TabPage == tabPage4)
            {
               if(mode==1||mode==2)
                {
                    string[] s = dbAdapter.personelgetir(personelno);
                    textBox2.Text = s[0];
                    textBox1.Text = s[1];
                    textBox7.Text = s[2];
                    maskedTextBox2.Text = s[3];
                }
               else
                {
                    textBox2.Text = "admin";
                }
            }
            else if (e.TabPage == tabPage6)
            {
                //önce bölüm-hastasayısı grafiğini chart1'e çizdiriyoruz
                GrafikAlaniHazirla(chart1, "Bölüm-Hastasayısı");
                GrafikBolumSeriDoldur(chart1);
                //sonra dr-hastasayısı grafiğini chart2'e ekleyelim
                GrafikAlaniHazirla(chart2, "Doktor-Hastasayısı");
                GrafikDrSeriDoldur(chart2);
               
            }

        }
       

       
        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pno=0;
            if (!int.TryParse(comboBox8.SelectedValue.ToString(), out pno))
                return;
            string[] s = dbAdapter.personelbilgigetir(pno);
            textBox13.Text = pno.ToString();
            textBox4.Text = s[0];
            textBox3.Text = s[1];
            textBox6.Text = s[2];
            maskedTextBox1.Text = s[3];
            comboBox5.SelectedValue=s[4];
            comboBox6.SelectedValue = s[5];
        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (dbAdapter.personelkontrol(textBox6.Text))
            {
                dbAdapter.Mesaj("Aynı E-mail adresine sahip personel mevcuttur farklı email adresi yazınız", "Hata");
                return;
            }
            byte unvno= Convert.ToByte(comboBox5.SelectedValue.ToString());
            byte bolno = Convert.ToByte(comboBox6.SelectedValue.ToString());
            dbAdapter.PersonelEkle(textBox4.Text, textBox3.Text, textBox6.Text, maskedTextBox1.Text, unvno, bolno);
            dbAdapter.personelgetir(comboBox8);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (dbAdapter.personelkontrol(textBox6.Text))
            {
                dbAdapter.Mesaj("Aynı E-mail adresine sahip personel mevcuttur farklı email adresi yazınız", "Hata");
                return;
            }
            int pno= Convert.ToInt32(comboBox8.SelectedValue.ToString());
            byte unvno = Convert.ToByte(comboBox5.SelectedValue.ToString());
            byte bolno = Convert.ToByte(comboBox6.SelectedValue.ToString());
            dbAdapter.PersonelDuzenle(pno,textBox4.Text, textBox3.Text, textBox6.Text, maskedTextBox1.Text, unvno, bolno);
            dbAdapter.personelgetir(comboBox8);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            int pno = Convert.ToByte(comboBox8.SelectedValue.ToString());
            dbAdapter.PersonelSil(pno);
            dbAdapter.personelgetir(comboBox8);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if(dbAdapter.personelkontrol(textBox7.Text))
            {
                dbAdapter.Mesaj("Aynı E-mail adresine sahip personel mevcuttur farklı email adresi yazınız", "Hata");
                return;
            }
            if(dbAdapter.Mesaj("Bilgileriniz Güncellensinmi?","Uyarı",MessageBoxButtons.YesNo)
                ==DialogResult.Yes)
            if (personelno!=-1&&textBox1.Text == textBox5.Text)
            {
                dbAdapter.PersonelBilgiDüzenle(personelno, textBox2.Text, textBox1.Text, textBox7.Text, maskedTextBox2.Text);
            }
            else
            {
                dbAdapter.Mesaj("Yazdığınız şifreniz eşleşmiyor");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (TCNo3.Text == "")
            {
                button18.Enabled = false;
                button13.Enabled = false;
                button19.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
                button10.Enabled = false;
                button11.Enabled = false;
                button12.Enabled = false;
                return;
            }
            
            try
            {
                long hastano = Convert.ToInt64(TCNo3.Text);
                String[] res = dbAdapter.HastaBilgiGetir(hastano);
                textBox12.Text = res[0];
                textBox11.Text = res[1];
                textBox10.Text = res[2];
                textBox9.Text = res[3];
                richTextBox4.Text = res[4];
                richTextBox3.Text = res[5];
                textBox8.Text = res[6];
                comboBox1.Text = "";
                comboBox4.Text = "";
                richTextBox5.Text ="";
                richTextBox6.Text ="";
                dbAdapter.tedavigetir(hastano, comboBox1);
                dbAdapter.recetegetir(hastano, comboBox4);
                button18.Enabled = true;
                button13.Enabled = true;
                button19.Enabled = true;
                button7.Enabled = true;
                button8.Enabled = true;
                button9.Enabled = true;
                button10.Enabled = true;
                button11.Enabled = true;
                button12.Enabled = true;
            }
            catch (Exception ex)
            {
                dbAdapter.Mesaj(ex.Message);
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            long hastano = Convert.ToInt64(TCNo3.Text);
            dbAdapter.tedaviekle(hastano, richTextBox5.Text);
            dbAdapter.tedavigetir(hastano, comboBox1);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            long hastano = Convert.ToInt64(TCNo3.Text);
            dbAdapter.receteekle(hastano, richTextBox6.Text);
            //dbAdapter.recetegetir() getir fonksiyonu aşırı yüklenmiş bir fonksiyondur
            //burada comboBox4'e veri getiriyor
            dbAdapter.recetegetir(hastano, comboBox4);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            long receteid = Convert.ToInt64(comboBox4.SelectedValue);
            dbAdapter.recetedegistir(receteid,  richTextBox6.Text);
            //dbAdapter.recetegetir() getir fonksiyonu aşırı yüklenmiş bir fonksiyondur
            //burada richTextBox6'ya veri getiriyor
            richTextBox6.Text=dbAdapter.recetegetir(receteid);
            long hastano = Convert.ToInt64(TCNo3.Text);
            dbAdapter.recetegetir(hastano, comboBox4);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            long receteid = Convert.ToInt64(comboBox4.SelectedValue);
            long hastano = Convert.ToInt64(TCNo3.Text);
            dbAdapter.recetesil(receteid);
            dbAdapter.recetegetir(hastano, comboBox4);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            long tedaviid = 0;
            if (!long.TryParse(comboBox1.SelectedValue.ToString(),out tedaviid))
                return;
            richTextBox5.Text = dbAdapter.tedavigetir(tedaviid);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            long hastano = Convert.ToInt64(TCNo3.Text);
            long tedaviid = Convert.ToInt64(comboBox1.SelectedValue);
            dbAdapter.tedavidegistir(tedaviid, richTextBox5.Text);
            richTextBox5.Text = dbAdapter.tedavigetir(tedaviid);
            dbAdapter.tedavigetir(hastano, comboBox1);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            long hastano = Convert.ToInt64(TCNo3.Text);
            long tedaviid = Convert.ToInt64(comboBox1.SelectedValue);
            dbAdapter.tedavisil(tedaviid);
            dbAdapter.tedavigetir(hastano, comboBox1);
        }
        public void EMailGonder2(string emailAddress)
        {
            string pdfpath = Application.StartupPath + @"\HastaBilgi.pdf";
            if (!File.Exists(pdfpath))
            {
                MessageBox.Show("Hata");
                return;
            }
            Process.Start("mailto:" + emailAddress + "?subject=Hasta Bilgilendirme &body=Bilgileriniz ektedir sağlıklı günler dileriz.&Attach="
               + pdfpath);
        }
        public void EMailGonder(string emailAddress)
        {
            string pdfpath = Application.StartupPath + @"\HastaBilgi.pdf";
            if (!File.Exists(pdfpath))
            {
                MessageBox.Show("Hata");
                return;
            }
            // Bir Outlook uygulaması oluşturun
            Outlook.Application application = new Outlook.Application();

            // Yeni bir e-posta oluşturun
            Outlook.MailItem mail = (Outlook.MailItem)application.CreateItem(Outlook.OlItemType.olMailItem);

            // E-posta bilgilerini ayarlayın
            mail.To = emailAddress;
            mail.Subject = "Hastane Otomasyonu Bilgilendirme";
            mail.Body = "Merhaba, talebiniz üzerine bilgileriniz pdf olarak ekte tarafınıza iletilmiştir.";

            // Dosya ekleyin
            mail.Attachments.Add(pdfpath, Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);

            // E-postayı gönderin
            mail.Send();          
        }
        private void button13_Click(object sender, EventArgs e)
        {
            if (textBox8.Text.Contains('@'))
                EMailGonder(textBox8.Text);
            else
                MessageBox.Show("Geçerli bir E-mail adresi olmalıdır.","Hata");
        }
       
        private void comboBox1_MouseDown(object sender, MouseEventArgs e)
        {
         }

        private void comboBox4_MouseDown(object sender, MouseEventArgs e)
        {
         }

        private void button16_Click(object sender, EventArgs e)
        {
            long hstno = Convert.ToInt64(TCNo2.Text);
            int drno=0;
            if(!int.TryParse(comboBox3.SelectedValue.ToString(),out drno))
                return;
            dbAdapter.RandevuKaydet(hstno, drno, dateTimePicker1.Value.ToShortDateString(),
                listBox1.SelectedItem.ToString());
            dbAdapter.RandevuBilgisiGetir(hstno, comboBox9);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TCNo2.Text numara almazsa return ile fonnksiyon sonlanır.
            if (!IsNumeric(TCNo2.Text) || TCNo2.Text=="")
                return;
            long hstno = Convert.ToInt64(TCNo2.Text);
            int drno = 0;
            if(!int.TryParse(comboBox3.SelectedValue.ToString(),out drno))
                return;
            dbAdapter.RandevuDurumuSorgula(dateTimePicker1.Value.ToShortDateString(), drno, listBox1);
            dbAdapter.RandevuBilgisiGetir(hstno, drno, comboBox9);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Byte bolumno = 0;
            if(comboBox2.SelectedValue!=null)
            if(!Byte.TryParse(comboBox2.SelectedValue.ToString(),out bolumno))
                return;
            comboBox3.Text = "";
            dbAdapter.doktorgetir(bolumno, comboBox3);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            long rndno=0;
            long hstno= Convert.ToInt64(TCNo2.Text);
            //comboBox9'dan veri çekemezsek fonsiyon return ile sonlanıyor
            if (!long.TryParse(comboBox9.SelectedValue.ToString(),out rndno))
                return;
            dbAdapter.RandevuGuncelle(rndno, hstno, dateTimePicker1.Value.ToShortDateString(),
                listBox1.SelectedItem.ToString());
            int drno = 0;
            if (!int.TryParse(comboBox3.SelectedValue.ToString(), out drno))
                return;
            dbAdapter.RandevuBilgisiGetir(hstno, drno, comboBox9);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            long rndno = 0;
            //comboBox9'dan veri çekemezsek fonsiyon return ile sonlanıyor
            if (!long.TryParse(comboBox9.SelectedValue.ToString(), out rndno))
                return;
            dbAdapter.RandevuSil(rndno);
            long hstno = Convert.ToInt64(TCNo2.Text);
            dbAdapter.RandevuBilgisiGetir(hstno, comboBox9);
        }

        private void comboBox8_Click(object sender, EventArgs e)
        {
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.PasswordChar = checkBox2.Checked ? '\0' : '*';
            textBox1.PasswordChar = checkBox2.Checked ? '\0' : '*';
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            long receteid = 0;
            if(!long.TryParse(comboBox4.SelectedValue.ToString(),out receteid))
                return;
            richTextBox6.Text = dbAdapter.recetegetir(receteid);

        }

        private void button18_Click(object sender, EventArgs e)
        {    
            button6_Click(sender, e); 
            if (TCNo3.Text == "")
                return;
            TahlilSonuc thfrm = new TahlilSonuc();
            thfrm.hstno = Convert.ToInt64(TCNo3.Text);
            thfrm.hstad = textBox12.Text + " " + textBox11.Text;
            thfrm.Show();
        }

        public string pdfformdoldur(string HstNo,string HstAd,string yas,string cinsiyet,
            string tedavi,string recete)
        {
            string s = "Bilgi üretilemedi";
            if(File.Exists("template.html"))
            {
                string content = File.ReadAllText("template.html");
                s = String.Format(content, HstNo, HstAd, yas, cinsiyet, tedavi, recete);
            }
            return s;
        }
        private void button19_Click(object sender, EventArgs e)
        {
            string hstadsoyad = textBox12.Text + " " + textBox11.Text;
            string htmlcont = pdfformdoldur(TCNo3.Text, hstadsoyad, textBox10.Text, textBox9.Text,
                richTextBox5.Text, richTextBox6.Text);
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.MarginTop = 10;
            converter.Options.MarginBottom = 10;
            converter.Options.MarginLeft = 40;
            converter.Options.MarginRight = 10;
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 100;
            converter.Options.WebPageWidth = 512;
            converter.Options.WebPageHeight = 0;
            converter.Options.WebPageFixedSize = false;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.ShrinkOnly;
            converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.NoAdjustment;

            PdfDocument doc = converter.ConvertHtmlString(htmlcont);
            doc.Save("HastaBilgi.pdf");

            // close pdf document
            doc.Close();
            Process.Start("HastaBilgi.pdf");
        }

        private void button20_Click(object sender, EventArgs e)
        {
            button1_Click(sender,e);
            if (TCNo.Text == "")
                return;
            TahlilSonuc thfrm = new TahlilSonuc();
            thfrm.hstno = Convert.ToInt64(TCNo.Text);
            thfrm.hstad = adi.Text + " " + soyadi.Text;
            //Sekreter tahlil sonuçlarını girer
            //Bu yüzden sekreter yekilendiriliyor ve öyle
            //Tahliller ekranı getiriliyor
            thfrm.yetki = true;
            thfrm.Show();
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void TCNo2_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void TCNo2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button5_Click(sender, e);
        }

        private void TCNo3_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void TCNo3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button6_Click(sender, e);
        }

        private void TCNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(sender, e);
        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }
    }
}
