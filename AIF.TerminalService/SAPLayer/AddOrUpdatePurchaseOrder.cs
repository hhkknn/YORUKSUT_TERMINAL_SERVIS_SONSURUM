using AIF.TerminalService.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{
    public class AddOrUpdatePurchaseOrder
    {
        public Response addOrUpdatePurchaseOrder(string dbName, int kullaniciId,PurchaseOrders purchaseOrders)
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

                connection = log.getSAPConnection(dbName,ID);

                if (connection.number == -1)
                {
                    return new Response { Val = -3100, Desc = "Hata Kodu - 3100 Veritabanı bağlantısı sırasında hata oluştu. ", _list = null };
                }

                clNum = connection.number;
                companyDbCode = connection.dbCode;
                SAPbobsCOM.Company oCompany = connection.oCompany;


                SAPbobsCOM.Documents oDocuments = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);

                oDocuments.CardCode = purchaseOrders.CarCode;

                DateTime dt = new DateTime(Convert.ToInt32(purchaseOrders.DocDate.Substring(0, 4)), Convert.ToInt32(purchaseOrders.DocDate.Substring(4, 2)), Convert.ToInt32(purchaseOrders.DocDate.Substring(6, 2)));

                oDocuments.DocDate = dt;


                dt = new DateTime(Convert.ToInt32(purchaseOrders.DocDueDate.Substring(0, 4)), Convert.ToInt32(purchaseOrders.DocDueDate.Substring(4, 2)), Convert.ToInt32(purchaseOrders.DocDueDate.Substring(6, 2)));

                oDocuments.DocDueDate = dt;

                oDocuments.BPL_IDAssignedToInvoice = 1;

                foreach (var item in purchaseOrders.PurchaseOrdersDetails)
                {
                    oDocuments.Lines.ItemCode = item.ItemCode;

                    oDocuments.Lines.Quantity = item.Quantity;

                    oDocuments.Lines.Add();
                }


                #region kullanıcı id
                oDocuments.UserFields.Fields.Item("U_T_KullaniciId").Value = kullaniciId;
                #endregion


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