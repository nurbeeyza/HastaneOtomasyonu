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
    public partial class TestFrm : Form
    {
        TestDb tdb = new TestDb();
       
        public TestFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {           
            if(tdb.dbCheck()==true)
            {
                this.Hide();
                LoginFrm lgfrm = new LoginFrm();
                lgfrm.Show();
            }
            else
            {
                if(tdb.ConnectionCheck(textBox1.Text))
                {
                    this.Hide();
                    LoginFrm lgfrm = new LoginFrm();
                    lgfrm.Show();
                }         
            }
        }
        private void TestFrm_Load(object sender, EventArgs e)
        {

        }
    }
}
