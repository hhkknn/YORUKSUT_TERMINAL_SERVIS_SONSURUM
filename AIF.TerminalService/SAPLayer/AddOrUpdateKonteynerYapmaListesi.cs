using AIF.TerminalService.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{

    public class AddOrUpdateKonteynerYapmaListesi
    {
        private string companyDbCode;
        int clNum = 0;
         
        public Response addOrUpdateKonteynerYapmaListesi(string dbName, KonteynerYapma konteynerYapma, List<KonteynerIcindenSilinenler> konteynerIcindenSilinenlers, string cekmeListesiKalemleriniGrupla)
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
                Recordset oRS_LineId = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                //DateTime dt = new DateTime(Convert.ToInt32(stockCounting.SayimTarihi.Substring(0, 4)), Convert.ToInt32(stockCounting.SayimTarihi.Substring(4, 2)), Convert.ToInt32(stockCounting.SayimTarihi.Substring(6, 2)));

                //oGeneralData.SetProperty("U_Tarih", dt);
                string condition = oCompany.DbServerType == BoDataServerTypes.dst_HANADB ? "IFNULL" : "ISNULL";
                //var formatdt = dt.Year.ToString() + "-" + dt.Month.ToString().PadLeft(2, '0') + "-" + dt.Day.ToString().PadLeft(2, '0');

                if (oCompany.DbServerType == BoDataServerTypes.dst_HANADB)
                {
                    oRS.DoQuery("Select \"DocEntry\",\"U_MuhatapKodu\",\"U_MuhatapAdi\" from \"@AIF_WMS_KNTYNR\" as T0 where T0.\"U_KonteynerNo\" = '" + konteynerYapma.KonteynerNumarasi + "'");
                }
                else
                {
                    oRS.DoQuery("Select \"DocEntry\",\"U_MuhatapKodu\",\"U_MuhatapAdi\" from \"@AIF_WMS_KNTYNR\" as T0 where T0.\"U_KonteynerNo\" = N'" + konteynerYapma.KonteynerNumarasi + "'");
                }

                if (oRS.RecordCount == 0) //Daha önce bu konteyner kayıt girilmiş mi?
                {
                    #region Konteyner Yapma
                    CompanyService oCompService = null;

                    GeneralService oGeneralService;

                    GeneralData oGeneralData;

                    GeneralData oChildSatir1;

                    GeneralDataCollection oChildrenSatir;

                    oCompService = oCompany.GetCompanyService();

                    //oCompany.StartTransaction();

                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_KNTYNR");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralData.SetProperty("U_KonteynerNo", konteynerYapma.KonteynerNumarasi.ToString());

                    oGeneralData.SetProperty("U_MuhatapKodu", oRS.Fields.Item("U_MuhatapKodu").Value.ToString());

                    oGeneralData.SetProperty("U_MuhatapAdi", oRS.Fields.Item("U_MuhatapAdi").Value.ToString());

                    oChildrenSatir = oGeneralData.Child("AIF_WMS_KNTYNR1");

                    foreach (var item in konteynerYapma.konteynerYapmaDetays)
                    {
                        oChildSatir1 = oChildrenSatir.Add();

                        oChildSatir1.SetProperty("U_PaletNo", item.PaletNo);

                        oChildSatir1.SetProperty("U_Barkod", item.Barkod);

                        oChildSatir1.SetProperty("U_MuhKatalogNo", item.MuhatapKatalogNo);

                        oChildSatir1.SetProperty("U_KalemKodu", item.KalemKodu);

                        oChildSatir1.SetProperty("U_Tanim", item.KalemTanimi);

                        oChildSatir1.SetProperty("U_SiparisNo", item.siparisNo);

                        oChildSatir1.SetProperty("U_SipSatirNo", item.siparisSatirNo);

                        oChildSatir1.SetProperty("U_Miktar", item.Quantity);

                        oChildSatir1.SetProperty("U_CekmeNo", item.CekmeListesiNo);

                        oChildSatir1.SetProperty("U_KoliMiktari", item.koliMiktari);

                        oChildSatir1.SetProperty("U_NetKilo", item.netKilo);

                        oChildSatir1.SetProperty("U_BrutKilo", item.brutKilo);

                        if (item.satirKaynagi != null && item.satirKaynagi != "")
                        {
                            oChildSatir1.SetProperty("U_Kaynak", item.satirKaynagi);
                        }

                        string[] cekmeNoListesi = new string[1];

                        if (string.IsNullOrEmpty(item.PaletNo))
                        {
                            var cc = item.satirKaynagi.Split(',');

                            if (cc.Count() > 1)
                            {
                                cekmeNoListesi = new string[cc.Count()];

                                cekmeNoListesi = item.satirKaynagi.Split(',');
                            }
                            else
                            {
                                cekmeNoListesi[0] = item.satirKaynagi;
                            }
                        }

                        if (item.UrunKonteynereDahaOnceEklendi != "Y")
                        {
                            if (cekmeNoListesi.Count() > 0)
                            {
                                double toplanan = 0; double tp = 0;
                                for (int i = 0; i <= cekmeNoListesi.Count() - 1; i++)
                                {
                                    string qq = "Select T1.\"LineId\",T0.\"DocEntry\",(T1.\"U_PlanSipMik\" - T1.\"U_ToplananMik\") as 'KalanMiktar' from \"@AIF_WMS_SIPKAR\" AS T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + item.CekmeListesiNo + "' and CONCAT(T1.\"U_SiparisNumarasi\", '-', T1.\"U_SipSatirNo\")  = '" + cekmeNoListesi[i].ToString() + "'";

                                    oRS_LineId.DoQuery(qq);


                                    if (oRS_LineId.RecordCount > 0)
                                    {
                                        int lineid = Convert.ToInt32(oRS_LineId.Fields.Item("LineId").Value);
                                        int toplanmasigerekenMiktar = Convert.ToInt32(oRS_LineId.Fields.Item("KalanMiktar").Value);

                                        GeneralService oGeneralService_SIPKAR;

                                        GeneralData oGeneralData_SIPKAR;

                                        GeneralData oChildSatir1_SIPKAR;

                                        GeneralDataCollection oChildrenSatir_SIPKAR;

                                        oCompService = oCompany.GetCompanyService();

                                        GeneralDataParams oGeneralParams_SIPKAR;

                                        //oCompany.StartTransaction();

                                        oGeneralService_SIPKAR = oCompService.GetGeneralService("AIF_WMS_SIPKAR");

                                        oGeneralData_SIPKAR = (SAPbobsCOM.GeneralData)oGeneralService_SIPKAR.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                        oGeneralParams_SIPKAR = (GeneralDataParams)oGeneralService_SIPKAR.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                        oGeneralParams_SIPKAR.SetProperty("DocEntry", Convert.ToInt32(oRS_LineId.Fields.Item("DocEntry").Value));
                                        oGeneralData_SIPKAR = oGeneralService_SIPKAR.GetByParams(oGeneralParams_SIPKAR);

                                        //oGeneralData.SetProperty("U_KonteynerNo", cekmeListesiOnaylama.KonteynerNumarasi.ToString());

                                        //DateTime dt = new DateTime(Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(0, 4)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(4, 2)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(6, 2)));

                                        oChildrenSatir_SIPKAR = oGeneralData_SIPKAR.Child("AIF_WMS_SIPKAR1");

                                        double acikmiktar = Convert.ToDouble(oRS_LineId.Fields.Item("KalanMiktar").Value);
                                        double toplanmasigerekilen = 0;

                                        double tp1 = Convert.ToDouble(item.Quantity);
                                        for (int z = 0; z <= oChildrenSatir_SIPKAR.Count - 1; z++)
                                        {
                                            int val = Convert.ToInt32(z);

                                            oChildSatir1_SIPKAR = oChildrenSatir_SIPKAR.Item(val);

                                            var aa = oChildSatir1_SIPKAR.GetProperty("LineId");
                                            var toplananMiktar = oChildSatir1_SIPKAR.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_ToplananMik"));
                                            var planlananMiktar = oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik"));
                                            double snc = 0;
                                            if (Convert.ToInt32(aa) == Convert.ToInt32(lineid))
                                            {
                                                snc = Convert.ToDouble(toplananMiktar) + item.Quantity;

                                                double yazilanmiktar = 0;

                                                if (acikmiktar <= item.Quantity)
                                                {
                                                    oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", acikmiktar);
                                                    item.Quantity = item.Quantity - acikmiktar;

                                                    yazilanmiktar = acikmiktar;
                                                }
                                                else
                                                {
                                                    oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", item.Quantity);
                                                    yazilanmiktar = item.Quantity;
                                                }

                                                //if (acikmiktar < (Convert.ToDouble(item.Quantity) - toplanan))
                                                //{

                                                //    oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", acikmiktar);
                                                //    toplanan += acikmiktar;
                                                //    toplanmasigerekilen = Convert.ToDouble(item.Quantity) - acikmiktar;
                                                //}
                                                //else if (Convert.ToDouble(item.Quantity) - toplanan < acikmiktar)
                                                //{


                                                //    oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", Convert.ToDouble(item.Quantity) - toplanan);
                                                //    toplanan += Convert.ToDouble(item.Quantity) - toplanan;
                                                //}
                                                //else
                                                //{
                                                //    oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", acikmiktar - toplanmasigerekilen);
                                                //} 

                                                //if (Convert.ToDouble(item.Quantity) - snc == 0)
                                                //{
                                                //    //oChildrenSatir_SIPKAR.Remove(val);
                                                //    oChildSatir1_SIPKAR.SetProperty("U_Gorunur", "N");
                                                //}

                                                if (toplanmasigerekenMiktar - yazilanmiktar == 0)
                                                {
                                                    oChildSatir1_SIPKAR.SetProperty("U_Gorunur", "N");
                                                }
                                            }
                                        }
                                        oGeneralService_SIPKAR.Update(oGeneralData_SIPKAR);
                                    }
                                }
                            }
                            else
                            {
                                oRS_LineId.DoQuery("Select T1.\"LineId\",T0.\"DocEntry\" from \"@AIF_WMS_SIPKAR\" AS T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + item.CekmeListesiNo + "' and T1.\"U_SipSatirNo\" = '" + item.siparisSatirNo + "' and T1.\"U_SiparisNumarasi\" = '" + item.siparisNo + "'");

                                if (oRS_LineId.RecordCount > 0)
                                {
                                    int lineid = Convert.ToInt32(oRS_LineId.Fields.Item("LineId").Value);

                                    GeneralService oGeneralService_SIPKAR;

                                    GeneralData oGeneralData_SIPKAR;

                                    GeneralData oChildSatir1_SIPKAR;

                                    GeneralDataCollection oChildrenSatir_SIPKAR;

                                    oCompService = oCompany.GetCompanyService();

                                    GeneralDataParams oGeneralParams_SIPKAR;

                                    //oCompany.StartTransaction();

                                    oGeneralService_SIPKAR = oCompService.GetGeneralService("AIF_WMS_SIPKAR");

                                    oGeneralData_SIPKAR = (SAPbobsCOM.GeneralData)oGeneralService_SIPKAR.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                    oGeneralParams_SIPKAR = (GeneralDataParams)oGeneralService_SIPKAR.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                    oGeneralParams_SIPKAR.SetProperty("DocEntry", Convert.ToInt32(oRS_LineId.Fields.Item("DocEntry").Value));
                                    oGeneralData_SIPKAR = oGeneralService_SIPKAR.GetByParams(oGeneralParams_SIPKAR);

                                    //oGeneralData.SetProperty("U_KonteynerNo", cekmeListesiOnaylama.KonteynerNumarasi.ToString());

                                    //DateTime dt = new DateTime(Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(0, 4)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(4, 2)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(6, 2)));

                                    oChildrenSatir_SIPKAR = oGeneralData_SIPKAR.Child("AIF_WMS_SIPKAR1");

                                    for (int z = 0; z <= oChildrenSatir_SIPKAR.Count - 1; z++)
                                    {
                                        int val = Convert.ToInt32(z);

                                        oChildSatir1_SIPKAR = oChildrenSatir_SIPKAR.Item(val);

                                        var aa = oChildSatir1_SIPKAR.GetProperty("LineId");
                                        var toplananMiktar = oChildSatir1_SIPKAR.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_ToplananMik"));
                                        var planlananMiktar = oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik"));
                                        double snc = 0;
                                        if (Convert.ToInt32(aa) == Convert.ToInt32(lineid))
                                        {
                                            snc = Convert.ToDouble(toplananMiktar) + item.Quantity;
                                            oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", Convert.ToDouble(item.Quantity));


                                            if (Convert.ToDouble(planlananMiktar) - snc == 0)
                                            {
                                                //oChildrenSatir_SIPKAR.Remove(val);
                                                oChildSatir1_SIPKAR.SetProperty("U_Gorunur", "N");
                                            }
                                        }
                                    }

                                    oGeneralService_SIPKAR.Update(oGeneralData_SIPKAR);
                                }
                            }
                        }
                    }

                    //oRS.DoQuery("Select ISNULL(MAX(\"DocEntry\"),0) + 1 from \"@AIF_WMS_KNTYNR\"");

                    //int maxdocentry = Convert.ToInt32(oRS.Fields.Item(0).Value);

                    //oGeneralData.SetProperty("DocNum", maxdocentry);

                    var resp = oGeneralService.Add(oGeneralData);

                    oRS.DoQuery("Select TOP 1 \"DocEntry\" from \"@AIF_WMS_KNTYNR\" order by \"DocEntry\" desc ");

                    int maxdocentry = Convert.ToInt32(oRS.Fields.Item(0).Value);

                    oGeneralData.SetProperty("DocNum", maxdocentry);

                    #endregion

                    if (resp != null)
                    {

                        #region Palet Tablosunu Güncelleme

                        var paletnolistesi = konteynerYapma.konteynerYapmaDetays.Select(x => x.PaletNo).Distinct();

                        foreach (var item in paletnolistesi)
                        {
                            if (item == "")
                            {
                                continue;
                            }
                            oRS.DoQuery("Select \"DocEntry\",\"U_PaletNo\" from \"@AIF_WMS_PALET\" as T0 where T0.\"U_PaletNo\" = '" + item + "'");

                            if (oRS.RecordCount > 0)
                            {
                                GeneralDataParams oGeneralParams;

                                oGeneralService = oCompService.GetGeneralService("AIF_WMS_PALET");

                                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(oRS.Fields.Item("DocEntry").Value));
                                oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                oGeneralData.SetProperty("U_PaletNo", oRS.Fields.Item("U_PaletNo").Value.ToString());

                                oGeneralData.SetProperty("U_Durum", "Y");

                                oChildrenSatir = oGeneralData.Child("AIF_WMS_PALET1");

                                if (oChildrenSatir.Count > 0)
                                {
                                    int drc = oChildrenSatir.Count;
                                    for (int rmv = 0; rmv < drc; rmv++)
                                        oChildrenSatir.Remove(0);
                                }

                                foreach (var itemx in konteynerYapma.konteynerYapmaDetays.Where(x => x.PaletNo == item))
                                {
                                    oChildSatir1 = oChildrenSatir.Add();

                                    oChildSatir1.SetProperty("U_Barkod", itemx.Barkod);

                                    oChildSatir1.SetProperty("U_MuhKatalogNo", itemx.MuhatapKatalogNo.ToString());

                                    oChildSatir1.SetProperty("U_KalemKodu", itemx.KalemKodu.ToString());

                                    oChildSatir1.SetProperty("U_Tanim", itemx.KalemTanimi.ToString());

                                    oChildSatir1.SetProperty("U_Miktar", Convert.ToDouble(itemx.Quantity));

                                    if (itemx.siparisNo != -1 && itemx.siparisNo != null)
                                    {
                                        oChildSatir1.SetProperty("U_SiparisNo", itemx.siparisNo.ToString());
                                    }

                                    if (itemx.siparisSatirNo != -1 && itemx.siparisSatirNo != null)
                                    {
                                        oChildSatir1.SetProperty("U_SipSatirNo", itemx.siparisSatirNo.ToString());
                                    }

                                    oChildSatir1.SetProperty("U_CekmeNo", itemx.CekmeListesiNo.ToString());

                                    if (itemx.satirKaynagi != null && itemx.satirKaynagi != "")
                                    {
                                        oChildSatir1.SetProperty("U_Kaynak", itemx.satirKaynagi.ToString());
                                    }
                                }
                            }

                            try
                            {
                                oGeneralService.Update(oGeneralData);
                            }
                            catch (Exception)
                            {
                                LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                                return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                            }
                        }

                        #endregion

                        #region Toplanan Siparişler tablosuna ekleme

                        try
                        {
                            if (konteynerYapma.konteynerYapmaDetays.Count > 0)
                            {
                                //SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                bool islemvar = false;
                                bool guncelleme = false;
                                oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");

                                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                foreach (var itemy in konteynerYapma.konteynerYapmaDetays.Where(x => x.siparisNo != -1 && x.siparisSatirNo != -1 && x.UrunKonteynereDahaOnceEklendi == ""))
                                {
                                    //orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_SiparisNumarasi\" = '" + itemy.siparisNo + "' and T0.\"U_SiparisSatirNo\" = '" + itemy.siparisSatirNo + "'");


                                    //if (orecord.RecordCount == 0)
                                    //{
                                    oRS.DoQuery("Select MAX(\"DocEntry\") from \"@AIF_WMS_TOPLANAN\" as T0 ");
                                    int docentry = Convert.ToInt32(oRS.Fields.Item(0).Value) + 1;

                                    islemvar = true;

                                    oGeneralData.SetProperty("DocNum", docentry.ToString());
                                    oGeneralData.SetProperty("U_BelgeNo", itemy.CekmeListesiNo.ToString());
                                    oGeneralData.SetProperty("U_SiparisNumarasi", itemy.siparisNo);
                                    oGeneralData.SetProperty("U_SiparisSatirNo", itemy.siparisSatirNo.ToString());
                                    oGeneralData.SetProperty("U_Miktar", Convert.ToDouble(itemy.Quantity));
                                    oGeneralData.SetProperty("U_KntynrNo", maxdocentry.ToString());
                                    oGeneralData.SetProperty("U_KalemKodu", itemy.KalemKodu.ToString());
                                    oGeneralData.SetProperty("U_KalemAdi", itemy.KalemTanimi.ToString());
                                    oGeneralData.SetProperty("U_PaletNo", itemy.PaletNo.ToString());

                                    docentry++;
                                    oGeneralService.Add(oGeneralData);
                                }

                                GeneralDataParams oGeneralParams;
                                foreach (var itemy in konteynerYapma.konteynerYapmaDetays.Where(x => x.siparisNo != -1 && x.siparisSatirNo != -1 && x.UrunKonteynereDahaOnceEklendi == "Y"))
                                {
                                    SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                    orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_SiparisNumarasi\" = '" + itemy.siparisNo + "' and T0.\"U_SiparisSatirNo\" = '" + itemy.siparisSatirNo + "' and \"U_PaletNo\" = '" + itemy.PaletNo + "' " + condition + "(\"U_KntynrNo\",'') ='' ");


                                    if (orecord.RecordCount == 0)
                                    {
                                        oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                        oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(orecord.Fields.Item("DocEntry").Value));
                                        oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                        oGeneralData.SetProperty("U_KntynrNo", maxdocentry.ToString());

                                    }

                                    oGeneralService.Update(oGeneralData);

                                }
                            }
                        }
                        catch (Exception)
                        {
                            //try
                            //{
                            //    oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            //}
                            //catch (Exception)
                            //{
                            //}
                            LoginCompany.ReleaseConnection(connection.number, companyDbCode,ID);
                            return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                        }
                        #endregion

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

                    GeneralDataCollection oChildrenSatir;

                    oCompService = oCompany.GetCompanyService();

                    GeneralDataParams oGeneralParams;

                    #region Sipariş karşılama ekranı değişkenleri
                    GeneralService oGeneralService_SIPKAR;

                    GeneralData oGeneralData_SIPKAR;

                    GeneralData oChildSatir1_SIPKAR;

                    GeneralDataCollection oChildrenSatir_SIPKAR;

                    oCompService = oCompany.GetCompanyService();

                    GeneralDataParams oGeneralParams_SIPKAR;

                    //oCompany.StartTransaction();

                    oGeneralService_SIPKAR = oCompService.GetGeneralService("AIF_WMS_SIPKAR");

                    oGeneralData_SIPKAR = (SAPbobsCOM.GeneralData)oGeneralService_SIPKAR.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralParams_SIPKAR = (GeneralDataParams)oGeneralService_SIPKAR.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);

                    #endregion

                    #region Konteyner Yapma
                    int konteynerNo = -1;
                    //oCompany.StartTransaction();

                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_KNTYNR");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(oRS.Fields.Item("DocEntry").Value));
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                    konteynerNo = Convert.ToInt32(oRS.Fields.Item("DocEntry").Value);

                    oGeneralData.SetProperty("U_KonteynerNo", konteynerYapma.KonteynerNumarasi.ToString());

                    //DateTime dt = new DateTime(Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(0, 4)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(4, 2)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(6, 2)));

                    oChildrenSatir = oGeneralData.Child("AIF_WMS_KNTYNR1");

                    if (oChildrenSatir.Count > 0)
                    {
                        int drc = oChildrenSatir.Count;
                        for (int rmv = 0; rmv < drc; rmv++)
                            oChildrenSatir.Remove(0);
                    }

                    foreach (var item in konteynerYapma.konteynerYapmaDetays)
                    {
                        oChildSatir1 = oChildrenSatir.Add();

                        oChildSatir1.SetProperty("U_PaletNo", item.PaletNo);

                        oChildSatir1.SetProperty("U_Barkod", item.Barkod);

                        oChildSatir1.SetProperty("U_MuhKatalogNo", item.MuhatapKatalogNo.ToString());

                        oChildSatir1.SetProperty("U_KalemKodu", item.KalemKodu.ToString());

                        oChildSatir1.SetProperty("U_Tanim", item.KalemTanimi.ToString());

                        oChildSatir1.SetProperty("U_Miktar", Convert.ToDouble(item.Quantity));

                        if (item.siparisNo != -1 && item.siparisNo != null)
                        {
                            oChildSatir1.SetProperty("U_SiparisNo", item.siparisNo.ToString());
                        }

                        if (item.siparisSatirNo != -1 && item.siparisSatirNo != null)
                        {
                            oChildSatir1.SetProperty("U_SipSatirNo", item.siparisSatirNo.ToString());
                        }

                        oChildSatir1.SetProperty("U_CekmeNo", item.CekmeListesiNo.ToString());

                        oChildSatir1.SetProperty("U_KoliMiktari", item.koliMiktari);

                        oChildSatir1.SetProperty("U_NetKilo", item.netKilo);

                        oChildSatir1.SetProperty("U_BrutKilo", item.brutKilo);

                        if (item.satirKaynagi != null && item.satirKaynagi != "")
                        {
                            oChildSatir1.SetProperty("U_Kaynak", item.satirKaynagi.ToString());
                        }

                        if (item.UrunKonteynereDahaOnceEklendi != "Y")
                        {
                            oRS_LineId.DoQuery("Select T1.\"LineId\",T0.\"DocEntry\" from \"@AIF_WMS_SIPKAR\" AS T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + item.CekmeListesiNo + "' and T1.\"U_SipSatirNo\" = '" + item.siparisSatirNo + "' and T1.\"U_SiparisNumarasi\" = '" + item.siparisNo + "'");

                            if (oRS_LineId.RecordCount > 0)
                            {
                                int lineid = Convert.ToInt32(oRS_LineId.Fields.Item("LineId").Value);

                                oGeneralParams_SIPKAR.SetProperty("DocEntry", Convert.ToInt32(oRS_LineId.Fields.Item("DocEntry").Value));
                                oGeneralData_SIPKAR = oGeneralService_SIPKAR.GetByParams(oGeneralParams_SIPKAR);

                                //oGeneralData.SetProperty("U_KonteynerNo", cekmeListesiOnaylama.KonteynerNumarasi.ToString());

                                //DateTime dt = new DateTime(Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(0, 4)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(4, 2)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(6, 2)));

                                oChildrenSatir_SIPKAR = oGeneralData_SIPKAR.Child("AIF_WMS_SIPKAR1");

                                for (int z = 0; z <= oChildrenSatir_SIPKAR.Count - 1; z++)
                                {
                                    int val = Convert.ToInt32(z);

                                    oChildSatir1_SIPKAR = oChildrenSatir_SIPKAR.Item(val);

                                    var aa = oChildSatir1_SIPKAR.GetProperty("LineId");
                                    var toplananMiktar = oChildSatir1_SIPKAR.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_ToplananMik"));
                                    var planlananMiktar = oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik"));
                                    double snc = 0;
                                    if (Convert.ToInt32(aa) == Convert.ToInt32(lineid))
                                    {
                                        snc = Convert.ToDouble(toplananMiktar) + item.Quantity;
                                        oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", Convert.ToDouble(snc));


                                        if (Convert.ToDouble(planlananMiktar) - snc == 0)
                                        {
                                            //oChildrenSatir_SIPKAR.Remove(val);

                                            oChildSatir1_SIPKAR.SetProperty("U_Gorunur", "N");
                                        }
                                    }
                                }

                                oGeneralService_SIPKAR.Update(oGeneralData_SIPKAR);
                            }
                        }
                    }
                    #endregion

                    try
                    {
                        oGeneralService.Update(oGeneralData);

                        #region Palet Tablosunu Güncelleme

                        var paletnolistesi = konteynerYapma.konteynerYapmaDetays.Select(x => x.PaletNo).Distinct();
                        bool islemvar = false;
                        foreach (var item in paletnolistesi)
                        {
                            oRS.DoQuery("Select \"DocEntry\",\"U_PaletNo\" from \"@AIF_WMS_PALET\" as T0 where T0.\"U_PaletNo\" = '" + item + "'");
                            islemvar = false;
                            if (oRS.RecordCount > 0)
                            {
                                islemvar = true;
                                oGeneralService = oCompService.GetGeneralService("AIF_WMS_PALET");

                                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(oRS.Fields.Item("DocEntry").Value));
                                oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                oGeneralData.SetProperty("U_PaletNo", oRS.Fields.Item("U_PaletNo").Value.ToString());

                                oChildrenSatir = oGeneralData.Child("AIF_WMS_PALET1");

                                if (oChildrenSatir.Count > 0)
                                {
                                    int drc = oChildrenSatir.Count;
                                    for (int rmv = 0; rmv < drc; rmv++)
                                        oChildrenSatir.Remove(0);
                                }

                                foreach (var itemx in konteynerYapma.konteynerYapmaDetays.Where(x => x.PaletNo == item))
                                {
                                    oChildSatir1 = oChildrenSatir.Add();


                                    oChildSatir1.SetProperty("U_Barkod", itemx.Barkod);

                                    oChildSatir1.SetProperty("U_MuhKatalogNo", itemx.MuhatapKatalogNo.ToString());

                                    oChildSatir1.SetProperty("U_KalemKodu", itemx.KalemKodu.ToString());

                                    oChildSatir1.SetProperty("U_Tanim", itemx.KalemTanimi.ToString());

                                    oChildSatir1.SetProperty("U_Miktar", Convert.ToDouble(itemx.Quantity));


                                    if (itemx.siparisNo != -1 && itemx.siparisNo != null)
                                    {
                                        oChildSatir1.SetProperty("U_SiparisNo", itemx.siparisNo.ToString());
                                    }

                                    if (itemx.siparisSatirNo != -1 && itemx.siparisSatirNo != null)
                                    {
                                        oChildSatir1.SetProperty("U_SipSatirNo", itemx.siparisSatirNo.ToString());
                                    }

                                    oChildSatir1.SetProperty("U_CekmeNo", itemx.CekmeListesiNo.ToString());

                                    if (itemx.satirKaynagi != null && itemx.satirKaynagi != "")
                                    {
                                        oChildSatir1.SetProperty("U_Kaynak", itemx.satirKaynagi.ToString());
                                    }
                                }
                            }

                            try
                            {
                                if (islemvar)
                                {
                                    oGeneralService.Update(oGeneralData);
                                }
                            }
                            catch (Exception)
                            {
                                LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                                return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                            }
                        }
                        #endregion
                        islemvar = false;
                        #region Toplanan Siparişler tablosuna ekleme

                        try
                        {
                            if (konteynerYapma.konteynerYapmaDetays.Count > 0)
                            {
                                //SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                if (cekmeListesiKalemleriniGrupla != "Y")
                                {
                                    islemvar = false;
                                    bool guncelleme = false;
                                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");

                                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                    foreach (var itemy in konteynerYapma.konteynerYapmaDetays.Where(x => x.siparisNo != -1 && x.siparisSatirNo != -1 && x.UrunKonteynereDahaOnceEklendi == ""))
                                    {
                                        //orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_SiparisNumarasi\" = '" + itemy.siparisNo + "' and T0.\"U_SiparisSatirNo\" = '" + itemy.siparisSatirNo + "'");


                                        //if (orecord.RecordCount == 0)
                                        //{
                                        oRS.DoQuery("Select MAX(\"DocEntry\") from \"@AIF_WMS_TOPLANAN\" as T0 ");
                                        int docentry = Convert.ToInt32(oRS.Fields.Item(0).Value) + 1;

                                        islemvar = true;

                                        oGeneralData.SetProperty("DocNum", docentry.ToString());
                                        oGeneralData.SetProperty("U_BelgeNo", itemy.CekmeListesiNo.ToString());
                                        oGeneralData.SetProperty("U_SiparisNumarasi", itemy.siparisNo);
                                        oGeneralData.SetProperty("U_SiparisSatirNo", itemy.siparisSatirNo.ToString());
                                        oGeneralData.SetProperty("U_Miktar", Convert.ToDouble(itemy.Quantity));
                                        oGeneralData.SetProperty("U_KntynrNo", konteynerNo.ToString());
                                        oGeneralData.SetProperty("U_KalemKodu", itemy.KalemKodu.ToString());
                                        oGeneralData.SetProperty("U_KalemAdi", itemy.KalemTanimi.ToString());
                                        oGeneralData.SetProperty("U_PaletNo", itemy.PaletNo.ToString());
                                        if (itemy.satirKaynagi != null && itemy.satirKaynagi != "")
                                        {
                                            oGeneralData.SetProperty("U_Kaynak", itemy.satirKaynagi.ToString());
                                        }

                                        docentry++;
                                        oGeneralService.Add(oGeneralData);
                                    }

                                    foreach (var itemy in konteynerYapma.konteynerYapmaDetays.Where(x => x.siparisNo != -1 && x.siparisSatirNo != -1 && x.UrunKonteynereDahaOnceEklendi == "Y"))
                                    {
                                        SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                        orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_SiparisNumarasi\" = '" + itemy.siparisNo + "' and T0.\"U_SiparisSatirNo\" = '" + itemy.siparisSatirNo + "' and \"U_PaletNo\" = '" + itemy.PaletNo + "' and " + condition + "(\"U_KntynrNo\",'') ='' ");


                                        if (orecord.RecordCount > 0)
                                        {
                                            oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                            oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(orecord.Fields.Item("DocEntry").Value));
                                            oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                            oGeneralData.SetProperty("U_KntynrNo", konteynerNo.ToString());

                                        }

                                        oGeneralService.Update(oGeneralData);
                                    }
                                }
                                else
                                {
                                    islemvar = false;
                                    bool guncelleme = false;
                                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");

                                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                    foreach (var itemy in konteynerYapma.konteynerYapmaDetays.Where(x => x.siparisNo != -1 && x.siparisSatirNo != -1 && x.PaletNo == "" && x.UrunKonteynereDahaOnceEklendi == ""))
                                    {
                                        //orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_SiparisNumarasi\" = '" + itemy.siparisNo + "' and T0.\"U_SiparisSatirNo\" = '" + itemy.siparisSatirNo + "'");


                                        //if (orecord.RecordCount == 0)
                                        //{
                                        oRS.DoQuery("Select MAX(\"DocEntry\") from \"@AIF_WMS_TOPLANAN\" as T0 ");
                                        int docentry = Convert.ToInt32(oRS.Fields.Item(0).Value) + 1;

                                        islemvar = true;

                                        oGeneralData.SetProperty("DocNum", docentry.ToString());
                                        oGeneralData.SetProperty("U_BelgeNo", itemy.CekmeListesiNo.ToString());
                                        oGeneralData.SetProperty("U_SiparisNumarasi", itemy.siparisNo);
                                        oGeneralData.SetProperty("U_SiparisSatirNo", itemy.siparisSatirNo.ToString());
                                        oGeneralData.SetProperty("U_Miktar", Convert.ToDouble(itemy.Quantity));
                                        oGeneralData.SetProperty("U_KntynrNo", konteynerNo.ToString());
                                        oGeneralData.SetProperty("U_KalemKodu", itemy.KalemKodu.ToString());
                                        oGeneralData.SetProperty("U_KalemAdi", itemy.KalemTanimi.ToString());
                                        oGeneralData.SetProperty("U_PaletNo", itemy.PaletNo.ToString());
                                        if (itemy.satirKaynagi != null && itemy.satirKaynagi != "")
                                        {
                                            oGeneralData.SetProperty("U_Kaynak", itemy.satirKaynagi.ToString());
                                        }

                                        docentry++;
                                        oGeneralService.Add(oGeneralData);
                                    }

                                    islemvar = false;

                                    foreach (var itemy in konteynerYapma.konteynerYapmaDetays.Where(x => x.siparisNo != -1 && x.siparisSatirNo != -1 && x.PaletNo == "" && x.UrunKonteynereDahaOnceEklendi == "Y"))
                                    {
                                        SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                        orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_SiparisNumarasi\" = '" + itemy.siparisNo + "' and T0.\"U_SiparisSatirNo\" = '" + itemy.siparisSatirNo + "' and \"U_PaletNo\" = '" + itemy.PaletNo + "' and " + condition + "(\"U_KntynrNo\",'') ='' ");

                                        if (orecord.RecordCount > 0)
                                        {
                                            islemvar = true;
                                            oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                            oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(orecord.Fields.Item("DocEntry").Value));
                                            oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                            oGeneralData.SetProperty("U_KntynrNo", konteynerNo.ToString());

                                        }

                                        if (islemvar)
                                        {
                                            oGeneralService.Update(oGeneralData);
                                        }
                                        islemvar = false;

                                    }
                                    islemvar = false;

                                    foreach (var itemy in konteynerYapma.konteynerYapmaDetays.Where(x => x.PaletNo != "" && x.UrunKonteynereDahaOnceEklendi == "Y"))
                                    {
                                        SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                        orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_Kaynak\" = '" + itemy.satirKaynagi + "' and \"U_PaletNo\" = '" + itemy.PaletNo + "' and " + condition + "(\"U_KntynrNo\",'') ='' ");


                                        if (orecord.RecordCount > 0)
                                        {
                                            islemvar = true;
                                            oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                            oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(orecord.Fields.Item("DocEntry").Value));
                                            oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                            oGeneralData.SetProperty("U_KntynrNo", konteynerNo.ToString());

                                        }

                                        if (islemvar)
                                        {
                                            oGeneralService.Update(oGeneralData);
                                        }
                                        islemvar = false;
                                    }

                                    double toplananMiktar = 0;
                                    double silinecekMiktar = 0;
                                    string toplananDocEntry = "";
                                    foreach (var itemxy in konteynerIcindenSilinenlers.Where(x => x.paletNo == ""))
                                    {

                                        silinecekMiktar = itemxy.miktar;
                                        SAPbobsCOM.Recordset oRSSilinenler = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                                        string sql = "SELECT \"DocEntry\",\"U_Miktar\" FROM \"@AIF_WMS_TOPLANAN\" where \"U_SiparisNumarasi\" = '" + itemxy.siparisNo + "' and \"U_SiparisSatirNo\" = '" + itemxy.siparisSatirNo + "' and \"U_BelgeNo\" = '" + itemxy.cekmeNo + "' and " + condition + "(\"U_PaletNo\",'')='' and \"U_Miktar\" >= " + itemxy.miktar + " ";

                                        oRSSilinenler.DoQuery(sql);

                                        toplananMiktar = Convert.ToDouble(oRSSilinenler.Fields.Item("U_Miktar").Value);

                                        while (!oRSSilinenler.EoF)
                                        {
                                            toplananDocEntry = oRSSilinenler.Fields.Item("DocEntry").Value.ToString();
                                            if (toplananMiktar <= 0)
                                            {
                                                break;
                                            }

                                            if (toplananMiktar >= silinecekMiktar)
                                            {
                                                sql = "DELETE FROM \"@AIF_WMS_TOPLANAN\" where  \"DocEntry\" = '" + toplananDocEntry + "' ";

                                                try
                                                {
                                                    oRS.DoQuery(sql);
                                                    toplananMiktar = toplananMiktar - silinecekMiktar;
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
                                                sql = "UPDATE \"@AIF_WMS_TOPLANAN\" SET \"U_Miktar\" = \"U_Miktar\" - " + itemxy.miktar + "  WHERE \"DocEntry\" = '" + toplananDocEntry + "' ";

                                                silinecekMiktar = silinecekMiktar - toplananMiktar;


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

                                                    LoginCompany.ReleaseConnection(connection.number, companyDbCode,ID);
                                                    return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + ex.Message, _list = null };
                                                }
                                            }

                                            oRSSilinenler.MoveNext();
                                        }
                                         
                                        sql = "UPDATE \"@AIF_WMS_SIPKAR1\" SET \"U_Gorunur\" = 'Y',\"U_PaletNo\" = ''  ,\"U_ToplananMik\" = (Select T1.\"U_ToplananMik\" FROM \"@AIF_WMS_SIPKAR1\" T1 WHERE T1.\"DocEntry\" = '" + itemxy.cekmeNo + "' and T1.\"U_SiparisNumarasi\" = '" + itemxy.siparisNo + "' and T1.\"U_SipSatirNo\" = '" + itemxy.siparisSatirNo + "') - " + (itemxy.miktar) + " FROM \"@AIF_WMS_SIPKAR1\" t0 WHERE \"DocEntry\" = '" + itemxy.cekmeNo + "' and \"U_SiparisNumarasi\" = '" + itemxy.siparisNo + "' and \"U_SipSatirNo\" = '" + itemxy.siparisSatirNo + "'";

                                        oRSSilinenler.DoQuery(sql);
                                    }
                                }

                                //}
                            }
                        }
                        catch (Exception)
                        {
                            //try
                            //{
                            //    oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            //}
                            //catch (Exception)
                            //{
                            //}
                            LoginCompany.ReleaseConnection(connection.number, companyDbCode,ID);
                            return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                        }
                        #endregion 

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

        public Response addOrUpdateKonteynerYapmaListesi_Final(string dbName, KonteynerYapma konteynerYapma, List<KonteynerIcindenSilinenler> konteynerIcindenSilinenlers, string cekmeListesiKalemleriniGrupla)
        {
            Random rastgele = new Random();
            int ID = rastgele.Next(0, 9999);
            try
            {
                #region SAP Bağlantı kısmı
                ConnectionList connection = new ConnectionList();

                LoginCompany log = new LoginCompany();

                log.DisconnectSAP(dbName);

                connection = log.getSAPConnection(dbName,ID);

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

                if (connection.number == -1)
                {
                    return new Response { Val = -3100, Desc = "Hata Kodu - 3100 Veritabanı bağlantısı sırasında hata oluştu. ", _list = null };
                }

                clNum = connection.number;
                companyDbCode = connection.dbCode;
                SAPbobsCOM.Company oCompany = connection.oCompany;
                #endregion

                #region SAP CompanyService ve GeneralData tanımlamaları
                CompanyService oCompService = null;

                GeneralService oGeneralService;

                GeneralData oGeneralData;

                GeneralData oChildSatir1;

                GeneralDataCollection oChildrenSatir;

                GeneralDataParams oGeneralParams;

                GeneralService oGeneralService_SIPKAR;

                GeneralData oGeneralData_SIPKAR;

                GeneralData oChildSatir1_SIPKAR;

                GeneralDataCollection oChildrenSatir_SIPKAR;

                oCompService = oCompany.GetCompanyService();

                GeneralDataParams oGeneralParams_SIPKAR;

                //oCompany.StartTransaction();

                oGeneralService_SIPKAR = oCompService.GetGeneralService("AIF_WMS_SIPKAR");

                oGeneralData_SIPKAR = (SAPbobsCOM.GeneralData)oGeneralService_SIPKAR.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                oGeneralParams_SIPKAR = (GeneralDataParams)oGeneralService_SIPKAR.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);

                #endregion

                Recordset oRS = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                Recordset oRS_LineId = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                #region Konteyner daha önce oluşturulmuş mu kontrolü 
                string condition = oCompany.DbServerType == BoDataServerTypes.dst_HANADB ? "IFNULL" : "ISNULL";


                if (oCompany.DbServerType == BoDataServerTypes.dst_HANADB)
                {
                    string sorgu = "Select \"DocEntry\",\"U_MuhatapKodu\",\"U_MuhatapAdi\" from \"@AIF_WMS_KNTYNR\" as T0 where T0.\"U_KonteynerNo\" = '" + konteynerYapma.KonteynerNumarasi + "'";

                    oRS.DoQuery("Select \"DocEntry\",\"U_MuhatapKodu\",\"U_MuhatapAdi\" from \"@AIF_WMS_KNTYNR\" as T0 where T0.\"U_KonteynerNo\" = '" + konteynerYapma.KonteynerNumarasi + "'");

                }
                else
                {
                    string sorgu = "Select \"DocEntry\",\"U_MuhatapKodu\",\"U_MuhatapAdi\" from \"@AIF_WMS_KNTYNR\" as T0 where T0.\"U_KonteynerNo\" = N'" + konteynerYapma.KonteynerNumarasi + "'";

                    oRS.DoQuery("Select \"DocEntry\",\"U_MuhatapKodu\",\"U_MuhatapAdi\" from \"@AIF_WMS_KNTYNR\" as T0 where T0.\"U_KonteynerNo\" = N'" + konteynerYapma.KonteynerNumarasi + "'");
                }


                #endregion

                double karsilanmasiGerekenMiktar = 0;
                double yazilacakMiktar = 0;
                if (oRS.RecordCount == 0) //Konteyner daha önce ekli değil ise buraya girer.
                {
                    string olusanKonteynerNumarasi = "";

                    #region Konteyner verilerini doldurma
                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_KNTYNR");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralData.SetProperty("U_KonteynerNo", konteynerYapma.KonteynerNumarasi.ToString());

                    oGeneralData.SetProperty("U_MuhatapKodu", oRS.Fields.Item("U_MuhatapKodu").Value.ToString());

                    oGeneralData.SetProperty("U_MuhatapAdi", oRS.Fields.Item("U_MuhatapAdi").Value.ToString());

                    oChildrenSatir = oGeneralData.Child("AIF_WMS_KNTYNR1");

                    foreach (var item in konteynerYapma.konteynerYapmaDetays)
                    {
                        oChildSatir1 = oChildrenSatir.Add();

                        oChildSatir1.SetProperty("U_PaletNo", item.PaletNo);

                        oChildSatir1.SetProperty("U_Barkod", item.Barkod);

                        oChildSatir1.SetProperty("U_MuhKatalogNo", item.MuhatapKatalogNo);

                        oChildSatir1.SetProperty("U_KalemKodu", item.KalemKodu);

                        oChildSatir1.SetProperty("U_Tanim", item.KalemTanimi);

                        oChildSatir1.SetProperty("U_SiparisNo", item.siparisNo);

                        oChildSatir1.SetProperty("U_SipSatirNo", item.siparisSatirNo);

                        oChildSatir1.SetProperty("U_Miktar", item.Quantity);

                        oChildSatir1.SetProperty("U_CekmeNo", item.CekmeListesiNo);

                        oChildSatir1.SetProperty("U_KoliMiktari", item.koliMiktari);

                        oChildSatir1.SetProperty("U_NetKilo", item.netKilo);

                        oChildSatir1.SetProperty("U_BrutKilo", item.brutKilo);

                        if (item.satirKaynagi != null && item.satirKaynagi != "")
                        {
                            oChildSatir1.SetProperty("U_Kaynak", item.satirKaynagi);
                        }
                    }

                    var resp = oGeneralService.Add(oGeneralData);


                    oRS.DoQuery("Select TOP 1 \"DocEntry\" from \"@AIF_WMS_KNTYNR\" order by \"DocEntry\" desc ");

                    olusanKonteynerNumarasi = oRS.Fields.Item(0).Value.ToString();

                    //oGeneralData.SetProperty("DocNum", maxdocentry);  
                    #endregion


                    #region Palet ile eklenen ürünlerin toplanan tablosuna konteyner numarasını yazma işlemi


                    if (cekmeListesiKalemleriniGrupla == "Y") //Genel parametreler ekranındaki çekme listesini grupla işaretli ise bu işlem yapılır.
                    {
                        foreach (var item in konteynerYapma.konteynerYapmaDetays.Where(x => x.PaletNo != ""))
                        {
                            #region Konteyner listesinin içerisinde bulunan satır kaynakları alınır.
                            string[] satirKaynagiListesi = new string[1];
                            var cc = item.satirKaynagi.Split(',');

                            if (cc.Count() > 1)
                            {
                                satirKaynagiListesi = new string[cc.Count()];

                                satirKaynagiListesi = item.satirKaynagi.Split(',');


                            }
                            else
                            {
                                satirKaynagiListesi[0] = item.satirKaynagi;
                            }
                            #endregion

                            #region Yukarıda alınan satır kaynakları kendi aralarında - ile split edilip toplanan tablosunda konteyner update edilir.
                            for (int i = 0; i <= satirKaynagiListesi.Count() - 1; i++)
                            {
                                SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_BelgeNo\" = '" + item.CekmeListesiNo + "' and T0.\"U_SiparisNumarasi\" = '" + satirKaynagiListesi[i].ToString().Split('-')[0].Trim() + "' and T0.\"U_SiparisSatirNo\" = '" + satirKaynagiListesi[i].ToString().Split('-')[1].Trim() + "' and \"U_PaletNo\" = '" + item.PaletNo + "' and " + condition + "(\"U_KntynrNo\",'') ='' ");


                                while (!orecord.EoF)
                                {
                                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");
                                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(orecord.Fields.Item("DocEntry").Value));
                                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                    oGeneralData.SetProperty("U_KntynrNo", olusanKonteynerNumarasi.ToString());

                                    oGeneralService.Update(oGeneralData);

                                    orecord.MoveNext();
                                }
                            }
                            #endregion
                        }

                        #region Toplanan ve sipariş karşılama ekranlarına datalar güncellenir.
                        foreach (var item in konteynerYapma.konteynerYapmaDetays.Where(x => x.UrunKonteynereDahaOnceEklendi != "Y"))
                        {
                            karsilanmasiGerekenMiktar = item.Quantity;
                            #region Konteyner listesinin içerisinde bulunan satır kaynakları alınır.
                            string[] satirKaynagiListesi = new string[1];
                            var cc = item.satirKaynagi.Split(',');

                            if (cc.Count() > 1)
                            {
                                satirKaynagiListesi = new string[cc.Count()];

                                satirKaynagiListesi = item.satirKaynagi.Split(',');


                            }
                            else
                            {
                                satirKaynagiListesi[0] = item.satirKaynagi;
                            }
                            #endregion

                            for (int i = 0; i <= satirKaynagiListesi.Count() - 1; i++)
                            {
                                if (karsilanmasiGerekenMiktar <= 0)
                                {
                                    break;
                                }

                                string kk = " Select T1.\"LineId\",T0.\"DocEntry\" from \"@AIF_WMS_SIPKAR\" AS T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + item.CekmeListesiNo + "' and T1.\"U_SipSatirNo\" = '" + satirKaynagiListesi[i].ToString().Split('-')[1].Trim() + "' and T1.\"U_SiparisNumarasi\" = '" + satirKaynagiListesi[i].ToString().Split('-')[0].Trim() + "'";


                                oRS_LineId.DoQuery("Select T1.\"LineId\",T0.\"DocEntry\" from \"@AIF_WMS_SIPKAR\" AS T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + item.CekmeListesiNo + "' and T1.\"U_SipSatirNo\" = '" + satirKaynagiListesi[i].ToString().Split('-')[1].Trim() + "' and T1.\"U_SiparisNumarasi\" = '" + satirKaynagiListesi[i].ToString().Split('-')[0].Trim() + "'");

                                if (oRS_LineId.RecordCount > 0)
                                {
                                    int lineid = Convert.ToInt32(oRS_LineId.Fields.Item("LineId").Value);

                                    oGeneralParams_SIPKAR.SetProperty("DocEntry", Convert.ToInt32(oRS_LineId.Fields.Item("DocEntry").Value));
                                    oGeneralData_SIPKAR = oGeneralService_SIPKAR.GetByParams(oGeneralParams_SIPKAR);

                                    oChildrenSatir_SIPKAR = oGeneralData_SIPKAR.Child("AIF_WMS_SIPKAR1");



                                    for (int z = 0; z <= oChildrenSatir_SIPKAR.Count - 1; z++)
                                    {
                                        if (karsilanmasiGerekenMiktar <= 0)
                                        {
                                            break;
                                        }
                                        int val = Convert.ToInt32(z);

                                        oChildSatir1_SIPKAR = oChildrenSatir_SIPKAR.Item(val);

                                        var aa = oChildSatir1_SIPKAR.GetProperty("LineId");
                                        var toplananMiktar = oChildSatir1_SIPKAR.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_ToplananMik"));
                                        var planlananMiktar = oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik"));
                                        var satiricinKalanMiktar = planlananMiktar - toplananMiktar;
                                        var siparisNo = oChildSatir1_SIPKAR.GetProperty("U_SiparisNumarasi").ToString();
                                        var siparisSatirNo = oChildSatir1_SIPKAR.GetProperty("U_SipSatirNo").ToString();
                                        double snc = 0;
                                        if (Convert.ToInt32(aa) == Convert.ToInt32(lineid))
                                        {

                                            if (karsilanmasiGerekenMiktar >= satiricinKalanMiktar)
                                            {
                                                oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", planlananMiktar);
                                                oChildSatir1_SIPKAR.SetProperty("U_Gorunur", "N");
                                                yazilacakMiktar = satiricinKalanMiktar;
                                                karsilanmasiGerekenMiktar = karsilanmasiGerekenMiktar - satiricinKalanMiktar;
                                            }
                                            else
                                            {
                                                snc = Convert.ToDouble(toplananMiktar) + karsilanmasiGerekenMiktar;
                                                oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", Convert.ToDouble(snc));
                                                yazilacakMiktar = karsilanmasiGerekenMiktar;
                                                karsilanmasiGerekenMiktar = karsilanmasiGerekenMiktar - snc;

                                            }


                                            #region Sipariş Karşılama Ekranındaki eşleşen kayıda göre toplanana kayıt atılır.
                                            oRS.DoQuery("Select MAX(\"DocEntry\") from \"@AIF_WMS_TOPLANAN\" as T0 ");
                                            int docentry = Convert.ToInt32(oRS.Fields.Item(0).Value) + 1;

                                            oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");

                                            oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                                            oGeneralData.SetProperty("DocNum", docentry.ToString());
                                            oGeneralData.SetProperty("U_BelgeNo", item.CekmeListesiNo.ToString());
                                            oGeneralData.SetProperty("U_SiparisNumarasi", siparisNo);
                                            oGeneralData.SetProperty("U_SiparisSatirNo", siparisSatirNo);
                                            oGeneralData.SetProperty("U_Miktar", Convert.ToDouble(yazilacakMiktar));
                                            oGeneralData.SetProperty("U_KntynrNo", olusanKonteynerNumarasi.ToString());
                                            oGeneralData.SetProperty("U_KalemKodu", item.KalemKodu.ToString());
                                            oGeneralData.SetProperty("U_KalemAdi", item.KalemTanimi.ToString());
                                            oGeneralData.SetProperty("U_PaletNo", item.PaletNo.ToString());
                                            if (item.satirKaynagi != null && item.satirKaynagi != "")
                                            {
                                                oGeneralData.SetProperty("U_Kaynak", item.satirKaynagi.ToString());
                                            }

                                            docentry++;
                                            oGeneralService.Add(oGeneralData);
                                            #endregion

                                        }
                                    }

                                    oGeneralService_SIPKAR.Update(oGeneralData_SIPKAR);
                                }
                            }


                        }
                        #endregion
                    }
                    else
                    {
                        #region Tüm satırlarda sipariş numarası ve satır numarası olması gerektiğinden dolayı burası sipariş ve satır numarasına eşitleyerek çalışır. 
                        foreach (var item in konteynerYapma.konteynerYapmaDetays.Where(x => x.PaletNo != ""))
                        {


                            SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                            orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_BelgeNo\" = '" + item.CekmeListesiNo + "' and T0.\"U_SiparisNumarasi\" = '" + item.siparisNo + "' and T0.\"U_SiparisSatirNo\" = '" + item.siparisSatirNo + "' and \"U_PaletNo\" = '" + item.PaletNo + "' and " + condition + "(\"U_KntynrNo\",'') ='' ");


                            while (!orecord.EoF)
                            {
                                oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");
                                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(orecord.Fields.Item("DocEntry").Value));
                                oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                oGeneralData.SetProperty("U_KntynrNo", olusanKonteynerNumarasi.ToString());

                                oGeneralService.Update(oGeneralData);

                                orecord.MoveNext();
                            }
                        }
                        #endregion

                        #region Toplanan ve sipariş karşılama ekranlarına datalar güncellenir.
                        foreach (var item in konteynerYapma.konteynerYapmaDetays.Where(x => x.UrunKonteynereDahaOnceEklendi != "Y"))
                        {
                            karsilanmasiGerekenMiktar = item.Quantity;
                            if (karsilanmasiGerekenMiktar <= 0)
                            {
                                break;
                            }
                            oRS_LineId.DoQuery("Select T1.\"LineId\",T0.\"DocEntry\" from \"@AIF_WMS_SIPKAR\" AS T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + item.CekmeListesiNo + "' and T1.\"U_SipSatirNo\" = '" + item.siparisSatirNo + "' and T1.\"U_SiparisNumarasi\" = '" + item.siparisNo + "'");

                            if (oRS_LineId.RecordCount > 0)
                            {
                                int lineid = Convert.ToInt32(oRS_LineId.Fields.Item("LineId").Value);

                                oGeneralParams_SIPKAR.SetProperty("DocEntry", Convert.ToInt32(oRS_LineId.Fields.Item("DocEntry").Value));
                                oGeneralData_SIPKAR = oGeneralService_SIPKAR.GetByParams(oGeneralParams_SIPKAR);

                                oChildrenSatir_SIPKAR = oGeneralData_SIPKAR.Child("AIF_WMS_SIPKAR1");



                                for (int z = 0; z <= oChildrenSatir_SIPKAR.Count - 1; z++)
                                {
                                    if (karsilanmasiGerekenMiktar <= 0)
                                    {
                                        break;
                                    }
                                    int val = Convert.ToInt32(z);

                                    oChildSatir1_SIPKAR = oChildrenSatir_SIPKAR.Item(val);

                                    var aa = oChildSatir1_SIPKAR.GetProperty("LineId");
                                    var toplananMiktar = oChildSatir1_SIPKAR.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_ToplananMik"));
                                    var planlananMiktar = oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik"));
                                    var satiricinKalanMiktar = planlananMiktar - toplananMiktar;
                                    var siparisNo = oChildSatir1_SIPKAR.GetProperty("U_SiparisNumarasi").ToString();
                                    var siparisSatirNo = oChildSatir1_SIPKAR.GetProperty("U_SipSatirNo").ToString();
                                    double snc = 0;
                                    if (Convert.ToInt32(aa) == Convert.ToInt32(lineid))
                                    {

                                        if (karsilanmasiGerekenMiktar >= satiricinKalanMiktar)
                                        {
                                            oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", planlananMiktar);
                                            oChildSatir1_SIPKAR.SetProperty("U_Gorunur", "N");
                                            yazilacakMiktar = satiricinKalanMiktar;
                                            karsilanmasiGerekenMiktar = karsilanmasiGerekenMiktar - satiricinKalanMiktar;
                                        }
                                        else
                                        {
                                            snc = Convert.ToDouble(toplananMiktar) + karsilanmasiGerekenMiktar;
                                            oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", Convert.ToDouble(snc));
                                            yazilacakMiktar = karsilanmasiGerekenMiktar;
                                            karsilanmasiGerekenMiktar = karsilanmasiGerekenMiktar - snc;

                                        }


                                        #region Sipariş Karşılama Ekranındaki eşleşen kayıda göre toplanana kayıt atılır.
                                        oRS.DoQuery("Select MAX(\"DocEntry\") from \"@AIF_WMS_TOPLANAN\" as T0 ");
                                        int docentry = Convert.ToInt32(oRS.Fields.Item(0).Value) + 1;

                                        oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");

                                        oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                                        oGeneralData.SetProperty("DocNum", docentry.ToString());
                                        oGeneralData.SetProperty("U_BelgeNo", item.CekmeListesiNo.ToString());
                                        oGeneralData.SetProperty("U_SiparisNumarasi", siparisNo);
                                        oGeneralData.SetProperty("U_SiparisSatirNo", siparisSatirNo);
                                        oGeneralData.SetProperty("U_Miktar", Convert.ToDouble(yazilacakMiktar));
                                        oGeneralData.SetProperty("U_KntynrNo", olusanKonteynerNumarasi.ToString());
                                        oGeneralData.SetProperty("U_KalemKodu", item.KalemKodu.ToString());
                                        oGeneralData.SetProperty("U_KalemAdi", item.KalemTanimi.ToString());
                                        oGeneralData.SetProperty("U_PaletNo", item.PaletNo.ToString());
                                        if (item.satirKaynagi != null && item.satirKaynagi != "")
                                        {
                                            oGeneralData.SetProperty("U_Kaynak", item.satirKaynagi.ToString());
                                        }

                                        docentry++;
                                        oGeneralService.Add(oGeneralData);
                                        #endregion

                                    }
                                }

                                oGeneralService_SIPKAR.Update(oGeneralData_SIPKAR);
                            }


                        }
                        #endregion

                    }
                    #endregion
                }
                else
                {
                    string islemYapilanKonteynerNo = "";
                    #region Öncelikle konteynerda olan dataların hepsi silinir daha sonra ekranda olanların hepsi yeniden yüklenir.

                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_KNTYNR");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(oRS.Fields.Item("DocEntry").Value));
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                    islemYapilanKonteynerNo = oRS.Fields.Item("DocEntry").Value.ToString();
                    oChildrenSatir = oGeneralData.Child("AIF_WMS_KNTYNR1");

                    if (oChildrenSatir.Count > 0)
                    {
                        int drc = oChildrenSatir.Count;
                        for (int rmv = 0; rmv < drc; rmv++)
                            oChildrenSatir.Remove(0);
                    }


                    foreach (var item in konteynerYapma.konteynerYapmaDetays)
                    {
                        oChildSatir1 = oChildrenSatir.Add();

                        oChildSatir1.SetProperty("U_PaletNo", item.PaletNo);

                        oChildSatir1.SetProperty("U_Barkod", item.Barkod);

                        oChildSatir1.SetProperty("U_MuhKatalogNo", item.MuhatapKatalogNo.ToString());

                        oChildSatir1.SetProperty("U_KalemKodu", item.KalemKodu.ToString());

                        oChildSatir1.SetProperty("U_Tanim", item.KalemTanimi.ToString());

                        oChildSatir1.SetProperty("U_Miktar", Convert.ToDouble(item.Quantity));

                        if (item.siparisNo != -1 && item.siparisNo != null)
                        {
                            oChildSatir1.SetProperty("U_SiparisNo", item.siparisNo.ToString());
                        }

                        if (item.siparisSatirNo != -1 && item.siparisSatirNo != null)
                        {
                            oChildSatir1.SetProperty("U_SipSatirNo", item.siparisSatirNo.ToString());
                        }


                        oChildSatir1.SetProperty("U_CekmeNo", item.CekmeListesiNo.ToString());

                        oChildSatir1.SetProperty("U_KoliMiktari", item.koliMiktari);

                        oChildSatir1.SetProperty("U_NetKilo", item.netKilo);

                        oChildSatir1.SetProperty("U_BrutKilo", item.brutKilo);

                        if (item.satirKaynagi != null && item.satirKaynagi != "")
                        {
                            oChildSatir1.SetProperty("U_Kaynak", item.satirKaynagi.ToString());
                        }
                    }


                    try
                    {
                        oGeneralService.Update(oGeneralData);
                    }
                    catch (Exception)
                    {
                        LoginCompany.ReleaseConnection(connection.number, companyDbCode,ID);
                        return new Response { Val = 0, Desc = "Başarısız. Konteyner verileri güncellenirken hata oluştu.", _list = null };
                    }


                    #endregion

                    #region Palet ile konteynera girişi yapılmış olan satır için silme yapılır.

                    if (cekmeListesiKalemleriniGrupla == "Y")
                    {
                        foreach (var item in konteynerIcindenSilinenlers.Where(x => x.paletNo != ""))
                        {
                            #region Konteyner listesinin içerisinde bulunan satır kaynakları alınır.
                            string[] satirKaynagiListesi = new string[1];
                            var cc = item.kaynak.Split(',');

                            if (cc.Count() > 1)
                            {
                                satirKaynagiListesi = new string[cc.Count()];

                                satirKaynagiListesi = item.kaynak.Split(',');


                            }
                            else
                            {
                                satirKaynagiListesi[0] = item.kaynak;
                            }
                            #endregion

                            #region Yukarıda alınan satır kaynakları kendi aralarında - ile split edilip toplanan tablosunda konteyner update edilir.
                            for (int i = 0; i <= satirKaynagiListesi.Count() - 1; i++)
                            {
                                SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_BelgeNo\" = '" + item.cekmeNo + "' and T0.\"U_SiparisNumarasi\" = '" + satirKaynagiListesi[i].ToString().Split('-')[0].Trim() + "' and T0.\"U_SiparisSatirNo\" = '" + satirKaynagiListesi[i].ToString().Split('-')[1].Trim() + "' and \"U_PaletNo\" = '" + item.paletNo + "' and " + condition + "(\"U_KntynrNo\",'') <>'' and \"U_Miktar\" = '" + item.miktar + "' ");




                                string j = "Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_BelgeNo\" = '" + item.cekmeNo + "' and T0.\"U_SiparisNumarasi\" = '" + satirKaynagiListesi[i].ToString().Split('-')[0].Trim() + "' and T0.\"U_SiparisSatirNo\" = '" + satirKaynagiListesi[i].ToString().Split('-')[1].Trim() + "' and \"U_PaletNo\" = '" + item.paletNo + "' and " + condition + "(\"U_KntynrNo\",'') <>'' and \"U_Miktar\" = '" + item.miktar + "' ";

                                if (orecord.RecordCount > 0)
                                {
                                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");
                                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(orecord.Fields.Item("DocEntry").Value));
                                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                    oGeneralData.SetProperty("U_KntynrNo", "");

                                    try
                                    {
                                        oGeneralService.Update(oGeneralData);
                                    }
                                    catch (Exception)
                                    {

                                        LoginCompany.ReleaseConnection(connection.number, companyDbCode,ID);
                                        return new Response { Val = 0, Desc = "Başarısız. Toplanan tablosundan konteyner numarası silinirken hata oluştu.", _list = null };
                                    }
                                }
                            }
                            #endregion


                            //LoginCompany.ReleaseConnection(connection.number, companyDbCode);
                            //return new Response { Val = 0, Desc = "Başarılı.", _list = null };
                        }
                    }
                    else
                    {
                        foreach (var item in konteynerIcindenSilinenlers.Where(x => x.paletNo != ""))
                        {
                            #region Tüm satırlarda sipariş numarası ve satır numarası olması gerektiğinden dolayı burası sipariş ve satır numarasına eşitleyerek çalışır. 
                            SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                            orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_BelgeNo\" = '" + item.cekmeNo + "' and T0.\"U_SiparisNumarasi\" = '" + item.siparisNo + "' and T0.\"U_SiparisSatirNo\" = '" + item.siparisSatirNo + "' and \"U_PaletNo\" = '" + item.paletNo + "' and " + condition + "(\"U_KntynrNo\",'') <>'' and \"U_Miktar\" = '" + item.miktar + "' ");


                            if (orecord.RecordCount > 0)
                            {
                                oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");
                                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(orecord.Fields.Item("DocEntry").Value));
                                oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                oGeneralData.SetProperty("U_KntynrNo", "");

                                try
                                {
                                    oGeneralService.Update(oGeneralData);
                                }
                                catch (Exception)
                                {

                                    LoginCompany.ReleaseConnection(connection.number, companyDbCode,ID);
                                    return new Response { Val = 0, Desc = "Başarısız. Toplanan tablosundan konteyner numarası silinirken hata oluştu.", _list = null };
                                }
                            }
                            #endregion


                            //LoginCompany.ReleaseConnection(connection.number, companyDbCode);
                            //return new Response { Val = 0, Desc = "Başarılı.", _list = null };
                        }
                    }

                    #endregion

                    #region Konteyner içerisinden paletsiz ürün silme işlemi

                    if (cekmeListesiKalemleriniGrupla == "Y")
                    {
                        double silinecekMiktar = 0;
                        double siparisKarsilamaDusulecekMiktar = 0;
                        double toplanandakiMiktar = 0;
                        foreach (var item in konteynerIcindenSilinenlers.Where(x => x.paletNo == ""))
                        {
                            silinecekMiktar = item.miktar;
                            #region Konteyner listesinin içerisinde bulunan satır kaynakları alınır.
                            string[] satirKaynagiListesi = new string[1];
                            var cc = item.kaynak.Split(',');

                            if (cc.Count() > 1)
                            {
                                satirKaynagiListesi = new string[cc.Count()];

                                satirKaynagiListesi = item.kaynak.Split(',');
                            }
                            else
                            {
                                satirKaynagiListesi[0] = item.kaynak;
                            }
                            #endregion

                            #region Toplanan tablosunda verileri güncelleme işlemi
                            for (int i = 0; i <= satirKaynagiListesi.Count() - 1; i++)
                            {
                                string sss = "Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_BelgeNo\" = '" + item.cekmeNo + "' and T0.\"U_SiparisNumarasi\" = '" + satirKaynagiListesi[i].ToString().Split('-')[0].Trim() + "' and T0.\"U_SiparisSatirNo\" = '" + satirKaynagiListesi[i].ToString().Split('-')[1].Trim() + "' and " + condition + " (T0.\"U_PaletNo\",'') = '' ";
                                SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_BelgeNo\" = '" + item.cekmeNo + "' and T0.\"U_SiparisNumarasi\" = '" + satirKaynagiListesi[i].ToString().Split('-')[0].Trim() + "' and T0.\"U_SiparisSatirNo\" = '" + satirKaynagiListesi[i].ToString().Split('-')[1].Trim() + "' and " + condition + " (T0.\"U_PaletNo\",'') = '' ");


                                while (!orecord.EoF)
                                {
                                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");
                                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(orecord.Fields.Item("DocEntry").Value));
                                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                    toplanandakiMiktar = oGeneralData.GetProperty("U_Miktar").ToString() == "" ? 0 : Convert.ToDouble(oGeneralData.GetProperty("U_Miktar"));


                                    if (silinecekMiktar >= toplanandakiMiktar)
                                    {
                                        oGeneralService.Delete(oGeneralParams);

                                        toplanandanAzaltmas.Add(new ToplanandanAzaltma
                                        {
                                            siparisNo = Convert.ToInt32(satirKaynagiListesi[i].ToString().Split('-')[0].Trim()),
                                            siparisSatirNo = Convert.ToInt32(satirKaynagiListesi[i].ToString().Split('-')[1].Trim()),
                                            miktar = Convert.ToInt32(toplanandakiMiktar)
                                        });

                                        silinecekMiktar = silinecekMiktar - toplanandakiMiktar;


                                    }
                                    else
                                    {
                                        oGeneralData.SetProperty("U_Miktar", toplanandakiMiktar - silinecekMiktar);
                                        oGeneralService.Update(oGeneralData);

                                        toplanandanAzaltmas.Add(new ToplanandanAzaltma
                                        {
                                            siparisNo = Convert.ToInt32(satirKaynagiListesi[i].ToString().Split('-')[0].Trim()),
                                            siparisSatirNo = Convert.ToInt32(satirKaynagiListesi[i].ToString().Split('-')[1].Trim()),
                                            miktar = Convert.ToInt32(silinecekMiktar)
                                        });

                                        silinecekMiktar = silinecekMiktar - toplanandakiMiktar;

                                    }

                                    if (silinecekMiktar <= 0)
                                    {
                                        i = i + 1; //Bu satır büyük fordan çıkması için yapıldı.
                                        break;
                                    }
                                    orecord.MoveNext();
                                }
                            }
                            #endregion

                            #region Sipariş karşılama ekranında toplanan miktarları azaltma işlemi

                            siparisKarsilamaDusulecekMiktar = item.miktar;
                            for (int i = 0; i <= satirKaynagiListesi.Count() - 1; i++)
                            {
                                if (siparisKarsilamaDusulecekMiktar <= 0)
                                {
                                    break;
                                }
                                oRS_LineId.DoQuery("Select T1.\"LineId\",T0.\"DocEntry\" from \"@AIF_WMS_SIPKAR\" AS T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + item.cekmeNo + "' and T1.\"U_SipSatirNo\" = '" + satirKaynagiListesi[i].ToString().Split('-')[1].Trim() + "' and T1.\"U_SiparisNumarasi\" = '" + satirKaynagiListesi[i].ToString().Split('-')[0].Trim() + "'");

                                if (oRS_LineId.RecordCount > 0)
                                {
                                    int lineid = Convert.ToInt32(oRS_LineId.Fields.Item("LineId").Value);

                                    oGeneralParams_SIPKAR.SetProperty("DocEntry", Convert.ToInt32(oRS_LineId.Fields.Item("DocEntry").Value));
                                    oGeneralData_SIPKAR = oGeneralService_SIPKAR.GetByParams(oGeneralParams_SIPKAR);

                                    oChildrenSatir_SIPKAR = oGeneralData_SIPKAR.Child("AIF_WMS_SIPKAR1");

                                    for (int z = 0; z <= oChildrenSatir_SIPKAR.Count - 1; z++)
                                    {
                                        if (siparisKarsilamaDusulecekMiktar <= 0)
                                        {
                                            break;
                                        }
                                        int val = Convert.ToInt32(z);

                                        oChildSatir1_SIPKAR = oChildrenSatir_SIPKAR.Item(val);

                                        var aa = oChildSatir1_SIPKAR.GetProperty("LineId");
                                        var toplananMiktar = oChildSatir1_SIPKAR.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_ToplananMik"));
                                        var planlananMiktar = oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik"));
                                        if (Convert.ToInt32(aa) == Convert.ToInt32(lineid))
                                        {
                                            int dusulmesiGerekenToplamMiktar = toplanandanAzaltmas.Where(x => x.siparisNo == Convert.ToInt32(satirKaynagiListesi[i].ToString().Split('-')[0].Trim()) && x.siparisSatirNo == Convert.ToInt32(satirKaynagiListesi[i].ToString().Split('-')[1].Trim())).Sum(y => y.miktar);

                                            oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", toplananMiktar - dusulmesiGerekenToplamMiktar);
                                            oChildSatir1_SIPKAR.SetProperty("U_Gorunur", "Y");
                                            oGeneralService_SIPKAR.Update(oGeneralData_SIPKAR);
                                            siparisKarsilamaDusulecekMiktar = siparisKarsilamaDusulecekMiktar - dusulmesiGerekenToplamMiktar;
                                        }


                                    }

                                }
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        double silinecekMiktar = 0;
                        double siparisKarsilamaDusulecekMiktar = 0;
                        double toplanandakiMiktar = 0;
                        foreach (var item in konteynerIcindenSilinenlers.Where(x => x.paletNo == ""))
                        {
                            silinecekMiktar = item.miktar;

                            #region Toplanan tablosunda verileri güncelleme işlemi 
                            SAPbobsCOM.Recordset orecord = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                            orecord.DoQuery("Select * from \"@AIF_WMS_TOPLANAN\" as T0 where T0.\"U_BelgeNo\" = '" + item.cekmeNo + "' and T0.\"U_SiparisNumarasi\" = '" + item.siparisNo + "' and T0.\"U_SiparisSatirNo\" = '" + item.siparisSatirNo + "' and " + condition + " (T0.\"U_PaletNo\",'') = '' ");


                            while (!orecord.EoF)
                            {
                                oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");
                                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(orecord.Fields.Item("DocEntry").Value));
                                oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                toplanandakiMiktar = oGeneralData.GetProperty("U_Miktar").ToString() == "" ? 0 : Convert.ToDouble(oGeneralData.GetProperty("U_Miktar"));


                                if (silinecekMiktar >= toplanandakiMiktar)
                                {
                                    oGeneralService.Delete(oGeneralParams);

                                    silinecekMiktar = silinecekMiktar - toplanandakiMiktar;

                                    toplanandanAzaltmas.Add(new ToplanandanAzaltma
                                    {
                                        siparisNo = Convert.ToInt32(item.siparisNo),
                                        siparisSatirNo = Convert.ToInt32(item.siparisSatirNo),
                                        miktar = Convert.ToInt32(toplanandakiMiktar)
                                    });
                                }
                                else
                                {
                                    oGeneralData.SetProperty("U_Miktar", toplanandakiMiktar - silinecekMiktar);
                                    oGeneralService.Update(oGeneralData);
                                    silinecekMiktar = silinecekMiktar - toplanandakiMiktar;

                                    toplanandanAzaltmas.Add(new ToplanandanAzaltma
                                    {
                                        siparisNo = Convert.ToInt32(item.siparisNo),
                                        siparisSatirNo = Convert.ToInt32(item.siparisSatirNo),
                                        miktar = Convert.ToInt32(silinecekMiktar - toplanandakiMiktar)
                                    });
                                }

                                if (silinecekMiktar <= 0)
                                {
                                    break;
                                }
                                orecord.MoveNext();
                            }
                            #endregion

                            #region Sipariş karşılama ekranında toplanan miktarları azaltma işlemi

                            siparisKarsilamaDusulecekMiktar = item.miktar;
                            if (siparisKarsilamaDusulecekMiktar <= 0)
                            {
                                break;
                            }
                            oRS_LineId.DoQuery("Select T1.\"LineId\",T0.\"DocEntry\" from \"@AIF_WMS_SIPKAR\" AS T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + item.cekmeNo + "' and T1.\"U_SipSatirNo\" = '" + item.siparisSatirNo + "' and T1.\"U_SiparisNumarasi\" = '" + item.siparisNo + "'");

                            if (oRS_LineId.RecordCount > 0)
                            {
                                int lineid = Convert.ToInt32(oRS_LineId.Fields.Item("LineId").Value);

                                oGeneralParams_SIPKAR.SetProperty("DocEntry", Convert.ToInt32(oRS_LineId.Fields.Item("DocEntry").Value));
                                oGeneralData_SIPKAR = oGeneralService_SIPKAR.GetByParams(oGeneralParams_SIPKAR);

                                oChildrenSatir_SIPKAR = oGeneralData_SIPKAR.Child("AIF_WMS_SIPKAR1");

                                for (int z = 0; z <= oChildrenSatir_SIPKAR.Count - 1; z++)
                                {
                                    if (siparisKarsilamaDusulecekMiktar <= 0)
                                    {
                                        break;
                                    }
                                    int val = Convert.ToInt32(z);

                                    oChildSatir1_SIPKAR = oChildrenSatir_SIPKAR.Item(val);

                                    var aa = oChildSatir1_SIPKAR.GetProperty("LineId");
                                    var toplananMiktar = oChildSatir1_SIPKAR.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_ToplananMik"));
                                    var planlananMiktar = oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik"));
                                    if (Convert.ToInt32(aa) == Convert.ToInt32(lineid))
                                    {
                                        int dusulmesiGerekenToplamMiktar = toplanandanAzaltmas.Where(x => x.siparisNo == Convert.ToInt32(item.siparisNo) && x.siparisSatirNo == Convert.ToInt32(item.siparisSatirNo)).Sum(y => y.miktar);

                                        oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", toplananMiktar - dusulmesiGerekenToplamMiktar);
                                        oChildSatir1_SIPKAR.SetProperty("U_Gorunur", "Y");
                                        oGeneralService_SIPKAR.Update(oGeneralData_SIPKAR);
                                        siparisKarsilamaDusulecekMiktar = siparisKarsilamaDusulecekMiktar - dusulmesiGerekenToplamMiktar;
                                    }


                                }

                            }

                            #endregion
                        }
                    }


                    #endregion

                    #region Ürün konteynere ilk defa atılıyorsa toplanan tablosuna ve sipariş karşılamaya verileri atılır.
                    karsilanmasiGerekenMiktar = 0;
                    yazilacakMiktar = 0;
                    if (cekmeListesiKalemleriniGrupla == "Y")
                    {
                        #region Toplanan ve sipariş karşılama ekranlarına datalar güncellenir.
                        foreach (var item in konteynerYapma.konteynerYapmaDetays.Where(x => x.UrunKonteynereDahaOnceEklendi != "Y"))
                        {
                            karsilanmasiGerekenMiktar = item.Quantity;
                            #region Konteyner listesinin içerisinde bulunan satır kaynakları alınır.
                            string[] satirKaynagiListesi = new string[1];
                            var cc = item.satirKaynagi.Split(',');

                            if (cc.Count() > 1)
                            {
                                satirKaynagiListesi = new string[cc.Count()];

                                satirKaynagiListesi = item.satirKaynagi.Split(',');


                            }
                            else
                            {
                                satirKaynagiListesi[0] = item.satirKaynagi;
                            }
                            #endregion

                            for (int i = 0; i <= satirKaynagiListesi.Count() - 1; i++)
                            {
                                if (karsilanmasiGerekenMiktar <= 0)
                                {
                                    break;
                                }
                                oRS_LineId.DoQuery("Select T1.\"LineId\",T0.\"DocEntry\" from \"@AIF_WMS_SIPKAR\" AS T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + item.CekmeListesiNo + "' and T1.\"U_SipSatirNo\" = '" + satirKaynagiListesi[i].ToString().Split('-')[1].Trim() + "' and T1.\"U_SiparisNumarasi\" = '" + satirKaynagiListesi[i].ToString().Split('-')[0].Trim() + "'");

                                if (oRS_LineId.RecordCount > 0)
                                {
                                    int lineid = Convert.ToInt32(oRS_LineId.Fields.Item("LineId").Value);

                                    oGeneralParams_SIPKAR.SetProperty("DocEntry", Convert.ToInt32(oRS_LineId.Fields.Item("DocEntry").Value));
                                    oGeneralData_SIPKAR = oGeneralService_SIPKAR.GetByParams(oGeneralParams_SIPKAR);

                                    oChildrenSatir_SIPKAR = oGeneralData_SIPKAR.Child("AIF_WMS_SIPKAR1");



                                    for (int z = 0; z <= oChildrenSatir_SIPKAR.Count - 1; z++)
                                    {
                                        if (karsilanmasiGerekenMiktar <= 0)
                                        {
                                            break;
                                        }
                                        int val = Convert.ToInt32(z);

                                        oChildSatir1_SIPKAR = oChildrenSatir_SIPKAR.Item(val);

                                        var aa = oChildSatir1_SIPKAR.GetProperty("LineId");
                                        var toplananMiktar = oChildSatir1_SIPKAR.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_ToplananMik"));
                                        var planlananMiktar = oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik"));
                                        var satiricinKalanMiktar = planlananMiktar - toplananMiktar;
                                        var siparisNo = oChildSatir1_SIPKAR.GetProperty("U_SiparisNumarasi").ToString();
                                        var siparisSatirNo = oChildSatir1_SIPKAR.GetProperty("U_SipSatirNo").ToString();
                                        double snc = 0;
                                        if (Convert.ToInt32(aa) == Convert.ToInt32(lineid))
                                        {

                                            if (karsilanmasiGerekenMiktar >= satiricinKalanMiktar)
                                            {
                                                oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", planlananMiktar);
                                                oChildSatir1_SIPKAR.SetProperty("U_Gorunur", "N");
                                                yazilacakMiktar = satiricinKalanMiktar;
                                                karsilanmasiGerekenMiktar = karsilanmasiGerekenMiktar - satiricinKalanMiktar;
                                            }
                                            else
                                            {
                                                snc = Convert.ToDouble(toplananMiktar) + karsilanmasiGerekenMiktar;
                                                oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", Convert.ToDouble(snc));
                                                yazilacakMiktar = karsilanmasiGerekenMiktar;
                                                karsilanmasiGerekenMiktar = karsilanmasiGerekenMiktar - snc;

                                            }


                                            #region Sipariş Karşılama Ekranındaki eşleşen kayıda göre toplanana kayıt atılır.
                                            oRS.DoQuery("Select MAX(\"DocEntry\") from \"@AIF_WMS_TOPLANAN\" as T0 ");
                                            int docentry = Convert.ToInt32(oRS.Fields.Item(0).Value) + 1;

                                            oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");

                                            oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                                            oGeneralData.SetProperty("DocNum", docentry.ToString());
                                            oGeneralData.SetProperty("U_BelgeNo", item.CekmeListesiNo.ToString());
                                            oGeneralData.SetProperty("U_SiparisNumarasi", siparisNo);
                                            oGeneralData.SetProperty("U_SiparisSatirNo", siparisSatirNo);
                                            oGeneralData.SetProperty("U_Miktar", Convert.ToDouble(yazilacakMiktar));
                                            oGeneralData.SetProperty("U_KntynrNo", islemYapilanKonteynerNo.ToString());
                                            oGeneralData.SetProperty("U_KalemKodu", item.KalemKodu.ToString());
                                            oGeneralData.SetProperty("U_KalemAdi", item.KalemTanimi.ToString());
                                            oGeneralData.SetProperty("U_PaletNo", item.PaletNo.ToString());
                                            if (item.satirKaynagi != null && item.satirKaynagi != "")
                                            {
                                                oGeneralData.SetProperty("U_Kaynak", item.satirKaynagi.ToString());
                                            }

                                            docentry++;
                                            oGeneralService.Add(oGeneralData);
                                            #endregion

                                        }
                                    }

                                    oGeneralService_SIPKAR.Update(oGeneralData_SIPKAR);
                                }
                            }


                        }
                        #endregion
                    }
                    else
                    {
                        #region Toplanan ve sipariş karşılama ekranlarına datalar güncellenir.
                        foreach (var item in konteynerYapma.konteynerYapmaDetays.Where(x => x.UrunKonteynereDahaOnceEklendi != "Y"))
                        {
                            karsilanmasiGerekenMiktar = item.Quantity;
                            if (karsilanmasiGerekenMiktar <= 0)
                            {
                                break;
                            }
                            oRS_LineId.DoQuery("Select T1.\"LineId\",T0.\"DocEntry\" from \"@AIF_WMS_SIPKAR\" AS T0 INNER JOIN \"@AIF_WMS_SIPKAR1\" AS T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"DocEntry\" = '" + item.CekmeListesiNo + "' and T1.\"U_SipSatirNo\" = '" + item.siparisSatirNo + "' and T1.\"U_SiparisNumarasi\" = '" + item.siparisNo + "'");

                            if (oRS_LineId.RecordCount > 0)
                            {
                                int lineid = Convert.ToInt32(oRS_LineId.Fields.Item("LineId").Value);

                                oGeneralParams_SIPKAR.SetProperty("DocEntry", Convert.ToInt32(oRS_LineId.Fields.Item("DocEntry").Value));
                                oGeneralData_SIPKAR = oGeneralService_SIPKAR.GetByParams(oGeneralParams_SIPKAR);

                                oChildrenSatir_SIPKAR = oGeneralData_SIPKAR.Child("AIF_WMS_SIPKAR1");



                                for (int z = 0; z <= oChildrenSatir_SIPKAR.Count - 1; z++)
                                {
                                    if (karsilanmasiGerekenMiktar <= 0)
                                    {
                                        break;
                                    }
                                    int val = Convert.ToInt32(z);

                                    oChildSatir1_SIPKAR = oChildrenSatir_SIPKAR.Item(val);

                                    var aa = oChildSatir1_SIPKAR.GetProperty("LineId");
                                    var toplananMiktar = oChildSatir1_SIPKAR.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_ToplananMik"));
                                    var planlananMiktar = oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik"));
                                    var satiricinKalanMiktar = planlananMiktar - toplananMiktar;
                                    var siparisNo = oChildSatir1_SIPKAR.GetProperty("U_SiparisNumarasi").ToString();
                                    var siparisSatirNo = oChildSatir1_SIPKAR.GetProperty("U_SipSatirNo").ToString();
                                    double snc = 0;
                                    if (Convert.ToInt32(aa) == Convert.ToInt32(lineid))
                                    {

                                        if (karsilanmasiGerekenMiktar >= satiricinKalanMiktar)
                                        {
                                            oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", planlananMiktar);
                                            oChildSatir1_SIPKAR.SetProperty("U_Gorunur", "N");
                                            yazilacakMiktar = satiricinKalanMiktar;
                                            karsilanmasiGerekenMiktar = karsilanmasiGerekenMiktar - satiricinKalanMiktar;
                                        }
                                        else
                                        {
                                            snc = Convert.ToDouble(toplananMiktar) + karsilanmasiGerekenMiktar;
                                            oChildSatir1_SIPKAR.SetProperty("U_ToplananMik", Convert.ToDouble(snc));
                                            yazilacakMiktar = karsilanmasiGerekenMiktar;
                                            karsilanmasiGerekenMiktar = karsilanmasiGerekenMiktar - snc;

                                        }


                                        #region Sipariş Karşılama Ekranındaki eşleşen kayıda göre toplanana kayıt atılır.
                                        oRS.DoQuery("Select MAX(\"DocEntry\") from \"@AIF_WMS_TOPLANAN\" as T0 ");
                                        int docentry = Convert.ToInt32(oRS.Fields.Item(0).Value) + 1;

                                        oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");

                                        oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                                        oGeneralData.SetProperty("DocNum", docentry.ToString());
                                        oGeneralData.SetProperty("U_BelgeNo", item.CekmeListesiNo.ToString());
                                        oGeneralData.SetProperty("U_SiparisNumarasi", siparisNo);
                                        oGeneralData.SetProperty("U_SiparisSatirNo", siparisSatirNo);
                                        oGeneralData.SetProperty("U_Miktar", Convert.ToDouble(yazilacakMiktar));
                                        oGeneralData.SetProperty("U_KntynrNo", islemYapilanKonteynerNo.ToString());
                                        oGeneralData.SetProperty("U_KalemKodu", item.KalemKodu.ToString());
                                        oGeneralData.SetProperty("U_KalemAdi", item.KalemTanimi.ToString());
                                        oGeneralData.SetProperty("U_PaletNo", item.PaletNo.ToString());
                                        if (item.satirKaynagi != null && item.satirKaynagi != "")
                                        {
                                            oGeneralData.SetProperty("U_Kaynak", item.satirKaynagi.ToString());
                                        }

                                        docentry++;
                                        oGeneralService.Add(oGeneralData);
                                        #endregion

                                    }
                                }

                                oGeneralService_SIPKAR.Update(oGeneralData_SIPKAR);
                            }


                        }
                        #endregion

                    }

                    #endregion



                }


                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
                return new Response { Val = 0, Desc = "Başarılı.", _list = null };
            }
            catch (Exception)
            {
                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
                return new Response { Val = 0, Desc = "Başarısız. Konteyner verileri güncellenirken hata oluştu.", _list = null };
            }
            finally
            {
                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
            }

        }

        List<ToplanandanAzaltma> toplanandanAzaltmas = new List<ToplanandanAzaltma>();
        public class ToplanandanAzaltma
        {
            public int siparisNo { get; set; }
            public int siparisSatirNo { get; set; }
            public int miktar { get; set; }
        } 
    }
}