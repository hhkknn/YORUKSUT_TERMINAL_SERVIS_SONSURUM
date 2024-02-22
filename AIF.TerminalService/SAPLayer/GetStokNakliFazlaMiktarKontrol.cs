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
    public class GetStokNakliFazlaMiktarKontrol
    {
        public Response getStokNakliFazlaMiktarKontrol(string dbName, List<string> docEntryList, List<string> WhsCodes, string mKod)
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

                    var query = "select T0.DocEntry as [BelgeNumarasi], T0.ItemCode as [KalemKodu], T0.\"LineNum\" as \"SiraNo\", ";

                    query += "T0.\"OpenQty\" -ISNULL((Select SUM(T99.\"Quantity\") from DRF1 AS T99 INNER JOIN ODRF AS T98 ON T99.\"DocEntry\" = T98.\"DocEntry\" where T99.\"BaseLine\" = T0.\"LineNum\" and T98.\"DocStatus\" = 'O' and T99.\"LineStatus\" = 'O' and T99.\"BaseEntry\" = T0.\"DocEntry\"),0) as \"AcikMiktar\" from WTQ1 as T0 ";

                    query += "INNER JOIN OWTQ AS T4 ON T0.\"DocEntry\" = T4.\"DocEntry\" ";
                    query += "where T0.DocEntry IN (" + docentryvalList + ") and T4.DocStatus='O' and T0.\"OpenQty\" > 0 ";

                    if (WhsCodes != null)
                    {
                        if (WhsCodes.Count > 0)
                        {
                            var values = "";
                            foreach (var item in WhsCodes)
                            {
                                values += "'" + item + "'" + ",";
                            }

                            if (values != "")
                            {
                                values = values.Remove(values.Length - 1, 1);
                            }

                            query += " and (T0.FromWhsCod IN (" + values + ") OR T0.WhsCode IN (" + values + "))";

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
                                        dt.TableName = "StokNakliFazlaGiris";

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