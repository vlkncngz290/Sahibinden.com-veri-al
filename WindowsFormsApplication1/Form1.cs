using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Data.SqlClient;



namespace WindowsFormsApplication1
    {
        public partial class Form1 : Form
        {
            public Form1()
            {
                InitializeComponent();
            }

            String VTbaslik;
            Int32 VTyil;
            Int32 VTkm;
            String VTrenk;
            Int32 VTfiyat;
            String VTbirim;
            DateTime VTtarih;
            String VTil;
            String VTilce;
            String VTmarka;

            string bagR = "Data Source=BHALTRN;" + "Initial Catalog=bha;" + "Integrated Security=SSPI;";
            DataTable table = new DataTable();

            public const int limit = 20;
            
            public void Tabling()
            {
                table.Columns.Add("baslik", typeof(string));
                table.Columns.Add("yil", typeof(string));
                table.Columns.Add("km", typeof(string));
                table.Columns.Add("renk", typeof(string));
                table.Columns.Add("fiyat", typeof(string));
                table.Columns.Add("tarih", typeof(string));
                table.Columns.Add("il", typeof(string));
                table.Columns.Add("ilce", typeof(string));
                table.Columns.Add("marka", typeof(string));
            }

            private void button1_Click(object sender, EventArgs e)
            {
                Uri url = new Uri("https://www.sahibinden.com/otomobil");
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string html = client.DownloadString(url);
                html = html.Replace("<br/>", "</td><td class='ilce'>");
                html = Regex.Replace(html, @"</td><td class='ilce'>\s", "");

                HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();
                dokuman.LoadHtml(html);
                HtmlNodeCollection fiyatlar = dokuman.DocumentNode.SelectNodes("//td[@class='searchResultsPriceValue']");
                HtmlNodeCollection sehirler = dokuman.DocumentNode.SelectNodes("//td[@class='searchResultsLocationValue']");
                HtmlNodeCollection ilceler = dokuman.DocumentNode.SelectNodes("//td[@class='ilce']");
                HtmlNodeCollection basliklar = dokuman.DocumentNode.SelectNodes("//td[@class='searchResultsTitleValue ']");
                HtmlNodeCollection yilKmRenkler = dokuman.DocumentNode.SelectNodes("//td[@class='searchResultsAttributeValue']");
                HtmlNodeCollection tarihler = dokuman.DocumentNode.SelectNodes("//td[@class='searchResultsDateValue']");
                HtmlNodeCollection marka = dokuman.DocumentNode.SelectNodes("//div[@class='classifiedSubtitle']");
            
                Tabling();
            
                for (int k = 0; k < limit; k++)
                {
                    string b = basliklar[k].InnerText.ToString();
                    string[] m = marka[k].InnerText.Split(' ');
                    table.Rows.Add(b.Trim(), yilKmRenkler[3 * k].InnerText.ToString().Trim(), yilKmRenkler[3 * k + 1].InnerText.ToString().Trim(), yilKmRenkler[3 * k + 2].InnerText.ToString().Trim(), 
                        fiyatlar[k].InnerText.ToString().Trim(), tarihler[k].InnerText.ToString().Trim(), 
                        sehirler[k].InnerText.ToString().Trim(), ilceler[k].InnerText.ToString().Trim(),m[0]);

                    VTbaslik = b.Trim();
                    VTyil=Int32.Parse(yilKmRenkler[3 * k].InnerText.ToString().Trim());
                    VTkm = kmBul(yilKmRenkler[3 * k + 1].InnerText.ToString().Trim());
                    VTrenk= yilKmRenkler[3 * k+ 2 ].InnerText.ToString().Trim();
                    VTfiyat = fiyatDuzenle(fiyatlar[k].InnerText.ToString().Trim());
                    VTbirim=birimBul(fiyatlar[k].InnerText.ToString().Trim());
                    VTtarih=tarihBul(tarihler[k].InnerText.ToString().Trim());
                    VTil=sehirler[k].InnerText.ToString().Trim();
                    VTilce=ilceler[k].InnerText.ToString().Trim();
                    VTmarka=m[0];
// INSERT INTO Varlik(baslik,yil,km,renk,fiyat,tarih,il,ilce,marka,birim)
        //Sütun isimleri ustteki parantez içinde yazan yazılarla BİRE BİR aynı olacak tatlım
//VALUES('"+baslik+"',"+yil+","+km+",'"+renk+"',"+fiyat+" ,'"+tarih+"' ,'"+il+"' ,'"+ilce+"' ,'"+marka+"' ,'"+birim+"');
                }  

            dataGridView1.DataSource = table;
            //EkleVT(VTbaslik, VTyil, VTkm, VTrenk, VTfiyat, VTbirim, VTtarih, VTil, VTilce, VTmarka);
            


            }

            private void EkleVT(String baslik, Int32 yil,Int32 km, String renk, Int32 fiyat, String birim, DateTime tarih,String il, String ilce, String marka)
            {
                /*
                 * SqlConnection conn = new SqlConnection();
                   conn.ConnectionString = "Data Source=BHALTRN;" + "Initial Catalog=bha;" + "Integrated Security=SSPI;";
                   conn.Open();
                */

                
                /*
                 * SqlConnection bagSQLR = new SqlConnection(bagR);
                   bagSQLR.Open();
                   string Sorgu = "INSERT INTO Varlik(yil,km,renk) VALUES (3,5,'gri')";
                   SqlCommand Komut = new SqlCommand(Sorgu, bagSQLR);
                   Komut.ExecuteNonQuery();
                   bagSQLR.Close();
                */

                
   
                using (SqlConnection conn = new SqlConnection(bagR))
                {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    conn.Open();
                    String tarihYeni = tarih.ToString("MM-dd");
                    String sorgu = "INSERT INTO Varlik(baslik,yil,km,renk,fiyat,tarih,il,ilce,marka,birim) VALUES('" + baslik + "'," + yil + "," + km + ",'" + renk + "'," + fiyat + " ,'" + tarih + "' ,'" + il + "' ,'" + ilce + "' ,'" + marka + "' ,'" + birim + "')";

                    /*                  {
                                          for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                                          {
                                              StrQuery = @"INSERT INTO Varlik(baslik,renk) VALUES ('" + dataGridView1.Rows[i].Cells["baslik"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["renk"].Value.ToString() + "');";

                                              //
                                              // Tatlım aşağıda yapılması gereken işlemler var
                                              //

                                              //StrQuery = @"INSERT INTO Varlik(baslik,yil,km,renk,fiyat,tarih,il,ilce,marka) VALUES ('" + dataGridView1.Rows[i].Cells["baslik"].Value.ToString() + "'," + dataGridView1.Rows[i].Cells["yil"].Value.ToString() + "," + dataGridView1.Rows[i].Cells["km"].Value.ToString() + ", '" + dataGridView1.Rows[i].Cells["renk"].Value.ToString() + "','" + dataGridView1.Rows[i].Cells["fiyat"].Value.ToString() + "','" + dataGridView1.Rows[i].Cells["tarih"].Value.ToString() + "','" + dataGridView1.Rows[i].Cells["il"].Value.ToString() + "','" + dataGridView1.Rows[i].Cells["ilce"].Value.ToString() + "','" + dataGridView1.Rows[i].Cells["marka"].Value.ToString() + "');";

                                          }
                                      }

                    */
                    comm.CommandText = sorgu;
                    comm.ExecuteNonQuery();
                }
                }
            }

            private void listView1_SelectedIndexChanged(object sender, EventArgs e)
            {

            }

            private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
            {



            }

            private void richTextBox1_TextChanged(object sender, EventArgs e)
            {

            }

            private void button2_Click(object sender, EventArgs e)
            {
                
            }

            public Int32 kmBul(String stKm)
            {
                Int32 gercekKm;
                int sayac;
                String geciciKm="";
                Char[] rakamlar = stKm.ToCharArray();
                for (sayac = 0; sayac < rakamlar.Length; sayac++)
                {
                    if (rakamlar[sayac] == '0')
                    {
                        geciciKm = geciciKm + rakamlar[sayac];
                    }

                    if (rakamlar[sayac] == '1')
                    {
                        geciciKm = geciciKm + rakamlar[sayac];
                    }

                    if (rakamlar[sayac] == '2')
                    {
                        geciciKm = geciciKm + rakamlar[sayac];
                    }

                    if (rakamlar[sayac] == '3')
                    {
                        geciciKm = geciciKm + rakamlar[sayac];
                    }

                    if (rakamlar[sayac] == '4')
                    {
                        geciciKm = geciciKm + rakamlar[sayac];
                    }

                    if (rakamlar[sayac] == '5')
                    {
                        geciciKm = geciciKm + rakamlar[sayac];
                    }

                    if (rakamlar[sayac] == '6')
                    {
                        geciciKm = geciciKm + rakamlar[sayac];
                    }

                    if (rakamlar[sayac] == '7')
                    {
                        geciciKm = geciciKm + rakamlar[sayac];
                    }

                    if (rakamlar[sayac] == '8')
                    {
                        geciciKm = geciciKm + rakamlar[sayac];
                    }

                    if (rakamlar[sayac] == '9')
                    {
                        geciciKm = geciciKm + rakamlar[sayac];
                    }

                }

                gercekKm = Int32.Parse(geciciKm);

                return gercekKm;
            }

            public Int32 fiyatDuzenle(String fiyat)
            {
                Int32 sonFiyat;
                String gercekFiyat = "";
                int sayac;
                Char[] rakamlar = fiyat.ToCharArray();
                for (sayac = 0; sayac < rakamlar.Length; sayac++)
                {
                    if (rakamlar[sayac] == '0')
                        {
                            gercekFiyat = gercekFiyat + rakamlar[sayac];
                        }
                    if (rakamlar[sayac] == '1')
                        {
                            gercekFiyat = gercekFiyat + rakamlar[sayac];
                        }
                    if (rakamlar[sayac] == '2')
                        {
                            gercekFiyat = gercekFiyat + rakamlar[sayac];
                        }
                    if (rakamlar[sayac] == '3')
                        {
                            gercekFiyat = gercekFiyat + rakamlar[sayac];
                        }
                    if (rakamlar[sayac] == '4')
                        {
                            gercekFiyat = gercekFiyat + rakamlar[sayac];
                        }
                    if (rakamlar[sayac] == '5')
                        {
                            gercekFiyat = gercekFiyat + rakamlar[sayac];
                        }
                    if (rakamlar[sayac] == '6')
                        {
                            gercekFiyat = gercekFiyat + rakamlar[sayac];
                        }
                    if (rakamlar[sayac] == '7')
                        {
                            gercekFiyat = gercekFiyat + rakamlar[sayac];
                        }
                    if (rakamlar[sayac] == '8')
                        {
                            gercekFiyat = gercekFiyat + rakamlar[sayac];
                        }
                    if (rakamlar[sayac] == '9')
                        {
                            gercekFiyat = gercekFiyat + rakamlar[sayac];
                        }
                }

                sonFiyat = Int32.Parse(gercekFiyat);
                return sonFiyat;
            }

            public String birimBul(String fiyat)
            {
                String birim = "";

                if (fiyat.Contains("TL")) birim = "TL";
                if (fiyat.Contains("$")) birim = "DOLAR";
                if (fiyat.Contains("€")) birim = "EURO";

                return birim;
            }

            public DateTime tarihBul(String tarih)
           {
            int ay=0;
            int gun=0;
            int yil = DateTime.Now.Year;

            if (tarih.Contains("Ocak")) ay = 1;
            if (tarih.Contains("Şubat")) ay = 2;
            if (tarih.Contains("Mart")) ay = 3;
            if (tarih.Contains("Nisan")) ay = 4;
            if (tarih.Contains("Mayıs")) ay = 5;
            if (tarih.Contains("Haziran")) ay = 6;
            if (tarih.Contains("Temmuz")) ay = 7;
            if (tarih.Contains("Ağustos")) ay = 8;
            if (tarih.Contains("Eylül")) ay = 9;
            if (tarih.Contains("Ekim")) ay = 10;
            if (tarih.Contains("Kasım")) ay = 11;
            if (tarih.Contains("Aralık")) ay = 12;
            gun = Int32.Parse(tarih.Substring(0, 2));

            DateTime sonTarih= new DateTime(yil,ay,gun);

            return sonTarih;
           }
   
    }
    }
