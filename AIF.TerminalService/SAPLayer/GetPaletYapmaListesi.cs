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
    public class GetPaletYapmaListesi
    {
        public Response getPaletYapmaListesi(string dbName, string pasifleriGoster, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    string query = "Select \"U_PaletNo\" as \"PaletNo\" from \"@AIF_WMS_PALET\" as T0 where 1=1 ";

                    if (pasifleriGoster != "Y")
                    {
                        query += " and (\"U_Durum\" = 'A' or \"U_Durum\" is null)";
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
                                        dt.TableName = "PaletListesi";
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


        public Response getPaletYapmaDetaylari(string dbName, string paletNo, string mKod, string cekmeListesiKalemleriniGrupla)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    string query = "";

                    if (cekmeListesiKalemleriniGrupla != "Y")
                    {
                        query = "Select T0.\"U_PaletNo\" as \"Palet No\",T1.\"U_Barkod\" as \"Barkod\",T1.\"U_MuhKatalogNo\" as \"MuhatapKatalogNo\",\"U_KalemKodu\" as \"Kalem Kodu\",\"U_Tanim\" as \"Kalem Tanimi\",\"U_Miktar\" as \"Miktar\",T0.\"U_ToplamKap\" as \"ToplamKap\",T0.\"U_NetKilo\" as \"NetKilo\",T0.\"U_BrutKilo\" as \"BrutKilo\",T1.\"U_SiparisNo\" as \"SiparisNumarasi\",T1.\"U_SipSatirNo\" as \"SiparisSatirNo\",\"U_CekmeNo\" as \"CekmeNo\",'' as \"SatirKaynagi\",ISNULL(T1.\"U_DetaySatirNo\",'-1') as \"DetaySatirNo\",T1.\"U_DepoKodu\" as \"DepoKodu\",T1.\"U_DepoAdi\" as \"DepoAdi\",T1.\"U_DepoYeriId\" as \"DepoYeriId\", T1.\"U_DepoYeriAdi\" as \"DepoYeriAdi\" from \"@AIF_WMS_PALET\" as T0 INNER JOIN \"@AIF_WMS_PALET1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" ";

                        if (paletNo != "")
                        {
                            query += " where 1=1 and T0.\"U_PaletNo\" = '" + paletNo + "' ";
                        }
                    }
                    else
                    {

                        query = "Select T0.\"U_PaletNo\" as \"Palet No\",T1.\"U_Barkod\" as \"Barkod\",T1.\"U_MuhKatalogNo\" as \"MuhatapKatalogNo\",\"U_KalemKodu\" as \"Kalem Kodu\",\"U_Tanim\" as \"Kalem Tanimi\",SUM(\"U_Miktar\") as \"Miktar\",T0.\"U_ToplamKap\" as \"ToplamKap\",T0.\"U_NetKilo\" as \"NetKilo\",T0.\"U_BrutKilo\" as \"BrutKilo\",-1 as \"SiparisNumarasi\",-1 as \"SiparisSatirNo\",\"U_CekmeNo\" as \"CekmeNo\",T1.\"U_Kaynak\" as \"SatirKaynagi\",ISNULL(T1.\"U_DetaySatirNo\",'-1') as \"DetaySatirNo\",T1.\"U_DepoKodu\" as \"DepoKodu\",T1.\"U_DepoAdi\" as \"DepoAdi\",T1.\"U_DepoYeriId\" as \"DepoYeriId\", T1.\"U_DepoYeriAdi\" as \"DepoYeriAdi\"  from \"@AIF_WMS_PALET\" as T0 INNER JOIN \"@AIF_WMS_PALET1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" ";


                        if (paletNo != "")
                        {
                            query += " where 1=1  and T0.\"U_PaletNo\" = '" + paletNo + "' ";
                        }

                        query += " GROUP BY T0.\"U_PaletNo\",T1.\"U_Barkod\",T1.\"U_MuhKatalogNo\",\"U_KalemKodu\",\"U_Tanim\",T0.\"U_ToplamKap\",T0.\"U_NetKilo\",T0.\"U_BrutKilo\",\"U_CekmeNo\",\"U_Kaynak\",T1.\"U_SipSatirNo\",T1.\"U_DetaySatirNo\" ";
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
                                        dt.TableName = "PaletListesiDetay";
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


        public Response getPaletNoVarmi(string dbName, string paletNo, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    string query = "Select \"U_PaletNo\" from \"@AIF_WMS_PALET\" as T0 where \"U_PaletNo\" = '" + paletNo + "' ";

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
                                        dt.TableName = "PaletListesi";
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

        public Response getPaletYapmaPartiler(string dbName, string paletNo, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    string query = "";

                    query = "Select T0.\"U_PaletNo\" as \"Palet No\",T2.\"U_Barkod\" as \"Barkod\",T2.\"U_KalemKodu\" as \"KalemKodu\",T2.\"U_Tanim\" as \"KalemAdi\", T2.\"U_PartiNo\" as \"PartiNo\",T2.\"U_Miktar\" as \"PartiMiktar\",ISNULL(T2.\"U_DepoKodu\",'') AS \"DepoKodu\",ISNULL(T2.\"U_DepoAdi\",'') as \"DepoAdi\",ISNULL(T2.\"U_DepoYeriId\",'') AS \"DepoYeriId\",ISNULL(T2.\"U_DepoYeriAdi\",'') as \"DepoYeriAdi\",ISNULL(T2.\"U_PartiSatirNo\",'') as \"PartiSatirNo\"  from \"@AIF_WMS_PALET\" as T0 ";

                    query += "INNER JOIN \"@AIF_WMS_PALET2\" AS T2 ON T0.\"DocEntry\" = T2.\"DocEntry\" ";


                    if (paletNo != "")
                    {
                        query += " where 1=1 and T0.\"U_PaletNo\" = '" + paletNo + "' ";
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
                                        dt.TableName = "PaletListesiParti";
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