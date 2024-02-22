using AIF.TerminalService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{
    public class AddOrUpdateGoodRecieptPO
    {
        public Response addOrUpdateGoodRecieptPO(string dbName, int kullaniciId, GoodRecieptPO goodRecieptPO)
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
                SAPbobsCOM.Documents oDocumentsOrders = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);

                SAPbobsCOM.Documents oDocuments = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes);

                oDocuments.CardCode = goodRecieptPO.CarCode;

                oDocuments.NumAtCard = goodRecieptPO.WayBillNo;

                DateTime dt = new DateTime(Convert.ToInt32(goodRecieptPO.DocDate.Substring(0, 4)), Convert.ToInt32(goodRecieptPO.DocDate.Substring(4, 2)), Convert.ToInt32(goodRecieptPO.DocDate.Substring(6, 2)));

                oDocuments.DocDate = dt;

                dt = new DateTime(Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(0, 4)), Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(4, 2)), Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(6, 2)));

                oDocuments.DocDueDate = dt;

                int i = 0;
                int depoyerisirasi = 0;

                foreach (var item in goodRecieptPO.GoodRecieptPODetails)
                {
                    //Satınalma siparişine bağlı satınalma siparişli mal girişi oluşturmaya yarar.
                    if (item.BaseEntry != null && item.BaseEntry != 0)
                    {
                        oDocumentsOrders.GetByKey(item.BaseEntry);
                        oDocuments.Lines.BaseEntry = item.BaseEntry;
                        oDocuments.Lines.BaseType = item.BaseType;
                        oDocuments.Lines.BaseLine = item.BaseLine;
                    }

                    oDocuments.Lines.ItemCode = item.ItemCode;

                    oDocuments.Lines.Quantity = item.Quantity;

                    if (item.WareHouse != "" && item.WareHouse != null)
                    {
                        oDocuments.Lines.WarehouseCode = item.WareHouse;
                    }

                    if (item.BinCode != null && item.BinCode != "")
                    {
                        oDocuments.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
                        oDocuments.Lines.BinAllocations.BinAbsEntry = Convert.ToInt32(item.BinCode);
                        oDocuments.Lines.BinAllocations.Quantity = Convert.ToDouble(item.Quantity);
                        oDocuments.Lines.BinAllocations.Add();
                    }
                    else if (item.BatchLists != null && item.BatchLists.Count() > 0)
                    {
                        //SATINALMA SİPARİŞLİ MAL GİRİŞİ EKRANINDA PARTİDE SEÇİLEN DEPO YERLERİ GETİRİLİR.
                        foreach (var item_BinCode in item.BatchLists.Where(x => x.BinCode != null && x.BinCode != ""))
                        {
                            oDocuments.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
                            oDocuments.Lines.BinAllocations.BinAbsEntry = Convert.ToInt32(item_BinCode.BinCode);
                            oDocuments.Lines.BinAllocations.Quantity = Convert.ToDouble(item_BinCode.BatchQuantity);
                            oDocuments.Lines.BinAllocations.Add();
                            i++;
                        }
                    }

                    i = 0;
                    foreach (var aifteam in item.BatchLists)
                    {
                        oDocuments.Lines.BatchNumbers.Add();
                        oDocuments.Lines.BatchNumbers.SetCurrentLine(i);
                        oDocuments.Lines.BatchNumbers.BatchNumber = aifteam.BatchNumber;
                        oDocuments.Lines.BatchNumbers.Quantity = aifteam.BatchQuantity;
                        i++;
                    }

                    oDocuments.Lines.Add();

                    i = 0;
                }

                #region şube için temel belgedeki şube tayin edilir

                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                oRS.DoQuery("Select \"BPLId\" from \"OBPL\""); //Eğer SAP şubeli bir yapıda ise ona göre işlem yapılıyor. OBPL tablosu Şube tablosudur. Kayıt varsa şubeli yapıda olduğunu gösterir.
                if (oRS.RecordCount > 0)
                {
                    oDocuments.Lines.SetCurrentLine(0);
                    if (oDocuments.Lines.BaseEntry == null || oDocuments.Lines.BaseEntry == 0)
                    {
                        oRS.DoQuery("Select BPLid from OWHS where WhsCode = '" + goodRecieptPO.GoodRecieptPODetails[0].WareHouse + "' "); //D21100//Eğer SAP şubeli bir yapıda ise ona göre işlem yapılıyor. owhs tablosu Şube tablosudur. Kayıt varsa şubeli yapıda olduğunu gösterir.
                        if (oRS.RecordCount > 0)
                        {
                            int sube = Convert.ToInt32(oRS.Fields.Item("BPLid").Value);

                            oDocuments.BPL_IDAssignedToInvoice = sube;
                            //oDocuments.BPL_IDAssignedToInvoice = oDocumentsOrders.BPL_IDAssignedToInvoice;
                        }
                    }
                    else
                    {
                        oDocuments.BPL_IDAssignedToInvoice = oDocumentsOrders.BPL_IDAssignedToInvoice;
                    }
                }
                else
                {
                    oDocuments.BPL_IDAssignedToInvoice = 1;
                }

                #endregion şube için temel belgedeki şube tayin edilir

                #region kullanıcı id

                oDocuments.UserFields.Fields.Item("U_T_KullaniciId").Value = kullaniciId;

                #endregion kullanıcı id

                int retval = oDocuments.Add();

                if (retval != 0)
                {
                    LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                    return new Response { _list = null, Val = -1200, Desc = "HATA KODU : -1200 HATA AÇIKLAMASI : " + oCompany.GetLastErrorDescription() };
                }
                else
                {
                    LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                    return new Response { _list = null, Val = 0, Desc = "KAYIT BAŞARILI." };
                }
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

        public Response addOrUpdateGoodRecieptPOFromOrder(string dbName, GoodRecieptPO goodRecieptPO)
        {
            Random rastgele = new Random();
            int ID = rastgele.Next(0, 9999);
            int clNum = 0;
            string companyDbCode = "";
            try
            {
                ConnectionList connection = new ConnectionList();

                LoginCompany log = new LoginCompany();

                log.DisconnectSAP(dbName);
                connection = log.getSAPConnection(dbName, ID);
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

                SAPbobsCOM.Documents oDocuments = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes);

                oDocuments.CardCode = goodRecieptPO.CarCode;

                DateTime dt = new DateTime(Convert.ToInt32(goodRecieptPO.DocDate.Substring(0, 4)), Convert.ToInt32(goodRecieptPO.DocDate.Substring(4, 2)), Convert.ToInt32(goodRecieptPO.DocDate.Substring(6, 2)));

                oDocuments.DocDate = dt;

                dt = new DateTime(Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(0, 4)), Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(4, 2)), Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(6, 2)));

                oDocuments.DocDueDate = dt;

                oDocuments.NumAtCard = goodRecieptPO.WayBillNo;

                oDocuments.BPL_IDAssignedToInvoice = 1;
                int i = 0;
                foreach (var item in goodRecieptPO.GoodRecieptPODetails)
                {
                    oDocuments.Lines.ItemCode = item.ItemCode;

                    oDocuments.Lines.Quantity = item.Quantity;

                    foreach (var aifteam in item.BatchLists)
                    {
                        oDocuments.Lines.BatchNumbers.Add();
                        oDocuments.Lines.BatchNumbers.SetCurrentLine(i);
                        oDocuments.Lines.BatchNumbers.BatchNumber = aifteam.BatchNumber;
                        oDocuments.Lines.BatchNumbers.Quantity = aifteam.BatchQuantity;
                        i++;
                    }

                    oDocuments.Lines.Add();

                    i = 0;
                }

                int retval = oDocuments.Add();

                if (retval != 0)
                {
                    LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                    return new Response { _list = null, Val = -1200, Desc = "HATA KODU : -1200 HATA AÇIKLAMASI : " + oCompany.GetLastErrorDescription() };
                }
                else
                {
                    LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                    return new Response { _list = null, Val = 0, Desc = "KAYIT BAŞARILI." };
                }
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

        public Response addOrUpdateGoodRecieptPODraft(string dbName, GoodRecieptPO goodRecieptPO)
        {
            Random rastgele = new Random();
            int ID = rastgele.Next(0, 9999);
            int clNum = 0;
            string companyDbCode = "";
            try
            {
                ConnectionList connection = new ConnectionList();

                LoginCompany log = new LoginCompany();
                log.DisconnectSAP(dbName);
                connection = log.getSAPConnection(dbName, ID);
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

                SAPbobsCOM.Documents oDocuments = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                oDocuments.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes;
                //böyle olumadı.yok böyledir diyorsan başka hata vardır
                oDocuments.CardCode = goodRecieptPO.CarCode;

                oDocuments.NumAtCard = goodRecieptPO.WayBillNo;

                DateTime dt = new DateTime(Convert.ToInt32(goodRecieptPO.TaxDate.Substring(0, 4)), Convert.ToInt32(goodRecieptPO.TaxDate.Substring(4, 2)), Convert.ToInt32(goodRecieptPO.TaxDate.Substring(6, 2)));

                oDocuments.TaxDate = dt;

                //dt = new DateTime(Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(0, 4)), Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(4, 2)), Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(6, 2)));

                //oDocuments.DocDueDate = dt;

                //oDocuments.BPL_IDAssignedToInvoice = 1;
                int i = 0;
                foreach (var item in goodRecieptPO.GoodRecieptPODetails)
                {
                    //Satınalma siparişine bağlı satınalma siparişli mal girişi oluşturmaya yarar.
                    //if (item.BaseEntry != null && item.BaseEntry != 0)
                    //{
                    //    oDocuments.Lines.BaseEntry = item.BaseEntry;
                    //    oDocuments.Lines.BaseType = item.BaseType;
                    //    oDocuments.Lines.BaseLine = item.BaseLine;
                    //}

                    oDocuments.Lines.ItemCode = item.ItemCode;

                    oDocuments.Lines.Quantity = item.Quantity;

                    if (item.WareHouse != "" && item.WareHouse != null)
                    {
                        oDocuments.Lines.WarehouseCode = item.WareHouse;
                    }

                    foreach (var aifteam in item.BatchLists)
                    {
                        oDocuments.Lines.BatchNumbers.Add();
                        oDocuments.Lines.BatchNumbers.SetCurrentLine(i);
                        oDocuments.Lines.BatchNumbers.BatchNumber = aifteam.BatchNumber;
                        oDocuments.Lines.BatchNumbers.Quantity = aifteam.BatchQuantity;
                        i++;
                    }

                    oDocuments.Lines.Add();

                    i = 0;
                }

                int retval = oDocuments.Add();

                if (retval != 0)
                {
                    LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                    return new Response { _list = null, Val = -1200, Desc = "HATA KODU : -1200 HATA AÇIKLAMASI : " + oCompany.GetLastErrorDescription() };
                }
                else
                {
                    LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                    return new Response { _list = null, Val = 0, Desc = "KAYIT BAŞARILI." };
                }
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