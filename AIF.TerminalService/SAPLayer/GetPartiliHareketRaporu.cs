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
    public class GetPartiliHareketRaporu
    {
        public Response getPartiliHareketRaporu(string dbName, string partiNo, string depoKodu, string kalemkodu, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "";

                    if (partiNo != "" && depoKodu != "" && kalemkodu != "")
                    {
                        query = "SELECT T0.\"DocDate\" as \"Belge Tarihi\", ";
                        query += "CASE WHEN T0.\"BaseType\" = '10000071' THEN 'Stok Kaydı' WHEN T0.\"BaseType\" = '67' THEN 'Stok Nakli' WHEN T0.\"BaseType\" = '60' THEN 'Üretime Çıkış' WHEN T0.\"BaseType\" = '15' THEN 'İrsaliye Çıkışı' WHEN T0.\"BaseType\" = '13' THEN 'Fatura Çıkışı' else 'Tanımsız' end as \"Belge Türü\",T0.\"BaseEntry\" as \"Belge No\", ";
                        query += "CASE WHEN T0.\"Direction\" = '1' THEN 'Cikis' ELSE 'Giris' end as \"Yön\", T0.\"Quantity\" as \"Miktar\" FROM IBT1 T0 ";
                        query += "WHERE T0.\"BatchNum\" = '" + partiNo + "' AND   T0.\"WhsCode\"  = '" + depoKodu + "' and T0.\"ItemCode\" = '" + kalemkodu + "' ORDER BY T0.\"DocDate\" ";
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
                                        dt.TableName = "PartiHareketRapor";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "PARTİ HAREKET RAPORU BULUNAMADI." };
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