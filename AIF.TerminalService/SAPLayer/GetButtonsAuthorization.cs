using AIF.TerminalService.DatabaseLayer;
using AIF.TerminalService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{
    public class GetButtonsAuthorization
    {
        public Response getButtonAuthorization(string userName, string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            DataTable dt_Ohem = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName,mKod);

                if (connstring != "")
                {
                    //var query = "Select \"U_SprsliMalGrs\" as \"Siparişli Mal Girişi\", \"U_SprsszMalGrs\" as \"Siparişsiz Mal Girişi\", \"U_BlgszMalGrs\" as \"Belgesiz Mal Girişi\", \"U_TlpszDepoNak\" as \"Talepsiz Depo Nakli\",\"U_TlbbgliDepoNak\" as \"Talebe Bağlı Depo Nakli\", \"U_DepoSayimi\" as \"Depo Sayımı\",\"U_BlgszMalCks\" as \"Belgesiz Mal Çıkışı\",\"U_SprsbagliTes\" as \"Siparişe Bağlı Teslimat\",\"U_SiparissizTes\" as \"Siparişsiz Teslimat\",\"U_BarkodOlustur\" as \"Barkod Oluştur\", \"U_UrtmMalCikis\" as \"Üretime Mal Çıkış\",\"U_UrtmdnMalGiris\" as \"Üretimden Mal Giriş\" from \"OUSR\"  as T0 INNER JOIN \"@AIF_WMS_BTN\" as T1 ON T0.\"USERID\" = T1.\"U_UserCode\" where T0.\"USER_CODE\" = '" + userName + "'";

                    var query = "Select \"U_SprsliMalGrs\" as \"Siparişli Mal Girişi\", \"U_SprsszMalGrs\" as \"Siparişsiz Mal Girişi\", \"U_BlgszMalGrs\" as \"Belgesiz Mal Girişi\", \"U_TlpszDepoNak\" as \"Talepsiz Depo Nakli\",\"U_TlbbgliDepoNak\" as \"Talebe Bağlı Depo Nakli\",\"U_TalepKabul\" as \"Talep Kabul\", \"U_DepoSayimi\" as \"Depo Sayımı\",\"U_BlgszMalCks\" as \"Belgesiz Mal Çıkışı\",\"U_SprsbagliTes\" as \"Siparişe Bağlı Teslimat\",\"U_SiparissizTes\" as \"Siparişsiz Teslimat\",\"U_BarkodOlustur\" as \"Barkod Oluştur\", \"U_UrtmMalCikis\" as \"Üretime Mal Çıkış\",\"U_UrtmdnMalGiris\" as \"Üretimden Mal Giriş\",\"U_MusteriFatIade\" as \"Müşteri Fatura İadesi\", \"U_TeslimatIade\" as \"Teslimat İadesi\", \"U_Raporlar\" as \"Raporlar\", \"U_SatistanIade\" as \"Satıştan İade\",\"U_CekmeListesi\" as \"Çekme Listesi\",\"U_PaletYapma\" as \"Palet Yapma\",\"U_KonteynerYapma\" as \"Konteyner Yapma\",\"U_MagazaIslemleri\" as \"Mağazacılık İşlemleri\",\"U_IadeTalep\" as \"İade Talep\"  from \"OHEM\"  as T0 INNER JOIN \"@AIF_WMS_BTN\" as T1 ON T0.\"empID\" = T1.\"U_UserCode\" where T0.\"empID\" = '" + userName + "'";
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);

                                        dt.TableName = "buttonList";
                                    }
                                }
                            }
                        }

                        //if (dt.Rows.Count == 0) //Ohemde empid ve code karışıklığından dolayı burası eklendi.
                        //{
                        //    #region Giren kişinin empID'sini bulur.

                        //    string empID = "";
                        //    string sql = "Select \"empID\" from OHEM as T0 where T0.\"Code\" = '" + userName + "'";
                        //    using (SqlConnection con = new SqlConnection(connstring))
                        //    {
                        //        using (SqlCommand cmd = new SqlCommand(sql, con))
                        //        {
                        //            cmd.CommandType = CommandType.Text;
                        //            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        //            {
                        //                using (dt_Ohem = new DataTable())
                        //                {
                        //                    sda.Fill(dt_Ohem);

                        //                    dt_Ohem.TableName = "empIDNo";

                        //                    if (dt_Ohem.Rows.Count > 0)
                        //                    {
                        //                        empID = dt_Ohem.Rows[0][0].ToString();
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }

                        //    #endregion Giren kişinin empID'sini bulur.

                        //    dt = new DataTable();
                        //    query = "Select \"U_SprsliMalGrs\" as \"Siparişli Mal Girişi\", \"U_SprsszMalGrs\" as \"Siparişsiz Mal Girişi\", \"U_BlgszMalGrs\" as \"Belgesiz Mal Girişi\", \"U_TlpszDepoNak\" as \"Talepsiz Depo Nakli\",\"U_TlbbgliDepoNak\" as \"Talebe Bağlı Depo Nakli\",\"U_TalepKabul\" as \"Talep Kabul\", \"U_DepoSayimi\" as \"Depo Sayımı\",\"U_BlgszMalCks\" as \"Belgesiz Mal Çıkışı\",\"U_SprsbagliTes\" as \"Siparişe Bağlı Teslimat\",\"U_SiparissizTes\" as \"Siparişsiz Teslimat\",\"U_BarkodOlustur\" as \"Barkod Oluştur\", \"U_UrtmMalCikis\" as \"Üretime Mal Çıkış\",\"U_UrtmdnMalGiris\" as \"Üretimden Mal Giriş\",\"U_MusteriFatIade\" as \"Müşteri Fatura İadesi\", \"U_TeslimatIade\" as \"Teslimat İadesi\", \"U_Raporlar\" as \"Raporlar\", \"U_SatistanIade\" as \"Satıştan İade\" from \"OHEM\"  as T0 INNER JOIN \"@AIF_WMS_BTN\" as T1 ON T0.\"empID\" = T1.\"U_UserCode\" where T0.\"empID\" = '" + empID + "'";
                        //    using (SqlConnection con = new SqlConnection(connstring))
                        //    {
                        //        using (SqlCommand cmd = new SqlCommand(query, con))
                        //        {
                        //            cmd.CommandType = CommandType.Text;
                        //            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        //            {
                        //                using (dt = new DataTable())
                        //                {
                        //                    sda.Fill(dt);

                        //                    dt.TableName = "buttonList";
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return new Response { _list = dt };
        }
    }
}