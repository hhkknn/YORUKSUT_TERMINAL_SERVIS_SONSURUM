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
    public class GetBatchNumber
    {
        public Response getBatchNumberByCardCodeAndItemCode(string dbName, string CardCode, string ItemCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName,mKod);

                if (connstring != "")
                {
                    string query = "";
                    if (mKod =="20TRMN")
                    {
                        query = "SELECT ROW_NUMBER() OVER(ORDER BY T0.BatchNum) AS ID,T0.BatchNum,T1.MnfSerial, SUM(T0.Quantity) as Quantity FROM IBT1 T0 INNER JOIN OBTN T1 ON T0.ItemCode = T1.ItemCode AND T0.BatchNum = T1.DistNumber WHERE T0.CardCode ='" + CardCode + "' and T0.ItemCode='" + ItemCode + "' and (T0.BaseType = '13'  or T0.BaseType = '15') group by T0.BatchNum ,T1.MnfSerial";

                        if (CardCode == "")
                        {
                            query = "SELECT ROW_NUMBER() OVER(ORDER BY T0.BatchNum) AS ID,T0.BatchNum, T1.MnfSerial,SUM(T0.Quantity) as Quantity FROM IBT1 T0 INNER JOIN OBTN T1 ON T0.ItemCode = T1.ItemCode AND T0.BatchNum = T1.DistNumber WHERE T0.ItemCode='" + ItemCode + "' group by T0.BatchNum,T1.MnfSerial  ";
                        }
                    }
                    else
                    {
                        query = "SELECT ROW_NUMBER() OVER(ORDER BY T0.BatchNum) AS ID,T0.BatchNum, SUM(T0.Quantity) as Quantity FROM IBT1 T0 WHERE T0.CardCode ='" + CardCode + "' and T0.ItemCode='" + ItemCode + "' and (T0.BaseType = '13'  or T0.BaseType = '15') group by T0.BatchNum ";

                        if (CardCode == "")
                        {
                            query = "SELECT ROW_NUMBER() OVER(ORDER BY T0.BatchNum) AS ID,T0.BatchNum, SUM(T0.Quantity) as Quantity FROM IBT1 T0 WHERE T0.ItemCode='" + ItemCode + "' group by T0.BatchNum  ";
                        }
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
                                        dt.TableName = "BatchNum";

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