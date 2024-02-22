using AIF.TerminalService.DatabaseLayer;
using AIF.TerminalService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{
    public class GetInvoices
    {
        public Response getInvoices(string dbName, string startDate, string endDate, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString getConnectionString = new GetConnectionString();
                string connstring = getConnectionString.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "SELECT DISTINCT T0.\"DocEntry\" as 'Fatura No',CONVERT(varchar, T0.\"DocDate\", 103) as 'Fatura Tarihi',T0.\"CardCode\" as 'Müşteri Kodu',T0.\"CardName\" as 'Müşteri Adı' FROM [dbo].[OINV]  T0 inner join INV1 T1 on T0.DocEntry = T1.DocEntry WHERE T0.\"DocDate\" >='" + startDate + "' and  T0.\"DocDate\" <='" + endDate + "' and T0.\"DocStatus\" = 'O' and T1.\"LineStatus\" = 'O' AND T0.DocType = 'I' ORDER BY T0.DocEntry ASC";

                    try
                    {
                        using (SqlConnection sqlConnection = new SqlConnection(connstring))
                        { 
                            using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                            {
                                lock (sqlConnection)
                                {
                                    sqlCommand.CommandType = CommandType.Text;
                                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                                    {
                                        using (dt = new DataTable())
                                        {
                                            sqlDataAdapter.Fill(dt);
                                            dt.TableName = "MusteriFaturaIadesi";
                                            if (dt.Rows.Count == 0)
                                            {
                                                return new Response { _list = null, Val = -222, Desc = "MÜŞTERİ FATURA İADESİ BULUNAMADI." };

                                            }
                                        }
                                    }
                                }
                            }

                        }


                    }
                    catch (Exception)
                    {


                    }
                }


            }
            catch (Exception)
            {
            }

            return new Response { _list = dt, Val = 0 };
        }

        public Response getInvoicesDetails(string dbName, List<string> docEntryList, string mKod)
        {
            DataTable dt = new DataTable();

            try
            {
                GetConnectionString getConnectionString = new GetConnectionString();
                string connstring = getConnectionString.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string docentryvalList = "";

                    foreach (var item in docEntryList)
                    {
                        docentryvalList += item + ",";
                        if (docentryvalList.Length > 1)
                        {
                            docentryvalList = docentryvalList.Remove(docentryvalList.Length - 1, 1);
                        }

                        var query = "SELECT T0.\"DocEntry\" as 'Fatura No',T1.\"ItemCode\" as 'Kalem Kodu',T1.\"Dscription\" as 'Kalem Adı',T1.\"OpenCreQty\" as 'Miktar', T1.\"UomCode\" as 'Birim',\"LineNum\" as 'Sıra' FROM OINV T0  INNER JOIN INV1 T1 ON T0.[DocEntry] = T1.[DocEntry] where T0.DocEntry IN (" + docentryvalList + ") and T1.\"LineStatus\" = 'O' AND T0.DocType = 'I'";

                        try
                        {
                            using (SqlConnection sqlConnection = new SqlConnection(connstring))
                            {
                                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                                {
                                    sqlCommand.CommandType = CommandType.Text;
                                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                                    {
                                        using (dt = new DataTable())
                                        {
                                            sqlDataAdapter.Fill(dt);
                                            dt.TableName = "FaturaDetayList";

                                            if (dt.Rows.Count == 0)
                                            {
                                                return new Response { _list = null, Val = -567, Desc = "FATURA BULUNAMADI." };
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            return new Response { _list = null, Val = -9996, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -9995, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
            }
            return new Response { _list = dt, Val = 0 };
        }
    }
}