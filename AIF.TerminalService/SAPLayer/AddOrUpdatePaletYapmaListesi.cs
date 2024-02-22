using AIF.TerminalService.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{

    public class AddOrUpdatePaletYapmaListesi
    {
        private string companyDbCode;
        int clNum = 0;


        public Response addOrUpdatePaletYapmaListesi(string dbName, string paletYapmadaDepoYeriSecilsin, PaletYapma paletYapma)
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

                connection = log.getSAPConnection(dbName, ID);

                if (connection.number == -1)
                {
                    return new Response { Val = -3100, Desc = "Hata Kodu - 3100 Veritabanı bağlantısı sırasında hata oluştu. ", _list = null };
                }

                clNum = connection.number;
                companyDbCode = connection.dbCode;
                SAPbobsCOM.Company oCompany = connection.oCompany;

                Recordset oRS = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                //DateTime dt = new DateTime(Convert.ToInt32(stockCounting.SayimTarihi.Substring(0, 4)), Convert.ToInt32(stockCounting.SayimTarihi.Substring(4, 2)), Convert.ToInt32(stockCounting.SayimTarihi.Substring(6, 2)));

                //oGeneralData.SetProperty("U_Tarih", dt);
                string condition = oCompany.DbServerType == BoDataServerTypes.dst_HANADB ? "IFNULL" : "ISNULL";
                //var formatdt = dt.Year.ToString() + "-" + dt.Month.ToString().PadLeft(2, '0') + "-" + dt.Day.ToString().PadLeft(2, '0');

                oRS.DoQuery("Select \"DocEntry\" from \"@AIF_WMS_PALET\" as T0 where T0.\"U_PaletNo\" = '" + paletYapma.PaletNumarasi + "'");

                if (oRS.RecordCount == 0) //Daha önce bu partiye kayıt girilmiş mi?
                {
                    CompanyService oCompService = null;

                    GeneralService oGeneralService;

                    GeneralData oGeneralData;

                    GeneralData oChildSatir1;

                    GeneralDataCollection oChildrenSatir1;

                    GeneralData oChildSatirParti;

                    GeneralDataCollection oChildrenSatirParti;

                    oCompService = oCompany.GetCompanyService();

                    //oCompany.StartTransaction();

                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_PALET");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralData.SetProperty("U_PaletNo", paletYapma.PaletNumarasi.ToString());

                    oGeneralData.SetProperty("U_Durum", paletYapma.Durum.ToString());

                    oGeneralData.SetProperty("U_ToplamKap", paletYapma.ToplamKap);

                    oGeneralData.SetProperty("U_NetKilo", paletYapma.NetKilo);

                    oGeneralData.SetProperty("U_BrutKilo", paletYapma.BrutKilo);

                    oChildrenSatir1 = oGeneralData.Child("AIF_WMS_PALET1");

                    foreach (var item in paletYapma.paletYapmaDetays)
                    {
                        oChildSatir1 = oChildrenSatir1.Add();

                        oChildSatir1.SetProperty("U_Barkod", item.Barkod);

                        oChildSatir1.SetProperty("U_MuhKatalogNo", item.MuhatapKatalogNo);

                        oChildSatir1.SetProperty("U_KalemKodu", item.KalemKodu);

                        oChildSatir1.SetProperty("U_Tanim", item.KalemTanimi);

                        oChildSatir1.SetProperty("U_Miktar", item.Quantity);

                        if (item.SiparisNumarasi != null && item.SiparisNumarasi != -1)
                        {
                            oChildSatir1.SetProperty("U_SiparisNo", item.SiparisNumarasi);
                        }

                        if (item.SiparisSatirNo != null && item.SiparisSatirNo != -1)
                        {
                            oChildSatir1.SetProperty("U_SipSatirNo", item.SiparisSatirNo);
                        }

                        oChildSatir1.SetProperty("U_DetaySatirNo", item.DetaySatirNo);

                        oChildSatir1.SetProperty("U_CekmeNo", item.CekmeNo);

                        oChildSatir1.SetProperty("U_Kaynak", item.Kaynak);

                        oChildSatir1.SetProperty("U_DepoKodu", item.DepoKodu);

                        oChildSatir1.SetProperty("U_DepoAdi", item.DepoAdi);

                        oChildSatir1.SetProperty("U_DepoYeriId", item.DepoYeriId);

                        oChildSatir1.SetProperty("U_DepoYeriAdi", item.DepoYeriAdi);


                        if (paletYapmadaDepoYeriSecilsin == "Y")
                        {
                            oChildrenSatirParti = oGeneralData.Child("AIF_WMS_PALET2");

                            foreach (var itemParti in item.PaletYapmaPartilers)
                            {
                                oChildSatirParti = oChildrenSatirParti.Add();

                                oChildSatirParti.SetProperty("U_Barkod", itemParti.Barkod);

                                oChildSatirParti.SetProperty("U_KalemKodu", itemParti.KalemKodu);

                                oChildSatirParti.SetProperty("U_Tanim", itemParti.KalemTanimi);

                                oChildSatirParti.SetProperty("U_PartiNo", itemParti.PartiNumarasi);

                                oChildSatirParti.SetProperty("U_Miktar", itemParti.Miktar);

                                oChildSatirParti.SetProperty("U_DepoKodu", itemParti.DepoKodu);

                                oChildSatirParti.SetProperty("U_DepoAdi", itemParti.DepoAdi);

                                oChildSatirParti.SetProperty("U_DepoYeriId", itemParti.DepoYeriId);

                                oChildSatirParti.SetProperty("U_DepoYeriAdi", itemParti.DepoYeriAdi);

                                oChildSatirParti.SetProperty("U_PartiSatirNo", itemParti.PartiSatirNo);
                            }
                        }
                    }


                    oRS.DoQuery("Select ISNULL(MAX(\"DocEntry\"),0) + 1 from \"@AIF_WMS_PALET\"");

                    int maxdocentry = Convert.ToInt32(oRS.Fields.Item(0).Value);

                    oGeneralData.SetProperty("DocNum", maxdocentry);

                    var resp = oGeneralService.Add(oGeneralData);

                    if (resp != null)
                    {
                        //if (oCompany.InTransaction)
                        //{
                        //    oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        //}
                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = 0, Desc = "Başarılı.", _list = null };
                    }
                    else
                    {
                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                    }
                }
                else
                {
                    CompanyService oCompService = null;

                    GeneralService oGeneralService;

                    GeneralData oGeneralData;

                    GeneralData oChildSatir1;

                    GeneralDataCollection oChildrenSatir1;

                    GeneralData oChildSatirParti;

                    GeneralDataCollection oChildrenSatirParti;

                    oCompService = oCompany.GetCompanyService();

                    GeneralDataParams oGeneralParams;

                    //oCompany.StartTransaction();

                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_PALET");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(oRS.Fields.Item("DocEntry").Value));
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                    oGeneralData.SetProperty("U_PaletNo", paletYapma.PaletNumarasi.ToString());

                    oGeneralData.SetProperty("U_Durum", paletYapma.Durum.ToString());

                    oGeneralData.SetProperty("U_ToplamKap", paletYapma.ToplamKap);

                    oGeneralData.SetProperty("U_NetKilo", paletYapma.NetKilo);

                    oGeneralData.SetProperty("U_BrutKilo", paletYapma.BrutKilo);


                    //DateTime dt = new DateTime(Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(0, 4)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(4, 2)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(6, 2)));

                    oChildrenSatir1 = oGeneralData.Child("AIF_WMS_PALET1");

                    if (oChildrenSatir1.Count > 0)
                    {
                        int drc = oChildrenSatir1.Count;
                        for (int rmv = 0; rmv < drc; rmv++)
                            oChildrenSatir1.Remove(0);
                    }

                    oChildrenSatirParti = oGeneralData.Child("AIF_WMS_PALET2");

                    if (oChildrenSatirParti.Count > 0)
                    {
                        int drc = oChildrenSatirParti.Count;
                        for (int rmv = 0; rmv < drc; rmv++)
                            oChildrenSatirParti.Remove(0);
                    }



                    foreach (var item in paletYapma.paletYapmaDetays)
                    {
                        oChildSatir1 = oChildrenSatir1.Add();

                        oChildSatir1.SetProperty("U_Barkod", item.Barkod);

                        oChildSatir1.SetProperty("U_MuhKatalogNo", item.MuhatapKatalogNo.ToString());

                        oChildSatir1.SetProperty("U_KalemKodu", item.KalemKodu.ToString());

                        oChildSatir1.SetProperty("U_Tanim", item.KalemTanimi.ToString());

                        oChildSatir1.SetProperty("U_Miktar", Convert.ToDouble(item.Quantity));

                        if (item.SiparisNumarasi != null && item.SiparisNumarasi != -1)
                        {
                            oChildSatir1.SetProperty("U_SiparisNo", item.SiparisNumarasi);
                        }

                        if (item.SiparisSatirNo != null && item.SiparisSatirNo != -1)
                        {
                            oChildSatir1.SetProperty("U_SipSatirNo", item.SiparisSatirNo);
                        }
                        oChildSatir1.SetProperty("U_DetaySatirNo", item.DetaySatirNo);


                        oChildSatir1.SetProperty("U_CekmeNo", item.CekmeNo);

                        oChildSatir1.SetProperty("U_Kaynak", item.Kaynak);

                        oChildSatir1.SetProperty("U_DepoKodu", item.DepoKodu);

                        oChildSatir1.SetProperty("U_DepoAdi", item.DepoAdi);

                        oChildSatir1.SetProperty("U_DepoYeriId", item.DepoYeriId);

                        oChildSatir1.SetProperty("U_DepoYeriAdi", item.DepoYeriAdi);

                        if (paletYapmadaDepoYeriSecilsin == "Y")
                        {

                            foreach (var itemParti in item.PaletYapmaPartilers)
                            {
                                oChildSatirParti = oChildrenSatirParti.Add();

                                oChildSatirParti.SetProperty("U_Barkod", itemParti.Barkod);

                                oChildSatirParti.SetProperty("U_KalemKodu", itemParti.KalemKodu);

                                oChildSatirParti.SetProperty("U_Tanim", itemParti.KalemTanimi);

                                oChildSatirParti.SetProperty("U_PartiNo", itemParti.PartiNumarasi);

                                oChildSatirParti.SetProperty("U_Miktar", itemParti.Miktar);

                                oChildSatirParti.SetProperty("U_DepoKodu", itemParti.DepoKodu);

                                oChildSatirParti.SetProperty("U_DepoAdi", itemParti.DepoAdi);

                                oChildSatirParti.SetProperty("U_DepoYeriId", itemParti.DepoYeriId);

                                oChildSatirParti.SetProperty("U_DepoYeriAdi", itemParti.DepoYeriAdi);

                                oChildSatirParti.SetProperty("U_PartiSatirNo", itemParti.PartiSatirNo);
                            }
                        }
                    }

                    try
                    {
                        oGeneralService.Update(oGeneralData);
                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = 0, Desc = "Başarılı.", _list = null };
                    }
                    catch (Exception)
                    {
                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                    }
                    //finally
                    //{
                    //    LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                    //}

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

    public class KaynakListesi
    {
    }
}