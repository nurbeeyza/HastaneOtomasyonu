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
    public partial class TahlilSonuc : Form
    {
        public long hstno=0;
        public string hstad="-";
        public bool yetki = false;
        DBManager dbAdapter = new DBManager();

        public TahlilSonuc()
        {
            InitializeComponent();
        }

        private void TahlilSonuc_Load(object sender, EventArgs e)
        {
            label3.Text = hstad;
            this.Text = hstad + " ait tahlil sonuçları";
            dbAdapter.hastayagoretahlilgetir(hstno, comboBox1);
            if(yetki)
            {
                DenetimPaneli.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dbAdapter.tahlilekle(hstno, richTextBox1.Text);
            dbAdapter.hastayagoretahlilgetir(hstno, comboBox1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int tahlilno = 0;
            if(!int.TryParse(comboBox1.SelectedValue.ToString(),out tahlilno))
                return;
            dbAdapter.tahlildegistir(tahlilno, richTextBox1.Text);
            dbAdapter.hastayagoretahlilgetir(hstno, comboBox1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int tahlilno = 0;
            if (!int.TryParse(comboBox1.SelectedValue.ToString(), out tahlilno))
                return;
            dbAdapter.tahlilsil(tahlilno);
            dbAdapter.hastayagoretahlilgetir(hstno, comboBox1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tahlilno = 0;
            if (!int.TryParse(comboBox1.SelectedValue.ToString(), out tahlilno))
                return;
            richTextBox1.Text=dbAdapter.tahlilgetir(tahlilno);
        }
    }
}
