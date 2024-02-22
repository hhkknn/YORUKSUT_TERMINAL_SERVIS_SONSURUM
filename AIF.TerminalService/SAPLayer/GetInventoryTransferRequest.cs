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
    public class GetInventoryTransferRequest
    {
        public Response getInventoryTransferRequestByDate(string dbName, string startDate, string endDate, string fromWhsCode, string toWhsCode, List<string> WhsCodes, string kullaniciKodu, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "select DISTINCT T0.\"DocEntry\" as \"BelgeNumarasi\",T0.\"CardCode\" as \"MuhatapKodu\", T0.\"CardName\" as \"MuhatapAdi\",CONVERT(varchar, T0.\"DocDueDate\", 103) as \"TransferTarihi\",CONVERT(varchar, T0.\"TaxDate\", 103) as \"BelgeTarihi\",T0.\"Filler\" as \"KaynakDepo\",T0.\"ToWhsCode\" as \"HedefDepo\",'AÇIK' as \"BelgeDurumu\" from OWTQ as T0 ";
                    //start date ve enddate filtresi,onat beyin isteğine istinaden transfertarihi yapıldı

                    query += "INNER JOIN WTQ1 as T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" ";

                    query += "INNER JOIN \"@AIF_WMS_USRWHS\" AS T3 ON T3.\"U_KullaniciKodu\" = '" + kullaniciKodu + "' "; //giren kullanıcıya göre filtrelemesi için eklendi 02.06.2022 chn 

                    query += "where T0.\"DocDueDate\" >= '" + startDate + "' and T0.\"DocDueDate\" <= '" + endDate + "' and T0.DocStatus='O' and T0.\"DocType\" = 'I' ";
                    if (fromWhsCode != "")
                    {
                        query += "and \"Filler\" = '" + fromWhsCode + "'";
                    }

                    if (toWhsCode != "")
                    {
                        query += "and \"ToWhsCode\" = '" + toWhsCode + "'";
                    }
                    if (WhsCodes != null)
                    {
                        if (WhsCodes.Count > 0)
                        {
                            #region Hedef depo ve kaynak depoya or ile ortak bakar. 19.9.22 Hakan
                            //var values = "";
                            //foreach (var item in WhsCodes)
                            //{
                            //    values += "'" + item + "'" + ",";
                            //}

                            //if (values != "")
                            //{
                            //    values = values.Remove(values.Length - 1, 1);
                            //}

                            //query += " and (T1.FromWhsCod IN (" + values + ") OR T1.WhsCode IN (" + values + "))  and T1.\"LineStatus\"='O'  and T1.OpenQty > 0 ";
                            ////query += " and (T1.FromWhsCod IN (" + values + ") OR T1.WhsCode IN (" + values + "))  and T1.\"LineStatus\"='O'"; //yetki olmayan depolar da geliyordu.kaldırıldı 03.06.2022 chn 
                            #endregion

                            var values = "";
                            foreach (var item in WhsCodes)
                            {
                                if (item.StartsWith("K-"))
                                {
                                    values += "'" + item.Replace("K-", "") + "'" + ",";
                                }
                            }

                            bool FromWhsCodVar = false;
                            if (values != "")
                            {
                                values = values.Remove(values.Length - 1, 1);
                                query += " and (T1.FromWhsCod IN (" + values + ") ";
                                FromWhsCodVar = true;
                            }



                            values = "";

                            foreach (var item in WhsCodes)
                            {
                                if (item.StartsWith("H-"))
                                {
                                    values += "'" + item.Replace("H-", "") + "'" + ",";
                                }
                            }

                            if (values != "")
                            {
                                values = values.Remove(values.Length - 1, 1);
                                if (FromWhsCodVar)
                                {
                                    query += " OR T1.WhsCode IN(" + values + "))";
                                }
                                else
                                {
                                    query += " AND T1.WhsCode IN(" + values + ")";
                                }
                            }
                        }
                        else
                        {
                            query += " and T1.FromWhsCod IN (-999) and T1.WhsCode IN (-999) ";
                        }
                        query += " and T1.\"LineStatus\"='O'  and T1.OpenQty > 0 ";
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
                                        dt.TableName = "StokNakliTransfer";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "STOK NAKLİ TALEBİ BULUNAMADI." };
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


        public Response getInventoryTransferRequestDetails(string dbName, List<string> docEntryList, List<string> WhsCodes, string kullaniciKodu, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string docentryvalList = "";

                    foreach (var item in docEntryList)
                    {
                        docentryvalList += item + ",";
                    }

                    if (docentryvalList.Length > 1)
                    {
                        docentryvalList = docentryvalList.Remove(docentryvalList.Length - 1, 1);
                    }

                    var query = "select T0.DocEntry as [BelgeNumarasi], T0.ItemCode as [KalemKodu], T0.Dscription as [KalemAdi], T0.OpenQty as [Miktar], T0.UomCode as [OlcuBirimi],T1.\"ManBtchNum\" as [Partili],T1.\"CodeBars\" as [Barkod],T0.\"FromWhsCod\" as \"DepoKodu\",T0.\"WhsCode\" as \"HedefDepo\",\"LineNum\" as \"SiraNo\",T2.\"OnHand\" as \"DepoMiktar\" ";


                    if (mKod == "10TRMN" || mKod == "20TRMN")
                    {
                        query += ",T1.U_KoliMiktari as \"KoliIciAD\" ,T1.U_PALET as \"PaletIciAD\",T1.U_PaletKoli  as \"PaletIciKoliAD\", ";
                    }
                    else
                    {
                        query += ",0 as \"KoliIciAD\" ,0 as \"PaletIciAD\",0 as \"PaletIciKoliAD\", ";
                    }
                    query += " (Select SUM(T99.\"Quantity\") from DRF1 AS T99 INNER JOIN ODRF AS T98 ON T99.\"DocEntry\" = T98.\"DocEntry\" where T99.\"BaseLine\" = T0.\"LineNum\" and T98.\"DocStatus\" = 'O' and T99.\"LineStatus\" = 'O' and T99.\"BaseEntry\" = T0.\"DocEntry\") as \"OnaydaBekleyenMiktar\" ";

                    if (mKod == "10TRMN" || mKod == "20TRMN")
                    {
                        query += ",\"U_UretimdenGonderildi\" as \"UretimdenGonderildi\", ";
                    }
                    else
                    {
                        query += ",'' as \"UretimdenGonderildi\",";
                    }

                    if (mKod == "10TRMN" || mKod == "20TRMN")
                    {
                        query += "cast('' as varchar(254)) as \"DepoYeriAdi\",cast('' as varchar(254)) as \"HDepoYeriAdi\",cast('' as varchar(254)) as \"HDepoYeriId\",";
                    }
                    else if (mKod == "30TRMN")
                    {
                        query += "T0.\"U_KaynakDYeri\" as \"DepoYeriAdi\",T0.\"U_HedefDYeri\" as \"HDepoYeriAdi\",(SELECT T67.\"AbsEntry\" FROM OBIN as T67 where T67.\"BinCode\" = T0.\"U_KaynakDYeri\") as \"DepoYeriId\",(SELECT T77.\"AbsEntry\" FROM OBIN as T77 where T77.\"BinCode\" = T0.\"U_HedefDYeri\") as \"HDepoYeriId\",";
                    }
                    query += "(Select \"WhsName\" from \"OWHS\" as t99 where t99.\"WhsCode\" = T0.\"WhsCode\") as \"HedefDepoAdi\",(Select \"WhsName\" from \"OWHS\" as t99 where t99.\"WhsCode\" = T0.\"FromWhsCod\") as \"DepoAdi\" from WTQ1 as T0 ";

                    query += "INNER JOIN OITM as T1 ON T0.ItemCode = T1.ItemCode ";
                    query += "INNER JOIN OITW as T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T2.\"WhsCode\" = T0.\"FromWhsCod\" ";
                    query += "INNER JOIN OWTQ as T4 ON T0.\"DocEntry\" = T4.\"DocEntry\" ";

                    query += "INNER JOIN \"@AIF_WMS_USRWHS\" AS T34 ON T34.\"U_KullaniciKodu\" = '" + kullaniciKodu + "' "; //giren kullanıcıya göre filtrelemesi için eklendi 02.06.2022 chn 

                    query += "where T0.DocEntry IN (" + docentryvalList + ") AND T4.DocStatus='O' and T0.\"OpenQty\" > 0 ";


                    if (WhsCodes != null)
                    {
                        #region Hedef depo ve kaynak depoya or ile ortak bakar. 19.9.22 Hakan
                        //if (WhsCodes.Count > 0)
                        //{
                        //    var values = "";
                        //    foreach (var item in WhsCodes)
                        //    {
                        //        values += "'" + item + "'" + ",";
                        //    }

                        //    if (values != "")
                        //    {
                        //        values = values.Remove(values.Length - 1, 1);
                        //    }

                        //    query += " and (T0.FromWhsCod IN (" + values + ") OR T0.WhsCode IN (" + values + "))";

                        //} 
                        #endregion

                        var values = "";
                        foreach (var item in WhsCodes)
                        {
                            if (item.StartsWith("K-"))
                            {
                                values += "'" + item.Replace("K-", "") + "'" + ",";
                            }
                        }

                        bool FromWhsCodVar = false;
                        if (values != "")
                        {
                            values = values.Remove(values.Length - 1, 1);
                            query += " and (T0.FromWhsCod IN (" + values + ") ";
                            FromWhsCodVar = true;
                        }



                        values = "";

                        foreach (var item in WhsCodes)
                        {
                            if (item.StartsWith("H-"))
                            {
                                values += "'" + item.Replace("H-", "") + "'" + ",";
                            }
                        }

                        if (values != "")
                        {
                            values = values.Remove(values.Length - 1, 1);
                            if (FromWhsCodVar)
                            {
                                query += " OR T0.WhsCode IN(" + values + "))";
                            }
                            else
                            {
                                query += " AND T0.WhsCode IN(" + values + ")";
                            }
                        }
                    }
                    else
                    {
                        query += " and T0.FromWhsCod IN (-999) and T0.WhsCode IN (-999) ";
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
                                        dt.TableName = "StokNakliList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "STOK NAKLİ TALEBİ BULUNAMADI." };
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

        public Response getDraftInventoryTransferRequestByDate(string dbName, string startDate, string endDate, List<string> WhsCodes, string kullaniciKodu, string talepsizDepoNaklindeTaslakBelgeOlustur, string fromWhsCode, string toWhsCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "select DISTINCT ISNULL(T1.\"BaseEntry\",T1.\"DocEntry\") as \"TalepNumarasi\",T0.\"DocEntry\" as \"BelgeNumarasi\",T0.\"CardCode\" as \"MuhatapKodu\", T0.\"CardName\" as \"MuhatapAdi\",CONVERT(varchar, T0.\"DocDueDate\", 103) as \"TransferTarihi\",CONVERT(varchar, T0.\"TaxDate\", 103) as \"BelgeTarihi\",T1.\"FromWhsCod\" AS \"KaynakDepo\",T1.\"WhsCode\"as \"HedefDepo\",'AÇIK' as \"BelgeDurumu\",case when ISNULL(T1.\"BaseEntry\",'-99') = '-99' then 'T' else 'G' end as TaslakGercek from ODRF as T0 ";

                    query += "INNER JOIN DRF1 as T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" ";
                    query += "LEFT JOIN OWTQ AS T2 ON T1.\"BaseEntry\" = T2.\"DocEntry\" ";

                    query += "INNER JOIN \"@AIF_WMS_USRWHS\" AS T3 ON T3.\"U_KullaniciKodu\" = '" + kullaniciKodu + "' "; //giren kullanıcıya göre filtrelemesi için eklendi 02.06.2022 chn 

                    query += "where T0.\"DocDueDate\" >= '" + startDate + "' and T0.\"DocDueDate\" <= '" + endDate + "' and T0.DocStatus='O' and T0.\"DocType\" = 'I' and T1.\"ObjType\" = '67' ";


                    if (talepsizDepoNaklindeTaslakBelgeOlustur != "Y")
                    {
                        query += "and T1.OpenQty > 0 ";
                    }
                    else
                    {
                        query += "AND CASE WHEN ISNULL(T1.U_AcikMiktar, 0) > 0 THEN case when ISNULL(T1.U_AcikMiktar, 0) <> T1.Quantity then T1.Quantity - ISNULL(T1.U_AcikMiktar, 0) else T1.Quantity end ELSE ISNULL(T1.U_AcikMiktar, 0) END > 0 ";
                    }


                    if (fromWhsCode != "")
                    {
                        query += "and T0.\"Filler\" = '" + fromWhsCode + "'";
                    }

                    if (toWhsCode != "")
                    {
                        query += "and T0.\"ToWhsCode\" = '" + toWhsCode + "'";
                    }
                    if (WhsCodes != null)
                    {
                        #region Hedef depo ve kaynak depoya or ile ortak bakar. 19.9.22 Hakan
                        //if (WhsCodes.Count > 0)
                        //{
                        //    var values = "";
                        //    foreach (var item in WhsCodes)
                        //    {
                        //        values += "'" + item + "'" + ",";
                        //    }

                        //    if (values != "")
                        //    {
                        //        values = values.Remove(values.Length - 1, 1);
                        //    }

                        //    //query += " and (T0.Filler IN (" + values + ") OR T0.ToWhsCode IN (" + values + "))";
                        //    query += " and (T1.FromWhsCod IN (" + values + ") OR T1.WhsCode IN (" + values + "))"; //parametre seçili olmayan depoları da getiriyordu kaldırıldı. 03.06.2022 chn

                        //} 
                        #endregion


                        if (WhsCodes.Count > 0)
                        {
                            var values = "";
                            foreach (var item in WhsCodes)
                            {
                                if (item.StartsWith("K-"))
                                {
                                    values += "'" + item.Replace("K-", "") + "'" + ",";
                                }
                            }

                            bool FromWhsCodVar = false;
                            if (values != "")
                            {
                                values = values.Remove(values.Length - 1, 1);
                                query += " and (T1.FromWhsCod IN (" + values + ") ";
                                FromWhsCodVar = true;
                            }



                            values = "";

                            foreach (var item in WhsCodes)
                            {
                                if (item.StartsWith("H-"))
                                {
                                    values += "'" + item.Replace("H-", "") + "'" + ",";
                                }
                            }

                            if (values != "")
                            {
                                values = values.Remove(values.Length - 1, 1);
                                if (FromWhsCodVar)
                                {
                                    query += " OR T1.WhsCode IN(" + values + "))";
                                }
                                else
                                {
                                    query += " AND T1.WhsCode IN(" + values + ")";
                                }
                            }
                        }
                        else
                        {
                            query += " and T1.FromWhsCod IN (-999) and T1.WhsCode IN (-999) ";
                        }
                    }

                    query += "  and T1.\"LineStatus\"='O' ";

                    if (talepsizDepoNaklindeTaslakBelgeOlustur == "Y")
                    {
                        query += " AND ISNULL(T2.\"DocStatus\",'O') = 'O' ";
                    }
                    else
                    {
                        query += " and T2.\"DocStatus\" = 'O' ";
                        query += " and (Select \"LineStatus\" from WTQ1 as T3 where T3.\"DocEntry\" = T2.\"DocEntry\" and T3.LineNum = T1.\"BaseLine\") = 'O'";
                    }


                    query += " group by T1.\"BaseEntry\",ISNULL(T1.\"BaseEntry\", T1.\"DocEntry\"),T0.\"DocEntry\",T0.\"CardCode\", T0.\"CardName\",CONVERT(varchar, T0.\"DocDueDate\", 103) ,CONVERT(varchar, T0.\"TaxDate\", 103),T1.\"FromWhsCod\",T1.\"WhsCode\"";

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
                                        dt.TableName = "StokNakliTransfer";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "STOK NAKLİ TALEBİ BULUNAMADI." };
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


        public Response getDraftInventoryTransferRequestDetails(string dbName, List<string> docEntryList, List<string> WhsCodes, string kullaniciKodu, string talepsizDepoNaklindeTaslakBelgeOlustur, string fromwhscode, string towhscode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string docentryvalList = "";

                    foreach (var item in docEntryList)
                    {
                        docentryvalList += item + ",";
                    }

                    if (docentryvalList.Length > 1)
                    {
                        docentryvalList = docentryvalList.Remove(docentryvalList.Length - 1, 1);
                    }

                    var query = "select T0.DocEntry as [BelgeNumarasi], T0.ItemCode as [KalemKodu], T0.Dscription as [KalemAdi], ISNULL(T0.U_AcikMiktar,0) AS[Miktar],T0.UomCode as [OlcuBirimi],T1.\"ManBtchNum\" as [Partili],T1.\"CodeBars\" as [Barkod],T0.\"FromWhsCod\" as \"DepoKodu\",T0.\"WhsCode\" as \"HedefDepo\",\"LineNum\" as \"SiraNo\",T2.\"OnHand\" as \"DepoMiktar\",T1.\"U_KoliMiktari\" as \"KoliIciAD\",T1.\"U_PALET\" as \"PaletIciAD\",T1.\"U_PaletKoli\" as \"PaletIciKoliAD\",case when ISNULL(T0.\"BaseEntry\",'-99') = '-99' then 'T' else 'G' end as TaslakGercek from DRF1 as T0  ";

                    query += "INNER JOIN OITM as T1 ON T0.ItemCode = T1.ItemCode ";
                    query += "INNER JOIN OITW as T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T2.\"WhsCode\" = T0.\"FromWhsCod\" ";
                    query += "INNER JOIN ODRF as T4 ON T0.\"DocEntry\" = T4.\"DocEntry\" ";

                    query += "INNER JOIN \"@AIF_WMS_USRWHS\" AS T34 ON T34.\"U_KullaniciKodu\" = '" + kullaniciKodu + "' "; //giren kullanıcıya göre filtrelemesi için eklendi 02.06.2022 chn 

                    query += "where T0.DocEntry IN (" + docentryvalList + ") and T4.DocStatus='O' and T0.\"ObjType\" = '67' ";

                    if (talepsizDepoNaklindeTaslakBelgeOlustur != "Y")
                    {
                        query += " and T0.\"OpenQty\" > 0  ";
                    }

                    if (WhsCodes != null)
                    {
                        #region Hedef depo ve kaynak depoya or ile ortak bakar. 19.9.22 Hakan
                        //if (WhsCodes.Count > 0)
                        //{
                        //    var values = "";
                        //    foreach (var item in WhsCodes)
                        //    {
                        //        values += "'" + item + "'" + ",";
                        //    }

                        //    if (values != "")
                        //    {
                        //        values = values.Remove(values.Length - 1, 1);
                        //    }

                        //    query += " and (T0.FromWhsCod IN (" + values + ") OR T0.WhsCode IN (" + values + "))";

                        //} 
                        #endregion

                        if (WhsCodes.Count > 0)
                        {
                            var values = "";
                            foreach (var item in WhsCodes)
                            {
                                if (item.StartsWith("K-"))
                                {
                                    values += "'" + item.Replace("K-", "") + "'" + ",";
                                }
                            }

                            bool FromWhsCodVar = false;
                            if (values != "")
                            {
                                values = values.Remove(values.Length - 1, 1);
                                query += " and (T0.FromWhsCod IN (" + values + ") ";
                                FromWhsCodVar = true;
                            }



                            values = "";

                            foreach (var item in WhsCodes)
                            {
                                if (item.StartsWith("H-"))
                                {
                                    values += "'" + item.Replace("H-", "") + "'" + ",";
                                }
                            }

                            if (values != "")
                            {
                                values = values.Remove(values.Length - 1, 1);
                                if (FromWhsCodVar)
                                {
                                    query += " OR T0.WhsCode IN(" + values + "))";
                                }
                                else
                                {
                                    query += " AND T0.WhsCode IN(" + values + ")";
                                }
                            }
                        }
                        else
                        {
                            query += " and T0.FromWhsCod IN (-999) and T0.WhsCode IN (-999) ";
                        }
                    }
                    else
                    {
                        if (fromwhscode != "")
                        {
                            query += " and (T0.FromWhsCod IN (" + fromwhscode + "))";
                        }

                        if (towhscode != "")
                        {
                            query += " and (T0.WhsCode IN (" + towhscode + "))";

                        }
                    }
                    query += " AND U_AcikMiktar>0";

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
                                        dt.TableName = "StokNakliList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "STOK NAKLİ TALEBİ BULUNAMADI." };
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