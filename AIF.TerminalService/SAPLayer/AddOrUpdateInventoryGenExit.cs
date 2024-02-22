using AIF.TerminalService.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{
    public class AddOrUpdateInventoryGenExit
    {
        public Response addOrUpdateInventoryGenExit(string dbName, int kullaniciId, InventoryGenExit inventoryGenExit)
        {
            Random rastgele = new Random();
            int ID = rastgele.Next(0, 9999);
            Logger logger = LogManager.GetCurrentClassLogger();

            //var requestJson_New = JsonConvert.SerializeObject(protocol);

            //logger.Info(" ");

            logger.Info("ID: " + ID + " addOrUpdateInventoryGenExit Servisine Geldi.");
            //logger.Info("ID: " + ID + " ISTEK :" + requestJson_New);

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
                    logger.Fatal("ID: " + ID + " " + "Hata Kodu - 3100 Veritabanı bağlantısı sırasında hata oluştu.");
                    return new Response { Val = -3100, Desc = "Hata Kodu - 3100 Veritabanı bağlantısı sırasında hata oluştu. ", _list = null };
                }

                clNum = connection.number;
                companyDbCode = connection.dbCode;
                SAPbobsCOM.Company oCompany = connection.oCompany;

                logger.Info("ID: " + ID + " Şirket bağlantısını başarıyla geçtik. Bağlantı sağladığımız DB :" + oCompany.CompanyDB + " clnum: " + clNum);

                SAPbobsCOM.Documents oDocuments = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);

                DateTime dt = new DateTime(Convert.ToInt32(inventoryGenExit.DocDate.Substring(0, 4)), Convert.ToInt32(inventoryGenExit.DocDate.Substring(4, 2)), Convert.ToInt32(inventoryGenExit.DocDate.Substring(6, 2)));

                oDocuments.DocDate = dt;
                //oDocuments.DocDueDate = dt;
                oDocuments.Comments = inventoryGenExit.Comments;
                //oDocuments.CardCode = inventoryGenEntry.CardCode;

                //dt = new DateTime(Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(0, 4)), Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(4, 2)), Convert.ToInt32(goodRecieptPO.DocDueDate.Substring(6, 2)));

                //oDocuments.DocDueDate = dt;

                //oDocuments.BPL_IDAssignedToInvoice = 1;
                int i = 0;
                foreach (var item in inventoryGenExit.InventoryGenExitLines)
                {
                    //Satınalma siparişine bağlı satınalma siparişli mal girişi oluşturmaya yarar.
                    //if (item.BaseEntry != null && item.BaseEntry != 0)
                    //{
                    //    oDocuments.Lines.BaseEntry = item.BaseEntry;
                    //    oDocuments.Lines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest;
                    //    oDocuments.Lines.BaseLine = item.BaseLine;
                    //}
                    //else
                    //{
                    oDocuments.Lines.ItemCode = item.ItemCode;
                    //}

                    oDocuments.Lines.Quantity = item.Quantity;

                    if (item.WareHouse != null && item.WareHouse != "")
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
                    else if (item.InventoryGenExitLinesBatch != null && item.InventoryGenExitLinesBatch.Count() > 0)
                    {
                        //BELGESİZ MAL GİRİŞİ EKRANINDA PARTİDE SEÇİLEN DEPO YERLERİ GETİRİLİR.
                        foreach (var item_BinCode in item.InventoryGenExitLinesBatch.Where(x => x.DepoYeriId != ""))
                        {
                            oDocuments.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
                            oDocuments.Lines.BinAllocations.BinAbsEntry = Convert.ToInt32(item_BinCode.DepoYeriId);
                            oDocuments.Lines.BinAllocations.Quantity = Convert.ToDouble(item_BinCode.BatchQuantity);
                            oDocuments.Lines.BinAllocations.Add();
                            i++;
                        }
                    }

                    foreach (var aifteam in item.InventoryGenExitLinesBatch)
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

                oRS.DoQuery("Select BPLid from OWHS where WhsCode = '" + inventoryGenExit.InventoryGenExitLines[0].WareHouse + "' "); //D21100//Eğer SAP şubeli bir yapıda ise ona göre işlem yapılıyor. OBPL tablosu Şube tablosudur. Kayıt varsa şubeli yapıda olduğunu gösterir.
                if (oRS.RecordCount > 0)
                {
                    int sube = Convert.ToInt32(oRS.Fields.Item("BPLid").Value);

                    oDocuments.BPL_IDAssignedToInvoice = sube;
                    //oDocuments.BPL_IDAssignedToInvoice = oDocumentsOrders.BPL_IDAssignedToInvoice;
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
                    logger.Fatal("ID: " + ID + " HATA KODU : -1200 HATA AÇIKLAMASI : " + oCompany.GetLastErrorDescription());
                    LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                    return new Response { _list = null, Val = -1200, Desc = "HATA KODU : -1200 HATA AÇIKLAMASI : " + oCompany.GetLastErrorDescription() };
                }
                else
                {
                    logger.Info("ID: " + ID + " KAYIT BAŞARILI.");
                    LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                    return new Response { _list = null, Val = 0, Desc = "KAYIT BAŞARILI." };
                }
            }
            catch (Exception ex)
            {
                logger.Fatal("ID: " + ID + " BİLİNMEYEN HATA OLUŞTU." + ex.Message);
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