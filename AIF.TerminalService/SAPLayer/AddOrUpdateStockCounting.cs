using AIF.TerminalService.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{

    public class AddOrUpdateStockCounting
    {
        private string companyDbCode;
        int clNum = 0;

        public Response addOrUpdateStockCounting(string dbName, StockCounting stockCounting)
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

                Recordset oRS = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                DateTime dt = new DateTime(Convert.ToInt32(stockCounting.SayimTarihi.Substring(0, 4)), Convert.ToInt32(stockCounting.SayimTarihi.Substring(4, 2)), Convert.ToInt32(stockCounting.SayimTarihi.Substring(6, 2)));

                //oGeneralData.SetProperty("U_Tarih", dt);
                string condition = oCompany.DbServerType == BoDataServerTypes.dst_HANADB ? "IFNULL" : "ISNULL";
                var formatdt = dt.Year.ToString() + "-" + dt.Month.ToString().PadLeft(2, '0') + "-" + dt.Day.ToString().PadLeft(2, '0');
                int record = 0;
                if (stockCounting.VarolanSayimaDevamEdilmekIstendi == "Y") //Depo sayımı tarafında var olan bir sayım listesi için güncelleme yapmak isteniyorsa işaret Y gönderilip sorgu yapılır sayım var ise güncellenir, değil ise yeni eklenir.
                {
                    oRS.DoQuery("Select * from \"@AIF_WMS_WHSCOUNT\" where \"U_SayimTarihi\" = '" + formatdt + "' and \"U_KullaniciId\"= '" + stockCounting.KullaniciId + "' and " + condition + "(\"U_SayimNumarasi\",0)=0");

                    record = oRS.RecordCount;
                }
                #region recordcount ile işlemler
                //if (record == 0) //Daha önce bu partiye kayıt girilmiş mi?
                //{


                //    CompanyService oCompService = null;

                //    GeneralService oGeneralService;

                //    GeneralData oGeneralData;

                //    GeneralData oChildSatir1;

                //    GeneralDataCollection oChildrenSatir;

                //    GeneralData oChildParti;

                //    GeneralDataCollection oChildrenParti;

                //    oCompService = oCompany.GetCompanyService();

                //    //oCompany.StartTransaction();

                //    oGeneralService = oCompService.GetGeneralService("AIF_WMS_WHSCOUNT");

                //    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                //    oGeneralData.SetProperty("U_SayimTarihi", dt.ToString());

                //    oGeneralData.SetProperty("U_KullaniciId", stockCounting.KullaniciId);

                //    oGeneralData.SetProperty("U_Aciklama", stockCounting.Aciklama);


                //    #region kullanici id ve adı
                //    //oGeneralData.SetProperty("U_KullaniciId", stockCounting.KullaniciId);

                //    oRS.DoQuery("Select * FROM OHEM WHERE \"empID\"= '" + stockCounting.KullaniciId + "'");

                //    string ad = oRS.Fields.Item("firstName").Value.ToString();
                //    string ikinciad = oRS.Fields.Item("middleName").Value.ToString();
                //    string soyad = oRS.Fields.Item("lastName").Value.ToString();

                //    if (ikinciad != "")
                //    {
                //        oGeneralData.SetProperty("U_KullaniciAdi", ad + " " + ikinciad + " " + soyad);
                //    }
                //    else
                //    {
                //        oGeneralData.SetProperty("U_KullaniciAdi", ad + " " + soyad);
                //    }

                //    #endregion

                //    oChildrenSatir = oGeneralData.Child("AIF_WMS_WHSCOUNT1");

                //    foreach (var item in stockCounting.stockCountings)
                //    {
                //        oChildSatir1 = oChildrenSatir.Add();

                //        oChildSatir1.SetProperty("U_Barkod", item.Barkod);

                //        oChildSatir1.SetProperty("U_KalemKodu", item.KalemKodu);

                //        oChildSatir1.SetProperty("U_KalemTanimi", item.KalemTanimi);

                //        oChildSatir1.SetProperty("U_DepoKodu", item.DepoKodu);

                //        oChildSatir1.SetProperty("U_DepoAdi", item.DepoAdi);

                //        oChildSatir1.SetProperty("U_Miktar", item.Miktar);

                //        oChildSatir1.SetProperty("U_OlcuBirimi", item.OlcuBirimi);

                //        if (item.DepoYeriId != null)
                //        {
                //            oChildSatir1.SetProperty("U_DepoYeriId", item.DepoYeriId.ToString());

                //            oChildSatir1.SetProperty("U_DepoYeriAdi", item.DepoYeriAdi.ToString());
                //        }
                //    }

                //    oChildrenParti = oGeneralData.Child("AIF_WMS_WHSCOUNT2");

                //    foreach (var item in stockCounting.StockCountingParties)
                //    {
                //        oChildParti = oChildrenParti.Add();

                //        oChildParti.SetProperty("U_KalemKodu", item.KalemKodu);

                //        oChildParti.SetProperty("U_DepoKodu", item.DepoKodu);

                //        oChildParti.SetProperty("U_PartiNo", item.PartiNo);

                //        oChildParti.SetProperty("U_Miktar", item.Miktar);
                //    }

                //    oRS.DoQuery("Select ISNULL(MAX(\"DocEntry\"),0) + 1 from \"@AIF_WMS_WHSCOUNT\"");

                //    int maxdocentry = Convert.ToInt32(oRS.Fields.Item(0).Value);

                //    oGeneralData.SetProperty("DocNum", maxdocentry);

                //    var resp = oGeneralService.Add(oGeneralData);

                //    if (resp != null)
                //    {
                //        //if (oCompany.InTransaction)
                //        //{
                //        //    oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                //        //}
                //        LoginCompany.ReleaseConnection(connection.number, companyDbCode);
                //        return new Response { Val = 0, Desc = "Başarılı.", _list = null };
                //    }
                //    else
                //    {
                //        LoginCompany.ReleaseConnection(connection.number, companyDbCode);
                //        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                //    }
                //}
                //else
                //{
                //    CompanyService oCompService = null;

                //    GeneralService oGeneralService;

                //    GeneralData oGeneralData;

                //    GeneralData oChildSatir1;

                //    GeneralDataCollection oChildrenSatir;

                //    GeneralData oChildParti;

                //    GeneralDataCollection oChildrenParti;

                //    oCompService = oCompany.GetCompanyService();

                //    GeneralDataParams oGeneralParams;

                //    //oCompany.StartTransaction();

                //    oGeneralService = oCompService.GetGeneralService("AIF_WMS_WHSCOUNT");

                //    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                //    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                //    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(oRS.Fields.Item("DocEntry").Value));
                //    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                //    oGeneralData.SetProperty("U_SayimTarihi", dt.ToString());

                //    oGeneralData.SetProperty("U_KullaniciId", stockCounting.KullaniciId);

                //    oGeneralData.SetProperty("U_Aciklama", stockCounting.Aciklama);

                //    //DateTime dt = new DateTime(Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(0, 4)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(4, 2)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(6, 2)));

                //    //oGeneralData.SetProperty("U_Tarih", dt);

                //    #region kullanici id ve adı
                //    //oGeneralData.SetProperty("U_KullaniciId", stockCounting.KullaniciId);

                //    oRS.DoQuery("Select * FROM OHEM WHERE \"empID\"= '" + stockCounting.KullaniciId + "'");

                //    string ad = oRS.Fields.Item("firstName").Value.ToString();
                //    string ikinciad = oRS.Fields.Item("middleName").Value.ToString();
                //    string soyad = oRS.Fields.Item("lastName").Value.ToString();

                //    if (ikinciad != "")
                //    {
                //        oGeneralData.SetProperty("U_KullaniciAdi", ad + " " + ikinciad + " " + soyad);
                //    }
                //    else
                //    {
                //        oGeneralData.SetProperty("U_KullaniciAdi", ad + " " + soyad);
                //    }

                //    #endregion

                //    oChildrenSatir = oGeneralData.Child("AIF_WMS_WHSCOUNT1");

                //    if (oChildrenSatir.Count > 0)
                //    {
                //        int drc = oChildrenSatir.Count;
                //        for (int rmv = 0; rmv < drc; rmv++)
                //            oChildrenSatir.Remove(0);
                //    }

                //    foreach (var item in stockCounting.stockCountings)
                //    {
                //        oChildSatir1 = oChildrenSatir.Add();

                //        oChildSatir1.SetProperty("U_Barkod", item.Barkod);

                //        oChildSatir1.SetProperty("U_KalemKodu", item.KalemKodu);

                //        oChildSatir1.SetProperty("U_KalemTanimi", item.KalemTanimi);

                //        oChildSatir1.SetProperty("U_DepoKodu", item.DepoKodu);

                //        oChildSatir1.SetProperty("U_DepoAdi", item.DepoAdi);

                //        oChildSatir1.SetProperty("U_Miktar", item.Miktar);

                //        oChildSatir1.SetProperty("U_OlcuBirimi", item.OlcuBirimi);

                //        oChildSatir1.SetProperty("U_DepoYeriId", item.DepoYeriId.ToString());

                //        oChildSatir1.SetProperty("U_DepoYeriAdi", item.DepoYeriAdi.ToString());
                //    }

                //    oChildrenParti = oGeneralData.Child("AIF_WMS_WHSCOUNT2");

                //    if (oChildrenParti.Count > 0)
                //    {
                //        int drc = oChildrenParti.Count;
                //        for (int rmv = 0; rmv < drc; rmv++)
                //            oChildrenParti.Remove(0);
                //    }

                //    foreach (var item in stockCounting.StockCountingParties)
                //    {
                //        oChildParti = oChildrenParti.Add();

                //        oChildParti.SetProperty("U_KalemKodu", item.KalemKodu);

                //        oChildParti.SetProperty("U_DepoKodu", item.DepoKodu);

                //        oChildParti.SetProperty("U_PartiNo", item.PartiNo);

                //        oChildParti.SetProperty("U_Miktar", item.Miktar);
                //    }


                //    try
                //    {
                //        oGeneralService.Update(oGeneralData);
                //        LoginCompany.ReleaseConnection(connection.number, companyDbCode);
                //        return new Response { Val = 0, Desc = "Başarılı.", _list = null };
                //    }
                //    catch (Exception)
                //    {
                //        LoginCompany.ReleaseConnection(connection.number, companyDbCode);
                //        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                //    }

                //} 
                #endregion

                if (stockCounting.BelgeNo == "") //Daha önce bu partiye kayıt girilmiş mi?
                {

                    CompanyService oCompService = null;

                    GeneralService oGeneralService;

                    GeneralData oGeneralData;

                    GeneralData oChildSatir1;

                    GeneralDataCollection oChildrenSatir;

                    GeneralData oChildParti;

                    GeneralDataCollection oChildrenParti;

                    oCompService = oCompany.GetCompanyService();

                    //oCompany.StartTransaction();

                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_WHSCOUNT");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralData.SetProperty("U_SayimTarihi", dt.ToString());

                    oGeneralData.SetProperty("U_KullaniciId", stockCounting.KullaniciId);

                    oGeneralData.SetProperty("U_Aciklama", stockCounting.Aciklama);


                    #region kullanici id ve adı
                    //oGeneralData.SetProperty("U_KullaniciId", stockCounting.KullaniciId);

                    oRS.DoQuery("Select * FROM OHEM WHERE \"empID\"= '" + stockCounting.KullaniciId + "'");

                    string ad = oRS.Fields.Item("firstName").Value.ToString();
                    string ikinciad = oRS.Fields.Item("middleName").Value.ToString();
                    string soyad = oRS.Fields.Item("lastName").Value.ToString();

                    if (ikinciad != "")
                    {
                        oGeneralData.SetProperty("U_KullaniciAdi", ad + " " + ikinciad + " " + soyad);
                    }
                    else
                    {
                        oGeneralData.SetProperty("U_KullaniciAdi", ad + " " + soyad);
                    }

                    #endregion

                    oChildrenSatir = oGeneralData.Child("AIF_WMS_WHSCOUNT1");

                    foreach (var item in stockCounting.stockCountings)
                    {
                        oChildSatir1 = oChildrenSatir.Add();

                        oChildSatir1.SetProperty("U_Barkod", item.Barkod);

                        oChildSatir1.SetProperty("U_KalemKodu", item.KalemKodu);

                        oChildSatir1.SetProperty("U_KalemTanimi", item.KalemTanimi);

                        oChildSatir1.SetProperty("U_DepoKodu", item.DepoKodu);

                        oChildSatir1.SetProperty("U_DepoAdi", item.DepoAdi);

                        oChildSatir1.SetProperty("U_Miktar", item.Miktar);

                        oChildSatir1.SetProperty("U_OlcuBirimi", item.OlcuBirimi);

                        if (item.DepoYeriId != null)
                        {
                            oChildSatir1.SetProperty("U_DepoYeriId", item.DepoYeriId.ToString());

                        }

                        if (item.DepoYeriAdi != null)
                        {
                            oChildSatir1.SetProperty("U_DepoYeriAdi", item.DepoYeriAdi.ToString());
                        }

                    }

                    oChildrenParti = oGeneralData.Child("AIF_WMS_WHSCOUNT2");

                    foreach (var item in stockCounting.StockCountingParties)
                    {
                        oChildParti = oChildrenParti.Add();

                        oChildParti.SetProperty("U_KalemKodu", item.KalemKodu);

                        oChildParti.SetProperty("U_DepoKodu", item.DepoKodu);

                        oChildParti.SetProperty("U_PartiNo", item.PartiNo);

                        oChildParti.SetProperty("U_Miktar", item.Miktar);
                    }

                    oRS.DoQuery("Select ISNULL(MAX(\"DocEntry\"),0) + 1 from \"@AIF_WMS_WHSCOUNT\"");

                    int maxdocentry = Convert.ToInt32(oRS.Fields.Item(0).Value);

                    oGeneralData.SetProperty("DocNum", maxdocentry);

                    var resp = oGeneralService.Add(oGeneralData);

                    if (resp != null)
                    {
                        //if (oCompany.InTransaction)
                        //{
                        //    oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        //}
                        LoginCompany.ReleaseConnection(connection.number, companyDbCode,ID);
                        return new Response { Val = 0, Desc = "Başarılı.", _list = null };
                    }
                    else
                    {
                        LoginCompany.ReleaseConnection(connection.number, companyDbCode,ID);
                        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                    }

                }
                else
                {

                    CompanyService oCompService = null;

                    GeneralService oGeneralService;

                    GeneralData oGeneralData;

                    GeneralData oChildSatir1;

                    GeneralDataCollection oChildrenSatir;

                    GeneralData oChildParti;

                    GeneralDataCollection oChildrenParti;

                    oCompService = oCompany.GetCompanyService();

                    GeneralDataParams oGeneralParams;

                    //oCompany.StartTransaction();

                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_WHSCOUNT");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(stockCounting.BelgeNo.ToString()));
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                    oGeneralData.SetProperty("U_SayimTarihi", dt.ToString());

                    oGeneralData.SetProperty("U_KullaniciId", stockCounting.KullaniciId);

                    oGeneralData.SetProperty("U_Aciklama", stockCounting.Aciklama);

                    //DateTime dt = new DateTime(Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(0, 4)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(4, 2)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(6, 2)));

                    //oGeneralData.SetProperty("U_Tarih", dt);

                    #region kullanici id ve adı
                    //oGeneralData.SetProperty("U_KullaniciId", stockCounting.KullaniciId);

                    oRS.DoQuery("Select * FROM OHEM WHERE \"empID\"= '" + stockCounting.KullaniciId + "'");

                    string ad = oRS.Fields.Item("firstName").Value.ToString();
                    string ikinciad = oRS.Fields.Item("middleName").Value.ToString();
                    string soyad = oRS.Fields.Item("lastName").Value.ToString();

                    if (ikinciad != "")
                    {
                        oGeneralData.SetProperty("U_KullaniciAdi", ad + " " + ikinciad + " " + soyad);
                    }
                    else
                    {
                        oGeneralData.SetProperty("U_KullaniciAdi", ad + " " + soyad);
                    }

                    #endregion

                    oChildrenSatir = oGeneralData.Child("AIF_WMS_WHSCOUNT1");

                    if (oChildrenSatir.Count > 0)
                    {
                        int drc = oChildrenSatir.Count;
                        for (int rmv = 0; rmv < drc; rmv++)
                            oChildrenSatir.Remove(0);
                    }

                    foreach (var item in stockCounting.stockCountings)
                    {
                        oChildSatir1 = oChildrenSatir.Add();

                        oChildSatir1.SetProperty("U_Barkod", item.Barkod);

                        oChildSatir1.SetProperty("U_KalemKodu", item.KalemKodu);

                        oChildSatir1.SetProperty("U_KalemTanimi", item.KalemTanimi);

                        oChildSatir1.SetProperty("U_DepoKodu", item.DepoKodu);

                        oChildSatir1.SetProperty("U_DepoAdi", item.DepoAdi);

                        oChildSatir1.SetProperty("U_Miktar", item.Miktar);

                        oChildSatir1.SetProperty("U_OlcuBirimi", item.OlcuBirimi);

                        if (item.DepoYeriId != null)
                        {
                            oChildSatir1.SetProperty("U_DepoYeriId", item.DepoYeriId.ToString());
                        }

                        if (item.DepoYeriAdi != null)
                        {
                            oChildSatir1.SetProperty("U_DepoYeriAdi", item.DepoYeriAdi.ToString());
                        }
                    }

                    oChildrenParti = oGeneralData.Child("AIF_WMS_WHSCOUNT2");

                    if (oChildrenParti.Count > 0)
                    {
                        int drc = oChildrenParti.Count;
                        for (int rmv = 0; rmv < drc; rmv++)
                            oChildrenParti.Remove(0);
                    }

                    foreach (var item in stockCounting.StockCountingParties)
                    {
                        oChildParti = oChildrenParti.Add();

                        oChildParti.SetProperty("U_KalemKodu", item.KalemKodu);

                        oChildParti.SetProperty("U_DepoKodu", item.DepoKodu);

                        oChildParti.SetProperty("U_PartiNo", item.PartiNo);

                        oChildParti.SetProperty("U_Miktar", item.Miktar);
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
                LoginCompany.ReleaseConnection(clNum, companyDbCode,ID);
                return new Response { Val = -9000, Desc = "Bilinmeyen hata oluştu. " + ex.ToString(), _list = null };
            }
            finally
            {
                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
            }

        }
    }
}