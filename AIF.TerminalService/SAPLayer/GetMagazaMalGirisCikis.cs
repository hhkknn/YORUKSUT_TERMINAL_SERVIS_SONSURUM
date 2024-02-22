using AIF.TerminalService.DatabaseLayer;
using AIF.TerminalService.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{
    //commit.
    public class GetMagazaMalGirisCikis
    {
        public Response getMagazaMalGirisCikis(string dbName, string dtBaslangic, string dtBitis, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                string condition = LoginCompany.oCompany.DbServerType == BoDataServerTypes.dst_HANADB ? "IFNULL" : "ISNULL";

                if (connstring != "")
                {
                    string Query = "";

                    Query = "SELECT * FROM (SELECT T0.\"DocEntry\" as \"belgeNo\",convert(varchar,T0.\"DocDate\",103) as \"tarih\",case when T0.\"ObjType\" = 59 then N'Mağaza Giriş' when T0.\"ObjType\" = 60 then N'Mağaza Çıkış' end as \"girisMiCikisMi\", ";
                    Query += " (SELECT T1.\"BPLName\" FROM OBPL T1 WITH (NOLOCK) WHERE T1.\"BPLId\" = T0.\"BPLId\") AS \"gondericiSube\", (SELECT " + condition + "(SUM(T98.\"Quantity\"),0) - (SELECT " + condition + "(SUM(T98.\"Quantity\"),0) FROM OIGN T99 WITH (NOLOCK) ";
                    Query += " INNER JOIN IGN1 T98 WITH (NOLOCK) ON T99.\"DocEntry\" = T98.\"DocEntry\" WHERE T99.\"Ref2\" = T0.\"DocNum\") FROM OIGE T99 ";
                    Query += " INNER JOIN IGE1 T98 WITH (NOLOCK) ON T99.\"DocEntry\" = T98.\"DocEntry\" WHERE T99.\"DocNum\" = T0.\"DocNum\") as Fark, (Select \"BPLName\" from OBPL as T97  WITH (NOLOCK) where T97.\"BPLId\" =  T0.\"U_AliciSubeId\") as \"AliciSube\" FROM OIGE T0 WITH (NOLOCK) ";
                    Query += " WHERE T0.\"CANCELED\" = 'N' AND T0.\"DocDate\" >= '" + dtBaslangic + "' AND T0.\"DocDate\" <= '" + dtBitis + "') AS tbl ";
                    Query += "WHERE " + condition + "(tbl.\"Fark\",0) > 0 ";
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(Query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "MagazaMalGirisCikis";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KAYIT BULUNAMADI." };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -999, Desc = "HATA OLUŞTU." + ex.Message };

                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -999, Desc = "HATA OLUŞTU." + ex.Message };

            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response getMagazaMalGirisCikisDetay(string dbName, string subeid, string belgeNo, string girisMiCikisMi, string goruntuleme, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);


                if (connstring != "")
                {
                    string Query = "";

                    if (girisMiCikisMi == "Mağaza Çıkış")//"Mal Çıkış" //ŞUBE FİLTRESİ KALDIRILDI
                    {
                        if (goruntuleme == "Y")
                        {
                            Query = "SELECT T0.\"DocEntry\",T0.\"DocDate\", T0.\"ObjType\",T0.\"U_AliciSubeId\",T0.\"BPLId\",T0.\"U_AliciAdi\",T0.\"U_Adres\",T0.\"U_ZipCode\",T0.\"U_City\", T0.\"U_County\",T1.\"ItemCode\",T1.\"Dscription\",T1.\"CodeBars\",T1.\"Quantity\" FROM OIGE T0 WITH (NOLOCK) INNER JOIN IGE1 T1 WITH (NOLOCK) ON T0.\"DocEntry\" = T1.\"DocEntry\" WHERE T0.\"CANCELED\" = 'N' AND T0.\"ObjType\" >= '60' AND T0.\"DocEntry\" = '" + belgeNo + "' ";
                        }
                        else
                        {
                            Query = "Select * from (SELECT T0.\"DocEntry\",T0.\"DocDate\", T0.\"ObjType\",T0.\"U_AliciSubeId\",T0.\"BPLId\",T0.\"U_AliciAdi\",T0.\"U_Adres\",T0.\"U_ZipCode\",T0.\"U_City\", T0.\"U_County\",T1.\"ItemCode\",T1.\"Dscription\",T1.\"CodeBars\",T1.\"Quantity\"  - (SELECT ISNULL(SUM(T98.\"Quantity\"),0) FROM OIGN T99 WITH (NOLOCK) INNER JOIN IGN1 T98 WITH (NOLOCK) ON T99.\"DocEntry\" = T98.\"DocEntry\" WHERE T99.\"Ref2\" = T0.\"DocNum\" and T98.\"ItemCode\" = T1.\"ItemCode\") as Quantity FROM OIGE T0 WITH (NOLOCK) INNER JOIN IGE1 T1 WITH (NOLOCK) ON T0.\"DocEntry\" = T1.\"DocEntry\" WHERE T0.\"CANCELED\" = 'N' AND T0.\"ObjType\" >= '60' AND T0.\"DocEntry\" = '" + belgeNo + "') as tbl where tbl.\"Quantity\" > 0 ";
                        }
                    }
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(Query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "MagazaMalGirisCikisDetay";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KAYIT BULUNAMADI." };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -999, Desc = "HATA OLUŞTU." + ex.Message };

                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -999, Desc = "HATA OLUŞTU." + ex.Message };

            }
            return new Response { _list = dt, Val = 0 };
        }

    }
}