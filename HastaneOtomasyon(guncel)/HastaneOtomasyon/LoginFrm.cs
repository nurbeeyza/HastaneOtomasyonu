using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HastaneOtomasyon
{
    public partial class LoginFrm : Form
    {
        public MainFrm mfrm = new MainFrm();
        DBManager DBAdapter = new DBManager();
        public LoginFrm()
        {
            InitializeComponent();
        }

        private void label3_MouseHover(object sender, EventArgs e)
        {
            label3.ForeColor = Color.Blue;
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            label3.ForeColor = Color.Black;
        }
        public void ShowForm(int kulno=0,int mode=0)
        {
            mfrm.personelno = kulno;
            mfrm.mode = mode; 
            this.Hide();
            mfrm.Show();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            (int,int) a = DBAdapter.KullaniciGirisi(textBox1.Text, textBox2.Text);
            int pno = a.Item1;
            int unvno = a.Item2;
            //Eğer unvno=0 gelirse 'admin' unvno=1 gelirse 'sekreter' unvno>1 gelirse 'doktorların hepsi'
            //anlamına gelmektedir.            
            if (unvno==0)
            {
                //ShowForm(personelno,mode) fonksiyonu yetki numarasını mform'a gönderir
                //bu numara ile yetki anlaşılabildiği için kullanıcının
                //yetkisine göre modülleri gösterebiliriz.
                //3:admin 2:sekreter 1:doktorların hepsi olarak belirlendi
                ShowForm(-1,3); 
            }
            else if(unvno==1)
            {
                ShowForm(pno,2);
            }
            else if (unvno > 1)
            {
                ShowForm(pno,1);
            }
            else
            {
                MessageBox.Show("Girdiğiniz bilgiler hatalıdır. Lütfen doğru girdiğinizden emin olunuz.");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Admin girişi yapmalısınız.");
            textBox1.Text = "admin";
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void LoginFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
