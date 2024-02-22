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
    public class GetBatchQuantity
    {
        public Response getBatchQuantity(string dbName, string warehouse, string BatchNumber, string ItemCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "select SUM(T1.Quantity) as Miktar from OBTN T0 inner join OBTQ T1 on T0.ItemCode = T1.ItemCode and T0.SysNumber = T1.SysNumber inner join OITM T2 on T0.ItemCode = T2.ItemCode where T1.Quantity > 0 and T0.DistNumber = N'" + BatchNumber + "' and T1.WhsCode = '" + warehouse + "' and T0.ItemCode = '" + ItemCode + "'";

                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "BatchSum";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "PARTİ NUMARASI BULUNAMADI." };
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

        public Response getBathByItemCode(string dbName, string warehouse, string ItemCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "select T0.ItemCode, T2.ItemName, T0.DistNumber as BatchNum,";
                    if (mKod == "20TRMN")
                    {
                        query += " T0.MnfSerial, ";
                    }
                    query += " T1.WhsCode,T3.WhsName, T1.Quantity ";
                    query += "from OBTN T0 ";
                    query += "inner join OBTQ T1 on T0.ItemCode = T1.ItemCode and T0.SysNumber = T1.SysNumber ";
                    query += "inner join OITM T2 on T0.ItemCode = T2.ItemCode ";
                    query += "inner join OWHS T3 on T1.WhsCode = T3.WhsCode ";
                    query += "where T1.Quantity > 0 and T0.ItemCode = '" + ItemCode + "' and T1.WhsCode = '" + warehouse + "' ";
                    query += "order by T1.WhsCode, T0.DistNumber";
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "BatchSum";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "PARTİ NUMARASI BULUNAMADI." };
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

        public Response getBathByItemCode_DepoYeri(string dbName, string warehouse, string ItemCode, string kaynakdepoYeriId, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select * from (select T0.ItemCode, T2.ItemName, T0.DistNumber as BatchNum,";
                    if (mKod == "20TRMN")
                    {
                        query += " T0.MnfSerial, ";
                    }
                    query += " T1.WhsCode,T3.WhsName, ISNULL(T4.OnHandQty,T1.Quantity) AS Quantity,T4.BinAbs,T5.BinCode ";
                    query += "from OBTN T0 ";
                    query += "inner join OBTQ T1 on T0.ItemCode = T1.ItemCode and T0.SysNumber = T1.SysNumber ";
                    query += "inner join OITM T2 on T0.ItemCode = T2.ItemCode ";
                    query += "inner join OWHS T3 on T1.WhsCode = T3.WhsCode ";
                    query += "LEFT JOIN OBBQ AS T4 ON T4.SnBMDAbs = T0.AbsEntry and T4.WhsCode = '" + warehouse + "' LEFT JOIN OBIN T5 ON T4.BinAbs = T5.AbsEntry and T5.WhsCode = '" + warehouse + "' ";
                    query += " where T1.Quantity > 0 and T0.ItemCode = '" + ItemCode + "' and T1.WhsCode = '" + warehouse + "') as tbl where 1=1  and tbl.Quantity >0 ";
                    if (kaynakdepoYeriId != "")
                    {
                        query += " and tbl.BinAbs = '" + kaynakdepoYeriId + "'";
                    }
                    query += "order by tbl.WhsCode, tbl.BatchNum";
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "BatchSum";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "PARTİ NUMARASI BULUNAMADI." };
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

        public Response getBatchByBatchNumber(string dbName, string BatchNumber, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "select T0.ItemCode as \"Kalem Kodu\", T2.ItemName as \"Kalem Tanımı\", T0.DistNumber as \"PartiNo\", T1.WhsCode as \"Depo Kodu\",T3.WhsName as \"Depo Adı\", T1.Quantity as \"Miktar\", T4.UomCode as \"Birim\" ";
                    query += "from OBTN T0 ";
                    query += "inner join OBTQ T1 on T0.ItemCode = T1.ItemCode and T0.SysNumber = T1.SysNumber ";
                    query += "inner join OITM T2 on T0.ItemCode = T2.ItemCode inner join OUOM T4 on T2.InvntryUom = T4.UomName ";
                    query += "inner join OWHS T3 on T1.WhsCode = T3.WhsCode ";
                    query += "where T1.Quantity > 0 and T0.DistNumber = N'" + BatchNumber + "'";
                    query += "order by T1.WhsCode, T0.DistNumber";
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "BatchSum";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "PARTİ NUMARASI BULUNAMADI." };
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

        public Response getBathByItemCodeToDraftDocument(string dbName, string warehouse, List<string> itemCodes, int DocEntry, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string query = "";

                    //"select T0.ItemCode, T2.ItemName, T0.DistNumber as BatchNum, T1.WhsCode,T3.WhsName, T1.Quantity ";
                    //query += "from OBTN T0 ";
                    //query += "inner join OBTQ T1 on T0.ItemCode = T1.ItemCode and T0.SysNumber = T1.SysNumber ";
                    //query += "inner join OITM T2 on T0.ItemCode = T2.ItemCode ";
                    //query += "inner join OWHS T3 on T1.WhsCode = T3.WhsCode ";
                    //query += "where T1.Quantity > 0 and T0.ItemCode = '" + ItemCode + "' and T1.WhsCode = '" + warehouse + "' ";
                    //query += "order by T1.WhsCode, T0.DistNumber";

                    query = "SELECT DRF1.ItemCode, OITM.ItemName,obtn.distnumber as BatchNum,";
                    if (mKod == "20TRMN")
                    {
                        query += "obtn.mnfserial AS MnfSerial,";
                    }
                    query += "(Select TOP 1 WhsCode from OBTQ as T11 where T11.ItemCode = DRF1.ItemCode and T11.SysNumber = obtn.SysNumber) as WhsCode, ";
                    query += "(Select TOP 1 WhsName from OBTQ as T11 INNER JOIN OWHS as T22 ON T11.WhsCode = T22.WhsCode where T11.ItemCode = DRF1.ItemCode and T11.SysNumber = obtn.SysNumber) as WhsCode, ";
                    //query += "(Select TOP 1 Quantity from OBTQ as T11 where T11.ItemCode = DRF1.ItemCode and T11.SysNumber = obtn.SysNumber) as Quantity, ";
                    //query +=" CASE WHEN ISNULL(DRF1.U_AcikMiktar, 0) > 0 THEN ISNULL(DRF1.U_AcikMiktar, 0) ELSE DRF16.Quantity END Quantity,";

                    #region sorgu yavaş geldiğinden bu kısım kaldırıldı 20221103
                    query += " DRF16.Quantity  AS Quantity,";
                    //query += "DRF16.Quantity - (Select ISNULL(SUM(T97.Quantity),0) from OWTR as t99 INNER JOIN WTR1 as T98 ON T99.DocEntry = T98.DocEntry LEFT JOIN IBT1 AS T97 ON T97.BaseEntry = odrf.DocEntry and T97.BaseType = '67' and T97.LineNum = T98.LineNum ";

                    //query += " and T97.BatchNum = obtn.DistNumber ";

                    //query += " where t99.draftKey = odrf.DocEntry) AS Quantity,"; 
                    #endregion

                    query += "odrf.docentry,DRF1.LineNum  ";
                    query += "FROM odrf JOIN DRF1 ON odrf.docentry = DRF1.docentry ";
                    query += "LEFT JOIN DRF16 ON DRF1.docentry = DRF16.absentry AND DRF1.linenum = drf16.linenum ";
                    query += "LEFT JOIN obtn ON DRF16.objabs = obtn.absentry ";
                    query += "INNER JOIN OITM ON OITM.ItemCode = DRF1.ItemCode ";
                    query += "where DRF1.dscription IS NOT NULL AND odrf.objtype = 67  and";
                    query += " odrf.DocEntry = '" + DocEntry + "'";

                    if (itemCodes != null)
                    {
                        if (itemCodes.Count > 0)
                        {
                            var values = "";
                            foreach (var item in itemCodes)
                            {
                                values += "'" + item + "'" + ",";
                            }

                            if (values != "")
                            {
                                values = values.Remove(values.Length - 1, 1);
                            }

                            query += " and (DRF1.ItemCode IN (" + values + "))";
                        }
                    }
                    //query += "order by T3OBTQ.WhsCode, obtn.DistNumber";
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "BatchSum";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "PARTİ NUMARASI BULUNAMADI." };
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

        public Response getBathByItemCodeToDraftDocument_DepoYerli(string dbName, string warehouse, List<string> itemCodes, int DocEntry, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string query = "";

                    //"select T0.ItemCode, T2.ItemName, T0.DistNumber as BatchNum, T1.WhsCode,T3.WhsName, T1.Quantity ";
                    //query += "from OBTN T0 ";
                    //query += "inner join OBTQ T1 on T0.ItemCode = T1.ItemCode and T0.SysNumber = T1.SysNumber ";
                    //query += "inner join OITM T2 on T0.ItemCode = T2.ItemCode ";
                    //query += "inner join OWHS T3 on T1.WhsCode = T3.WhsCode ";
                    //query += "where T1.Quantity > 0 and T0.ItemCode = '" + ItemCode + "' and T1.WhsCode = '" + warehouse + "' ";
                    //query += "order by T1.WhsCode, T0.DistNumber";

                    query = "SELECT DRF1.ItemCode, OITM.ItemName,obtn.distnumber as BatchNum,";
                    if (mKod == "20TRMN")
                    {
                        query += "obtn.mnfserial AS MnfSerial,";
                    }
                    query += "(Select TOP 1 WhsCode from OBTQ as T11 where T11.ItemCode = DRF1.ItemCode and T11.SysNumber = obtn.SysNumber) as WhsCode, ";
                    query += "(Select TOP 1 WhsName from OBTQ as T11 INNER JOIN OWHS as T22 ON T11.WhsCode = T22.WhsCode where T11.ItemCode = DRF1.ItemCode and T11.SysNumber = obtn.SysNumber) as WhsCode, ";
                    //query += "(Select TOP 1 Quantity from OBTQ as T11 where T11.ItemCode = DRF1.ItemCode and T11.SysNumber = obtn.SysNumber) as Quantity, ";
                    //query +=" CASE WHEN ISNULL(DRF1.U_AcikMiktar, 0) > 0 THEN ISNULL(DRF1.U_AcikMiktar, 0) ELSE DRF16.Quantity END Quantity,";
                    query += "DRF19.Quantity AS Quantity, ";
                    query += "odrf.docentry,DRF1.LineNum, ";

                    //query+=" DRF19.BinAbs as KaynakDepoId, T5.BinCode as KaynakDepoAdi, ";

                    query += "(Select TT.BinAbs from DRF19 as TT where TT.DocEntry = ODRF.DocEntry and TT.BinActTyp = '2') as KaynakDepoId,(Select TZ.BinCode from DRF19 as TT INNER JOIN OBIN AS TZ ON TT.BinAbs = TZ.AbsEntry where TT.DocEntry = ODRF.DocEntry and TT.BinActTyp = '2' ) as KaynakDepoAdi, ";
                    query += "(Select TT.BinAbs from DRF19 as TT where TT.DocEntry = ODRF.DocEntry and TT.BinActTyp = '1') as HedefDepoId,(Select TZ.BinCode from DRF19 as TT INNER JOIN OBIN AS TZ ON TT.BinAbs = TZ.AbsEntry where TT.DocEntry = ODRF.DocEntry and TT.BinActTyp = '1' ) as HedefDepoAdi ";
                    query += "FROM odrf JOIN DRF1 ON odrf.docentry = DRF1.docentry ";
                    query += "LEFT JOIN DRF16 ON DRF1.docentry = DRF16.absentry AND DRF1.linenum = drf16.linenum ";
                    query += "LEFT JOIN obtn ON DRF16.objabs = obtn.absentry ";
                    query += "INNER JOIN OITM ON OITM.ItemCode = DRF1.ItemCode ";
                    query += "INNER JOIN DRF19 ON ODRF.DocEntry = DRF19.DocEntry and  DRF19.BinActTyp=1";
                    query += "INNER JOIN OBIN T5 ON DRF19.BinAbs = T5.AbsEntry ";
                    query += "where DRF1.dscription IS NOT NULL AND odrf.objtype = 67  and";
                    query += " odrf.DocEntry = '" + DocEntry + "'";
                    //query += " and DRF19.BinActTyp = '2' ";
                    if (itemCodes != null)
                    {
                        if (itemCodes.Count > 0)
                        {
                            var values = "";
                            foreach (var item in itemCodes)
                            {
                                values += "'" + item + "'" + ",";
                            }

                            if (values != "")
                            {
                                values = values.Remove(values.Length - 1, 1);
                            }

                            query += " and (DRF1.ItemCode IN (" + values + "))";
                        }
                    }
                    //query += "order by T3OBTQ.WhsCode, obtn.DistNumber";
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "BatchSum";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "PARTİ NUMARASI BULUNAMADI." };
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

        public Response getBatchByItemCodePalet(string dbName, string warehouse, string ItemCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string query = "";

                    query += "SELECT T0.\"U_PaletNo\",T1.\"U_Barkod\",T1.\"U_KalemKodu\" as \"ItemCode\",T1.\"U_Tanim\" as \"ItemName\",T1.\"U_PartiNo\" as \"BatchNum\",T1.\"U_DepoKodu\" as \"WhsCode\",T1.\"U_DepoAdi\" as \"WhsName\",T1.\"U_Miktar\" as \"Quantity\" FROM \"@AIF_WMS_PALET\" T0 ";
                    query += "INNER JOIN \"@AIF_WMS_PALET2\" T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" ";
                    query += "WHERE T1.\"U_KalemKodu\" = '" + ItemCode + "' AND T1.\"U_DepoKodu\" = '" + warehouse + "' "; 
                    //query += "order by T1.U_KalemKodu";
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "PaletPartileri";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "PARTİ NUMARASI BULUNAMADI." };
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