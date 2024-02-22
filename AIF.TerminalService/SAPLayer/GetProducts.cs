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
    public class GetProducts
    {
        public Response getProductsByBarCodeWithWareHouse(string dbName, string barCode, string wareHouse, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select TOP 1 T0.\"ItemCode\" as \"Kalem Kodu\",T0.\"ItemName\" as \"Kalem Tanımı\",T2.\"UomCode\" as \"Ölçü Birimi\",T0.\"CodeBars\" as \"Barkod\",T0.\"ManBtchNum\" as \"Partili\",ISNULL(T1.\"OnHand\",0) as \"Depo Miktari\"";


                    if (mKod == "10TRMN" || mKod == "20TRMN")
                    {
                        query += ",T0.U_KoliMiktari as \"KoliIciAD\" ,T0.U_PALET as \"PaletIciAD\",T0.U_PaletKoli  as \"PaletIciKoliAD\" ";
                    }
                    else
                    {
                        query += ",0 as \"KoliIciAD\" ,0 as \"PaletIciAD\",0 as \"PaletIciKoliAD\" ";
                    }

                    query += ",T3.\"Substitute\" as \"MuhatapKatalogNo\" from OITM as T0 INNER JOIN OITW as T1 ON T0.\"ItemCode\" = T1.\"ItemCode\" LEFT JOIN OUOM as T2 ON T0.InvntryUom = T2.UomName LEFT JOIN OSCN as T3 ON  T0.\"ItemCode\" = T3.\"ItemCode\" where CodeBars = '" + barCode + "' ";

                    if (wareHouse != "")
                    {
                        query += "and T1.\"WhsCode\" = '" + wareHouse + "'";
                    }

                    query += " AND T0.\"validFor\" = 'Y' and ISNULL(T0.\"AssetClass\",'') = ''  and ISNULL(T0.\"InvntItem\",'') = 'Y' ";

                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "ItemCodeByBarCode";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KALEM BULUNAMADI." };
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

        public Response getProductsByItemCodeWithWareHouse(string dbName, string itemCode, string wareHouse, string cardCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select TOP 1 T0.\"ItemCode\" as \"Kalem Kodu\",T0.\"ItemName\" as \"Kalem Tanımı\",T2.\"UomCode\" as \"Ölçü Birimi\",T0.\"CodeBars\" as \"Barkod\",T0.\"ManBtchNum\" as \"Partili\",ISNULL(T1.\"OnHand\",0) as \"Depo Miktari\" ";

                    if (mKod == "10TRMN" || mKod == "20TRMN")
                    {
                        query += ",T0.U_KoliMiktari as \"KoliIciAD\" ,T0.U_PALET as \"PaletIciAD\",T0.U_PaletKoli  as \"PaletIciKoliAD\" ";
                    }
                    else
                    {
                        query += ",0 as \"KoliIciAD\" ,0 as \"PaletIciAD\",0  as \"PaletIciKoliAD\" ";

                    }

                    query += ",T3.\"Substitute\" as \"MuhatapKatalogNo\" from OITM as T0 INNER JOIN OITW as T1 ON T0.\"ItemCode\" = T1.\"ItemCode\" LEFT JOIN OUOM as T2 ON T0.InvntryUom = T2.UomName LEFT JOIN OSCN as T3 ON  T0.\"ItemCode\" = T3.\"ItemCode\"  where T0.\"ItemCode\" = '" + itemCode + "'";

                    if (wareHouse != "")
                    {
                        query += "and T1.\"WhsCode\" = '" + wareHouse + "'";
                    }


                    query += " AND T0.\"validFor\" = 'Y' and ISNULL(T0.\"AssetClass\",'') = '' and ISNULL(T0.\"InvntItem\",'') = 'Y' ";
                    //if (cardCode != "")
                    //{
                    //    query += "and T3.\"CardCode\" = '" + cardCode + "'";
                    //}
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "ItemCodeByItemCode";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KALEM BULUNAMADI." };
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

        public Response getProductsByItemCode(string dbName, string itemCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select T0.\"ItemCode\" as \"Kalem Kodu\",T0.\"ItemName\" as \"Kalem Tanımı\",T1.\"UomCode\" as \"Ölçü Birimi\",T0.\"CodeBars\" as \"Barkod\",T0.\"ManBtchNum\" as \"Partili\" from OITM as T0 left join OUOM T1 on T0.InvntryUom = T1.UomName where T0.\"ItemCode\" = '" + itemCode + "'";


                    query += " AND T0.\"validFor\" = 'Y' and ISNULL(T0.\"AssetClass\",'') = '' and ISNULL(T0.\"InvntItem\",'') = 'Y' ";

                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "ItemCodeByItemCode";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KALEM BULUNAMADI." };
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

        public Response getProductsByBarCode(string dbName, string barCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select T0.\"ItemCode\" as \"Kalem Kodu\",T0.\"ItemName\" as \"Kalem Tanımı\",T1.\"UomCode\" as \"Ölçü Birimi\",T0.\"CodeBars\" as \"Barkod\",T0.\"ManBtchNum\" as \"Partili\" from OITM as T0 left join OUOM T1 on T0.InvntryUom = T1.UomName where CodeBars = '" + barCode + "' ";

                    query += " AND T0.\"validFor\" = 'Y' and ISNULL(T0.\"AssetClass\",'') = '' and ISNULL(T0.\"InvntItem\",'') = 'Y' ";
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "ItemCodeByBarCode";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KALEM BULUNAMADI." };
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

        public Response getProductsByBarCodeWithWhsDetails(string dbName, string barCode, string fiyatListId, List<string> WhsCodes, string kullaniciId, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string query = "";

                    if (fiyatListId == "")
                    {
                        query = "select T0.\"ItemCode\" ,T0.\"ItemName\",T0.CodeBars,T2.WhsCode,T2.WhsName,T1.OnHand,T3.UomCode from OITM T0 inner join OITW T1 on T0.ItemCode = t1.ItemCode inner join OWHS T2 on T1.WhsCode = T2.WhsCode LEFT join OUOM T3 on T0.InvntryUom = T3.UomName where T0.CodeBars ='" + barCode + "'  and T1.[OnHand] > 0";
                    }
                    else
                    {
                        query = "Select tbl.* , case when tbl.\"WhsCode\" = T4.\"U_VarsDepoKodu\" THEN 1 else 2 END as \"Sira\"  from ( ";
                        query += "select T0.\"ItemCode\" ,T0.\"ItemName\",T0.\"CodeBars\",T2.\"WhsCode\",T2.\"WhsName\",T1.\"OnHand\",T3.\"UomCode\",cast(I1.\"Price\" as Decimal(16,2)) AS \"Price\" from OITM T0 ";
                        query += "inner join OITW T1 on T0.\"ItemCode\" = t1.\"ItemCode\" ";
                        query += "inner join OWHS T2 on T1.\"WhsCode\" = T2.\"WhsCode\" ";
                        query += "LEFT join OUOM T3 on T0.\"InvntryUom\" = T3.\"UomName\" ";
                        query += "inner join ITM1 I1 on I1.\"ItemCode\" = T0.\"ItemCode\" ";
                        query += "where T0.\"CodeBars\" = '" + barCode + "' and I1.\"PriceList\" = '" + fiyatListId + "' and T1.\"OnHand\" > 0 ";

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

                                query += " and T2.WhsCode IN (" + values + ")) as tbl ";
                            }
                        }

                        query += "LEFT JOIN \"@AIF_WMS_USRWHS\" T4 ON tbl.\"WhsCode\" = T4.\"U_VarsDepoKodu\" and T4.\"U_KullaniciKodu\" = '" + kullaniciId + "' order by \"Sira\" ";
                    }


                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "ItemCodeByBarCode";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KALEM BULUNAMADI." };
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

        public Response getProductsByItemCodeWithWhsDetails(string dbName, string itemCode, string fiyatListId, List<string> WhsCodes, string kullaniciId, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    string query = "";

                    if (fiyatListId == "")
                    {
                        query = "select T0.\"ItemCode\",T0.\"ItemName\",T0.CodeBars,T2.WhsCode,T2.WhsName,T1.OnHand,T3.UomCode from OITM T0 inner join OITW T1 on T0.ItemCode = t1.ItemCode inner join OWHS T2 on T1.WhsCode = T2.WhsCode LEFT join OUOM T3 on T0.InvntryUom = T3.UomName where T0.ItemCode ='" + itemCode + "'  and T1.[OnHand] > 0";
                    }
                    else
                    {
                        query = "Select tbl.* , case when tbl.\"WhsCode\" = T4.\"U_VarsDepoKodu\" THEN 1 else 2 END as \"Sira\"  from ( ";
                        query += "select T0.\"ItemCode\",T0.\"ItemName\",T0.\"CodeBars\",T2.\"WhsCode\",T2.\"WhsName\",T1.\"OnHand\",T3.\"UomCode\",cast(I1.\"Price\" as Decimal(16,2)) AS \"Price\" from OITM T0 ";
                        query += "inner join OITW T1 on T0.\"ItemCode\" = t1.\"ItemCode\" ";
                        query += "inner join OWHS T2 on T1.\"WhsCode\" = T2.\"WhsCode\" ";
                        query += "LEFT join OUOM T3 on T0.\"InvntryUom\" = T3.\"UomName\" ";
                        query += "inner join ITM1 I1 on I1.\"ItemCode\" = T0.\"ItemCode\" ";
                        query += "where T0.\"ItemCode\" ='" + itemCode + "' and I1.\"PriceList\" = '" + fiyatListId + "' and T1.\"OnHand\" > 0 ";

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

                                query += " and T2.WhsCode IN (" + values + ")) as tbl ";
                            }
                        }

                        query += "LEFT JOIN \"@AIF_WMS_USRWHS\" T4 ON tbl.\"WhsCode\" = T4.\"U_VarsDepoKodu\" and T4.\"U_KullaniciKodu\" = '" + kullaniciId + "' order by \"Sira\" ";
                    }

                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "ItemCodeByItemCode";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KALEM BULUNAMADI." };
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

        public Response getProducts(string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select TOP 1 '' as \"ItemCode\",'' as \"ItemName\",'' as \"CodeBars\" from OITM UNION ALL Select \"ItemCode\", (ISNULL(\"ItemCode\",'') + '-' +  ISNULL(\"ItemName\",'')) as \"ItemName\",ISNULL(\"CodeBars\",'') AS \"CodeBars\" from OITM";


                    query += " where \"validFor\" = 'Y' and ISNULL(\"AssetClass\",'') = '' and ISNULL(\"InvntItem\",'') = 'Y' ";

                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "ItemCodes";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KALEM BULUNAMADI." };
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

        public Response getProductsByMuhatapKatalogNoWithWareHouse(string dbName, string barCode, string wareHouse, string cardCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    var query = "Select ";
                    if (cardCode == "")
                    {
                        query += "TOP 1";
                    }
                    else
                    {
                        query += "DISTINCT";
                    }



                    query += " T0.\"ItemCode\" as \"Kalem Kodu\",T0.\"ItemName\" as \"Kalem Tanımı\",T2.\"UomCode\" as \"Ölçü Birimi\",T0.\"CodeBars\" as \"Barkod\",T0.\"ManBtchNum\" as \"Partili\",ISNULL(T1.\"OnHand\",0) as \"Depo Miktari\" ";




                    if (mKod == "10TRMN" || mKod == "20TRMN")
                    {
                        query += ",T0.U_KoliMiktari as \"KoliIciAD\" ,T0.U_PALET as \"PaletIciAD\",T0.U_PaletKoli  as \"PaletIciKoliAD\" ";
                    }
                    else
                    {
                        query += ",0  as \"KoliIciAD\",0 as \"PaletIciAD\",0 as \"PaletIciKoliAD\" ";
                    }

                    query += ",T3.\"Substitute\" as \"MuhatapKatalogNo\" from OITM as T0 INNER JOIN OITW as T1 ON T0.\"ItemCode\" = T1.\"ItemCode\" LEFT JOIN OUOM as T2 ON T0.InvntryUom = T2.UomName LEFT JOIN OSCN as T3 ON  T0.\"ItemCode\" = T3.\"ItemCode\"  where \"Substitute\" = '" + barCode + "' ";

                    if (wareHouse != "")
                    {
                        query += "and T1.\"WhsCode\" = '" + wareHouse + "'";
                    }

                    if (cardCode != "")
                    {
                        query += "and T3.\"CardCode\" = '" + cardCode + "'";
                    }

                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "ItemCodeByMuhatapKatalogNo";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KALEM BULUNAMADI." };
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

        public Response getProductsMuhatapKatalogNo(string dbName, string barCode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {

                    string query = "SELECT T0.\"CardCode\" as \"MUHATAP KODU\", T1.\"CardName\" AS \"MUHATAP ADI\",T0.\"ItemCode\" AS \"ÜRÜN KODU\",T2.\"ItemName\" AS \"ÜRÜN ADI\", T0.\"Substitute\" AS \"KATALOG NO\" FROM OSCN T0 LEFT JOIN OCRD AS T1 ON T0.\"CardCode\" = T1.\"CardCode\" LEFT JOIN OITM AS T2 ON T0.\"ItemCode\" = T2.\"ItemCode\" WHERE T0.\"Substitute\" = '" + barCode + "'";


                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "MuhatapKatalogNoListesi";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KATALOG BULUNAMADI." };
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

        public Response PaletKalemleriniGetir(string dbName, string paletNo, string wareHouse, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "Select T0.\"U_PaletNo\" AS \"PaletNo\",T1.\"U_KalemKodu\" as \"KalemKodu\",T1.\"U_Tanim\" as \"KalemAdi\", T4.\"UomCode\" AS \"OlcuBirimi\",T1.\"U_Barkod\" as \"Barkod\",T3.\"ManBtchNum\" AS \"Partili\",T1.\"U_Miktar\" as \"Miktar\",T1.\"U_DepoKodu\" as \"DepoKoduPlt\",T1.\"U_DepoAdi\" as \"DepoAdi\",T1.\"U_DepoYeriId\" as \"DepoYeriId\",T1.\"U_DepoYeriAdi\" as \"DepoYeriAdi\",T1.\"U_Miktar\" as \"ToplananMiktar\" ";


                    if (mKod == "10TRMN" || mKod == "20TRMN")
                    {
                        query += ",ISNULL(T3.\"U_KoliMiktari\",0) AS \"KoliIciAD\",ISNULL(T3.\"U_PALET\",0) AS \"PaletIciAD\",ISNULL(T3.\"U_PaletKoli\",0) AS \"PaletIciKoliAD\"";
                    }
                    else
                    {
                        query += ",0 as \"KoliIciAD\" ,0 as \"PaletIciAD\",0 as \"PaletIciKoliAD\" ";
                    }

                    query += ",T5.\"Substitute\" AS \"MuhatapKatalogNo\",T1.\"U_DetaySatirNo\" as \"DetaySatirNo\" FROM \"@AIF_WMS_PALET\" T0 ";
                    query += "INNER JOIN \"@AIF_WMS_PALET1\" T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" INNER JOIN OITM AS T3 ON T1.\"U_KalemKodu\" = T3.\"ItemCode\" LEFT JOIN OUOM AS T4 ON T3.\"InvntryUom\" = T4.\"UomName\" LEFT JOIN OSCN AS T5 ON T1.\"U_KalemKodu\" = T5.\"ItemCode\" ";

                    query += "WHERE T0.\"U_PaletNo\" = '" + paletNo + "' ";

                    if (wareHouse != "")
                    {
                        query += "AND T1.\"U_DepoKodu\" = '" + wareHouse + "' order by T1.\"LineId\"";
                    }

                    //query += " group by T0.\"U_PaletNo\",T1.\"U_Barkod\" ,T1.\"U_KalemKodu\",T1.\"U_Tanim\",T1.\"U_DepoKodu\" ,T1.\"U_DepoAdi\" ,T1.\"U_Miktar\",T4.\"UomCode\",T3.\"ManBtchNum\",T3.\"U_KoliMiktari\",T3.\"U_PALET\",T3.\"U_PaletKoli\",T5.\"Substitute\",T1.\"LineId\" order by T1.\"LineId\" ";

                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "PaletKalemleriniGetir";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KALEM BULUNAMADI." };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -111, Desc = "HATA OLUŞTU." + ex.Message };

                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -111, Desc = "HATA OLUŞTU." + ex.Message };

            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response PaletPartiDetaylariniGetir(string dbName, string paletNo,string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "select T2.\"U_KalemKodu\" as \"KalemKodu\",T2.\"U_PartiNo\" as \"PartiNo\",T2.\"U_Miktar\" AS \"Miktar\",T2.\"U_DepoYeriId\" as \"DepoYeriId\",T2.\"U_DepoYeriAdi\" as \"DepoYeriAdi\",T2.\"U_PartiSatirNo\" as \"PartiSatirNo\" FROM \"@AIF_WMS_PALET\" T0 INNER JOIN \"@AIF_WMS_PALET2\" T2 ON T0.\"DocEntry\" = T2.\"DocEntry\" WHERE T0.\"U_PaletNo\" = '" + paletNo + "' order by T2.\"LineId\" ";

                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandTimeout = 1000;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "PaletPartiDetaylariniGetir";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "KALEM BULUNAMADI." };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -111, Desc = "HATA OLUŞTU." + ex.Message };

                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -111, Desc = "HATA OLUŞTU." + ex.Message };

            }
            return new Response { _list = dt, Val = 0 };
        }
    }
}