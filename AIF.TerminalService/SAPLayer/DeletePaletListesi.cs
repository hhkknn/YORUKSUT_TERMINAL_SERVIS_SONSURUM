using AIF.TerminalService.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{

    public class DeletePaletListesi
    {
        private string companyDbCode;
        int clNum = 0;


        public Response deletePaletListesi(string dbName, List<SilinecekPaletler> silinecekPaletlers, string cekmeListesiKalemleriniGrupla)
        {
            Random rastgele = new Random();
            int ID = rastgele.Next(0, 9999);
            try
            {
                //ConnectionList connection = new ConnectionList();

                //LoginCompany log = new LoginCompany();

                //connection = log.getSAPConnection(dbName);

                //if (connection.number == -1)
                //{

                //    for (int ix = 1; ix <= 3; ix++)
                //    {
                //        connection = log.getSAPConnection(dbName);

                //        if (connection.number > -1)
                //        {
                //            break;
                //        }
                //    }

                //}

                ConnectionList connection = new ConnectionList();

                LoginCompany log = new LoginCompany();

                log.DisconnectSAP(dbName);

                connection = log.getSAPConnection(dbName,ID);

                if (connection.number == -1)
                {
                    return new Response { Val = -3100, Desc = "Hata Kodu - 3100 Veritabanı bağlantısı sırasında hata oluştu. ", _list = null };
                }

                clNum = connection.number;
                companyDbCode = connection.dbCode;
                SAPbobsCOM.Company oCompany = connection.oCompany;

                oCompany.StartTransaction();
                Recordset oRS = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string sql = "";

                foreach (var item in silinecekPaletlers)
                {
                    if (cekmeListesiKalemleriniGrupla != "Y")
                    {
                        sql = "DELETE FROM \"@AIF_WMS_KNTYNR1\" where \"U_PaletNo\" = '" + item.paletNo + "' and \"U_SiparisNo\" = '" + item.siparisNo + "' and \"U_SipSatirNo\" = '" + item.siparisSatirNo + "' and \"U_CekmeNo\" = '" + item.cekmeNo + "' ";
                    }
                    else
                    {
                        sql = "DELETE FROM \"@AIF_WMS_KNTYNR1\" where \"U_PaletNo\" = '" + item.paletNo + "' and \"U_KalemKodu\" = '" + item.kalemKodu + "' and \"U_CekmeNo\" = '" + item.cekmeNo + "' ";

                    }


                    try
                    {
                        oRS.DoQuery(sql);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        catch (Exception)
                        {
                        }

                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + ex.Message, _list = null };
                    }


                    if (cekmeListesiKalemleriniGrupla != "Y")
                    {

                        sql = "DELETE FROM \"@AIF_WMS_KOLIDTY1\" where \"U_PaletNo\" = '" + item.paletNo + "' and \"U_SiparisNo\" = '" + item.siparisNo + "' and \"U_SatirNo\" = '" + item.siparisSatirNo + "'";

                        //Çekme numarası alanı inner join ile yapılacak. 
                    }
                    else
                    {
                        sql = "DELETE FROM \"@AIF_WMS_KOLIDTY1\" where \"U_PaletNo\" = '" + item.paletNo + "' and \"U_KalemKodu\" = '" + item.kalemKodu + "'";

                        //Çekme numarası alanı inner join ile yapılacak.  
                    }

                    try
                    {
                        oRS.DoQuery(sql);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        catch (Exception)
                        {
                        }

                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + ex.Message, _list = null };
                    }



                    if (cekmeListesiKalemleriniGrupla != "Y")
                    {
                        sql = "DELETE FROM \"@AIF_WMS_TOPLANAN\" where \"U_PaletNo\" = '" + item.paletNo + "' and \"U_SiparisNumarasi\" = '" + item.siparisNo + "' and \"U_SiparisSatirNo\" = '" + item.siparisSatirNo + "' and \"U_BelgeNo\" = '" + item.cekmeNo + "' ";
                    }
                    else
                    {
                        sql = "DELETE FROM \"@AIF_WMS_TOPLANAN\" where \"U_PaletNo\" = '" + item.paletNo + "' and \"U_KalemKodu\" = '" + item.kalemKodu + "' and \"U_BelgeNo\" = '" + item.cekmeNo + "' ";
                    }

                    try
                    {
                        oRS.DoQuery(sql);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        catch (Exception)
                        {
                        }

                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + ex.Message, _list = null };
                    }



                    if (cekmeListesiKalemleriniGrupla != "Y")
                    {
                        sql = "UPDATE \"@AIF_WMS_SIPKAR1\" SET \"U_Gorunur\" = 'Y', \"U_PaletNo\" = '' WHERE \"DocEntry\" = '" + item.cekmeNo + "' and \"U_SipSatirNo\" = '" + item.siparisSatirNo + "' and \"U_SiparisNumarasi\" = '" + item.siparisNo + "' and \"U_PaletNo\" = '" + item.paletNo + "'";
                    }
                    else
                    {
                        //sql = "UPDATE \"@AIF_WMS_SIPKAR1\" SET \"U_Gorunur\" = 'Y', \"U_PaletNo\" = '' WHERE \"DocEntry\" = '" + item.cekmeNo + "' and \"U_UrunKodu\" = '" + item.kalemKodu + "' and \"U_PaletNo\" = '" + item.paletNo + "'"; 

                        string[] cekmeNoListesi = new string[1];
                        if (item.kaynak.Contains(","))
                        {
                            var cc = item.kaynak.Split(',');

                            if (cc.Count() > 1)
                            {
                                cekmeNoListesi = new string[cc.Count()];

                                cekmeNoListesi = item.kaynak.Split(',');
                            }
                        }
                        else
                        {
                            var cc = item.kaynak.Split(',');

                            if (cc.Count() > 1)
                            {
                                cekmeNoListesi = new string[cc.Count()];

                                cekmeNoListesi = item.kaynak.Split(',');
                            }
                            else
                            {
                                cekmeNoListesi[0] = item.kaynak;
                            }
                        }

                        SAPbobsCOM.Recordset oRS_SIPKAR = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                        double silinecekMiktar = item.miktar;
                        double sipkarMiktar = 0;
                        string sipkarLineId = "";
                        for (int i = 0; i <= cekmeNoListesi.Count() - 1; i++)
                        {
                            if (silinecekMiktar == 0)
                            {
                                break;
                            }

                            if (cekmeNoListesi[i] == null)
                            {
                                continue;
                            }
                            sql = "Select \"LineId\",\"U_ToplananMik\" FROM \"@AIF_WMS_SIPKAR1\" t0 WHERE \"DocEntry\" = '" + item.cekmeNo + "' and \"U_UrunKodu\" = '" + item.kalemKodu + "' and CONCAT(\"U_SiparisNumarasi\", '-', \"U_SipSatirNo\")  = '" + cekmeNoListesi[i].ToString() + "'";

                            oRS_SIPKAR.DoQuery(sql);

                            sipkarMiktar = Convert.ToDouble(oRS_SIPKAR.Fields.Item("U_ToplananMik").Value);
                            sipkarLineId = oRS_SIPKAR.Fields.Item("LineId").Value.ToString();

                            while (!oRS_SIPKAR.EoF)
                            {
                                if (sipkarMiktar <= 0)
                                {
                                    break;
                                }

                                if (sipkarMiktar > silinecekMiktar)
                                {
                                    sql = "UPDATE \"@AIF_WMS_SIPKAR1\" SET \"U_Gorunur\" = 'Y',\"U_PaletNo\" = ''  ,\"U_ToplananMik\" = " + (sipkarMiktar - silinecekMiktar) + " FROM \"@AIF_WMS_SIPKAR1\" t0 WHERE \"DocEntry\" = '" + item.cekmeNo + "' and \"LineId\" = '" + sipkarLineId + "'";


                                    try
                                    {
                                        oRS.DoQuery(sql);
                                    }
                                    catch (Exception ex)
                                    {
                                        try
                                        {
                                            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                                        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + ex.Message, _list = null };
                                    }
                                }
                                else
                                {
                                    sql = "UPDATE \"@AIF_WMS_SIPKAR1\" SET \"U_Gorunur\" = 'Y',\"U_PaletNo\" = '',\"U_ToplananMik\" = 0 FROM \"@AIF_WMS_SIPKAR1\" t0 WHERE \"DocEntry\" = '" + item.cekmeNo + "' and \"LineId\" = '" + sipkarLineId + "'";

                                    silinecekMiktar = silinecekMiktar - sipkarMiktar;


                                    try
                                    {
                                        oRS.DoQuery(sql);
                                    }
                                    catch (Exception ex)
                                    {
                                        try
                                        {
                                            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                                        }
                                        catch (Exception)
                                        {
                                        }

                                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                                        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + ex.Message, _list = null };
                                    }
                                }

                                oRS_SIPKAR.MoveNext();
                            }


                            //sql = "UPDATE \"@AIF_WMS_SIPKAR1\" SET \"U_Gorunur\" = 'Y',\"U_PaletNo\" = ''  ,\"U_ToplananMik\" = 0 FROM \"@AIF_WMS_SIPKAR1\" t0 WHERE \"DocEntry\" = '" + item.cekmeNo + "' and \"U_UrunKodu\" = '" + item.kalemKodu + "' and(SELECT STRING_AGG(CONCAT(\"U_SiparisNumarasi\", '-', \"U_SipSatirNo\"), ',') FROM \"@AIF_WMS_SIPKAR1\" WHERE \"DocEntry\" = t0.\"DocEntry\" AND \"U_UrunKodu\" = t0.\"U_UrunKodu\") = '" + item.kaynak + "'";

                            //sql = "UPDATE \"@AIF_WMS_SIPKAR1\" SET \"U_Gorunur\" = 'Y',\"U_PaletNo\" = ''  ,\"U_ToplananMik\" = 0 FROM \"@AIF_WMS_SIPKAR1\" t0 WHERE \"DocEntry\" = '" + item.cekmeNo + "' and \"U_UrunKodu\" = '" + item.kalemKodu + "' and CONCAT(\"U_SiparisNumarasi\", '-', \"U_SipSatirNo\")  = '" + cekmeNoListesi[i].ToString() + "'";

                            //try
                            //{
                            //    oRS.DoQuery(sql);
                            //}
                            //catch (Exception ex)
                            //{
                            //    try
                            //    {
                            //        oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            //    }
                            //    catch (Exception)
                            //    {
                            //    }

                            //    LoginCompany.ReleaseConnection(connection.number, companyDbCode);
                            //    return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + ex.Message, _list = null };
                            //}
                        }
                    }


                }


                if (oCompany.InTransaction)
                {
                    oCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                    LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                    return new Response { Val = 0, Desc = "Başarılı.", _list = null };
                }
                else
                {
                    LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                    return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. Transaction Error. ", _list = null };
                }


            }
            catch (Exception ex)
            {
                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
                return new Response { Val = -9000, Desc = "Bilinmeyen hata oluştu. " + ex.ToString(), _list = null };
            }
            finally
            {
                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
            }

        }
    }
}