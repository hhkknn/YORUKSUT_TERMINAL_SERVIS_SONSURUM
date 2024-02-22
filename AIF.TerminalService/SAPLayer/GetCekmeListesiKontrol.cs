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
    public class GetCekmeListesiKontrol
    {
        public Response getCekmeListesiKontrol(string dbName, string belgeNo, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string query = "";

                    //query = "select T0.\"DocEntry\", T1.\"U_PlanSipMik\" - ISNULL((select SUM(T3.\"U_Miktar\") from \"@AIF_WMS_TOPLANAN\" T3 where T3.\"U_SiparisNumarasi\" = T1.\"U_SiparisNumarasi\" AND T3.\"U_SiparisSatirNo\" = T1.\"U_SipSatirNo\" and T3.\"U_BelgeNo\" = T1.\"U_UrunKodu\"),0) - ISNULL((select SUM(T3.\"U_Miktar\") from \"@AIF_WMS_TOPLANAN\" T3 where T3.\"U_KalemKodu\" = T1.\"U_UrunKodu\" and T3.\"U_BelgeNo\" = T1.\"U_UrunKodu\"),0) as \"Kalan Miktar\", T1.\"U_SipSatirNo\",T1.\"U_SiparisNumarasi\",T1.\"U_UrunKodu\" from \"@AIF_WMS_SIPKAR\" T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" T1 on T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + belgeNo + "' ";

                    query = "SELECT  T0.\"DocEntry\",T1.\"U_PlanSipMik\",ISNULL((SELECT  SUM(T3.\"U_Miktar\") FROM \"@AIF_WMS_TOPLANAN\" T3 ";
                    query += "WHERE T3.\"U_SiparisNumarasi\" = T1.\"U_SiparisNumarasi\" AND T3.\"U_SiparisSatirNo\" = T1.\"U_SipSatirNo\" AND T3.\"U_KalemKodu\" = T1.\"U_UrunKodu\" AND T3.\"U_BelgeNo\" = T0.\"DocEntry\"), 0 ) AS \"Toplanan\", T1.\"U_PlanSipMik\" - ISNULL((SELECT  SUM(T3.\"U_Miktar\") FROM \"@AIF_WMS_TOPLANAN\" T3 ";
                    query += "WHERE T3.\"U_SiparisNumarasi\" = T1.\"U_SiparisNumarasi\" AND T3.\"U_SiparisSatirNo\" = T1.\"U_SipSatirNo\" AND T3.\"U_KalemKodu\" = T1.\"U_UrunKodu\" AND T3.\"U_BelgeNo\" = T0.\"DocEntry\"), 0 ) AS \"Kalan\", T1.\"U_SipSatirNo\", T1.\"U_SiparisNumarasi\", T1.\"U_UrunKodu\" FROM \"@AIF_WMS_SIPKAR\" T0 ";
                    query += "INNER JOIN \"@AIF_WMS_SIPKAR1\" T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" ";
                    query += "WHERE T0.\"DocEntry\" = '" + belgeNo + "' AND ISNULL(T1.\"U_SipSatirNo\",'')<>'' AND ISNULL(T1.\"U_SiparisNumarasi\",'')<> ''";

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
                                        dt.TableName = "CekmeListesiKontrol";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "ÇEKME LİSTESİ KONROL BULUNAMADI." };
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