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
    public class GetSevkiyatAdresi
    {
        public Response getSevkiyatAdresi(string dbName, string cardcode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    var query = "select '' as 'Address' UNION ALL SELECT T1.\"Address\" FROM OCRD T0  INNER JOIN CRD1 T1 ON T0.\"CardCode\" = T1.\"CardCode\" where 1=1 ";

                    if (cardcode != "")
                    {
                        query += "AND T0.\"CardCode\" ='" + cardcode + "' AND T1.\"AdresType\" = 'S' ";
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
                                        dt.TableName = "SevkiyatAdresi";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -222, Desc = "SEVKİYAT ADRESİ BULUNAMADI." };
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