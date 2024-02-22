using AIF.TerminalService.DatabaseLayer;
using AIF.TerminalService.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace AIF.TerminalService.SAPLayer
{
    public class LoginCompany
    {
        public static SAPbobsCOM.Company oCompany;
        public static SAPbobsCOM.Company oCompany1;
        public static SAPbobsCOM.Company oCompany2;
        public static SAPbobsCOM.Company oCompany3;
        public static SAPbobsCOM.Company oCompany4;
        public static SAPbobsCOM.Company oCompanyGecici { get; set; }

        #region connect old
        //public Response Connect(string dbName, string mKod)
        //{
        //    DataTable dt = new DataTable();
        //    DataTable dtSirketBilgileri = new DataTable();
        //    try
        //    {
        //        GetConnectionString n = new GetConnectionString();
        //        string connstring = n.getConnectionString(dbName, mKod);

        //        if (connstring != "")
        //        {

        //            if (dt.Rows.Count > 0)
        //            {
        //                string query = "Select TOP 1 * from \"@AIF_WMS_CONSTRNG\"";

        //                using (SqlConnection con = new SqlConnection(connstring))
        //                {
        //                    using (SqlCommand cmd = new SqlCommand(query, con))
        //                    {
        //                        cmd.CommandType = CommandType.Text;
        //                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
        //                        {
        //                            using (dtSirketBilgileri = new DataTable())
        //                            {
        //                                sda.Fill(dtSirketBilgileri);
        //                                dtSirketBilgileri.TableName = "SirketBilgileri";

        //                                if (dtSirketBilgileri.Rows.Count > 0)
        //                                {
        //                                    ConnectCompany(dtSirketBilgileri, mKod);
        //                                }
        //                                else
        //                                {
        //                                    return new Response { _list = null, Val = -999, Desc = "SAP GİRİŞİ SAĞLANAMADI." };
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return new Response { _list = null, Val = -777, Desc = dbName + " İÇİN BAĞLANTI BİLGİSİ BULUNAMADI." };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return new Response { _list = dt, Val = 0 };
        //} 
        #endregion

        //DataTable dtSirketBilgileri = new DataTable();
        public Response Connect(string userName, string password, string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            DataTable dtSirketBilgileri = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    //var query = "Select \"USER_CODE\" from \"OUSR\"  where \"USER_CODE\"= '" + userName + "' and \"E_Mail\" = '" + password + "'";

                    string query = "";


                    if (mKod != "70TRMN")
                    {
                        query = "select Cast(\"Code\" as varchar(10)) as \"Kod\",\"ExtEmpNo\" as \"Pass\",\"firstName\" as Ad,\"lastName\" as Soyad from OHEM where Cast(\"empID\" as varchar(10)) = '" + userName + "' and \"ExtEmpNo\"='" + password + "' and \"Active\" = 'Y' "; //empID eşleştirmesi yerine Code yapıldı. 
                    }
                    else
                    {
                        query = "select Cast(\"empID\" as varchar(10)) as \"Kod\",\"ExtEmpNo\" as \"Pass\",\"firstName\" as Ad,\"lastName\" as Soyad from OHEM where Cast(\"empID\" as varchar(10)) = '" + userName + "' and \"ExtEmpNo\"='" + password + "' and \"Active\" = 'Y' "; //empID eşleştirmesi yerine Code yapıldı. 

                    }
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "userList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -888, Desc = "KULLANICI ADI VEYA ŞİFRE HATALI!" };
                                        }
                                    }
                                }
                            }
                        }

                        if (dt.Rows.Count > 0)
                        {
                            query = "Select TOP 1 * from \"@AIF_WMS_CONSTRNG\"";

                            using (SqlConnection con = new SqlConnection(connstring))
                            {
                                using (SqlCommand cmd = new SqlCommand(query, con))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                    {
                                        using (dtSirketBilgileri = new DataTable())
                                        {
                                            sda.Fill(dtSirketBilgileri);


                                            dtSirketBilgileri.TableName = "SirketBilgileri";


                                            if (dtSirketBilgileri.Rows.Count > 0)
                                            {
                                                ConnectCompany(dtSirketBilgileri, mKod);
                                            }
                                            else
                                            {
                                                return new Response { _list = null, Val = -999, Desc = "SAP GİRİŞİ SAĞLANAMADI." };
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    return new Response { _list = null, Val = -777, Desc = dbName + " İÇİN BAĞLANTI BİLGİSİ BULUNAMADI." };
                }
            }
            catch (Exception ex)
            {
            }
            return new Response { _list = dt, Val = 0 };
        }

        public void ConnectCompany(DataTable _dtSirketBilgileri, string mKod, int sayi = -1)
        {
            string dbCode = _dtSirketBilgileri.Rows[0]["U_CompanyDBCode"].ToString();

            string GUID = Guid.NewGuid().ToString();

            Logger logger = LogManager.GetCurrentClassLogger();

            logger.Info("ID: " + sayi + " " + "ConnectCompany methoduna geldi DbCode : " + dbCode);

            string errDesc = "";
            int Connect = Connlist.Where(x => x.dbCode == _dtSirketBilgileri.Rows[0]["U_CompanyDBCode"].ToString()).Count();
            if (Connect == 0)
            {
                //SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
                oCompany = new SAPbobsCOM.Company();
                oCompany1 = new SAPbobsCOM.Company();
                oCompany2 = new SAPbobsCOM.Company();
                oCompany3 = new SAPbobsCOM.Company();
                oCompany4 = new SAPbobsCOM.Company();

                //Nedenini tam bilemedim henüz ama Anatolya için oCompany0.SLDServer satırını açmamız gerekiyor ama açarsak otat çalışmayacak gibi geliyor o yüzden comittimde kapatıyorum o satırları.

                oCompany.LicenseServer = _dtSirketBilgileri.Rows[0]["U_LicenseServer"].ToString();
                if (mKod == "30TRMN" || mKod == "70TRMN")
                {
                    oCompany.SLDServer = _dtSirketBilgileri.Rows[0]["U_LicenseServer"].ToString();
                }
                oCompany.Server = _dtSirketBilgileri.Rows[0]["U_Server"].ToString();
                oCompany.UserName = _dtSirketBilgileri.Rows[0]["U_UserName"].ToString();
                oCompany.Password = _dtSirketBilgileri.Rows[0]["U_Password"].ToString();
                oCompany.CompanyDB = _dtSirketBilgileri.Rows[0]["U_CompanyDBCode"].ToString();
                oCompany.DbServerType = (SAPbobsCOM.BoDataServerTypes)Convert.ToInt32(_dtSirketBilgileri.Rows[0]["U_DbServerType"].ToString());
                oCompany.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;
                oCompany.Connect();

                logger.Info("ID: " + sayi + " " + "oCompany Connect Edildi. Bağlantı Durumu : " + oCompany.Connected + "." + "Bağlantı Hatası: " + oCompany.GetLastErrorDescription());

                _connlist.Add(new ConnectionList { oCompany = oCompany, isAvailable = oCompany.Connected ? "0" : "1", error = oCompany.GetLastErrorDescription(), number = 1, isConnected = oCompany.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID, pass = _dtSirketBilgileri.Rows[0]["U_Password"].ToString() });

                logger.Info("ID: " + sayi + " " + "oCompany connliste eklendi.");

                GUID = Guid.NewGuid().ToString();

                oCompany1.LicenseServer = _dtSirketBilgileri.Rows[0]["U_LicenseServer"].ToString();
                if (mKod == "30TRMN" || mKod == "70TRMN")
                {
                    oCompany1.SLDServer = _dtSirketBilgileri.Rows[0]["U_LicenseServer"].ToString();
                }
                oCompany1.Server = _dtSirketBilgileri.Rows[0]["U_Server"].ToString();
                oCompany1.UserName = _dtSirketBilgileri.Rows[0]["U_UserName"].ToString();
                oCompany1.Password = _dtSirketBilgileri.Rows[0]["U_Password"].ToString();
                oCompany1.CompanyDB = _dtSirketBilgileri.Rows[0]["U_CompanyDBCode"].ToString();
                oCompany1.DbServerType = (SAPbobsCOM.BoDataServerTypes)Convert.ToInt32(_dtSirketBilgileri.Rows[0]["U_DbServerType"].ToString());
                oCompany1.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;
                oCompany1.Connect();

                logger.Info("ID: " + sayi + " " + "oCompany1 Connect Edildi. Bağlantı Durumu : " + oCompany1.Connected + "." + "Bağlantı Hatası: " + oCompany1.GetLastErrorDescription());

                _connlist.Add(new ConnectionList { oCompany = oCompany1, isAvailable = oCompany1.Connected ? "0" : "1", error = oCompany1.GetLastErrorDescription(), number = 2, isConnected = oCompany1.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID, pass = _dtSirketBilgileri.Rows[0]["U_Password"].ToString() });

                logger.Info("ID: " + sayi + " " + "oCompany1 connliste eklendi.");


                GUID = Guid.NewGuid().ToString();

                oCompany2.LicenseServer = _dtSirketBilgileri.Rows[0]["U_LicenseServer"].ToString();
                if (mKod == "30TRMN" || mKod == "70TRMN")
                {
                    oCompany2.SLDServer = _dtSirketBilgileri.Rows[0]["U_LicenseServer"].ToString();
                }
                oCompany2.Server = _dtSirketBilgileri.Rows[0]["U_Server"].ToString();
                oCompany2.UserName = _dtSirketBilgileri.Rows[0]["U_UserName"].ToString();
                oCompany2.Password = _dtSirketBilgileri.Rows[0]["U_Password"].ToString();
                oCompany2.CompanyDB = _dtSirketBilgileri.Rows[0]["U_CompanyDBCode"].ToString();
                oCompany2.DbServerType = (SAPbobsCOM.BoDataServerTypes)Convert.ToInt32(_dtSirketBilgileri.Rows[0]["U_DbServerType"].ToString());
                oCompany2.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;
                oCompany2.Connect();

                logger.Info("ID: " + sayi + " " + "oCompany2 Connect Edildi. Bağlantı Durumu : " + oCompany2.Connected + "." + "Bağlantı Hatası: " + oCompany2.GetLastErrorDescription());

                _connlist.Add(new ConnectionList { oCompany = oCompany2, isAvailable = oCompany2.Connected ? "0" : "1", error = oCompany2.GetLastErrorDescription(), number = 3, isConnected = oCompany2.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID, pass = _dtSirketBilgileri.Rows[0]["U_Password"].ToString() });

                logger.Info("ID: " + sayi + " " + "oCompany2 connliste eklendi.");

                GUID = Guid.NewGuid().ToString();

                oCompany3.LicenseServer = _dtSirketBilgileri.Rows[0]["U_LicenseServer"].ToString();
                if (mKod == "30TRMN" || mKod == "70TRMN")
                {
                    oCompany3.SLDServer = _dtSirketBilgileri.Rows[0]["U_LicenseServer"].ToString();
                }
                oCompany3.Server = _dtSirketBilgileri.Rows[0]["U_Server"].ToString();
                oCompany3.UserName = _dtSirketBilgileri.Rows[0]["U_UserName"].ToString();
                oCompany3.Password = _dtSirketBilgileri.Rows[0]["U_Password"].ToString();
                oCompany3.CompanyDB = _dtSirketBilgileri.Rows[0]["U_CompanyDBCode"].ToString();
                oCompany3.DbServerType = (SAPbobsCOM.BoDataServerTypes)Convert.ToInt32(_dtSirketBilgileri.Rows[0]["U_DbServerType"].ToString());
                oCompany3.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;
                oCompany3.Connect();

                logger.Info("ID: " + sayi + " " + "oCompany3 Connect Edildi. Bağlantı Durumu : " + oCompany3.Connected + "." + "Bağlantı Hatası: " + oCompany3.GetLastErrorDescription());

                _connlist.Add(new ConnectionList { oCompany = oCompany3, isAvailable = oCompany3.Connected ? "0" : "1", error = oCompany3.GetLastErrorDescription(), number = 4, isConnected = oCompany3.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID, pass = _dtSirketBilgileri.Rows[0]["U_Password"].ToString() });

                logger.Info("ID: " + sayi + " " + "oCompany3 connliste eklendi.");

                GUID = Guid.NewGuid().ToString();

                oCompany4.LicenseServer = _dtSirketBilgileri.Rows[0]["U_LicenseServer"].ToString();
                if (mKod == "30TRMN" ||mKod== "70TRMN")
                {
                    oCompany4.SLDServer = _dtSirketBilgileri.Rows[0]["U_LicenseServer"].ToString();
                }
                oCompany4.Server = _dtSirketBilgileri.Rows[0]["U_Server"].ToString();
                oCompany4.UserName = _dtSirketBilgileri.Rows[0]["U_UserName"].ToString();
                oCompany4.Password = _dtSirketBilgileri.Rows[0]["U_Password"].ToString();
                oCompany4.CompanyDB = _dtSirketBilgileri.Rows[0]["U_CompanyDBCode"].ToString();
                oCompany4.DbServerType = (SAPbobsCOM.BoDataServerTypes)Convert.ToInt32(_dtSirketBilgileri.Rows[0]["U_DbServerType"].ToString());
                oCompany4.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;
                oCompany4.Connect();

                logger.Info("ID: " + sayi + " " + "oCompany4 Connect Edildi. Bağlantı Durumu : " + oCompany4.Connected + "." + "Bağlantı Hatası: " + oCompany4.GetLastErrorDescription());

                _connlist.Add(new ConnectionList { oCompany = oCompany4, isAvailable = oCompany4.Connected ? "0" : "1", error = oCompany4.GetLastErrorDescription(), number = 5, isConnected = oCompany4.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID, pass = _dtSirketBilgileri.Rows[0]["U_Password"].ToString() });

                logger.Info("ID: " + sayi + " " + "oCompany4 connliste eklendi.");
            }

            errDesc = oCompany.GetLastErrorDescription();
        }
        public ConnectionList getSAPConnection(string CompanyDBCode, int sayi = -1)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            var a = Connlist.Where(x => x.dbCode == CompanyDBCode && x.isAvailable == "0");

            logger.Info("ID: " + sayi + " " + "_connlist içerisindeki uygun dbler sorgulandı.");

            if (a.ToList().Count > 0)
            {
                ConnectionList oCompany = a.First();
                foreach (var item in Connlist.Where(x => x.dbCode == CompanyDBCode && x.isAvailable == "0" && x.number == oCompany.number))
                {
                    item.isAvailable = "1";

                    logger.Info("ID: " + sayi + " " + "|| Çalışan: getSAPConnection  || GUID:" + item.ID + " || Şirket:" + item.dbCode + " || Bağ.No: " + item.number + " || Bağ.Durum:" + item.isConnected + " || Kullanılabilir?:" + item.isAvailable + " || Bağlantı Saati:" + item.baglantiNesnesininOlusturulmaSaati.ToString("HH:mm:ss"));
                }

                logger.Info("ID: " + sayi + " " + "_connlist içerisindeki uygun db bulundu DbCode : " + oCompany.oCompany.CompanyDB);
                return oCompany;
            }
            else
            {
                logger.Info("ID: " + sayi + " " + "_connlist içerisindeki uygun db bulunamadı.");
                return new ConnectionList { number = -1, error = "" };
            }
        }

        public List<ConnectionList> DisconnectSAP(string dbCode)
        {
            if (_connlist == null)
            {
                return new List<ConnectionList>();
            }

            DateTime dt = DateTime.Now;
            List<ConnectionList> disconnectEdilenler = new List<ConnectionList>();

            //foreach (var item in _connlist.Where(z => z.dbCode == dbCode))
            foreach (var item in _connlist)
            {
                if ((dt - item.baglantiNesnesininOlusturulmaSaati).TotalMinutes >= 5)
                {
                    disconnectEdilenler.Add(item);
                }
                //else if ((dt - item.baglantiNesnesininKullanilmayaBaslamaSaati).TotalMinutes > 2)
                //{
                //    disconnectEdilenler.Add(item);
                //}
            }

            Logger logger = LogManager.GetCurrentClassLogger();
            foreach (var item in disconnectEdilenler)
            {
                string GUID = Guid.NewGuid().ToString();

                oCompanyGecici = new SAPbobsCOM.Company();

                oCompanyGecici.LicenseServer = item.oCompany.LicenseServer;
                oCompanyGecici.SLDServer = item.oCompany.SLDServer;
                oCompanyGecici.Server = item.oCompany.Server;
                oCompanyGecici.UserName = item.oCompany.UserName;
                oCompanyGecici.Password = _connlist.Where(x => x.dbCode == dbCode).Select(y => y.pass).FirstOrDefault();
                oCompanyGecici.CompanyDB = dbCode;
                oCompanyGecici.DbServerType = (SAPbobsCOM.BoDataServerTypes)Convert.ToInt32(item.oCompany.DbServerType);
                oCompanyGecici.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;

                item.oCompany.Disconnect();

                logger.Info("DisconnectSAP çalışıyor : ClNumber alanı " + item.number + " alanı disconnect edildi.");

                int resp = oCompanyGecici.Connect();

                if (resp == 0)
                {
                    _connlist.Add(new ConnectionList { oCompany = oCompanyGecici, isAvailable = oCompanyGecici.Connected ? "0" : "1", error = oCompanyGecici.GetLastErrorDescription(), number = item.number, isConnected = oCompanyGecici.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID, pass = _connlist.Where(x => x.dbCode == dbCode).Select(y => y.pass).FirstOrDefault() });
                    //item.isAvailable = "0";

                    logger.Info("DisconnectSAP çalışıyor : ClNumber alanı " + item.number + " alanı yerine yeni connect tayin edildi.");
                }
                else
                {
                    var ress = oCompanyGecici.GetLastErrorDescription();
                }

                _connlist.RemoveAll(x => x.ID == item.ID);
            }

            try
            {
                //var requestJson = JsonConvert.SerializeObject(_connlist);

                foreach (var item in _connlist)
                {
                    //logger.Info("-----------------------------------------------------------------------");
                    //logger.Info("DisconnectSAP çalışıyor : CREATE DATE =  " + item.baglantiNesnesininOlusturulmaSaati);
                    //logger.Info("DisconnectSAP çalışıyor : COMPANY DB  =  " + item.oCompany.CompanyDB);
                    //logger.Info("DisconnectSAP çalışıyor : CONN NUMBER =  " + item.number);
                    //logger.Info("DisconnectSAP çalışıyor : CONNECTED   =  " + item.isConnected);
                    //logger.Info("DisconnectSAP çalışıyor : AVALIABLE   =  " + item.isAvailable);
                    //logger.Info("DisconnectSAP çalışıyor : ERROR       =  " + item.error);
                    //logger.Info("-----------------------------------------------------------------------"); 

                    logger.Info("|| Çalışan: DisconnectSAP     || GUID:" + item.ID + " || Şirket:" + item.dbCode + " || Bağ.No: " + item.number + " || Bağ.Durum:" + item.isConnected + " || Kullanılabilir?:" + item.isAvailable + " || Bağlantı Saati:" + item.baglantiNesnesininOlusturulmaSaati.ToString("HH:mm:ss"));
                }
            }
            catch (Exception ex)
            {
            }

            return _connlist;
        }

        public static void ReleaseConnection(int no, string CompanyDBCode, int sayi = -1)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            logger.Info("ID: " + sayi + " " + "İŞLEM YAPILMADAN ÖNCE:   " + CompanyDBCode + " İÇİN UYGUN BAĞLANTI SAYISI: (" + _connlist.Where(w => w.dbCode == CompanyDBCode && w.isAvailable == "0").Count().ToString() + ")");
            foreach (var item in _connlist.Where(w => w.number == no && w.dbCode == CompanyDBCode))
            {
                item.isAvailable = "0";
                item.baglantiNesnesininKullanilmayaBaslamaSaati = new DateTime();

                logger.Info("ID: " + sayi + " " + "|| Çalışan: ReleaseConnection || GUID:" + item.ID + " || Şirket:" + item.dbCode + " || Bağ.No: " + item.number + " || Bağ.Durum:" + item.isConnected + " || Kullanılabilir?:" + item.isAvailable + " || Bağlantı Saati:" + item.baglantiNesnesininOlusturulmaSaati.ToString("HH:mm:ss"));
            }

            logger.Info("ID: " + sayi + " " + "İŞLEM YAPILDIKTAN SONRA: " + CompanyDBCode + " İÇİN UYGUN BAĞLANTI SAYISI: (" + _connlist.Where(w => w.dbCode == CompanyDBCode && w.isAvailable == "0").Count().ToString() + ")");
        }

        private static List<ConnectionList> _connlist;
        public static List<ConnectionList> Connlist
        {
            get
            {
                if (_connlist == null)
                    _connlist = new List<ConnectionList>();
                return _connlist;
            }

            set
            {
                _connlist = value;
            }
        }

    }
}