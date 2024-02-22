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
    public class GetIadeTalepleri
    {

        public Response getIadeTalepleri(string dbName, string startDate, string endDate, List<string> WhsCodes, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName,mKod);

                if (connstring != "")
                {

                    var query = "select DISTINCT T0.DocNum as [Belge Numarası],T0.CardCode as [Müşteri Kodu], T0.CardName as [Müşteri Adı], CONVERT(varchar, T0.TaxDate, 103) as [Belge Tarihi], CONVERT(varchar, T0.DocDueDate, 103) as [Teslimat Tarihi],T0.ShipToCode as [Sevkiyat Adresi] from ORRR as T0 INNER JOIN RRR1 as T1 ON T0.DocEntry = T1.DocEntry where T0.DocDueDate >= '" + startDate + "' and T0.DocDueDate<='" + endDate + "' and T0.\"DocStatus\" = 'O'and T0.\"DocType\" = 'I'";

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
                                        dt.TableName = "İadeTalepleri";

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


        public Response getIadeTalepDetay(string dbName, List<string> docEntryList, List<string> WhsCodes, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

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

                    var query = "select T0.DocEntry as \"BelgeNumarasi\", T0.ItemCode as \"KalemKodu\", T0.Dscription as \"KalemAdi\", T0.OpenQty as \"Miktar\", T0.UomCode as \"OlcuBirimi\",T1.\"ManBtchNum\" as \"Partili\",T1.\"CodeBars\" as \"Barkod\",T0.\"WhsCode\" as \"DepoKodu\",\"LineNum\" as \"SiraNo\",T2.\"OnHand\" as \"DepoMiktar\" ";

                    if (mKod == "10TRMN" || mKod == "20TRMN")
                    {
                        query += ",T1.\"U_KoliMiktari\" as \"KoliIciAD\",T1.\"U_PALET\" as \"PaletIciAD\",T1.\"U_PaletKoli\" as \"PaletIciKoliAD\", ";
                    }
                    else
                    {
                        query += ", 0 as \"KoliIciAD\",0 as \"PaletIciAD\",0 as \"PaletIciKoliAD\", ";
                    }

                    query += "T4.\"Substitute\" as \"MuhatapKatalogNo\",T6.\"WhsName\" as \"DepoTanimi\",CAST(T7.\"AbsEntry\" as VARCHAR(50)) as \"DepoYeriId\",T7.\"BinCode\" as \"DepoYeriAdi\" from RRR1 as T0 INNER JOIN OITM as T1 ON T0.ItemCode = T1.ItemCode INNER JOIN OITW as T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T2.\"WhsCode\" = T0.\"WhsCode\"  INNER JOIN ORRR AS T5 ON T0.\"DocEntry\" = T5.\"DocEntry\" LEFT JOIN OSCN as T4 ON T5.\"CardCode\" = T4.\"CardCode\" and  T0.\"ItemCode\" = T4.\"ItemCode\" INNER JOIN OWHS AS T6 ON T6.\"WhsCode\" = T0.\"WhsCode\" LEFT JOIN OBIN T7 ON T2.\"DftBinAbs\" = T7.\"AbsEntry\" where T0.DocEntry IN (" + docentryvalList + ") and T0.OpenQty>0";

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
                                        dt.TableName = "IadeTalepDetay";

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