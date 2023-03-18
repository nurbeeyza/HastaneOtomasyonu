using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HastaneOtomasyon
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Database test classımızı oluşturuyoruz 
            TestDb tdb = new TestDb();
            //Eğer server adı doğruysa database'e ulaşabiliyoruz demektir
            //login formunu gösteriyoruz
            if (tdb.dbCheck())
            {
                LoginFrm lfrm = new LoginFrm();
                Application.Run(lfrm);
            }
            //Eğer server bilgileri yoksa yada bir terslik varsa yeni 
            //doğru bilgilerin kaydı için testformu'nu açıyoruz.
            else
            {
                TestFrm frm = new TestFrm();
                Application.Run(frm);
            }
            
            
        }
    }
}
