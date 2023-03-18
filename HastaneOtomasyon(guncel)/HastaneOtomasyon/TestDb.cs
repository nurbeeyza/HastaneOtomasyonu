using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace HastaneOtomasyon
{
    internal class TestDb
    {
        SqlConnection con;
        public bool ConnectionCheck(string srvName = "")
        {
            con = new SqlConnection(@"server=" + srvName + ";initial catalog=master;integrated security=yes");
            con.Open();
            if (con.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    //Otomatik database oluşturuyoruz
                    string dbgen = "DROP DATABASE IF EXISTS KlinikDb;"+
                        "CREATE DATABASE KlinikDb;";
                    SqlCommand cmd = new SqlCommand(dbgen, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    //Otomatik tabloları oluşturuyoruz
                    con = new SqlConnection(@"server=" + srvName + ";initial catalog=KlinikDb;integrated security=yes");
                    //Tabloların oluşturma kodlarının olduğu dosyayı okuyoruz
                    string tblgen = File.ReadAllText("tblgen.gen",Encoding.UTF8);
                    cmd = new SqlCommand(tblgen, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    saveConnection(srvName);
                    return true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message,"Hata");
                    return false;
                }
            }
            else return false;
        }
        public void saveConnection(string srvname)
        {
            File.WriteAllText("dbdata.set", srvname);
         }

        public bool dbCheck()
        {
            //Database bağlantısı kaydı mevcut ise önceden
            //başarılı bir database kaydı yapılmış demektir 
            //bu kod direkt database kaydı işlemini atlar ve true değeri döndürerek.
            //diğer modülleri bilgilendirir
            //Böylece sürekli kullanıcının verileri girmesini isteyerek onu sıkmaz 
            //Eğer database kaydı yapılamışsa bu sefer bu kod
            //önce database bağlantısını sınar sonra başarılı olursa
            //database bilgilerini saklar ve programın diğer modülleri böylece 
            //çalışabilir.
            if (!File.Exists("dbdata.set"))
                return false;
            else
            {
                string s = File.ReadAllText("dbdata.set");
                con = new SqlConnection(@"server=" + s + ";initial catalog=KlinikDb;integrated security=yes");
                try
                {
                    con.Open();
                }
                catch (Exception)
                {
                    if (con.State != System.Data.ConnectionState.Open)
                        return false;
                    else
                    {
                        con.Close();
                        return true;
                    }
                }
            }
            return true;
        } 

    }

}
