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
    public class GetKonteynerYapmaListesi
    {
        public Response getKonteynerYapmaListesi(string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    string query = "Select \"U_KonteynerNo\" as \"KonteynerNo\" from \"@AIF_WMS_KNTYNR\" as T0 ";
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
                                        dt.TableName = "KonteynerListesi";
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


        public Response getKonteynerYapmaDetaylari(string dbName, string konteynerNo, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                { 
                    string query = " ";

                    if (LoginCompany.oCompany.DbServerType==SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                    {
                        query = "Select T0.\"U_KonteynerNo\" as \"Konteyner No\",T1.\"U_PaletNo\" as \"Palet No\",T1.\"U_Barkod\" as \"Barkod\",T1.\"U_MuhKatalogNo\" as \"MuhatapKatalogNo\",\"U_KalemKodu\" as \"Kalem Kodu\",\"U_Tanim\" as \"Kalem Tanimi\",\"U_Miktar\" as \"Miktar\",T1.\"U_SiparisNo\" as \"SiparisNumarasi\",T1.\"U_SipSatirNo\" as \"SiparisSatirNo\",'Y' as \"UrunKonteynereDahaOnceEklendi\", \"U_CekmeNo\" as \"CekmeListesiNo\",T1.\"U_KoliMiktari\" as \"KoliMiktari\",T1.\"U_NetKilo\" as \"NetKilo\", T1.\"U_BrutKilo\" as \"BrutKilo\",T1.\"U_Kaynak\" as \"Kaynak\" from \"@AIF_WMS_KNTYNR\" as T0 INNER JOIN \"@AIF_WMS_KNTYNR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"U_KonteynerNo\" = '" + konteynerNo + "' ";
                    }
                    else
                    {
                        query = "Select T0.\"U_KonteynerNo\" as \"Konteyner No\",T1.\"U_PaletNo\" as \"Palet No\",T1.\"U_Barkod\" as \"Barkod\",T1.\"U_MuhKatalogNo\" as \"MuhatapKatalogNo\",\"U_KalemKodu\" as \"Kalem Kodu\",\"U_Tanim\" as \"Kalem Tanimi\",\"U_Miktar\" as \"Miktar\",T1.\"U_SiparisNo\" as \"SiparisNumarasi\",T1.\"U_SipSatirNo\" as \"SiparisSatirNo\",'Y' as \"UrunKonteynereDahaOnceEklendi\", \"U_CekmeNo\" as \"CekmeListesiNo\",T1.\"U_KoliMiktari\" as \"KoliMiktari\",T1.\"U_NetKilo\" as \"NetKilo\", T1.\"U_BrutKilo\" as \"BrutKilo\",T1.\"U_Kaynak\" as \"Kaynak\" from \"@AIF_WMS_KNTYNR\" as T0 INNER JOIN \"@AIF_WMS_KNTYNR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"U_KonteynerNo\" = N'" + konteynerNo + "' ";
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
                                        dt.TableName = "KonteynerListesiDetay";
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