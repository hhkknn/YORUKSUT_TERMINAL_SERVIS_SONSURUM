using AIF.TerminalService.DatabaseLayer;
using AIF.TerminalService.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{
    //commit.
    public class GetCekmeListesi
    {
        public Response getCekmeListesi(string dbName, string baslangicTarihi, string bitisTarihi, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    //var query = "SELECT \"AbsEntry\",CONVERT(varchar, \"PickDate\", 103) as \"PickDate\",\"Name\",\"Remarks\" FROM \"OPKL\" WHERE \"PickDate\" >='" + baslangicTarihi + "' and \"PickDate\" <='" + bitisTarihi + "'";
                    //var query = "Select T0.\"DocEntry\",T0.\"U_MusteriKodu\",T0.\"U_MusteriAdi\",\"U_Aciklama\" from \"@AIF_WMS_SIPKAR\" as T0 where T0.\"U_OnayDurumu\" = 'O' and (\"U_TerminalGizle\" <> 'Y' or \"U_TerminalGizle\" is null)";

                    var query = "Select * from (SELECT T0.\"DocEntry\",T0.\"U_MusteriKodu\",T0.\"U_MusteriAdi\",\"U_Aciklama\",(Select COUNT(T1.\"DocEntry\") from \"@AIF_WMS_SIPKAR1\" as T1 where T1.\"DocEntry\" = T0.\"DocEntry\"  and T1.\"U_Gorunur\" = 'Y') as \"SatirSayisi\" FROM \"@AIF_WMS_SIPKAR\" AS T0 WHERE T0.\"U_OnayDurumu\" = 'O' AND(\"U_TerminalGizle\" <> 'Y' OR \"U_TerminalGizle\" IS NULL)) as tbl where tbl.\"SatirSayisi\" > 0";
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

        public Response getCekmeListesiDetaylari(string dbName, string docEntry, string mKod, string cekmeListesiKalemleriniGrupla)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string query = "";

                    if (cekmeListesiKalemleriniGrupla != "Y")
                    {
                        query = "Select * from (Select T1.\"U_SiparisNumarasi\" as \"SiparisNumarasi\",(SELECT TOP 1 \"Substitute\" FROM OSCN AS T97 WHERE T97.\"CardCode\" = T0.\"U_MusteriKodu\" AND T97.\"ItemCode\" = T1.\"U_UrunKodu\" AND T97.\"IsDefault\" = 'Y') as \"MuhKat\",T1.\"U_UrunKodu\" as \"UrunKodu\",T1.\"U_UrunTanimi\" as \"UrunTanimi\",T1.\"U_SipSatirNo\" as \"SipSatirNo\",T1.\"U_PlanSipMik\" - CASE WHEN T1.\"U_ToplananMik\" > 0 THEN ISNULL((Select SUM(T99.\"U_Miktar\") from \"@AIF_WMS_TOPLANAN\" as T99 where T99.\"U_SiparisNumarasi\" = T1.\"U_SiparisNumarasi\" and T99.\"U_SiparisSatirNo\" = T1.\"U_SipSatirNo\" and T99.\"U_BelgeNo\" = T0.\"DocEntry\"  and ISNULL(T99.\"U_TeslimatNo\",'')=''),0) else 0 end as \"PlanlananSiparisMiktari\",(Select TOP 1 \"Substitute\" from OSCN as T97 where T97.\"CardCode\" = T0.\"U_MusteriKodu\" and T97.\"ItemCode\" = T1.\"U_UrunKodu\" and T97.\"IsDefault\" = 'Y') as \"MuhatapKatalogNo\",(Select \"CodeBars\" from \"OITM\" as T96 where T96.\"ItemCode\" = T1.\"U_UrunKodu\") as \"Barkod\",(Select \"ManBtchNum\" from \"OITM\" as T96 where T96.\"ItemCode\" = T1.\"U_UrunKodu\") as \"Partili\",T1.\"LineId\" as \"SatirNo\",'' as \"PaletNo\",cast(0 as decimal(15,2)) as \"Miktar\",'' as \"SatirKaynagi\",T0.\"U_MuhKatGoster\" as \"MuhKatGoster\" from \"@AIF_WMS_SIPKAR\" as T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\"  where T0.\"DocEntry\" = '" + docEntry + "' and T1.\"U_Gorunur\" = 'Y') as tbl where tbl.\"PlanlananSiparisMiktari\" > 0 ";
                    }
                    else
                    {
                        query = "Select * from (Select '' as \"SiparisNumarasi\", (SELECT TOP 1 \"Substitute\" FROM OSCN AS T97 WHERE T97.\"CardCode\" = T0.\"U_MusteriKodu\" AND T97.\"ItemCode\" = T1.\"U_UrunKodu\" AND T97.\"IsDefault\" = 'Y') as \"MuhKat\", T1.\"U_UrunKodu\" as \"UrunKodu\",T1.\"U_UrunTanimi\" as \"UrunTanimi\",'' as \"SipSatirNo\",(Select SUM(T54.U_PlanSipMik) from \"@AIF_WMS_SIPKAR1\" as T54 where T54.\"DocEntry\" = T0.\"DocEntry\" and T54.\"U_UrunKodu\" = T1.\"U_UrunKodu\") - ISNULL((Select SUM(T99.\"U_Miktar\") from \"@AIF_WMS_TOPLANAN\" as T99 where T99.\"U_BelgeNo\" = T0.\"DocEntry\"  and ISNULL(T99.\"U_TeslimatNo\",'')='' and T99.\"U_KalemKodu\" = T1.\"U_UrunKodu\"),0) as \"PlanlananSiparisMiktari\",(Select TOP 1 \"Substitute\" from OSCN as T97 where T97.\"CardCode\" = T0.\"U_MusteriKodu\" and T97.\"ItemCode\" = T1.\"U_UrunKodu\" and T97.\"IsDefault\" = 'Y') as \"MuhatapKatalogNo\",(Select \"CodeBars\" from \"OITM\" as T96 where T96.\"ItemCode\" = T1.\"U_UrunKodu\") as \"Barkod\",(Select \"ManBtchNum\" from \"OITM\" as T96 where T96.\"ItemCode\" = T1.\"U_UrunKodu\") as \"Partili\",'' as \"SatirNo\",'' as \"PaletNo\",cast(0 as decimal(15,2)) as \"Miktar\",STRING_AGG(cast(T1.\"U_SiparisNumarasi\" as varchar) + '-' + cast(\"U_SipSatirNo\" as varchar), ',') as \"SatirKaynagi\",T0.\"U_MuhKatGoster\" as \"MuhKatGoster\"  from \"@AIF_WMS_SIPKAR\" as T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\"  where T0.\"DocEntry\" = '" + docEntry + "' and T1.\"U_Gorunur\" = 'Y' group by T1.\"U_UrunKodu\",T1.\"U_UrunTanimi\",T0.\"U_MusteriKodu\",T0.\"U_MuhKatGoster\",T0.\"DocEntry\") as tbl where tbl.\"PlanlananSiparisMiktari\" > 0 ";
                    }

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

                    string query = "  select DISTINCT tbl.* from (Select  T0.\"U_BelgeNo\",(SELECT Count(\"DocEntry\") FROM \"@AIF_WMS_KNTYNR1\" AS T98 WHERE T98.\"U_PaletNo\" = T0.\"U_PaletNo\") AS \"KonteynerVarmi\",T1.\"CardCode\" as \"MusteriKodu\",T1.\"CardName\" as \"MusteriAdi\",T3.\"U_Aciklama\" from \"@AIF_WMS_TOPLANAN\" as T0 INNER JOIN ORDR AS T1 ON T0.\"U_SiparisNumarasi\" = T1.\"DocEntry\" INNER JOIN RDR1 AS T2 ON T1.\"DocEntry\" = T2.\"DocEntry\" and T2.\"LineNum\" = T0.\"U_SiparisSatirNo\" LEFT JOIN \"@AIF_WMS_SIPKAR\" AS T3 ON T3.\"DocEntry\" = T0.\"U_BelgeNo\" ) as tbl INNER JOIN RDR1 AS T99 ON tbl.\"U_BelgeNo\" = T99.\"DocEntry\" INNER JOIN ORDR AS T98 ON T98.\"DocEntry\" = T99.\"DocEntry\" WHERE tbl.\"KonteynerVarmi\" <= 0 and T98.\"DocDueDate\" >= '" + baslangicTarihi + "'  and  T98.\"DocDueDate\" <= '" + bitisTarihi + "'";


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

        public Response getToplamaListesiDetay(string dbName, string docEntry, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    //string query = "select * from (Select T0.\"U_SiparisNumarasi\" as \"SiparisNumarasi\",T1.\"DocDate\" as \"SiparisTarihi\",T1.\"DocDueDate \" as \"TeslimatTarihi\",T0.\"U_SiparisSatirNo\" as \"SiparisSatirNo\",T2.\"ItemCode\" as \"UrunKodu\",T2.\"Dscription\" as \"UrunTanimi\",T2.\"Quantity\" as \"ToplamSiparisMiktari\", (T2.\"Quantity\" - T2.\"OpenQty\") as \"SevkSipMiktari\", (T2.\"OpenQty\" - T0.\"U_Miktar\") AS \"AcikSiparisMiktari\",T2.\"WhsCode\" as \"SiparisDepoKodu\",T0.\"U_Miktar\" as \"ToplananMiktar\",T0.\"U_PaletNo\" as \"PaletNo\",(T2.\"OpenQty\" - ISNULL(T0.\"U_Miktar\", 0)) as \"PlanlananSiparisMiktari\",(SELECT Count(\"DocEntry\") FROM \"@AIF_WMS_KNTYNR1\" AS T98 WHERE T98.\"U_PaletNo\" = T0.\"U_PaletNo\") AS \"KonteynerVarmi\",T0.\"U_TeslimatNo\",T0.\"U_BelgeNo\" AS \"BelgeNo\",T1.\"CardCode\" as \"MusteriKodu\",T1.\"CardName\" as \"MusteriAdi\",T0.\"DocEntry\" as \"ToplananDocEntry\" from \"@AIF_WMS_TOPLANAN\" as T0 INNER JOIN ORDR AS T1 ON T0.\"U_SiparisNumarasi\" = T1.\"DocEntry\" INNER JOIN RDR1 AS T2 ON T1.\"DocEntry\" = T2.\"DocEntry\" and T2.\"LineNum\" = T0.\"U_SiparisSatirNo\") as tbl WHERE tbl.\"KonteynerVarmi\" <= 0 and tbl.\"BelgeNo\" = '" + docEntry + "' ";


                    string query = "select * from (Select T0.\"U_SiparisNumarasi\" as \"SiparisNumarasi\",T1.\"DocDate\" as \"SiparisTarihi\",T1.\"DocDueDate \" as \"TeslimatTarihi\",T0.\"U_SiparisSatirNo\" as \"SiparisSatirNo\",T2.\"ItemCode\" as \"UrunKodu\",T2.\"Dscription\" as \"UrunTanimi\",T2.\"Quantity\" as \"ToplamSiparisMiktari\", (T2.\"Quantity\" - T2.\"OpenQty\") as \"SevkSipMiktari\", (T2.\"OpenQty\" - T0.\"U_Miktar\") AS \"AcikSiparisMiktari\",T2.\"WhsCode\" as \"SiparisDepoKodu\",T0.\"U_Miktar\" as \"ToplananMiktar\",T0.\"U_PaletNo\" as \"PaletNo\",(T2.\"OpenQty\" - ISNULL(T0.\"U_Miktar\", 0)) as \"PlanlananSiparisMiktari\",(SELECT Count(\"DocEntry\") FROM \"@AIF_WMS_KNTYNR1\" AS T98 WHERE T98.\"U_SiparisNo\" = T0.\"U_SiparisNumarasi\" and T98.\"U_SipSatirNo\" = T0.\"U_SiparisSatirNo\") AS \"KonteynerVarmi\",T0.\"U_TeslimatNo\",T0.\"U_BelgeNo\" AS \"BelgeNo\",T1.\"CardCode\" as \"MusteriKodu\",T1.\"CardName\" as \"MusteriAdi\",T0.\"DocEntry\" as \"ToplananDocEntry\" from \"@AIF_WMS_TOPLANAN\" as T0 INNER JOIN ORDR AS T1 ON T0.\"U_SiparisNumarasi\" = T1.\"DocEntry\" INNER JOIN RDR1 AS T2 ON T1.\"DocEntry\" = T2.\"DocEntry\" and T2.\"LineNum\" = T0.\"U_SiparisSatirNo\") as tbl WHERE tbl.\"KonteynerVarmi\" <= 0 and tbl.\"BelgeNo\" = '" + docEntry + "' ";

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
                                        dt.TableName = "ToplamaListesiDetay";

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

        public Response getCekmeListesiMusterileri(string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select DISTINCT tbl.\"U_MusteriKodu\" as \"MusteriKodu\",tbl.\"U_MusteriAdi\" as \"MusteriAdi\" from (SELECT T0.\"U_MusteriKodu\",T0.\"U_MusteriAdi\",\"U_Aciklama\",(Select COUNT(T1.\"DocEntry\") from \"@AIF_WMS_SIPKAR1\" as T1 where T1.\"DocEntry\" = T0.\"DocEntry\") as \"SatirSayisi\" FROM \"@AIF_WMS_SIPKAR\" AS T0) as tbl ";
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
        private string companyDbCode;
        int clNum = 0;
        public Response getCekmeListesiMusteriyeGoreUrunListesi(string dbName, string urunKodu, string mKod,string musterikod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    #region old 2022.04.12 öncesi
                    //var query = "SELECT  tbl.\"BelgeNo\" as \"CekmeListesiNo\",T2.\"U_UrunKodu\" AS \"UrunKodu\", T2.\"U_UrunTanimi\" AS \"UrunTanimi\", tbl.Aciklama,Cast(T2.\"U_SiparisNumarasi\"  as varchar(50)) as \"SiparisNumarasi\",cast(T2.\"U_SipSatirNo\" as varchar(50)) as \"SiparisSatirNo\", T2.\"U_PlanSipMik\" - CASE WHEN T2.\"U_ToplananMik\" > 1 THEN ISNULL((SELECT SUM(T99.\"U_Miktar\") FROM \"@AIF_WMS_TOPLANAN\" AS T99 WHERE T99.\"U_SiparisNumarasi\" = T2.\"U_SiparisNumarasi\" AND T99.\"U_SiparisSatirNo\" = T2.\"U_SipSatirNo\" AND T99.\"U_BelgeNo\" = tbl.\"BelgeNo\" AND ISNULL(T99.\"U_TeslimatNo\", '') = ''),0) ELSE 0 END AS \"Miktar\"  FROM (SELECT T0.\"DocEntry\" AS \"BelgeNo\", \"U_Aciklama\" AS \"Aciklama\", T0.\"U_MuhKatGoster\", (SELECT COUNT(T1.\"DocEntry\") FROM \"@AIF_WMS_SIPKAR1\" AS T1 WHERE T1.\"DocEntry\" = T0.\"DocEntry\") AS \"SatirSayisi\" FROM \"@AIF_WMS_SIPKAR\" AS T0 WHERE T0.\"U_OnayDurumu\" = 'O') AS tbl INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T2 ON tbl.\"BelgeNo\" = T2.\"DocEntry\" INNER JOIN OITM AS T3 ON T3.\"ItemCode\" = T2.\"U_UrunKodu\" WHERE tbl.\"SatirSayisi\" > 0   AND (T2.\"U_UrunKodu\" = '" + urunKodu + "' OR T2.\"U_MuhKatNo\" = '" + urunKodu + "' OR T3.\"CodeBars\" = '" + urunKodu + "')"; 
                    #endregion

                    // cem sorguya müşteri kodu eklendi ,T0.U_MusteriKodu

                    string condition = LoginCompany.oCompany.DbServerType == BoDataServerTypes.dst_HANADB ? "IFNULL" : "ISNULL";

                    var query = " SELECT tbl.\"BelgeNo\" AS \"CekmeListesiNo\", T2.\"U_UrunKodu\" AS \"UrunKodu\",T2.\"U_UrunTanimi\" AS \"UrunTanimi\", tbl.\"Aciklama\", '-1' AS \"SiparisNumarasi\", '-1' AS \"SiparisSatirNo\", SUM(T2.\"U_PlanSipMik\") - SUM(T2.\"U_ToplananMik\") AS \"Miktar\", STRING_AGG(concat(T2.\"U_SiparisNumarasi\", '-', T2.\"U_SipSatirNo\"), ',') AS \"Kaynak\" FROM (SELECT T0.\"DocEntry\" AS \"BelgeNo\",\"U_Aciklama\" AS \"Aciklama\",T0.\"U_MuhKatGoster\", (SELECT COUNT(T1.\"DocEntry\") FROM \"@AIF_WMS_SIPKAR1\" AS T1 WHERE T1.\"DocEntry\" = T0.\"DocEntry\") AS \"SatirSayisi\",T0.\"U_MusteriKodu\",T0.\"U_TerminalGizle\" FROM \"@AIF_WMS_SIPKAR\" AS T0 WHERE T0.\"U_OnayDurumu\" = 'O') AS tbl INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T2 ON tbl.\"BelgeNo\" = T2.\"DocEntry\" INNER JOIN OITM AS T3 ON T3.\"ItemCode\" = T2.\"U_UrunKodu\" WHERE tbl.\"SatirSayisi\" > 0 AND(T2.\"U_UrunKodu\" = '" + urunKodu + "' OR T2.\"U_MuhKatNo\" = '" + urunKodu + "'  OR T3.\"CodeBars\" = '" + urunKodu + "' ) and tbl.\"U_MusteriKodu\"='" + musterikod + "' and  T2.U_Gorunur='Y' and " + condition + "(tbl.\"U_TerminalGizle\",'N') ='N'  GROUP BY tbl.\"BelgeNo\" ,T2.\"U_UrunKodu\",T2.\"U_UrunTanimi\", tbl.\"Aciklama\" ";

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
                                        dt.TableName = "CekmeListesiMusteriyeGoreUrun";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "ÜRÜN BULUNAMADI." };
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