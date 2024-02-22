using AIF.TerminalService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{
    public class AddOrUpdateSiparisKabul
    {

        public Response addOrUpdateSiparisKabul(string dbName, string durum, string olusturan, string mKod, SiparisKabul siparisKabul)
        {
            Random rastgele = new Random();
            int ID = rastgele.Next(0, 9999);
            int clNum = 0;
            string companyDbCode = "";
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
                oCompany.StartTransaction();

                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRS2 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRS3 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRS4 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                string ItemCode = "";
                double acceptQty = 0;
                string sql = "";

                #region @RT_ORDRACP sipariş kabul general data

                SAPbobsCOM.GeneralDataParams oGeneralParams = null;

                SAPbobsCOM.GeneralService oGeneralService;

                SAPbobsCOM.GeneralData oGeneralData;

                SAPbobsCOM.CompanyService oCompService = oCompany.GetCompanyService();

                oGeneralService = oCompService.GetGeneralService("RT_ORDRACP");

                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                oGeneralParams = ((SAPbobsCOM.GeneralDataParams)(oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)));

                #region belge numarasını update için getirir
                int belgeno = 0;
                sql = "Select \"DocEntry\" from \"@RT_ORDRACP\" where \"U_OrderRefNo\" = '" + siparisKabul.siparisRefNo + "' ";

                oRS4.DoQuery(sql);

                belgeno = Convert.ToInt32(oRS4.Fields.Item("DocEntry").Value);

                #endregion

                if (belgeno == 0)
                {
                    sql = "Select MAX(\"DocEntry\") from \"@RT_ORDRACP\"";
                    oRS3.DoQuery(sql);

                    int maxxDocentry = Convert.ToInt32(oRS3.Fields.Item(0).Value) + 1;
                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                    //oGeneralData.SetProperty("DocEntry", maxxDocentry.ToString());
                    oGeneralData.SetProperty("Code", maxxDocentry.ToString());

                    oGeneralData.SetProperty("U_OrderRefNo", siparisKabul.siparisRefNo);
                    oGeneralData.SetProperty("U_WareHouse", "");
                    oGeneralData.SetProperty("U_OrderDate", siparisKabul.siparisTarihi);
                    oGeneralData.SetProperty("U_OrderAcpDate", siparisKabul.kabulTarihi);
                    oGeneralData.SetProperty("U_Branchid", siparisKabul.subeId);
                    oGeneralData.SetProperty("U_SourceWareHouse", siparisKabul.kaynakDepo);
                    oGeneralData.SetProperty("U_TargetWareHouse", siparisKabul.hedefDepo);
                    oGeneralData.SetProperty("U_Status", siparisKabul.durumNo);

                    #region belge ekli olmadığında taslak kaydetmeden direk onaylandı yapılırsa durum tamamlandı=2 yapılır
                    //if (durum == "GuncelleTaslak")
                    //{
                    //    oGeneralData.SetProperty("U_Status", "1");
                    //}

                    //if (durum == "GuncelleOnaylandi")
                    //{
                    //    oGeneralData.SetProperty("U_Status", "2");

                    //}
                    #endregion

                    SAPbobsCOM.GeneralData oChildDetay;

                    SAPbobsCOM.GeneralDataCollection oChildrenDetay;

                    oChildrenDetay = oGeneralData.Child("RT_ORDRACP1");


                    foreach (var item in siparisKabul.siparisKabulDetays)
                    {
                        oChildDetay = oChildrenDetay.Add();

                        oChildDetay.SetProperty("U_ItemCode", item.urunKodu);

                        oChildDetay.SetProperty("U_ItemName", item.urunTanimi);

                        oChildDetay.SetProperty("U_CodeBar", item.barkod);

                        oChildDetay.SetProperty("U_Quantity", item.miktar);

                        //oChildDetay.SetProperty("U_AppQuantity", item.onaylananMiktar);

                        oChildDetay.SetProperty("U_AcceptedQty", item.kabulEdilenMiktar);

                        oChildDetay.SetProperty("U_LineId", item.siraNumarasi);
                    }
                }
                else //Güncelle
                {
                    oGeneralParams.SetProperty("Code", belgeno.ToString()); //belgeno
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                    oGeneralData.SetProperty("U_WareHouse", "");
                    oGeneralData.SetProperty("U_OrderDate", siparisKabul.siparisTarihi);
                    oGeneralData.SetProperty("U_OrderAcpDate", siparisKabul.kabulTarihi);
                    oGeneralData.SetProperty("U_Branchid", siparisKabul.subeId);
                    oGeneralData.SetProperty("U_SourceWareHouse", siparisKabul.kaynakDepo);
                    oGeneralData.SetProperty("U_TargetWareHouse", siparisKabul.hedefDepo);
                    oGeneralData.SetProperty("U_Status", siparisKabul.durumNo);


                    SAPbobsCOM.GeneralData oChildDetay;

                    SAPbobsCOM.GeneralDataCollection oChildrenDetay;

                    oChildrenDetay = oGeneralData.Child("RT_ORDRACP1");


                    for (int i = oChildrenDetay.Count - 1; i >= 0; i--)
                    {
                        oChildrenDetay.Remove(i);
                    }


                    foreach (var item in siparisKabul.siparisKabulDetays)
                    {
                        oChildDetay = oChildrenDetay.Add();

                        oChildDetay.SetProperty("U_ItemCode", item.urunKodu);

                        oChildDetay.SetProperty("U_ItemName", item.urunTanimi);

                        oChildDetay.SetProperty("U_CodeBar", item.barkod);

                        oChildDetay.SetProperty("U_Quantity", item.miktar);

                        //oChildDetay.SetProperty("U_AppQuantity", item.onaylananMiktar);

                        oChildDetay.SetProperty("U_AcceptedQty", item.kabulEdilenMiktar);

                        oChildDetay.SetProperty("U_LineId", item.siraNumarasi);
                    }
                }

                try
                {
                    if (belgeno == 0)
                    {
                        oGeneralService.Add(oGeneralData);
                         
                        #region RT_ORDR kapatma işlemi
                        //RT_ORDR alanında Statüs alanı 4 e çekliecek yani kapalı yapılması gerekiyor.
                        //SAPbobsCOM.GeneralDataParams oGeneralParams_RTORDR = null;

                        //SAPbobsCOM.GeneralService oGeneralService_RTORDR;

                        //SAPbobsCOM.GeneralData oGeneralData_RTORDR;

                        //SAPbobsCOM.CompanyService oCompService_RTORDR = oCompany.GetCompanyService();

                        //oGeneralService_RTORDR = oCompService_RTORDR.GetGeneralService("RT_ORDR");

                        //oGeneralData_RTORDR = (SAPbobsCOM.GeneralData)oGeneralService_RTORDR.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                        //oGeneralParams_RTORDR = ((SAPbobsCOM.GeneralDataParams)(oGeneralService_RTORDR.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)));

                        //SAPbobsCOM.Recordset oRSRTORDR = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                        //oRSRTORDR.DoQuery("Select \"Code\" from \"@RT_ORDR\" where \"U_OrderRefNo\" = '" + siparisKabul.siparisRefNo + "'");
                        //if (oRSRTORDR.RecordCount > 0)
                        //{
                        //    oGeneralParams_RTORDR.SetProperty("Code", oRSRTORDR.Fields.Item("Code").Value.ToString()); //belgeno
                        //    oGeneralData_RTORDR = oGeneralService_RTORDR.GetByParams(oGeneralParams_RTORDR);


                        //    oGeneralData_RTORDR.SetProperty("U_Status", siparisKabul.durumNo);

                        //    oGeneralService_RTORDR.Update(oGeneralData_RTORDR);

                        //}
                        #endregion

                        //SAPbobsCOM.Recordset ors = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                        //ors.DoQuery("Select TOP 1 \"Code\" from \"@RT_ORDR\" order by \"DocEntry\" desc ");

                        //response.magazaSiparisEkleme = new MagazaSiparisEkleme();
                        //response.magazaSiparisEkleme.DocEntry = Convert.ToInt32(ors.Fields.Item(0).Value);
                        //response.magazaSiparisEkleme.siparisRefNo = magazaSiparisEkleme.subeId + "-" + response.magazaSiparisEkleme.DocEntry;
                    }
                    else
                    {
                        oGeneralService.Update(oGeneralData);
                        //response.magazaSiparisEkleme = new MagazaSiparisEkleme();
                        //response.magazaSiparisEkleme.DocEntry = Convert.ToInt32(magazaSiparisEkleme.DocEntry);
                        //response.magazaSiparisEkleme.siparisRefNo = magazaSiparisEkleme.subeId + "-" + response.magazaSiparisEkleme.DocEntry;
                    }

                }
                catch (Exception ex)
                {
                    try
                    {
                        if (oCompany.InTransaction)
                        {
                            oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                    return new Response { _list = null, Val = -1200, Desc = "MAĞAZA SİPARİŞ KABUL TABLOSUNA VERİ KAYDEDİLİRKEN HATA OLUŞTU. : " + ex.Message };

                }

                #endregion @RT_ORDRACP sipariş kabul general data

                if (durum == "GuncelleOnaylandi")
                {
                    #region oInventoryGenExit - Mal Çıkışı 

                    string kaynakDepo = siparisKabul.kaynakDepo;

                    SAPbobsCOM.Documents goodIssue = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);

                    goodIssue.DocDate = siparisKabul.kabulTarihi;

                    sql = "Select \"BPLid\" from OWHS where \"WhsCode\"='" + kaynakDepo + "'";

                    oRS.DoQuery(sql);

                    goodIssue.BPL_IDAssignedToInvoice = Convert.ToInt32(oRS.Fields.Item(0).Value);

                    int retval = 0;
                    ItemCode = "";
                    acceptQty = 0;
                    foreach (var item in siparisKabul.siparisKabulDetays)
                    {
                        ItemCode = item.urunKodu.ToString();
                        acceptQty = Convert.ToDouble((item.kabulEdilenMiktar), System.Globalization.CultureInfo.InvariantCulture);
                        if (acceptQty == 0)
                        {
                            continue;
                        }
                        goodIssue.Lines.ItemCode = ItemCode;
                        goodIssue.Lines.Quantity = acceptQty;
                        goodIssue.Lines.WarehouseCode = kaynakDepo;
                        goodIssue.Lines.Add();

                    }
                    #region KTA KAYIT
                    goodIssue.UserFields.Fields.Item("U_Olusturan").Value = olusturan;
                    #endregion

                    retval = goodIssue.Add();

                    if (retval == 0)
                    {
                        SAPbobsCOM.Recordset orSS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        int sira = 0;
                        foreach (var item in siparisKabul.siparisKabulDetays)
                        {
                            acceptQty = Convert.ToDouble((item.kabulEdilenMiktar), System.Globalization.CultureInfo.InvariantCulture);
                            if (acceptQty == 0)
                            {
                                continue;
                            }

                            goodIssue.GetByKey(Convert.ToInt32(oCompany.GetNewObjectKey()));

                            goodIssue.Lines.SetCurrentLine(sira);

                            orSS.DoQuery("Select \"StockPrice\" from \"IGE1\" as T0 where T0.\"LineNum\" = '" + goodIssue.Lines.LineNum + "' and T0.\"DocEntry\" = '" + goodIssue.DocEntry + "'");

                            item.fiyat = Convert.ToDouble(orSS.Fields.Item(0).Value);

                            sira++;

                        }

                        string getnewObjectKey = oCompany.GetNewObjectKey();

                        //return Convert.ToInt32(getnewObjectKey); ???geri dönecek mi

                        //response.istekDurumu = true;
                        //response.istekHatasi = "Mal çıkışı başarıyla oluştu.";
                    }
                    else
                    {
                        try
                        {
                            if (oCompany.InTransaction)
                            {
                                oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }
                        }
                        catch (Exception)
                        {
                        }

                        string errDesc = oCompany.GetLastErrorDescription();

                        LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                        return new Response { _list = null, Val = -1200, Desc = "GİRİŞ YAPILACAK ÜRÜN BULUNAMAMIŞTIR. : " + errDesc };

                    }

                    #endregion oInventoryGenExit - Mal Çıkışı 

                    #region oInventoryGenExit - Mal Girişi 

                    string hedefDepo = siparisKabul.hedefDepo;

                    SAPbobsCOM.Documents goodReciept = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);

                    goodReciept.DocDate = siparisKabul.kabulTarihi;

                    sql = "Select \"BPLid\" from OWHS where \"WhsCode\"='" + hedefDepo + "'";
                    oRS2.DoQuery(sql);

                    goodReciept.BPL_IDAssignedToInvoice = Convert.ToInt32(oRS2.Fields.Item(0).Value);

                    int retval2 = 0;
                    ItemCode = "";
                    acceptQty = 0;

                    foreach (var item in siparisKabul.siparisKabulDetays)
                    {
                        ItemCode = item.urunKodu.ToString();
                        acceptQty = Convert.ToDouble((item.kabulEdilenMiktar), System.Globalization.CultureInfo.InvariantCulture);
                        if (acceptQty == 0)
                        {
                            continue;
                        }
                        goodReciept.Lines.ItemCode = ItemCode;
                        goodReciept.Lines.Quantity = acceptQty;
                        goodReciept.Lines.WarehouseCode = hedefDepo;
                        goodReciept.Lines.UnitPrice = item.fiyat;
                        goodReciept.Lines.Add();

                    }
                    #region KTA KAYIT
                    goodReciept.UserFields.Fields.Item("U_Olusturan").Value = olusturan;
                    #endregion

                    retval2 = goodReciept.Add();

                    if (retval2 == 0)
                    {
                        string getnewObjectKey = oCompany.GetNewObjectKey();
                        //SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Mal Çıkışı Başarıyla Oluştu");

                        //return Convert.ToInt32(getnewObjectKey); ???geri dönecek mi

                        //response.istekDurumu = true;
                        //response.istekHatasi = "Mal girişi başarıyla oluştu.";


                        #region RT_ORDR kapatma işlemi
                        //RT_ORDR alanında Statüs alanı 4 e çekliecek yani kapalı yapılması gerekiyor.
                        SAPbobsCOM.GeneralDataParams oGeneralParams_RTORDR = null;

                        SAPbobsCOM.GeneralService oGeneralService_RTORDR;

                        SAPbobsCOM.GeneralData oGeneralData_RTORDR;

                        SAPbobsCOM.CompanyService oCompService_RTORDR = oCompany.GetCompanyService();

                        oGeneralService_RTORDR = oCompService_RTORDR.GetGeneralService("RT_ORDR");

                        oGeneralData_RTORDR = (SAPbobsCOM.GeneralData)oGeneralService_RTORDR.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                        oGeneralParams_RTORDR = ((SAPbobsCOM.GeneralDataParams)(oGeneralService_RTORDR.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)));

                        SAPbobsCOM.Recordset oRSRTORDR = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                        oRSRTORDR.DoQuery("Select \"Code\" from \"@RT_ORDR\" where \"U_OrderRefNo\" = '" + siparisKabul.siparisRefNo + "'");
                        if (oRSRTORDR.RecordCount > 0)
                        {
                            oGeneralParams_RTORDR.SetProperty("Code", oRSRTORDR.Fields.Item("Code").Value.ToString()); //belgeno
                            oGeneralData_RTORDR = oGeneralService_RTORDR.GetByParams(oGeneralParams_RTORDR);


                            oGeneralData_RTORDR.SetProperty("U_Status", "4");

                            oGeneralService_RTORDR.Update(oGeneralData_RTORDR);

                        }
                        #endregion
                    }
                    else
                    {
                        try
                        {
                            if (oCompany.InTransaction)
                            {
                                oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }
                        }
                        catch (Exception)
                        {
                        }

                        string errDesc = oCompany.GetLastErrorDescription();

                        LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                        return new Response { _list = null, Val = -1200, Desc = "MAL GİRİŞİ OLUŞTURULURKEN HATA OLUŞTU. : " + oCompany.GetLastErrorDescription() };

                    }

                    #endregion oInventoryGenExit - Mal Girişi 

                }

                try
                {
                    if (oCompany.InTransaction)
                    {
                        oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    }
                }
                catch (Exception)
                {
                }

                LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                return new Response { _list = null, Val = 0, Desc = "KAYIT BAŞARILI." };


            }
            catch (Exception ex)
            {
                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
                return new Response { _list = null, Val = -999, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
            }
            finally
            {
                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
            }

        }

    }
}