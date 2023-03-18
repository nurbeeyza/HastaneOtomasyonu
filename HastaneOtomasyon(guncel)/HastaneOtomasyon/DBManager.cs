using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace HastaneOtomasyon
{    
    public struct tablo
    {
        public int id;
        public string adi;
    }
  
    internal class DBManager
    {
        SqlConnection con; 
        
        public DialogResult Mesaj(string text, string caption = "", MessageBoxButtons btn= MessageBoxButtons.OK)
        {
            return MessageBox.Show(text, caption,btn);
        }
        public DBManager(string path= "dbdata.set")
        {
            string[] s;
            if(File.Exists(path))
            {
                s=File.ReadAllLines(path);
                //ilk satırda server adı bilgisi yer alıyor onu okuyarak 
                //SqlConnection nesnemizi oluşturuyoruz.
                con = new SqlConnection(@"server=" + s[0] + ";initial catalog=KlinikDb;integrated security=yes");
            }
            else
            {
                //Eğer dosyada sıkıntı varsa program hata mesajı gönderiyor ve kapanıyor
                Mesaj("Server bilgilerini tutan dosyada hata var", "Hata");
                Application.Exit();
            }
        }
        public DataTable cbDoldur(String sql, ComboBox cb)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cb.DataSource = dt;
                cb.ValueMember = dt.Columns[0].ColumnName;
                cb.DisplayMember = dt.Columns[1].ColumnName;
                return dt;
            }
            catch(Exception ex)
            {
                Mesaj(ex.Message,"Hata oluştu:");
                return null;
            }
        }
        public void lbDoldur(String sql, ListBox lb)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                lb.DataSource = dt;
                lb.ValueMember = dt.Columns[0].ColumnName;
                lb.DisplayMember = dt.Columns[1].ColumnName;
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata oluştu:");
            }
        }
        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }
       
        //Birden fazla değikeni return etmek mümkün bunu
        //(int,int,...) şeklinde belirtiyoruz buna tuple deniliyor
        public (int,int) KullaniciGirisi(string variable,string sifre)
        {
            string sql;
            SqlCommand cmd;
            SqlDataReader sdr;
            int unvanno = -1;
            int pno = 0;
            //Varsayılan olarak adminin şifresi 0000 olarak belirlendi
            if (variable.ToLower() == "admin" && sifre == "0000")
                return (0,0);

            con.Open();
            if (variable.Contains('@'))
            {
                sql = "SELECT perno,unvanno FROM personel WHERE email=@email AND sifre=@sifre;";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@email", variable);
                cmd.Parameters.AddWithValue("@sifre", sifre);
                sdr = cmd.ExecuteReader();
                if(sdr.Read())
                {
                    pno = sdr.GetInt32(0);
                    unvanno = sdr.GetByte(1);
                }                           
            }
            else if(IsNumeric(variable))
            {
                sql = "SELECT perno,unvanno FROM personel WHERE perno=@perno AND sifre=@sifre;";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@perno", variable);
                cmd.Parameters.AddWithValue("@sifre", sifre);
                sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    pno = sdr.GetInt32(0);
                    unvanno = sdr.GetByte(1);
                }              
            }
            con.Close();
            return (pno,unvanno);
        }
        public void HastaEkle(Int64 hstno,string adi,string soyadi,byte yas, string cinsiyet, string adres, string uyruk,string email
            ,string hstliklar,string ilaclar,string telno,string ctelno)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "INSERT INTO hastatbl(hastano, adi, soyadi, yas,cinsiyet,adres, uyruk, email, hastaliklar, kulilaclar,telno,ceptelno)" +
                    " values(@hstno,@adi,@sadi,@yas,@cinsiyet,@adres,@uyruk,@email,@hstliklar,@kulilaclar,@telno,@ceptelno)";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@hstno", hstno);
                cmd.Parameters.AddWithValue("@adi", adi);
                cmd.Parameters.AddWithValue("@sadi", soyadi);
                cmd.Parameters.AddWithValue("@yas", yas);
                cmd.Parameters.AddWithValue("@cinsiyet", cinsiyet);
                cmd.Parameters.AddWithValue("@adres", adres);
                cmd.Parameters.AddWithValue("@uyruk", uyruk);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@hstliklar", hstliklar);
                cmd.Parameters.AddWithValue("@kulilaclar", ilaclar);
                cmd.Parameters.AddWithValue("@telno", telno);
                cmd.Parameters.AddWithValue("@ceptelno", ctelno);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Hasta kaydı başarılı", "Bilgi");
            }
            catch(Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void HastaGuncelle(Int64 hstno, string adi, string soyadi, byte yas, string cinsiyet, string adres, string uyruk, string email
            , string hstliklar, string ilaclar, string telno, string ctelno)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "UPDATE hastatbl SET adi=@adi, soyadi=@sadi, yas=@yas, cinsiyet=@cinsiyet,adres=@adres,uyruk=@uyruk, email=@email" +
                    ", hastaliklar=@hstliklar, kulilaclar=@kulilaclar,telno=@telno,ceptelno=@ceptelno WHERE hastano=@hstno";

                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@hstno", hstno);
                cmd.Parameters.AddWithValue("@adi", adi);
                cmd.Parameters.AddWithValue("@sadi", soyadi);
                cmd.Parameters.AddWithValue("@yas", yas);
                cmd.Parameters.AddWithValue("@cinsiyet", cinsiyet);
                cmd.Parameters.AddWithValue("@adres", adres);
                cmd.Parameters.AddWithValue("@uyruk", uyruk);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@hstliklar", hstliklar);
                cmd.Parameters.AddWithValue("@kulilaclar", ilaclar);
                cmd.Parameters.AddWithValue("@telno", telno);
                cmd.Parameters.AddWithValue("@ceptelno", ctelno);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Hasta kaydı güncellemesi başarılı", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void HastaSil(Int64 hstno)
        {
            String sql;
            SqlCommand cmd;
            if(MessageBox.Show("Hasta bilgilerini silmek istediğinizden eminmisiniz?"
                ,"Uyarı",MessageBoxButtons.YesNoCancel)
                ==DialogResult.Yes)
            try
            {
                con.Open();
                sql = "DELETE FROM hastatbl WHERE hastano=@hstno";

                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@hstno", hstno);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Hasta kaydı silinmiştir", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }           
        }
        public String[] HastaGetir(Int64 hastano)
        {
            String s = "SELECT adi,soyadi,yas,cinsiyet,uyruk,adres,telno,ceptelno,email,hastaliklar,kulilaclar FROM hastatbl WHERE hastano=@hstno";
            String[] res = new string[11];
            SqlCommand cmd = new SqlCommand(s, con);
            cmd.Parameters.AddWithValue("@hstno", hastano);
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            res[0] = "";
            if (sdr.Read())
            {
                res[0] = sdr.GetString(0);
                res[1] = sdr.GetString(1);  
                res[2] = sdr.GetByte(2).ToString();
                res[3] = sdr.GetString(3);
                res[4] = sdr.GetString(4);                    
                res[5] = sdr.GetString(5);
                res[6] = sdr.GetString(6);
                res[7] = sdr.GetString(7);
                res[8] = sdr.GetString(8);
                res[9] = sdr.GetString(9);
                res[10] = sdr.GetString(10);
            }
            sdr.Close();
            con.Close();
            if (res[0] == "")
                Mesaj("Bu numaraya sahip hasta bulunamadı!", "Bilgi");

            return res;
        }
        public String[] HastaBilgiGetir(Int64 hastano)
        {
            String s = "SELECT adi,soyadi,yas,cinsiyet,hastaliklar,kulilaclar,email FROM hastatbl WHERE hastano=@hstno";
            String[] res = new string[7];
            SqlCommand cmd = new SqlCommand(s, con);
            cmd.Parameters.AddWithValue("@hstno", hastano);
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            res[0] = "";
            if (sdr.Read())
            {
                res[0] = sdr.GetString(0);
                res[1] = sdr.GetString(1);
                res[2] = sdr.GetByte(2).ToString();
                res[3] = sdr.GetString(3);
                res[4] = sdr.GetString(4);
                res[5] = sdr.GetString(5);
                res[6] = sdr.GetString(6);
            }
            sdr.Close();
            con.Close();
            if (res[0] == "")
                Mesaj("Bu numaraya sahip hasta bulunamadı!", "Bilgi");

            return res;
        }
        public String HastaAdiGetir(Int64 hastano)
        {
            String s = "SELECT CONCAT(adi,' ',soyadi) as adsoyad FROM hastatbl WHERE hastano=@hstno";
            String res = "";
            SqlCommand cmd = new SqlCommand(s, con);
            cmd.Parameters.AddWithValue("@hstno", hastano);
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {
                res = sdr.GetString(0);
            }
            sdr.Close();
            con.Close();
            if (res == "")
                Mesaj("Bu numaraya sahip hasta bulunamadı!", "Bilgi");
            return res;
        }
        public bool HastaNoSorgula(long hstno)
        {
            String s = "SELECT adi FROM hastatbl WHERE hastano=@hstno";
            String res = "";
            SqlCommand cmd = new SqlCommand(s, con);
            cmd.Parameters.AddWithValue("@hstno", hstno);
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {
                res = sdr.GetString(0);
            }
            sdr.Close();
            con.Close();
            if (res != "")
                return true;
            else
                return false;
        }
        public void RandevuDurumuSorgula(string randevutarihi,int doktorno,ListBox lb)
        {
            //Seçili doktorumuzun mesai saatleridir 
            //Bu kısmı istediğiniz gibi yazabilirsiniz
            string[] saatler = { "8:30","9:00","9:30","10:00"
                    ,"10:30","11:00","11:30","13:30","14:00",
                    "14:30","15:00","15:30","16:00" };

            string s = "SELECT randevusaati FROM randevutbl " +
                "WHERE doktorno = @drno AND randevutarih = @rndtar;";
            SqlDataReader sdr;
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@drno", doktorno);
                cmd.Parameters.AddWithValue("@rndtar", randevutarihi);
                sdr = cmd.ExecuteReader();
                int i =0;
                lb.Items.Clear();
                while (sdr.Read())
                {
                    for (int j = 0; j < saatler.Length; j++)
                    {
                        if (saatler[j] != sdr.GetString(0))
                        {
                            lb.Items.Add(saatler[j]);
                        }
                    }
                    i++;
                }
                if (i ==0)
                    lb.Items.AddRange(saatler);
                sdr.Close();
                con.Close();
            }
            catch(Exception ex)
            {             
                Mesaj(ex.Message, "Hata");
            }
       
        }
        public void RandevuBilgisiGetir(long hstno,int drno,ComboBox cb)
        {
            String sql = String.Format("SELECT randevuno,concat(randevutarih,' ',randevusaati) as rndadi" +
                " FROM randevutbl WHERE hastano={0} AND doktorno={1}",hstno,drno);
            cbDoldur(sql, cb);
        }
        public void RandevuBilgisiGetir(long hstno, ComboBox cb)
        {
            String sql = String.Format("SELECT randevuno,concat(randevutarih,' ',randevusaati) as rndadi" +
                " FROM randevutbl WHERE hastano={0}", hstno);
            cbDoldur(sql, cb);
        }
        public int[] RandevuBilgisiGetir(int randevuno)
        {
            SqlCommand cmd;
            int[] res = new int[2];
            string s = "SELECT r.doktorno,p.unvanno FROM randevutbl as r,personel as p" +
                " WHERE p.perno=r.doktorno AND r.randevuno=@rndno;";
            try
            {
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@rndno", randevuno);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    res[0] = sdr.GetInt32(0);
                    res[1] = sdr.GetByte(1);
                }
                sdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message);
            }
            return res;
        }
        public void RandevuKaydet(long hstno,int drno,string rndtar,string rndsaat)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "INSERT INTO randevutbl (hastano,doktorno,randevutarih,randevusaati)" +
                    " VALUES(@hstno,@drno,@rndtar,@rndsaat);";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@hstno", hstno);
                cmd.Parameters.AddWithValue("@drno", drno);
                cmd.Parameters.AddWithValue("@rndtar", rndtar);
                cmd.Parameters.AddWithValue("@rndsaat", rndsaat);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Randevu Bilgisi kaydedildi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void RandevuGuncelle(long rndno,long hstno,string rndtar, string rndsaat)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "UPDATE randevutbl SET hastano=@hstno,randevutarih=@rndtar,randevusaati=@rndsaat" +
                    " WHERE randevuno=@rndno";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@rndno", rndno);
                cmd.Parameters.AddWithValue("@hstno", hstno);
                cmd.Parameters.AddWithValue("@rndtar", rndtar);
                cmd.Parameters.AddWithValue("@rndsaat", rndsaat);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Randevu Bilgisi Güncellendi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void RandevuSil(long rndno)
        {
            String sql;
            SqlCommand cmd;
            if(Mesaj("Geçerli Randevu Bilgisi silinsin mi?","Uyarı",MessageBoxButtons.YesNoCancel)==DialogResult.Yes)
            try
            {
                con.Open();
                sql = "DELETE FROM randevutbl WHERE randevuno=@rndno";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@rndno", rndno);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Randevu Bilgisi Silindi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void tedavigetir(long hastano,ComboBox cb)
        {
            string s= "SELECT tedaviid,SUBSTRING(tedavi,1,20) AS adi FROM tedavitbl as t," +
                " hastatbl as h WHERE t.hastano=h.hastano AND t.hastano="+hastano.ToString()+";";
            try
            {
                cbDoldur(s, cb);
            }
            catch(Exception ex)
            {
                Mesaj(ex.Message);
                return;
            }
        }
        public void hastayagoretahlilgetir(long hstno,ComboBox cb)
        {
            String s = "SELECT tahlilno,tahlilno FROM tahliltbl WHERE hastano="+hstno.ToString();
            cbDoldur(s, cb);
        }
        public string tahlilgetir(int tahlilno)
        {
            SqlCommand cmd;
            string res = "Tahlil Kaydı Bulunamadı";
            string s = "SELECT tahlil FROM tahliltbl" +
                " WHERE tahlilno=@tahlilno;";
            try
            {
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@tahlilno", tahlilno);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    res = sdr.GetString(0);
                }
                sdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message);
            }
            return res;
        }
        public void tahlilekle(long hstno, string tahlil)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "INSERT INTO tahliltbl (hastano,tahlil)" +
                    " VALUES(@hstno,@tahlil);";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@hstno", hstno);
                cmd.Parameters.AddWithValue("@tahlil", tahlil);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Tahlil bilgisi kaydedildi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void tahlildegistir(int tahlilid, string tahlil)
        {
            String sql;
            SqlCommand cmd;
            if(Mesaj("Tahlil bilgilerini düzenlemek istediğinizden eminmisiniz?","Uyarı",MessageBoxButtons.YesNoCancel)==DialogResult.Yes)
            try
            {
                con.Open();
                sql = "UPDATE tahliltbl SET tahlil=@tahlil WHERE tahlilno=@tahlilno";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@tahlilno", tahlilid);
                cmd.Parameters.AddWithValue("@tahlil", tahlil);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Tahlil bilgisi düzenlendi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void tahlilsil(int tahlilid)
        {
            String sql;
            SqlCommand cmd;
            if (Mesaj("Tahlil bilgilerini silmek istediğinizden eminmisiniz?", "Uyarı", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                try
                {
                    con.Open();
                    sql = "DELETE FROM tahliltbl WHERE tahlilno=@tahlilid";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@tahlilno", tahlilid);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    Mesaj("Tahlil bilgisi silindi", "Bilgi");
                }
                catch (Exception ex)
                {
                    Mesaj(ex.Message, "Hata");
                }
        }
        public string tedavigetir(long tdvid)
        {
            SqlCommand cmd;
            string res = "Tedavi Kaydı Bulunamadı";
            string s = "SELECT tedavi FROM tedavitbl" +
                " WHERE tedaviid=@tdvid;";
            try
            {
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@tdvid", tdvid);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    res = sdr.GetString(0);
                }
                sdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message);
            }
            return res;
        }
        public void tedaviekle(long hstno, string tedavi)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "INSERT INTO tedavitbl (hastano,tedavi)" +
                    " VALUES(@hstno,@tedavi);";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@hstno", hstno);
                cmd.Parameters.AddWithValue("@tedavi", tedavi);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Tedavi kaydedildi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void tedavidegistir(long tedaviid,  string tedavi)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "UPDATE tedavitbl SET tedavi=@tedavi WHERE tedaviid=@tedaviid";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@tedaviid", tedaviid);
                cmd.Parameters.AddWithValue("@tedavi", tedavi);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Tedavi bilgileri değiştirildi.", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void tedavisil(long tedaviid)
        {
            String sql;
            SqlCommand cmd;
            if (Mesaj("Hastaya ait Tedavi Bilgisini Silmek İstediğinizden eminmisiniz?", "Uyarı", MessageBoxButtons.YesNoCancel)
              == DialogResult.Yes)
                try
            {
                con.Open();
                sql = "DELETE FROM tedavitbl WHERE tedaviid=@tedaviid";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@tedaviid", tedaviid);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Tedavi bilgisi silindi.", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void recetegetir(long hastano, ComboBox cb)
        {

            string s = "SELECT receteid,receteno FROM recetetbl" +
                " WHERE hastano=" + hastano.ToString() + ";";
            try
            {
                cbDoldur(s, cb);
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message);
                return;
            }
        }
        public string recetegetir(long receteid)
        {
            SqlCommand cmd;
            string res = "Reçete Kaydı Bulunamadı";
            string s = "SELECT recete FROM recetetbl" +
                " WHERE receteid=@receteid;";
            try
            {
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@receteid", receteid);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    res = sdr.GetString(0);
                }
                sdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message);
            }
            return res;
        }
        public void receteekle(long hstno,string recete)
        {
            String sql;
            SqlCommand cmd;
            Random rnd = new Random();
            int receteno = rnd.Next(100000, 999999);
            try
            {
                con.Open();
                sql = "INSERT INTO recetetbl (hastano,receteno,recete)" +
                    " VALUES(@hstno,@receteno,@recete);";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@hstno", hstno);
                cmd.Parameters.AddWithValue("@receteno", receteno);
                cmd.Parameters.AddWithValue("@recete", recete);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Reçete kaydı başarılı", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void recetedegistir(long receteid, string recete)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "UPDATE recetetbl SET recete=@recete WHERE receteid=@receteid";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@receteid", receteid);
                cmd.Parameters.AddWithValue("@recete", recete);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Reçete kaydı güncellendi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void recetesil(long receteid)
        {
            String sql;
            SqlCommand cmd;
            if(Mesaj("Reçete Bilgisini Silmek İstediğinizden eminmisiniz?","Uyarı",MessageBoxButtons.YesNoCancel)
                ==DialogResult.Yes)
            try
            {
                con.Open();
                sql = "DELETE FROM recetetbl WHERE receteid=@receteid";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@receteid", receteid);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Reçete kaydı silindi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void bolumgetir(ComboBox cb)
        {
            String sql = "SELECT* FROM bolumtbl";
            cbDoldur(sql, cb);
        }
       
        public void unvangetir(ComboBox cb)
        {
            String sql = "SELECT* FROM unvantbl";
            cbDoldur(sql, cb);
        }
       
        public void doktorgetir(byte bolumno,ComboBox cb)
        {
            //ünvan numarası 1'den büyük olmalı yoksa sekreterleride getirir çünkü 
            //sekreterlerin ümvan numarası 1 olarak tanımlandı
            String sql = "SELECT p.perno,CONCAT(u.unvanadi,' ',p.adsoyad) as dradi FROM personel as p,unvantbl as u WHERE p.unvanno=u.unvanid AND" +
                " p.unvanno>1 AND p.bolumno=+"+bolumno.ToString()+";";
            cbDoldur(sql, cb);
        }
        public string personeladigetir(int pno)
        {
            String s = "SELECT CONCAT(u.unvanadi,' ',p.adsoyad) as padi FROM personel as p,unvantbl as u" +
                " WHERE p.unvanno=u.unvanid AND p.perno=@pno;";
            SqlCommand cmd;
            string res = "";
            try
            {
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@pno",pno);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    res = sdr.GetString(0);
                }
                sdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message);
            }
            return res;
        }
        public void personelgetir(ComboBox cb)
        {
            String sql = "SELECT p.perno,CONCAT(u.unvanadi,' ',p.adsoyad) as padi FROM personel as p,unvantbl as u WHERE p.unvanno=u.unvanid;";
            cbDoldur(sql, cb);
        }
        public bool personelkontrol(string email)
        {
            String s = "SELECT email FROM personel WHERE email=@email;";
            SqlCommand cmd;
            SqlDataReader sdr;
            bool res = false;
            try
            {
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@email",email);
                con.Open();
                sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    res = true;
                }
                sdr.Close();
                con.Close();
                return res;
            }
            catch(Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
            return res;
            }
        public string[] personelgetir(int pno)
        {
            String s = "SELECT adsoyad,sifre,email,telno FROM personel WHERE perno=@pno;";
            SqlCommand cmd;
            SqlDataReader sdr;
            string[] res = new string[4];
            try
            {
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@pno", pno);
                con.Open();
                sdr = cmd.ExecuteReader();                
                if (sdr.Read())
                {
                    res[0] = sdr.GetString(0);
                    res[1] = sdr.GetString(1);
                    res[2] = sdr.GetString(2);
                    res[3] = sdr.GetString(3);

                }
                sdr.Close();
                con.Close();
            }
            catch(Exception ex)
            {
                Mesaj(ex.Message,"Hata");
            }
            return res;
        }
        public string[] personelbilgigetir(int pno)
        {
            String s = "SELECT adsoyad,sifre,email,telno,unvanno,bolumno FROM personel WHERE perno=@pno;";
            SqlCommand cmd;
            SqlDataReader sdr;
            string[] res = new string[6];
            try
            {
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@pno", pno);
                con.Open();
                sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    res[0] = sdr.GetString(0);
                    res[1] = sdr.GetString(1);
                    res[2] = sdr.GetString(2);
                    res[3] = sdr.GetString(3);
                    res[4] = sdr.GetByte(4).ToString();
                    res[5] = sdr.GetByte(5).ToString();
                }
                sdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
            return res;
        }

        public void PersonelEkle(string adsoyad,string sifre,string email,string telno
            ,byte unvno,byte bolumno)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "INSERT INTO personel (adsoyad,sifre,email,telno,unvanno,bolumno)" +
                    " VALUES(@adsoyad,@sifre,@email,@telno,@unvno,@bno);";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@adsoyad", adsoyad);
                cmd.Parameters.AddWithValue("@sifre", sifre);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@telno", telno);
                cmd.Parameters.AddWithValue("@unvno", unvno);
                cmd.Parameters.AddWithValue("@bno", bolumno);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Personel kaydı başarılı", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void PersonelDuzenle(int pno,string adsoyad, string sifre, string email, string telno
            , byte unvno, byte bolumno)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "UPDATE personel SET adsoyad=@adsoyad,sifre=@sifre,email=@email,telno=@telno,unvanno=@unvno,bolumno=@bno" +
                    " WHERE perno=@pno";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@pno", pno);
                cmd.Parameters.AddWithValue("@adsoyad", adsoyad);
                cmd.Parameters.AddWithValue("@sifre", sifre);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@telno", telno);
                cmd.Parameters.AddWithValue("@unvno", unvno);
                cmd.Parameters.AddWithValue("@bno", bolumno);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Personel kaydı güncellendi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void PersonelSil(int pno)
        {
            String sql;
            SqlCommand cmd;
            if(Mesaj("Personel Bilgisi Silinsinmi?","Uyarı",MessageBoxButtons.YesNo)==DialogResult.Yes)
            try
            {
                con.Open();
                sql = "DELETE FROM personel WHERE perno=@pno";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@pno", pno);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Personel kaydı silindi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public void PersonelBilgiDüzenle(int pno, string adsoyad, string sifre,string email,string telno)
        {
            String sql;
            SqlCommand cmd;
            try
            {
                con.Open();
                sql = "UPDATE personel SET adsoyad=@adsoyad,sifre=@sifre,email=@email,telno=@telno" +
                    " WHERE perno=@pno";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@pno", pno);
                cmd.Parameters.AddWithValue("@adsoyad", adsoyad);
                cmd.Parameters.AddWithValue("@sifre", sifre);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@telno", telno);
                cmd.ExecuteNonQuery();
                con.Close();
                Mesaj("Bilgileriniz güncellendi", "Bilgi");
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message, "Hata");
            }
        }
        public tablo[] doktorgetir()
        {
            string s = "SELECT perno,Concat(u.unvanadi,' ',adsoyad) FROM personel as p,unvantbl as u WHERE p.unvanno=u.unvanid AND u.unvanid>1;";
            SqlCommand cmd;
            List<tablo> bolumler = new List<tablo>();
            tablo bl = new tablo();
            try
            {
                cmd = new SqlCommand(s, con);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    bl.id = sdr.GetInt32(0);
                    bl.adi = sdr.GetString(1);
                    bolumler.Add(bl);
                }
                sdr.Close();
                con.Close();
                return bolumler.ToArray();
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message);
            }
            return null;
        }
        public long doktordoluluk(int drno)
        {
            String s = "SELECT Count(randevuno) FROM randevutbl WHERE doktorno=@drno;";
            SqlCommand cmd;
            int res=0;
            try
            {
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@drno", drno);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    res = sdr.GetInt32(0);
                }
                sdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message);
            }
            return res;
        }
        public tablo[] bolumgetir()
        {
            string s = "SELECT bolumid,bolumadi FROM bolumtbl;";
            SqlCommand cmd;
            List<tablo> bolumler=new List<tablo>();
            tablo bl =new tablo();
            try
            {
                cmd = new SqlCommand(s, con);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    bl.id = sdr.GetByte(0);
                    bl.adi = sdr.GetString(1);
                    bolumler.Add(bl);  
                }
                sdr.Close();
                con.Close();
                return bolumler.ToArray();
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message);
            }
            return null;
        }
        public long bolumdoluluk(byte bno)
        {
            String s = "SELECT Count(randevuno) as sayi FROM randevutbl WHERE doktorno IN " +
                "(SELECT p.perno FROM personel as p,bolumtbl as b WHERE " +
                "p.bolumno=b.bolumid AND bolumid=@bno);";
            SqlCommand cmd;
            long res = 0;
            try
            {
                cmd = new SqlCommand(s, con);
                cmd.Parameters.AddWithValue("@bno", bno);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    res = sdr.GetInt32(0);
                }
                con.Close();
                sdr.Close();
            }
            catch (Exception ex)
            {
                Mesaj(ex.Message);
            }
            return res;
        }
    }
}
