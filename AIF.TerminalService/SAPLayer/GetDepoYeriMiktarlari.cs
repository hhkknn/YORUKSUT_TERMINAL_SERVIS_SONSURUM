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
    public class GetDepoYeriMiktarlari
    {
        public Response getDepoYeriMiktarlari(string dbName, string urunKod, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName,mKod);

                if (connstring != "")
                {
                    //var query = "SELECT T0.\"WhsCode\", T0.\"BinCode\", T1.\"ItemCode\", T2.\"ItemName\", T1.\"OnHandQty\" FROM \"OBIN\" T0 LEFT JOIN \"OIBQ\" T1 ON T0.\"WhsCode\" = T1.\"WhsCode\" AND T0.\"AbsEntry\" = T1.\"BinAbs\" LEFT JOIN \"OITM\" T2 ON T1.\"ItemCode\" = T2.\"ItemCode\"  where T1.\"ItemCode\" ='" + urunKod + "' ";
                    var query = "SELECT T0.\"AbsEntry\" as \"AbsEntry\",T0.\"BinCode\" as \"Depo Yeri\", T1.\"ItemCode\" as \"Ürün Kodu\", T2.\"ItemName\" as \"Ürün Adı\", T1.\"OnHandQty\" as \"Miktar\" FROM \"OBIN\" T0 LEFT JOIN \"OIBQ\" T1 ON T0.\"WhsCode\" = T1.\"WhsCode\" AND T0.\"AbsEntry\" = T1.\"BinAbs\" LEFT JOIN \"OITM\" T2 ON T1.\"ItemCode\" = T2.\"ItemCode\"  where T1.\"ItemCode\" ='" + urunKod + "' ";

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
                                        dt.TableName = "DepoYeriMiktarlari";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "DETAY BULUNAMADI." };
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