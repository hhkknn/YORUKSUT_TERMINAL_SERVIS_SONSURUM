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
    public class GetCountingCustomTable
    {
        //commit
        public Response getCountingCustomTable(string dbName, string kullaniciId, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select \"DocEntry\" as \"Belge No\", CONVERT(varchar, \"U_SayimTarihi\",103) as \"Sayım Tarihi\",\"U_KullaniciId\" as \"Kullanıcı Id\",\"U_KullaniciAdi\" as \"Kullanıcı Adı\",\"U_Aciklama\" as \"Açıklama\" from \"@AIF_WMS_WHSCOUNT\" where ISNULL(\"U_SayimNumarasi\",'') = '' and \"U_KullaniciId\" = '" + kullaniciId + "' ";

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
                                        dt.TableName = "StockCountingData";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -555, Desc = "PARTİ NUMARASI GİRİŞİ YAPILMAMIŞTIR." };
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -9999, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -9998, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response getLinesCountingCustomTableByDocEntry(string dbName, int DocEntry, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    //var query = "Select \"U_Barkod\" as \"Barkod\",\"U_KalemKodu\" as \"Ürün Kodu\", \"U_KalemTanimi\" as \"Ürün Adı\", \"U_DepoKodu\" as \"Depo Kodu\", \"U_DepoAdi\" as \"Depo Adı\", \"U_Miktar\" as \"Miktar\",\"U_OlcuBirimi\" as \"Ölçü Birimi\",\"U_DepoYeriId\" as \"DepoYeriId\",\"U_DepoYeriAdi\" as \"DepoYeriAdi\" from \"@AIF_WMS_WHSCOUNT1\" where \"DocEntry\" = " + DocEntry + "";

                    var query = "Select T1.\"U_Barkod\" as \"Barkod\",T1.\"U_KalemKodu\" as \"Ürün Kodu\", T1.\"U_KalemTanimi\" as \"Ürün Adı\", T1.\"U_DepoKodu\" as \"Depo Kodu\", T1.\"U_DepoAdi\" as \"Depo Adı\", T1.\"U_Miktar\" as \"Miktar\",T1.\"U_OlcuBirimi\" as \"Ölçü Birimi\",T1.\"U_DepoYeriId\" as \"DepoYeriId\",T1.\"U_DepoYeriAdi\" as \"DepoYeriAdi\",T0.\"U_Aciklama\" as \"Açıklama\" from \"@AIF_WMS_WHSCOUNT\" T0 INNER JOIN \"@AIF_WMS_WHSCOUNT1\" T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T1.\"DocEntry\" = " + DocEntry + "";

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
                                        dt.TableName = "StockCountingLinesData";

                                        if (dt.Rows.Count == 0)
                                        {
                                            //return new Response { _list = null, Val = -555, Desc = "PARTİ NUMARASI GİRİŞİ YAPILMAMIŞTIR." };
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -9999, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -9998, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response getPartiesCountingCustomTableByDocEntry(string dbName, int DocEntry, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select \"U_KalemKodu\" as \"Ürün Kodu\", \"U_DepoKodu\" as \"Depo Kodu\",\"U_PartiNo\" as \"Parti No\",\"U_Miktar\" as \"Miktar\" from \"@AIF_WMS_WHSCOUNT2\" where \"DocEntry\" = " + DocEntry + "";

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
                                        dt.TableName = "StockCountingPartiesData";

                                        if (dt.Rows.Count == 0)
                                        {
                                            //return new Response { _list = null, Val = -555, Desc = "PARTİ NUMARASI GİRİŞİ YAPILMAMIŞTIR." };
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -9999, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -9998, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response getCountingCustomTableByDate(string dbName, string kullaniciId, string sayimTarihi, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select \"DocEntry\" as \"Belge No\", CONVERT(varchar, \"U_SayimTarihi\",103) as \"Sayım Tarihi\",\"U_KullaniciId\" as \"Kullanıcı Id\",\"U_KullaniciAdi\" as \"Kullanıcı Adı\",\"U_Aciklama\" as \"Açıklama\" from \"@AIF_WMS_WHSCOUNT\" where ISNULL(\"U_SayimNumarasi\",'') = '' and \"U_KullaniciId\" = '" + kullaniciId + "' and \"U_SayimTarihi\" = '"+ sayimTarihi + "' ";

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
                                        dt.TableName = "StockCountingDataByDate";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -555, Desc = "STOK SAYIM FİŞİ BULUNAMAMIŞTIR." };
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -9999, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -9998, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
            }
            return new Response { _list = dt, Val = 0 };
        }

    }
}