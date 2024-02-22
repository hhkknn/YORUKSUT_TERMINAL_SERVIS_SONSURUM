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
    //commit.
    public class GetCekmeListesiToplanan
    {
        public Response getCekmeListesiToplanan(string dbName, string baslangicTarihi, string bitisTarihi, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    //var query = "SELECT \"AbsEntry\",CONVERT(varchar, \"PickDate\", 103) as \"PickDate\",\"Name\",\"Remarks\" FROM \"OPKL\" WHERE \"PickDate\" >='" + baslangicTarihi + "' and \"PickDate\" <='" + bitisTarihi + "'";
                    var query = "Select T0.\"DocEntry\",T0.\"U_MusteriKodu\",T0.\"U_MusteriAdi\",\"U_Aciklama\" from \"@AIF_WMS_SIPKAR\" as T0 where T0.\"U_OnayDurumu\" = 'O' and (\"U_TerminalGizle\" <> 'Y' or \"U_TerminalGizle\" is null)";

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
                                        dt.TableName = "CekmeListesi";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "ÇEKME LİSTESİ BULUNAMADI." };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response getCekmeListesiDetaylari(string dbName, string docEntry, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select * from (Select T1.\"U_SiparisNumarasi\" as \"SiparisNumarasi\",T1.\"U_UrunKodu\" as \"UrunKodu\",T1.\"U_UrunTanimi\" as \"UrunTanimi\",T1.\"U_SipSatirNo\" as \"SipSatirNo\",T1.\"U_PlanSipMik\" - ISNULL((Select SUM(T99.\"U_Miktar\") from \"@AIF_WMS_TOPLANAN\" as T99 where T99.\"U_SiparisNumarasi\" = T1.\"U_SiparisNumarasi\" and T99.\"U_SiparisSatirNo\" = T1.\"U_SipSatirNo\" and T99.\"U_BelgeNo\" = T0.\"DocEntry\"),0) as \"PlanlananSiparisMiktari\",(Select TOP 1 \"Substitute\" from OSCN as T97 where T97.\"CardCode\" = T0.\"U_MusteriKodu\") as \"MuhatapKatalogNo\",(Select \"CodeBars\" from \"OITM\" as T96 where T96.\"ItemCode\" = T1.\"U_UrunKodu\") as \"Barkod\",(Select \"ManBtchNum\" from \"OITM\" as T96 where T96.\"ItemCode\" = T1.\"U_UrunKodu\") as \"Partili\",T1.\"LineId\" as \"SatirNo\",'' as \"PaletNo\",cast(0 as decimal(15,2)) as \"Miktar\" from \"@AIF_WMS_SIPKAR\" as T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\"  where T0.\"DocEntry\" = '" + docEntry + "') as tbl where tbl.\"PlanlananSiparisMiktari\" > 0 ";

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
                                        dt.TableName = "CekmeListesiDetay";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "ÇEKME LİSTESİ DETAYLARI GETİRİLEMEDİ." };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response getCekmeListesiKoliDetaylari(string dbName, string docEntry, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    var query = "SELECT T1.\"U_SiparisNo\",T1.\"U_SatirNo\", T1.\"U_KoliAdedi\", T1.\"U_KoliIciAdedi\", T1.\"U_ToplamMiktar\" FROM \"@AIF_WMS_KOLIDTY\" T0 INNER JOIN \"@AIF_WMS_KOLIDTY1\" T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" WHERE T0.\"U_BelgeNo\" ='" + docEntry + "'";

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
                                        dt.TableName = "CekmeListesiKoliDetay";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "ÇEKME LİSTESİ KOLİ DETAYLARI GETİRİLEMEDİ." };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return new Response { _list = dt, Val = 0 };
        }
         
        public Response getToplamaListesi(string dbName, string baslangicTarihi, string bitisTarihi, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    //string query = "  select * from (Select T0.\"U_SiparisNumarasi\" as \"SiparisNumarasi\",T1.\"DocDate\" as \"SiparisTarihi\",T1.\"DocDueDate \" as \"TeslimatTarihi\",T0.\"U_SiparisSatirNo\" as \"SiparisSatirNo\",T2.\"ItemCode\" as \"UrunKodu\",T2.\"Dscription\" as \"UrunTanimi\",T2.\"Quantity\" as \"ToplamSiparisMiktari\", (T2.\"Quantity\" - T2.\"OpenQty\") as \"SevkSipMiktari\", (T2.\"OpenQty\" - T0.\"U_Miktar\") AS \"AcikSiparisMiktari\",T2.\"WhsCode\" as \"SiparisDepoKodu\",T0.\"U_Miktar\" as \"ToplananMiktar\",T0.\"U_PaletNo\" as \"PaletNo\",(T2.\"OpenQty\" - ISNULL(T0.\"U_Miktar\", 0)) as \"PlanlananSiparisMiktari\",(SELECT Count(\"DocEntry\") FROM \"@AIF_WMS_KNTYNR1\" AS T98 WHERE T98.\"U_PaletNo\" = T0.\"U_PaletNo\") AS \"KonteynerVarmi\",T0.\"U_TeslimatNo\",T0.\"DocEntry\" AS \"ToplananDocEntry\",T0.\"CardCode\" as \"MusteriKodu\",T0.\"CardName\" as \"MusteriAdi\" from \"@AIF_WMS_TOPLANAN\" as T0 INNER JOIN ORDR AS T1 ON T0.\"U_SiparisNumarasi\" = T1.\"DocEntry\" INNER JOIN RDR1 AS T2 ON T1.\"DocEntry\" = T2.\"DocEntry\" and T2.\"LineNum\" = T0.\"U_SiparisSatirNo\") as tbl WHERE tbl.\"KonteynerVarmi\" <= 0 and tbl.\"TeslimatTarihi\" >= '" + baslangicTarihi + "'  and  tbl.\"TeslimatTarihi\" <= '" + bitisTarihi + "'";

                    string query = "  select * from (Select DISTINCT T0.\"U_BelgeNo\",T1.\"DocDueDate \" as \"TeslimatTarihi\",(SELECT Count(\"DocEntry\") FROM \"@AIF_WMS_KNTYNR1\" AS T98 WHERE T98.\"U_PaletNo\" = T0.\"U_PaletNo\") AS \"KonteynerVarmi\",T1.\"CardCode\" as \"MusteriKodu\",T1.\"CardName\" as \"MusteriAdi\",T3.\"U_Aciklama\" from \"@AIF_WMS_TOPLANAN\" as T0 INNER JOIN ORDR AS T1 ON T0.\"U_SiparisNumarasi\" = T1.\"DocEntry\" INNER JOIN RDR1 AS T2 ON T1.\"DocEntry\" = T2.\"DocEntry\" and T2.\"LineNum\" = T0.\"U_SiparisSatirNo\" LEFT JOIN \"@AIF_WMS_SIPKAR\" AS T3 ON T3.\"DocEntry\" = T0.\"U_BelgeNo\" ) as tbl WHERE tbl.\"KonteynerVarmi\" <= 0 and tbl.\"TeslimatTarihi\" >= '" + baslangicTarihi + "'  and  tbl.\"TeslimatTarihi\" <= '" + bitisTarihi + "'";


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
                                        dt.TableName = "ToplamaListesi";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "TOPLAMA LİSTESİ BULUNAMADI." };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return new Response { _list = dt, Val = 0 };
        }

    }
}