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
    public class GetWareHouse
    {
        public Response getWareHouse(string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    var query = string.Format(@"select '' as 'WhsCode', '' as 'WhsName' UNION ALL select WhsCode, WhsName from OWHS where Inactive='N'");
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
                                        dt.TableName = "wareHouseList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -222, Desc = "SİSTEMDE DEPO BULUNAMADI." };
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

        public Response getWareHouseByUserCode(string dbName, string kullaniciId, string belgeTipi, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "";
                     
                    if (belgeTipi == "")
                    {
                        query = "select '' as 'WhsCode', '' as 'WhsName','' as \"DepoYeriZorunlu\" UNION ALL Select \"U_DepoKodu\" as 'WhsCode',\"U_DepoAdi\" as 'WhsName',T2.\"BinActivat\" as \"DepoYeriZorunlu\"  from \"@AIF_WMS_USRWHS\" as T0 INNER JOIN \"@AIF_WMS_USRWHS1\" as T1 ON T0.\"DocEntry\"=T1.\"DocEntry\" LEFT JOIN OWHS AS T2 ON T1.\"U_DepoKodu\" = T2.\"WhsCode\" where T0.\"U_KullaniciKodu\" = '" + kullaniciId + "' and \"U_Secim\" = 'Y'";
                    }
                    else
                    {

                        query = "select '' as \"WhsCode\", '' as \"WhsName\",'' as \"DepoYeriZorunlu\",'' as \"Secim\",'' as \"TamYetki\",'' as \"SipMalGrs\",'' as \"U_BlgszMalGrs\",'' as \"TlpszDepK\",'' as \"TlpszDepH\",'' as \"TlpBagDepK\",'' as \"TlpBagDepH\",'' as \"TlpKabulK\",'' as \"TlpKabulH\",'' as \"BlgszMalC\",'' as \"SipBagTes\",'' as \"SprsszTes\",'' as \"TeslmtIade\",'' as \"SatisIade\",'' AS \"MagazaTalepleri\",'' AS \"IadeTalep\" UNION ALL ";
                        query += "select * from (";
                        query += " Select ";
                        query += "T1.\"U_DepoKodu\" as \"WhsCode\", ";
                        query += "T1.\"U_DepoAdi\" as \"WhsName\", ";
                        query += "T2.\"BinActivat\" as \"DepoYeriZorunlu\", T1.\"U_Secim\", ";
                        query += "ISNULL(T1.\"U_TamYetki\",'N') as 'U_TamYetki',  ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_SipMalGrs\",'N') end as 'U_SipMalGrs' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_BlgszMalGrs\",'N') end as 'U_BlgszMalGrs' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpszDepK\",'N') end as 'U_TlpszDepK' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpszDepH\",'N') end as 'U_TlpszDepH' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpBagDepK\",'N') end as 'U_TlpBagDepK' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpBagDepH\",'N') end as 'U_TlpBagDepH' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpKabulK\",'N') end as 'U_TlpKabulK' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpKabulH\",'N') end as 'U_TlpKabulH' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_BlgszMalC\",'N') end as 'U_BlgszMalC' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_SipBagTes\",'N') end as 'U_SipBagTes' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_SprsszTes\",'N') end as 'U_SprsszTes' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TeslmtIade\",'N') end as 'U_TeslmtIade' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_SatisIade\",'N') end as 'U_SatisIade', ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_MagazaIslemleri\",'N') end as 'U_MagazaIslemleri', ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_IadeTalep\",'N') end as 'U_IadeTalep' ";
                        query += "from \"@AIF_WMS_USRWHS\" as T0 ";
                        query += "INNER JOIN \"@AIF_WMS_USRWHS1\" as T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" ";
                        query += "LEFT JOIN OWHS AS T2 ON T1.\"U_DepoKodu\" = T2.\"WhsCode\" ";
                        query += "where T0.\"U_KullaniciKodu\" = '" + kullaniciId + "' and T1.\"U_Secim\" = 'Y' ";
                        query += ") as tbl ";
                        if (belgeTipi != "")
                        {
                            query += "where ISNULL(tbl." + belgeTipi + ",'N') = 'Y'";
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
                                        dt.TableName = "wareHouseList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -222, Desc = "SİSTEMDE DEPO BULUNAMADI." };
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

        public Response getWareHouseDetailsWithQty(string dbName, string warehouse, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    var query = "SELECT T1.[ItemCode] as \"Kalem Kodu\", T1.[ItemName] as \"Kalem Adı\", T0.[OnHand] \"Stokta\", T2.[UomCode] as \"Birim\" FROM [dbo].[OITW]  T0 INNER JOIN OITM T1 ON T0.[ItemCode] = T1.[ItemCode]  LEFT JOIN OUOM T2 ON T1.InvntryUom = T2.UomName where 1=1";
                    if (warehouse != "")
                    {
                        query += "and T0.WhsCode = '" + warehouse + "'";

                    }
                    query += " and T0.[OnHand] > 0";
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
                                        dt.TableName = "wareHouseList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -222, Desc = "SİSTEMDE DEPO BULUNAMADI." };
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

        public Response getWareHouseByUserCodeAddName(string dbName, string kullaniciId,string belgeTipi, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    //var query = "select '' as 'WhsCode', '' as 'WhsName' UNION ALL Select \"U_DepoKodu\" as 'WhsCode', (ISNULL(\"U_DepoKodu\",'')+'-'+ISNULL(\"U_DepoAdi\",'')) as 'WhsName'  from \"@AIF_WMS_USRWHS\" as T0 INNER JOIN \"@AIF_WMS_USRWHS1\" as T1 ON T0.\"DocEntry\"=T1.\"DocEntry\" where T0.\"U_KullaniciKodu\" = '" + UserCode + "' and \"U_Secim\" = 'Y'";
                    string query = "";

                    if (belgeTipi == "")
                    {
                        query = "select '' as 'WhsCode', '' as 'WhsName' UNION ALL Select \"U_DepoKodu\" as 'WhsCode', (ISNULL(\"U_DepoKodu\",'')+'-'+ISNULL(\"U_DepoAdi\",'')) as 'WhsName'  from \"@AIF_WMS_USRWHS\" as T0 INNER JOIN \"@AIF_WMS_USRWHS1\" as T1 ON T0.\"DocEntry\"=T1.\"DocEntry\" LEFT JOIN OWHS AS T2 ON T1.\"U_DepoKodu\" = T2.\"WhsCode\" where T0.\"U_KullaniciKodu\" = '" + kullaniciId + "' and \"U_Secim\" = 'Y'";
                    }
                    else
                    {

                        query = "select '' as \"WhsCode\", '' as \"WhsName\" UNION ALL ";
                        query += "select \"WhsCode\" as 'WhsCode',(ISNULL(\"WhsCode\",'')+'-'+ISNULL(\"WhsName\",'')) as 'WhsName' from (";
                        query += " Select ";
                        query += "T1.\"U_DepoKodu\" as \"WhsCode\", ";
                        query += "T1.\"U_DepoAdi\" as \"WhsName\", ";
                        query += "T1.\"U_Secim\", ";
                        query += "ISNULL(T1.\"U_TamYetki\",'N') as 'U_TamYetki',  ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_SipMalGrs\",'N') end as 'U_SipMalGrs' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_BlgszMalGrs\",'N') end as 'U_BlgszMalGrs' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpszDepK\",'N') end as 'U_TlpszDepK' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpszDepH\",'N') end as 'U_TlpszDepH' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpBagDepK\",'N') end as 'U_TlpBagDepK' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpBagDepH\",'N') end as 'U_TlpBagDepH' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpKabulK\",'N') end as 'U_TlpKabulK' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TlpKabulH\",'N') end as 'U_TlpKabulH' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_BlgszMalC\",'N') end as 'U_BlgszMalC' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_SipBagTes\",'N') end as 'U_SipBagTes' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_SprsszTes\",'N') end as 'U_SprsszTes' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_TeslmtIade\",'N') end as 'U_TeslmtIade' , ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_SatisIade\",'N') end as 'U_SatisIade', ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_MagazaIslemleri\",'N') end as 'U_MagazaIslemleri', ";
                        query += "CASE WHEN T1.U_TamYetki = 'Y' then  'Y' else ISNULL(T1.\"U_IadeTalep\",'N') end as 'U_IadeTalep' ";
                        query += "from \"@AIF_WMS_USRWHS\" as T0 ";
                        query += "INNER JOIN \"@AIF_WMS_USRWHS1\" as T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" ";
                        query += "LEFT JOIN OWHS AS T2 ON T1.\"U_DepoKodu\" = T2.\"WhsCode\" ";
                        query += "where T0.\"U_KullaniciKodu\" = '" + kullaniciId + "' and T1.\"U_Secim\" = 'Y' ";
                        query += ") as tbl ";
                        if (belgeTipi != "")
                        {
                            query += "where ISNULL(tbl." + belgeTipi + ",'N') = 'Y'";
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
                                        dt.TableName = "wareHouseList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -222, Desc = "SİSTEMDE DEPO BULUNAMADI." };
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

        public Response getBelgeBazliDepolar(string dbName, string kullaniciId, string belgeTipi, string mKod)
        {
            string sql = "SELECT * FROM (SELECT T1.\"U_DepoKodu\" AS \"WhsCode\",T1.\"U_DepoAdi\" AS \"WhsName\",T2.\"BinActivat\" AS \"DepoYeriZorunlu\", T1.\"U_Secim\", ISNULL(T1.\"U_TamYetki\",'N') AS \"U_TamYetki\",CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_SipMalGrs\", 'N') END AS \"U_SipMalGrs\",CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_BlgszMalGrs\", 'N') END AS \"U_BlgszMalGrs\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_TlpszDepK\", 'N') END AS \"U_TlpszDepK\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_TlpszDepH\", 'N') END AS \"U_TlpszDepH\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_TlpBagDepK\", 'N') END AS \"U_TlpBagDepK\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_TlpBagDepH\", 'N') END AS \"U_TlpBagDepH\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_TlpKabulK\", 'N') END AS \"U_TlpKabulK\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_TlpKabulH\", 'N') END AS \"U_TlpKabulH\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_BlgszMalC\", 'N') END AS \"U_BlgszMalC\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_SipBagTes\", 'N') END AS \"U_SipBagTes\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_SprsszTes\", 'N') END AS \"U_SprsszTes\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_TeslmtIade\", 'N') END AS \"U_TeslmtIade\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_SatisIade\", 'N') END AS \"U_SatisIade\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_MagazaIslemleri\", 'N') END AS \"U_MagazaIslemleri\", CASE WHEN T1.\"U_TamYetki\" = 'Y' THEN 'Y' ELSE ISNULL(T1.\"U_IadeTalep\", 'N') END AS \"U_IadeTalep\" FROM \"@AIF_WMS_USRWHS\" AS T0 INNER JOIN \"@AIF_WMS_USRWHS1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" LEFT JOIN OWHS AS T2 ON T1.\"U_DepoKodu\" = T2.\"WhsCode\" WHERE T0.\"U_KullaniciKodu\" = '" + kullaniciId + "' AND T1.\"U_Secim\" = 'Y') AS tbl ";

            if (belgeTipi != "")
            {
                sql += "where ISNULL(tbl." + belgeTipi + ",'N') = 'Y'";
            }


            //sql += "WHERE ISNULL(tbl.\"U_BlgszMalGrs\", 'N') = 'Y'";

            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    var query = string.Format(sql);
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
                                        dt.TableName = "BelgeBazliDepolar";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -222, Desc = "SİSTEMDE DEPO BULUNAMADI." };
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