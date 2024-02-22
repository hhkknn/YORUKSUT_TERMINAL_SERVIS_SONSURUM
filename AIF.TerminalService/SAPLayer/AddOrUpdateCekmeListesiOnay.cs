using AIF.TerminalService.Models;
using NLog;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{

    public class AddOrUpdateCekmeListesiOnay
    {
        private string companyDbCode;
        int clNum = 0;


        public Response addOrUpdateCekmeListesiOnay(string dbName, CekmeListesiOnaylama cekmeListesiOnaylama, List<SilinenPaletNoListesi> silinenPaletNoListesis, string cekmeListesiKalemleriniGrupla)
        {
            Random rastgele = new Random();
            int ID = rastgele.Next(0, 9999);
            Logger logger = LogManager.GetCurrentClassLogger();

            //var requestJson_New = JsonConvert.SerializeObject(protocol);

            //logger.Info(" ");

            logger.Info("ID: " + ID + " addOrUpdateCekmeListesiOnay Servisine Geldi.");
            //logger.Info("ID: " + ID + " ISTEK :" + requestJson_New);

            SAPbobsCOM.Company oCompany = null;

            try
            {
                ConnectionList connection = new ConnectionList();

                LoginCompany log = new LoginCompany();

                log.DisconnectSAP(dbName);

                connection = log.getSAPConnection(dbName, ID);

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

                if (connection.number == -1)
                {
                    logger.Fatal("ID: " + ID + " " + "Hata Kodu - 3100 Veritabanı bağlantısı sırasında hata oluştu.");
                    return new Response { Val = -3100, Desc = "Hata Kodu - 3100 Veritabanı bağlantısı sırasında hata oluştu. ", _list = null };
                }

                clNum = connection.number;
                companyDbCode = connection.dbCode;
                oCompany = connection.oCompany;

                logger.Info("ID: " + ID + " Şirket bağlantısını başarıyla geçtik. Bağlantı sağladığımız DB :" + oCompany.CompanyDB + " clnum: " + clNum);

                oCompany.StartTransaction();

                Recordset oRS = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                Recordset oRS_Kalem = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string condition = oCompany.DbServerType == BoDataServerTypes.dst_HANADB ? "IFNULL" : "ISNULL";
                string muhatapkatalognumaralariniGoster = "";
                string musteriKodu = "";
                oRS.DoQuery("Select \"DocEntry\",\"U_MuhKatGoster\",\"U_MusteriKodu\" from \"@AIF_WMS_SIPKAR\" as T0 where T0.\"DocEntry\" = '" + cekmeListesiOnaylama.BelgeNo + "'");
                muhatapkatalognumaralariniGoster = oRS.Fields.Item("U_MuhKatGoster").Value.ToString();
                musteriKodu = oRS.Fields.Item("U_MusteriKodu").Value.ToString();

                if (oRS.RecordCount == 0) //Daha önce bu partiye kayıt girilmiş mi?
                {
                    return null;
                }
                else
                {
                    string sipariskarsilamaDocEntry = "";

                    CompanyService oCompService = null;

                    GeneralService oGeneralService;

                    GeneralData oGeneralData;

                    GeneralData oChildSatir1;

                    GeneralData oChildSatir1_SIPKAR;

                    GeneralDataCollection oChildrenSatir;

                    GeneralDataCollection oChildrenSatir_SIPKAR;

                    oCompService = oCompany.GetCompanyService();

                    GeneralDataParams oGeneralParams;

                    //oCompany.StartTransaction();

                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_SIPKAR");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                    sipariskarsilamaDocEntry = oRS.Fields.Item("DocEntry").Value.ToString();
                    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(sipariskarsilamaDocEntry));
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                    oChildrenSatir = oGeneralData.Child("AIF_WMS_SIPKAR1");
                    oChildrenSatir_SIPKAR = oGeneralData.Child("AIF_WMS_SIPKAR1");

                    #region Toplanan Siparişler tablosuna ekleme

                    try
                    {
                        //Toplanan tablosuna atılırken eğer gruplama ile yapılıyorsa her bir sipariş karşılama ekranının satırı için veri atılmalı.
                        if (cekmeListesiKalemleriniGrupla != "Y")
                        {
                            if (cekmeListesiOnaylama.cekmeListesiOnaylamaDetays.Count > 0)
                            {
                                oRS.DoQuery("Select MAX(\"DocEntry\") from \"@AIF_WMS_TOPLANAN\" as T0 ");
                                int docentry = Convert.ToInt32(oRS.Fields.Item(0).Value) + 1;

                                oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");

                                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                foreach (var itemy in cekmeListesiOnaylama.cekmeListesiOnaylamaDetays.Where(x => x.PaletNo != ""))
                                {
                                    oGeneralData.SetProperty("DocNum", docentry.ToString());
                                    oGeneralData.SetProperty("U_BelgeNo", cekmeListesiOnaylama.BelgeNo.ToString());
                                    oGeneralData.SetProperty("U_SiparisNumarasi", itemy.SipNo);
                                    oGeneralData.SetProperty("U_SiparisSatirNo", itemy.SatirNo.ToString());
                                    oGeneralData.SetProperty("U_Miktar", Convert.ToDouble(itemy.Miktar));
                                    oGeneralData.SetProperty("U_PaletNo", itemy.PaletNo.ToString());
                                    oGeneralData.SetProperty("U_KalemKodu", itemy.KalemKodu.ToString());
                                    oGeneralData.SetProperty("U_KalemAdi", itemy.KalemTanimi.ToString());
                                    oGeneralData.SetProperty("U_Kaynak", itemy.satirKaynagi == null ? "" : itemy.satirKaynagi);

                                    docentry++;
                                    oGeneralService.Add(oGeneralData);

                                }
                            }
                        }
                        else
                        {
                            //oRS.DoQuery("Select \"DocEntry\",\"U_MuhKatGoster\",\"U_MusteriKodu\" from \"@AIF_WMS_SIPKAR\" as T0 where T0.\"DocEntry\" = '" + cekmeListesiOnaylama.BelgeNo + "'");

                            //oGeneralService = oCompService.GetGeneralService("AIF_WMS_SIPKAR");

                            //oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                            //oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                            //oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(oRS.Fields.Item("DocEntry").Value));
                            //oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                            ////oGeneralData.SetProperty("U_KonteynerNo", cekmeListesiOnaylama.KonteynerNumarasi.ToString());

                            ////DateTime dt = new DateTime(Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(0, 4)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(4, 2)), Convert.ToInt32(telemeAnalizTakibi.Tarih.Substring(6, 2)));

                            //oChildrenSatir_SIPKAR = oGeneralData.Child("AIF_WMS_SIPKAR1");


                            oRS.DoQuery("Select MAX(\"DocEntry\") from \"@AIF_WMS_TOPLANAN\" as T0 ");
                            int docentry = Convert.ToInt32(oRS.Fields.Item(0).Value) + 1;

                            oGeneralService = oCompService.GetGeneralService("AIF_WMS_TOPLANAN");

                            oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                            foreach (var itemy in cekmeListesiOnaylama.cekmeListesiOnaylamaDetays.Where(x => x.PaletNo != ""))
                            {
                                double kalanMiktar = itemy.Miktar;
                                for (int z = 0; z <= oChildrenSatir_SIPKAR.Count - 1; z++)
                                {
                                    if (kalanMiktar == 0)
                                    {
                                        continue;
                                    }
                                    int val = Convert.ToInt32(z);

                                    oChildSatir1_SIPKAR = oChildrenSatir_SIPKAR.Item(val);

                                    string kalemKodu = oChildSatir1_SIPKAR.GetProperty("U_UrunKodu").ToString();
                                    var toplananMiktar = oChildSatir1_SIPKAR.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_ToplananMik"));
                                    var planlananMiktar = oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1_SIPKAR.GetProperty("U_PlanSipMik"));
                                    string gorunur = oChildSatir1_SIPKAR.GetProperty("U_Gorunur").ToString();
                                    if (gorunur == "N")
                                    {
                                        continue;
                                    }
                                    double snc = 0;
                                    if (kalemKodu == itemy.KalemKodu)
                                    {

                                        if (kalanMiktar >= (planlananMiktar - toplananMiktar))
                                        {

                                            oGeneralData.SetProperty("DocNum", docentry.ToString());
                                            oGeneralData.SetProperty("U_BelgeNo", cekmeListesiOnaylama.BelgeNo.ToString());
                                            oGeneralData.SetProperty("U_SiparisNumarasi", oChildSatir1_SIPKAR.GetProperty("U_SiparisNumarasi"));
                                            oGeneralData.SetProperty("U_SiparisSatirNo", oChildSatir1_SIPKAR.GetProperty("U_SipSatirNo").ToString());
                                            oGeneralData.SetProperty("U_Miktar", Convert.ToDouble(planlananMiktar - toplananMiktar));
                                            oGeneralData.SetProperty("U_PaletNo", itemy.PaletNo);
                                            oGeneralData.SetProperty("U_KalemKodu", oChildSatir1_SIPKAR.GetProperty("U_UrunKodu"));
                                            oGeneralData.SetProperty("U_KalemAdi", oChildSatir1_SIPKAR.GetProperty("U_UrunTanimi"));
                                            oGeneralData.SetProperty("U_Kaynak", itemy.satirKaynagi == null ? "" : itemy.satirKaynagi);

                                            docentry++;
                                            oGeneralService.Add(oGeneralData);
                                            //oChildSatir1.SetProperty("U_ToplananMik", Convert.ToDouble(planlananMiktar));
                                            //oChildSatir1.SetProperty("U_Gorunur", "N");
                                            kalanMiktar = kalanMiktar - (planlananMiktar - toplananMiktar);
                                        }
                                        else
                                        {

                                            oGeneralData.SetProperty("DocNum", docentry.ToString());
                                            oGeneralData.SetProperty("U_BelgeNo", cekmeListesiOnaylama.BelgeNo.ToString());
                                            oGeneralData.SetProperty("U_SiparisNumarasi", oChildSatir1_SIPKAR.GetProperty("U_SiparisNumarasi"));
                                            oGeneralData.SetProperty("U_SiparisSatirNo", oChildSatir1_SIPKAR.GetProperty("U_SipSatirNo").ToString());
                                            oGeneralData.SetProperty("U_Miktar", Convert.ToDouble(kalanMiktar));
                                            oGeneralData.SetProperty("U_PaletNo", itemy.PaletNo);
                                            oGeneralData.SetProperty("U_KalemKodu", oChildSatir1_SIPKAR.GetProperty("U_UrunKodu"));
                                            oGeneralData.SetProperty("U_KalemAdi", oChildSatir1_SIPKAR.GetProperty("U_UrunTanimi"));
                                            oGeneralData.SetProperty("U_Kaynak", itemy.satirKaynagi == null ? "" : itemy.satirKaynagi);

                                            docentry++;
                                            oGeneralService.Add(oGeneralData);

                                            kalanMiktar = 0;
                                            //oChildSatir1.SetProperty("U_ToplananMik", Convert.ToDouble(kalanMiktar));
                                            ////snc = Convert.ToDouble(kalanMiktar) + item.Miktar;
                                            //if (Convert.ToDouble(planlananMiktar) - kalanMiktar == 0)
                                            //{
                                            //    oChildSatir1.SetProperty("U_Gorunur", "N");
                                            //}
                                        }
                                        //snc = Convert.ToDouble(toplananMiktar) + item.Miktar;
                                        //oChildSatir1.SetProperty("U_ToplananMik", Convert.ToDouble(item.Miktar));

                                        //oChildSatir1.SetProperty("U_PaletNo", item.PaletNo);

                                        //if (Convert.ToDouble(planlananMiktar) - snc == 0)
                                        //{
                                        //    //oChildrenSatir.Remove(val);
                                        //    oChildSatir1.SetProperty("U_Gorunur", "N"); //Sipariş karşılamada satır silme yerine görünüp görünmemesi gerektiği parametresini buradan tabloya gönderiyoruz.
                                        //}
                                    }
                                }

                            }
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        catch (Exception)
                        {
                        }
                        logger.Fatal("ID: " + ID + " Hata Kodu - 5200 hata oluştu. : " + oCompany.GetLastErrorDescription());

                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                    }

                    #endregion

                    #region Sipariş karşılama işlemleri


                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_SIPKAR");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(sipariskarsilamaDocEntry));
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                    oChildrenSatir = oGeneralData.Child("AIF_WMS_SIPKAR1");

                    int ix = 0;
                    foreach (var item in cekmeListesiOnaylama.cekmeListesiOnaylamaDetays)
                    {
                        if (cekmeListesiKalemleriniGrupla != null && cekmeListesiKalemleriniGrupla != "Y")
                        {
                            if (item.SiparisKarsilamaLineId != "")
                            {
                                double kalanmik = item.Miktar;
                                for (int z = 0; z <= oChildrenSatir.Count - 1; z++)
                                {
                                    int val = Convert.ToInt32(z);

                                    oChildSatir1 = oChildrenSatir.Item(val);

                                    var aa = oChildSatir1.GetProperty("LineId");
                                    var toplananMiktar = oChildSatir1.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1.GetProperty("U_ToplananMik"));
                                    var planlananMiktar = oChildSatir1.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1.GetProperty("U_PlanSipMik"));
                                    double snc = 0;
                                    if (Convert.ToInt32(aa) == Convert.ToInt32(item.SiparisKarsilamaLineId))
                                    {
                                        snc = Convert.ToDouble(toplananMiktar) + item.Miktar;
                                        oChildSatir1.SetProperty("U_ToplananMik", Convert.ToDouble(item.Miktar));

                                        oChildSatir1.SetProperty("U_PaletNo", item.PaletNo);

                                        oChildSatir1.SetProperty("U_Kaynak", item.satirKaynagi == null ? "" : item.satirKaynagi);

                                        if (Convert.ToDouble(planlananMiktar) - snc == 0)
                                        {
                                            //oChildrenSatir.Remove(val);
                                            oChildSatir1.SetProperty("U_Gorunur", "N"); //Sipariş karşılamada satır silme yerine görünüp görünmemesi gerektiği parametresini buradan tabloya gönderiyoruz.
                                        }
                                    }
                                }
                            }
                            else
                            {
                                oChildSatir1 = oChildrenSatir.Add();

                                oChildSatir1.SetProperty("U_SiparisNumarasi", item.SipNo);


                                //if (muhatapkatalognumaralariniGoster == "Y")
                                //{
                                //    oRS_Kalem.DoQuery("Select \"ItemCode\" from \"OSCN\" where \"CardCode\" = '" + musteriKodu + "' and \"Substitute\" = '" + item.KalemKodu + "' ");

                                //    if (oRS_Kalem.RecordCount == 0)
                                //    {
                                //        try
                                //        {
                                //            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                                //        }
                                //        catch (Exception)
                                //        {
                                //        }



                                //        LoginCompany.ReleaseConnection(connection.number, companyDbCode);
                                //        return new Response { Val = 0, Desc = "Ürün Kodu bulunamadı.", _list = null };
                                //    }
                                //    else
                                //    {
                                //        item.KalemKodu = oRS_Kalem.Fields.Item("ItemCode").Value.ToString();
                                //    }
                                //}

                                oChildSatir1.SetProperty("U_UrunKodu", item.KalemKodu);

                                //oChildSatir1.SetProperty("U_MuhKatalogNo", item.MuhatapKatalogNo.ToString());

                                //oChildSatir1.SetProperty("U_Barkod", item.Barkod.ToString());

                                oChildSatir1.SetProperty("U_ToplananMik", Convert.ToDouble(item.Miktar));

                                oChildSatir1.SetProperty("U_PaletNo", item.PaletNo);

                                oChildSatir1.SetProperty("U_Kaynak", item.satirKaynagi == null ? "" : item.satirKaynagi);
                            }
                        }
                        else
                        {

                            double kalanMiktar = item.Miktar;

                            for (int z = 0; z <= oChildrenSatir.Count - 1; z++)
                            {
                                if (kalanMiktar <= 0)
                                {
                                    continue;
                                }
                                int val = Convert.ToInt32(z);

                                oChildSatir1 = oChildrenSatir.Item(val);

                                string kalemKodu = oChildSatir1.GetProperty("U_UrunKodu").ToString();
                                var toplananMiktar = oChildSatir1.GetProperty("U_ToplananMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1.GetProperty("U_ToplananMik"));
                                var planlananMiktar = oChildSatir1.GetProperty("U_PlanSipMik").ToString() == "" ? 0 : Convert.ToDouble(oChildSatir1.GetProperty("U_PlanSipMik"));
                                string gorunur = oChildSatir1.GetProperty("U_Gorunur").ToString();
                                if (gorunur == "N")
                                {
                                    continue;
                                }
                                double snc = 0;
                                if (kalemKodu == item.KalemKodu)
                                {
                                    if (kalanMiktar >= (planlananMiktar - toplananMiktar))
                                    {
                                        oChildSatir1.SetProperty("U_ToplananMik", Convert.ToDouble(planlananMiktar));
                                        oChildSatir1.SetProperty("U_Gorunur", "N");
                                        kalanMiktar = kalanMiktar - (planlananMiktar - toplananMiktar);
                                    }
                                    else
                                    {
                                        if (kalanMiktar > (planlananMiktar - toplananMiktar))
                                        {
                                            oChildSatir1.SetProperty("U_ToplananMik", Convert.ToDouble(planlananMiktar - toplananMiktar) + toplananMiktar);
                                            kalanMiktar = kalanMiktar - Convert.ToDouble(planlananMiktar - toplananMiktar);
                                            oChildSatir1.SetProperty("U_Gorunur", "N");
                                        }
                                        else
                                        {
                                            oChildSatir1.SetProperty("U_ToplananMik", Convert.ToDouble(toplananMiktar + kalanMiktar));
                                            kalanMiktar = kalanMiktar - Convert.ToDouble(planlananMiktar - toplananMiktar);

                                            if (kalanMiktar == 0)
                                            {
                                                oChildSatir1.SetProperty("U_Gorunur", "N");
                                            }
                                        }
                                        //oChildSatir1.SetProperty("U_ToplananMik", Convert.ToDouble(kalanMiktar));
                                        //snc = Convert.ToDouble(kalanMiktar) + item.Miktar;
                                        //if (Convert.ToDouble(planlananMiktar) - kalanMiktar == 0)
                                        //{
                                        //    oChildSatir1.SetProperty("U_Gorunur", "N");
                                        //}
                                        //kalanMiktar = 0; //chn
                                    }
                                    //snc = Convert.ToDouble(toplananMiktar) + item.Miktar;
                                    //oChildSatir1.SetProperty("U_ToplananMik", Convert.ToDouble(item.Miktar));

                                    //oChildSatir1.SetProperty("U_PaletNo", item.PaletNo);

                                    //if (Convert.ToDouble(planlananMiktar) - snc == 0)
                                    //{
                                    //    //oChildrenSatir.Remove(val);
                                    //    oChildSatir1.SetProperty("U_Gorunur", "N"); //Sipariş karşılamada satır silme yerine görünüp görünmemesi gerektiği parametresini buradan tabloya gönderiyoruz.
                                    //}
                                }
                            }
                        }

                        ix++;

                    }

                    try
                    {

                        oGeneralService.Update(oGeneralData);

                    }
                    catch (Exception)
                    {
                        try
                        {
                            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        catch (Exception)
                        {
                        }
                        logger.Fatal("ID: " + ID + " Hata Kodu - 5400 hata oluştu. : " + oCompany.GetLastErrorDescription());

                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = -5200, Desc = "Hata Kodu - 5400 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                    }

                    #endregion

                    try
                    {

                        #region Palet Tablosunu İşlemleri

                        int paletSayisi = 0;
                        var paletnolistesi = cekmeListesiOnaylama.cekmeListesiOnaylamaDetays.Select(x => x.PaletNo).Distinct();
                        bool islemvar = false;
                        string palet = "";
                        bool guncelleme = false;
                        foreach (var item in paletnolistesi)
                        {
                            oRS.DoQuery("Select \"DocEntry\",\"U_PaletNo\" from \"@AIF_WMS_PALET\" as T0 where T0.\"U_PaletNo\" = '" + item + "'");
                            islemvar = false;
                            guncelleme = false;
                            if (oRS.RecordCount > 0)
                            {
                                paletSayisi++;
                                guncelleme = true;
                                islemvar = true;
                                oGeneralService = oCompService.GetGeneralService("AIF_WMS_PALET");

                                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(oRS.Fields.Item("DocEntry").Value));
                                oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                oGeneralData.SetProperty("U_PaletNo", item);

                                oChildrenSatir = oGeneralData.Child("AIF_WMS_PALET1");

                                if (oChildrenSatir.Count > 0)
                                {
                                    //int drc = oChildrenSatir.Count;
                                    //for (int rmv = 0; rmv < drc; rmv++)
                                    //    oChildrenSatir.Remove(0)

                                    //for (int z = 0; z <= oChildrenSatir.Count - 1; z++)
                                    //{
                                    //    int val = Convert.ToInt32(z);

                                    //    oChildSatir1 = oChildrenSatir.Item(val);

                                    //    var aa = oChildSatir1.GetProperty("U_PaletNo");

                                    //    if (aa.ToString() == item.ToString())
                                    //    {
                                    //        oChildrenSatir.Remove(z);
                                    //    }
                                    //}
                                }

                                foreach (var itemx in cekmeListesiOnaylama.cekmeListesiOnaylamaDetays.Where(x => x.PaletNo == item))
                                {
                                    oChildSatir1 = oChildrenSatir.Add();


                                    //if (muhatapkatalognumaralariniGoster == "Y")
                                    //{
                                    //    oRS_Kalem.DoQuery("Select \"ItemCode\" from \"OSCN\" where \"CardCode\" = '" + musteriKodu + "' and \"Substitute\" = '" + itemx.KalemKodu + "' ");

                                    //    if (oRS_Kalem.RecordCount == 0)
                                    //    {
                                    //        try
                                    //        {
                                    //            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                                    //        }
                                    //        catch (Exception)
                                    //        {
                                    //        }



                                    //        LoginCompany.ReleaseConnection(connection.number, companyDbCode);
                                    //        return new Response { Val = 0, Desc = "Ürün Kodu bulunamadı.", _list = null };
                                    //    }
                                    //    else
                                    //    {
                                    //        itemx.KalemKodu = oRS_Kalem.Fields.Item("ItemCode").Value.ToString();
                                    //    }
                                    //}

                                    oChildSatir1.SetProperty("U_KalemKodu", itemx.KalemKodu);

                                    oChildSatir1.SetProperty("U_Tanim", itemx.KalemTanimi);

                                    oChildSatir1.SetProperty("U_MuhKatalogNo", itemx.MuhatapKatalogNo.ToString());

                                    oChildSatir1.SetProperty("U_Barkod", itemx.Barkod.ToString());

                                    oChildSatir1.SetProperty("U_Miktar", Convert.ToDouble(itemx.Miktar));

                                    oChildSatir1.SetProperty("U_SiparisNo", itemx.SipNo);

                                    oChildSatir1.SetProperty("U_SipSatirNo", itemx.SatirNo);

                                    oChildSatir1.SetProperty("U_CekmeNo", cekmeListesiOnaylama.BelgeNo);

                                    oChildSatir1.SetProperty("U_Kaynak", itemx.satirKaynagi == null ? "" : itemx.satirKaynagi);

                                }
                            }
                            else
                            {
                                paletSayisi++;

                                oRS.DoQuery("Select ISNULL(MAX(\"DocEntry\"),0) + 1 from \"@AIF_WMS_PALET\"");

                                int maxdocentry = Convert.ToInt32(oRS.Fields.Item(0).Value);


                                oGeneralService = oCompService.GetGeneralService("AIF_WMS_PALET");

                                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);


                                oGeneralData.SetProperty("DocNum", maxdocentry);
                                oGeneralData.SetProperty("U_PaletNo", item);

                                oGeneralData.SetProperty("U_Durum", "A");

                                oChildrenSatir = oGeneralData.Child("AIF_WMS_PALET1");

                                foreach (var itemx in cekmeListesiOnaylama.cekmeListesiOnaylamaDetays.Where(x => x.PaletNo == item))
                                {
                                    islemvar = true;
                                    oChildSatir1 = oChildrenSatir.Add();

                                    //if (muhatapkatalognumaralariniGoster == "Y")
                                    //{
                                    //    oRS_Kalem.DoQuery("Select \"ItemCode\" from \"OSCN\" where \"CardCode\" = '" + musteriKodu + "' and \"Substitute\" = '" + itemx.MuhatapKatalogNo + "' ");

                                    //    if (oRS_Kalem.RecordCount == 0)
                                    //    {
                                    //        try
                                    //        {
                                    //            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                                    //        }
                                    //        catch (Exception)
                                    //        {
                                    //        }



                                    //        LoginCompany.ReleaseConnection(connection.number, companyDbCode);
                                    //        return new Response { Val = 0, Desc = "Ürün Kodu bulunamadı.", _list = null };
                                    //    }
                                    //    else
                                    //    {
                                    //        itemx.KalemKodu = oRS_Kalem.Fields.Item("ItemCode").Value.ToString();
                                    //    }
                                    //}

                                    oChildSatir1.SetProperty("U_KalemKodu", itemx.KalemKodu);

                                    oChildSatir1.SetProperty("U_Tanim", itemx.KalemTanimi);

                                    oChildSatir1.SetProperty("U_MuhKatalogNo", itemx.MuhatapKatalogNo.ToString());

                                    oChildSatir1.SetProperty("U_Barkod", itemx.Barkod.ToString());

                                    oChildSatir1.SetProperty("U_Miktar", Convert.ToDouble(itemx.Miktar));

                                    if (itemx.SipNo != -1)
                                    {
                                        oChildSatir1.SetProperty("U_SiparisNo", itemx.SipNo.ToString());
                                    }

                                    if (itemx.SatirNo != -1)
                                    {
                                        oChildSatir1.SetProperty("U_SipSatirNo", itemx.SatirNo.ToString());
                                    }

                                    oChildSatir1.SetProperty("U_CekmeNo", cekmeListesiOnaylama.BelgeNo);

                                    oChildSatir1.SetProperty("U_Kaynak", itemx.satirKaynagi == null ? "" : itemx.satirKaynagi);
                                }
                            }

                            try
                            {
                                if (islemvar)
                                {
                                    if (guncelleme)
                                    {
                                        oGeneralService.Update(oGeneralData);
                                    }
                                    else
                                    {
                                        oGeneralService.Add(oGeneralData);
                                    }
                                }

                                if (paletnolistesi.Count() == paletSayisi)
                                {
                                    try
                                    {
                                        //oCompany.EndTransaction(BoWfTransOpt.wf_Commit);

                                        //oGeneralService = oCompService.GetGeneralService("AIF_WMS_SIPKAR");

                                        //oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                        //oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                        //oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(cekmeListesiOnaylama.BelgeNo));
                                        //oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                        //oGeneralData.SetProperty("U_OnayDurumu", "D");

                                        //oGeneralService.Update(oGeneralData);

                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                            }
                            catch (Exception)
                            {

                                try
                                {
                                    oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                                }
                                catch (Exception)
                                {
                                }
                                logger.Fatal("ID: " + ID + " Hata Kodu - 5200 hata oluştu. : " + oCompany.GetLastErrorDescription());

                                LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                                return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                            }
                        }
                        #endregion


                        #region Koli Tablosunu Güncelleme

                        try
                        {
                            if (cekmeListesiOnaylama.cekmeListesiOnaylamaKoliDetays.Count > 0)
                            {
                                oRS.DoQuery("Select \"DocEntry\" from \"@AIF_WMS_KOLIDTY\" as T0 where T0.\"U_BelgeNo\" = '" + cekmeListesiOnaylama.BelgeNo + "'");
                                islemvar = false;
                                guncelleme = false;

                                if (oRS.RecordCount > 0)
                                {
                                    //paletSayisi++;
                                    guncelleme = true;
                                    islemvar = true;
                                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_KOLIDTY");

                                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                                    oGeneralParams.SetProperty("DocEntry", Convert.ToInt32(oRS.Fields.Item("DocEntry").Value));
                                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                    oGeneralData.SetProperty("U_BelgeNo", Convert.ToInt32(cekmeListesiOnaylama.BelgeNo));

                                    oChildrenSatir = oGeneralData.Child("AIF_WMS_KOLIDTY1");

                                    //if (oChildrenSatir.Count > 0)
                                    //{
                                    //    //int drc = oChildrenSatir.Count;
                                    //    //for (int rmv = 0; rmv < drc; rmv++)
                                    //    //    oChildrenSatir.Remove(0);

                                    //}

                                    foreach (var itemy in cekmeListesiOnaylama.cekmeListesiOnaylamaKoliDetays)
                                    {


                                        #region satır silme palet no yazırılıp listeleme yapılması ııcın kaldırıldı
                                        //for (int z = 0; z <= oChildrenSatir.Count - 1; z++)
                                        //{
                                        //    int val = Convert.ToInt32(z);

                                        //    oChildSatir1 = oChildrenSatir.Item(val);

                                        //    var aa = oChildSatir1.GetProperty("U_SatirNo");
                                        //    var bb = oChildSatir1.GetProperty("U_SiparisNo");

                                        //    if (aa.ToString() == itemy.sapSatirNo.ToString() && bb.ToString() == itemy.siparisNumarasi)
                                        //    {
                                        //        oChildrenSatir.Remove(z);
                                        //    }
                                        //} 
                                        #endregion

                                        oChildSatir1 = oChildrenSatir.Add();

                                        oChildSatir1.SetProperty("U_SiparisNo", itemy.siparisNumarasi);
                                        oChildSatir1.SetProperty("U_SatirNo", Convert.ToInt32(itemy.sapSatirNo));
                                        oChildSatir1.SetProperty("U_KoliAdedi", Convert.ToDouble(itemy.KoliAdedi));
                                        oChildSatir1.SetProperty("U_KoliIciAdedi", Convert.ToDouble(itemy.KoliIciAdedi));
                                        oChildSatir1.SetProperty("U_ToplamMiktar", Convert.ToDouble(itemy.ToplamMiktar));
                                        oChildSatir1.SetProperty("U_PaletNo", itemy.PaletNo.ToString());
                                        if (itemy.KalemKodu != null && itemy.KalemKodu != "")
                                        {
                                            oChildSatir1.SetProperty("U_KalemKodu", itemy.KalemKodu.ToString());
                                        }
                                        oChildSatir1.SetProperty("U_Kaynak", itemy.satirKaynagi == null ? "" : itemy.satirKaynagi);

                                    }

                                }
                                else
                                {
                                    oGeneralService = oCompService.GetGeneralService("AIF_WMS_KOLIDTY");

                                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                    oGeneralData.SetProperty("U_BelgeNo", Convert.ToInt32(cekmeListesiOnaylama.BelgeNo));


                                    oChildrenSatir = oGeneralData.Child("AIF_WMS_KOLIDTY1");

                                    foreach (var itemy in cekmeListesiOnaylama.cekmeListesiOnaylamaKoliDetays)
                                    {
                                        islemvar = true;

                                        oChildSatir1 = oChildrenSatir.Add();

                                        oChildSatir1.SetProperty("U_SiparisNo", itemy.siparisNumarasi);
                                        oChildSatir1.SetProperty("U_SatirNo", Convert.ToInt32(itemy.sapSatirNo));
                                        oChildSatir1.SetProperty("U_KoliAdedi", Convert.ToDouble(itemy.KoliAdedi));
                                        oChildSatir1.SetProperty("U_KoliIciAdedi", Convert.ToDouble(itemy.KoliIciAdedi));
                                        oChildSatir1.SetProperty("U_ToplamMiktar", Convert.ToDouble(itemy.ToplamMiktar));
                                        oChildSatir1.SetProperty("U_PaletNo", itemy.PaletNo.ToString());
                                        if (itemy.KalemKodu != null && itemy.KalemKodu != "")
                                        {
                                            oChildSatir1.SetProperty("U_KalemKodu", itemy.KalemKodu.ToString());
                                        }
                                        oChildSatir1.SetProperty("U_Kaynak", itemy.satirKaynagi == null ? "" : itemy.satirKaynagi);

                                    }
                                }

                                if (islemvar)
                                {
                                    if (guncelleme)
                                    {
                                        oGeneralService.Update(oGeneralData);
                                        //oCompany.EndTransaction(BoWfTransOpt.wf_Commit);

                                    }
                                    else
                                    {
                                        oGeneralService.Add(oGeneralData);

                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            try
                            {
                                oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }
                            catch (Exception)
                            {
                            }

                            logger.Fatal("ID: " + ID + " Hata Kodu - 5200 hata oluştu. : " + oCompany.GetLastErrorDescription());

                            LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);

                            return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                        }
                        #endregion
                        try
                        {
                            if (oCompany.InTransaction)
                            {
                                oCompany.EndTransaction(BoWfTransOpt.wf_Commit);

                                #region SilinenPaletNumaralari

                                foreach (var item in silinenPaletNoListesis)
                                {

                                    string sss = "Select \"DocEntry\" from \"@AIF_WMS_PALET\" as T0 where T0.\"U_PaletNo\" = '" + item.paletNo + "' ";
                                    oRS.DoQuery(sss);

                                    int doc = Convert.ToInt32(oRS.Fields.Item(0).Value);

                                    if (doc == 0)
                                    {
                                        continue;
                                    }
                                    sss = "DELETE from \"@AIF_WMS_PALET1\" where \"DocEntry\" = '" + doc + "' and \"U_SiparisNo\" = '" + item.siparisNo + "' and \"U_SipSatirNo\" = '" + item.siparisSatirNo + "'  ";

                                    oRS.DoQuery(sss);

                                    sss = "SELECT \"U_KalemKodu\" from \"@AIF_WMS_PALET1\" where \"DocEntry\" = '" + doc + "' and \"U_SiparisNo\" = '" + item.siparisNo + "' and \"U_SipSatirNo\" = '" + item.siparisSatirNo + "'  ";


                                    oRS.DoQuery(sss);

                                    if (oRS.RecordCount == 0)
                                    {
                                        sss = "DELETE from \"@AIF_WMS_PALET\" where \"DocEntry\" = '" + doc + "' ";

                                        oRS.DoQuery(sss);
                                    }

                                    //    if (oRS.Fields.Item(0).Value.ToString() == "0")
                                    //    {
                                    //        continue;
                                    //    }

                                    //    string lineid = oRS.Fields.Item(1).Value.ToString();


                                    //    oGeneralService = oCompService.GetGeneralService("AIF_WMS_PALET");

                                    //    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                    //    oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);

                                    //    int docEntry = Convert.ToInt32(oRS.Fields.Item("DocEntry").Value);
                                    //    oGeneralParams.SetProperty("DocEntry", docEntry);

                                    //    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                                    //    if (oRS.RecordCount > 0)
                                    //    {
                                    //        oChildrenSatir.Remove(Convert.ToInt32(oRS.Fields.Item(1).Value) - 1);
                                    //    }

                                    //    oGeneralService.Update(oGeneralData);

                                    //    oRS.DoQuery("Select T1.\"U_KalemKodu\" from \"@AIF_WMS_PALET\" as T0 INNER JOIN \"@AIF_WMS_PALET1\" as T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" where T0.\"U_PaletNo\" = '" + item.paletNo + "' ");
                                    //    if (oRS.Fields.Item(0).Value.ToString() == "")
                                    //    {
                                    //        oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                                    //        oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);

                                    //        oGeneralParams.SetProperty("DocEntry", docEntry);

                                    //        oGeneralService.Delete(oGeneralParams);
                                    //    }

                                    //}


                                }
                                #endregion
                            }
                        }
                        catch (Exception)
                        {
                            oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);

                        }

                        logger.Info("ID: " + ID + " Kayıt Başarılı.");

                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = 0, Desc = "Kayıt Başarılı.", _list = null };
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
                        logger.Fatal("ID: " + ID + " Hata Kodu - 5200 hata oluştu. : " + oCompany.GetLastErrorDescription());

                        LoginCompany.ReleaseConnection(connection.number, companyDbCode, ID);
                        return new Response { Val = -5200, Desc = "Hata Kodu - 5200 hata oluştu. " + oCompany.GetLastErrorDescription(), _list = null };
                    } 

                }

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
                logger.Fatal("ID: " + ID + " Bilinmeyen hata oluştu. " + ex.ToString());

                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
                return new Response { Val = -9000, Desc = "Bilinmeyen hata oluştu. " + ex.ToString(), _list = null };
            }
            finally
            {
                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
                //try
                //{
                //    if (oCompany != null)
                //    {
                //        oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                //    }
                //}
                //catch (Exception)
                //{
                //}
            }

        }
    }
}