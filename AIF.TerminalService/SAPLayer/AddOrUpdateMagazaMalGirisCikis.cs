using AIF.TerminalService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{
    public class AddOrUpdateMagazaMalGirisCikis
    {

        public Response addOrUpdateMagazaMalGirisCikis(string dbName, string subeId, string mKod, string olusturan, MalGirisCikis malGirisCikis)
        {
            Random rastgele = new Random();
            int ID = rastgele.Next(0, 9999);
            int clNum = 0;
            string companyDbCode = "";
            SAPbobsCOM.Company oCompany = null;
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
                oCompany = connection.oCompany;
                oCompany.StartTransaction();


                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRS2 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRS3 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                string ItemCode = "";
                double acceptQty = 0;
                string sql = "";
                string varsayilanDepo = "";

                #region şube varsayılan deposu

                sql = "Select \"DflWhs\"from OBPL where \"Disabled\" = 'N' and \"BPLId\"='" + subeId + "'";
                oRS3.DoQuery(sql);


                if (oRS3.RecordCount > 0)
                {
                    varsayilanDepo = oRS3.Fields.Item("DflWhs").Value.ToString();
                }
                #endregion şube varsayılan deposu

                #region oInventoryGenExit - Mal Girişi 

                if (malGirisCikis.girisMiCikisMi == "Mağaza Giriş")
                {
                    //string hedefDepo = siparisKabul.hedefDepo;

                    SAPbobsCOM.Documents goodReciept = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);

                    goodReciept.DocDate = malGirisCikis.tarih;

                    //sql = "Select \"BPLid\" from OWHS where \"WhsCode\"='" + hedefDepo + "'";
                    //oRS2.DoQuery(sql);

                    //goodReciept.BPL_IDAssignedToInvoice = Convert.ToInt32(oRS2.Fields.Item(0).Value);

                    goodReciept.BPL_IDAssignedToInvoice = Convert.ToInt32(subeId);
                    goodReciept.Reference2 = malGirisCikis.malCikisNo;

                    int retval2 = 0;
                    ItemCode = "";
                    acceptQty = 0;

                    foreach (var item in malGirisCikis.malGirisCikisDetays)
                    {
                        ItemCode = item.urunKodu.ToString();
                        acceptQty = Convert.ToDouble((item.miktar), System.Globalization.CultureInfo.InvariantCulture);
                        if (acceptQty == 0)
                        {
                            continue;
                        }
                        goodReciept.Lines.ItemCode = ItemCode;
                        goodReciept.Lines.Quantity = acceptQty;
                        goodReciept.Lines.WarehouseCode = varsayilanDepo;
                        goodReciept.Lines.Add();

                    }

                    #region KTA KAYIT
                    goodReciept.UserFields.Fields.Item("U_AliciAdi").Value = malGirisCikis.aliciAdi;
                    goodReciept.UserFields.Fields.Item("U_Adres").Value = malGirisCikis.aliciAdres;
                    goodReciept.UserFields.Fields.Item("U_City").Value = malGirisCikis.ilAdi;
                    goodReciept.UserFields.Fields.Item("U_County").Value = malGirisCikis.ilceAdi;
                    goodReciept.UserFields.Fields.Item("U_ZipCode").Value = malGirisCikis.postaKodu;
                    goodReciept.UserFields.Fields.Item("U_Olusturan").Value = olusturan;
                    goodReciept.UserFields.Fields.Item("U_AliciSubeId").Value = malGirisCikis.aliciSubeId;

                    #endregion

                    retval2 = goodReciept.Add();

                    if (retval2 == 0)
                    {
                        string getnewObjectKey = oCompany.GetNewObjectKey();
                        //SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Mal Çıkışı Başarıyla Oluştu");

                        //return Convert.ToInt32(getnewObjectKey);

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
                        return new Response { _list = null, Val = 0, Desc = "MAL GİRİŞİ BAŞARILI." };

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
                        return new Response { _list = null, Val = -1200, Desc = "MAL GİRİŞİ OLUŞTURULURKEN HATA OLUŞTU : " + oCompany.GetLastErrorDescription() };

                    }
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
                    LoginCompany.ReleaseConnection(connection.number, connection.dbCode, ID);
                    return new Response { _list = null, Val = 0, Desc = "İŞLEM BAŞARISIZ." };
                }

              
                #endregion oInventoryGenExit - Mal Girişi  

            }
            catch (Exception ex)
            {
                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
                return new Response { _list = null, Val = -999, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
            }
            finally
            {
                if (oCompany.InTransaction)
                {
                    oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                }
                LoginCompany.ReleaseConnection(clNum, companyDbCode, ID);
            }

        }

    }
}