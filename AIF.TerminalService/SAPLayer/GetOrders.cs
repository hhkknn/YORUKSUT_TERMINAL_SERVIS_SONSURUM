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
    public class GetOrders
    {

        public Response getOrdersByDate(string dbName, string startDate, string endDate, List<string> WhsCodes, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName,mKod);

                if (connstring != "")
                {

                    var query = "select DISTINCT T0.DocNum as [Belge Numarası],T0.CardCode as [Müşteri Kodu], T0.CardName as [Müşteri Adı], CONVERT(varchar, T0.TaxDate, 103) as [Belge Tarihi], CONVERT(varchar, T0.DocDueDate, 103) as [Teslimat Tarihi],T0.ShipToCode as [Sevkiyat Adresi] from ORDR as T0 INNER JOIN RDR1 as T1 ON T0.DocEntry = T1.DocEntry where T0.DocDueDate >= '" + startDate + "' and T0.DocDueDate<='" + endDate + "' and T0.\"DocStatus\" = 'O'and T0.\"DocType\" = 'I'";

                    if (WhsCodes != null)
                    {
                        if (WhsCodes.Count > 0)
                        {
                            //string whsCodeValues = string.Join(",", SatisTeklifi.SeciliSatirlarMatriksAltGrubus.ToList().Select(x => "'" + x.MatriksAltGrupKodu + "'"));
                            //string whsCodeValues = string.Join(",", "'" + WhsCodes.Select(x=>x.First()) + "'");

                            //string joined = WhsCodes.Aggregate((a, x) => "'" + a + "'" + "," + "'" + x + "'");

                            var values = "";
                            foreach (var item in WhsCodes)
                            {
                                values += "'" + item + "'" + ",";
                            }

                            if (values != "")
                            {
                                values = values.Remove(values.Length - 1, 1);
                            }

                            query += " and WhsCode IN (" + values + ") and T1.LineStatus = 'O'";

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
                                        dt.TableName = "SatisSiparisiTarihli";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "SATIŞ SİPARİŞİ BULUNAMADI." };
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


        public Response getOrdersDetails(string dbName, List<string> docEntryList, List<string> WhsCodes, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName,mKod);

                if (connstring != "")
                {
                    string docentryvalList = "";

                    foreach (var item in docEntryList)
                    {
                        docentryvalList += item + ",";
                    }

                    if (docentryvalList.Length > 1)
                    {
                        docentryvalList = docentryvalList.Remove(docentryvalList.Length - 1, 1);
                    }

                    var query = "select T0.DocEntry as [BelgeNumarasi], T0.ItemCode as [KalemKodu], T0.Dscription as [KalemAdi], T0.OpenQty as [Miktar], T0.UomCode as [OlcuBirimi],T1.\"ManBtchNum\" as [Partili],T1.\"CodeBars\" as [Barkod],T0.\"WhsCode\" as \"DepoKodu\",\"LineNum\" as \"SiraNo\",T2.\"OnHand\" as \"DepoMiktar\" ";
                     
                    if (mKod == "10TRMN" || mKod == "20TRMN")
                    {
                        query += ",T1.U_KoliMiktari as \"KoliIciAD\" ,T1.U_PALET as \"PaletIciAD\",T1.U_PaletKoli  as \"PaletIciKoliAD\", ";
                    }
                    else
                    {
                        query += ",0 as \"KoliIciAD\" ,0 as \"PaletIciAD\",0 as \"PaletIciKoliAD\", ";
                    }

                    query +="T3.\"NumAtCard\" as[MusteriSipNo], T3.\"ShipToCode\" as [SevkiyatAdresi],(Select t99.\"WhsName\" from OWHS t99 where t99.\"WhsCode\" = T0.\"WhsCode\") as \"DepoAdi\" from RDR1 as T0 INNER JOIN OITM as T1 ON T0.ItemCode = T1.ItemCode INNER JOIN OITW as T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" INNER JOIN ORDR as T3 ON T0.\"DocEntry\" = T3.\"DocEntry\" and T2.\"WhsCode\" = T0.\"WhsCode\"  where T0.DocEntry IN (" + docentryvalList + ") and T0.OpenQty>0";

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

                            query += " and T0.WhsCode IN (" + values + ")";
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
                                        dt.TableName = "SatisSiparisiList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -333, Desc = "SATIŞ SİPARİŞİ BULUNAMADI." };
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