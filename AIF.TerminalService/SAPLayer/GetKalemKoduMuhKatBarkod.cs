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
    public class GetKalemKoduMuhKatBarkod
    {
        public Response getKalemKoduMuhKatBarkod(string dbName, string itemCode, string cardCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    string query = "Select TOP 1 T0.\"ItemCode\" as \"Kalem Kodu\",T0.\"CodeBars\" as \"Barkod\",T1.\"Substitute\" as \"MuhatapKatalogNo\" from OITM as T0 LEFT JOIN OSCN AS T1 ON T0.\"ItemCode\" = T1.\"ItemCode\" ";

                    if (cardCode != "")
                    {
                        query += " and T1.\"CardCode\" = '" + cardCode + "' ";
                    }

                    query += " where T0.\"ItemCode\" = '" + itemCode + "'";
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
                                        dt.TableName = "KalemMuhKatBarkod";
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