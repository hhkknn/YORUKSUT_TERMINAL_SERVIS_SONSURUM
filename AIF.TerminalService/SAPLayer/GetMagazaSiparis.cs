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
    public class GetMagazaSiparis
    {
        public Response getMagazaSiparis(string dbName, string subeid,string kapalilariListele, string baslangic, string bitis, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string sql = "";

                    sql = "select (Select \"BPLName\" from OBPL as T1 where T1.\"BPLId\" = T0.\"U_Branchid\") as \"Magaza\",\"U_Branchid\" as Subeid, ";
                    sql += "T0.\"U_OrderDate\" as \"Tarih\",(select \"Descr\" from UFD1 as T2 where T2.\"FieldID\" = (Select \"FieldID\" from CUFD where \"AliasID\"='Status' and ";
                    sql += "\"TableID\"='@RT_ORDR') and T2.\"TableID\"='@RT_ORDR' and T2.\"FldValue\"=T0.\"U_Status\") as \"Durum\", ";
                    sql += "T0.\"U_OrderRefNo\" as \"SipariSRefNo\",T0.\"U_OrderTermDate\" as \"TerminTarihi\",T0.\"U_Status\" as \"DurumNo\"  from \"@RT_ORDR\" as T0 where 1=1 and \"U_Branchid\"='" + subeid + "'  and  (Select Count(T2.DocEntry) from WTR1 as T2 where T2.\"BaseEntry\" = T0.\"U_StockTransferNo\" AND T2.\"BaseType\" = '1250000001') > 0 ";

                    if (kapalilariListele == "Y")
                    {
                        sql += " and (\"U_Status\" = '2' OR  \"U_Status\" = '4')";
                    }
                    else
                    {
                        sql += " and \"U_Status\" = '2' and T0.\"U_OrderDate\" >='" + baslangic + "' and T0.\"U_OrderDate\" <= '" + bitis + "'";
                    }
                     
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "MagazaSiparis";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "SİPARİŞ BULUNAMADI." };
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
            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response getMagazaSiparisSecimDetay(string dbName, string siparisRefNo, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string sql = "";

                    sql = "Select T0.\"U_OrderRefNo\",T0.\"U_OrderDate\",T1.\"U_CodeBar\",T1.\"U_ItemCode\",T1.\"U_ItemName\",T1.\"U_Quantity\", ";
                    sql += "(Select ISNULL(SUM(T3.\"U_AppQuantity\"),0) from \"@RT_ORDRAPR\" as T2 INNER JOIN \"@RT_ORDRAPR1\" as T3 ON T2.\"Code\"=T3.\"Code\" ";
                    sql += "where T0.\"U_OrderRefNo\" = T2.\"U_OrderRefNo\" and T3.\"U_ItemCode\"=T1.\"U_ItemCode\")as Onaylanan, ";

                    #region kabul edilen miktar ordracp den getirildi
                    sql += "(Select ISNULL(SUM(T5.\"U_AcceptedQty\"),0) from \"@RT_ORDRACP\" as T4 INNER JOIN \"@RT_ORDRACP1\" as T5 ON T4.\"Code\"=T5.\"Code\" ";
                    sql += "where T0.\"U_OrderRefNo\" = T4.\"U_OrderRefNo\" and T5.\"U_ItemCode\"=T1.\"U_ItemCode\" and T5.\"U_LineId\" = T1.\"U_LineId\")as KabulEdilen, ";
                    #endregion

                    sql += " \"U_Branchid\",\"U_LineId\" as \"SiraNumarasi\",T0.\"U_Status\" as \"Durum\" from \"@RT_ORDR\" as T0 INNER JOIN \"@RT_ORDR1\" as T1 ON T0.\"Code\" = T1.\"Code\"  ";
                    sql += " where T0.\"U_OrderRefNo\"='" + siparisRefNo + "'  ";

                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "MagazaSiparisSecimDetay";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "SİPARİŞ BULUNAMADI." };
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
            }
            return new Response { _list = dt, Val = 0 };
        }
    }
}