using Newtonsoft.Json;
using NLog;
using SAPDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPDataAccess.Methods
{
    public class ApiConnect
    {
        public static SAPbobsCOM.Company oCompany { get; set; }
        public static SAPbobsCOM.Company oCompany2 { get; set; }
        public static SAPbobsCOM.Company oCompany3 { get; set; }
        public static SAPbobsCOM.Company oCompany4 { get; set; }
        public static SAPbobsCOM.Company oCompany5 { get; set; }
        public static SAPbobsCOM.Company oCompanyGecici { get; set; }
        public static string Error { get; set; }
        public static bool IsConnect { get; set; }
        public static SAPbobsCOM.CompanyService oCompanyService { get; set; }

        private static List<ConnectionList> _connlist;

        public ApiConnect()
        {
            //Connect();
        }

        public List<ConnectionList> DisconnectSAP(string dbCode)
        {
            if (_connlist == null)
            {
                return new List<ConnectionList>();
            }
             
            DateTime dt = DateTime.Now;
            List<ConnectionList> disconnectEdilenler = new List<ConnectionList>();

            foreach (var item in _connlist.Where(z => z.dbCode == dbCode))
            {
                if ((dt - item.baglantiNesnesininOlusturulmaSaati).TotalMinutes >= 15)
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

                logger.Info("DisconnectSAP çalışıyor : ClNumber alanı " + item.number + " alanı disconnect edildi.");

                string GUID = Guid.NewGuid().ToString();

                oCompanyGecici = new SAPbobsCOM.Company();


                oCompanyGecici.LicenseServer = "10.10.227.14:40000";
                oCompanyGecici.Server = "10.10.227.14:30015";
                oCompanyGecici.UserName = "manager";
                oCompanyGecici.Password = "1234";
                oCompanyGecici.CompanyDB = dbCode;
                oCompanyGecici.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
                oCompanyGecici.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;

                item.oCompany.Disconnect();

                int resp = oCompanyGecici.Connect();

                if (resp == 0)
                {
                    _connlist.Add(new ConnectionList { oCompany = oCompanyGecici, isAvailable = oCompanyGecici.Connected ? "0" : "1", error = oCompanyGecici.GetLastErrorDescription(), number = item.number, isConnected = oCompanyGecici.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID });
                    //item.isAvailable = "0";

                    logger.Info("DisconnectSAP çalışıyor : ClNumber alanı " + item.number + " alanı yerine yeni connect tayin edildi.");
                }


                _connlist.RemoveAll(x => x.ID == item.ID);
            }



            var requestJson = JsonConvert.SerializeObject(_connlist);
            logger.Info("DisconnectSAP çalışıyor : Son kalan listei " + requestJson);
            return _connlist;
        }

        public ConnectionList Connect(string dbCode, int sayi = -1)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            logger.Info("ID: " + sayi + " " + "Connect methoduna geldi DbCode : " + dbCode);


            //WriteToFile writeToFile = new WriteToFile();

            //writeToFile.writeToFile("Connect methoduna geldi DbCode : " + dbCode + "- ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            IsConnect = false;
            Error = "";
            var conexist = _connlist != null ? _connlist.Where(z => z.dbCode == dbCode).Count() : 0;
            if (conexist == 0)
            {

                logger.Info("ID: " + sayi + " " + "conexist == 0 if koşuluna girdi");
                //writeToFile.writeToFile("conexist==0 if koşuluna girdi - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);
                try
                {
                    if (_connlist == null)
                    {
                        logger.Info("ID: " + sayi + " " + "conexist==null if koşuluna girdi");
                        //writeToFile.writeToFile("conexist==null if koşuluna girdi - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);
                        _connlist = new List<ConnectionList>();
                    }
                    oCompany = new SAPbobsCOM.Company();
                    oCompany2 = new SAPbobsCOM.Company();
                    oCompany3 = new SAPbobsCOM.Company();
                    oCompany4 = new SAPbobsCOM.Company();
                    oCompany5 = new SAPbobsCOM.Company();
                }
                catch (Exception ex)
                {
                    logger.Fatal("ID: " + sayi + " " + "oCompany = new SAPbobsCOM.Company(); işlemleri yapılırken hata oluştu :");
                    //writeToFile.writeToFile("oCompany = new SAPbobsCOM.Company(); işlemleri yapılırken hata oluştu : " + ex.Message + "- ID: " + sayi + " Tarih-Saat: " + DateTime.Now);
                }
                var aa = ConnectCompany(dbCode, sayi);

                if (oCompany == null)
                {
                    logger.Fatal("ID: " + sayi + " " + "ConnectCompany methodu null döndü DbCode :");
                    //writeToFile.writeToFile("ConnectCompany methodu null döndü DbCode :" + dbCode + " - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);
                    Console.WriteLine("Hata oluştu");
                }

                //if (oCompany.Connected)
                //{
                //    IsConnect = true;
                //    Error = "";
                //    oCompanyService = oCompany.GetCompanyService();
                //}
                //else
                //{
                //    IsConnect = false;
                //    Error = "Sap bağlantısı yapılamadı";
                //}

                return aa;
            }
            else
            {
                logger.Info("ID: " + sayi + " " + "Daha önce bağlantı sağlandığı için else kısmına geldi DbCode :" + dbCode);
                //writeToFile.writeToFile("Daha önce bağlantı sağlandığı için else kısmına geldi DbCode :" + dbCode + " - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

                var a = _connlist.Where(x => x.isAvailable == "0" && x.dbCode == dbCode);

                logger.Info("ID: " + sayi + " " + "Oluşan şirket bağlantılarından sorgulama yapıldı. Bulunan uygun bağlantı sayısı " + a.Count().ToString() + " DbCode :" + dbCode);
                //writeToFile.writeToFile("Oluşan şirket bağlantılarından sorgulama yapıldı. Bulunan uygun bağlantı sayısı " + a.Count().ToString() + " DbCode :" + dbCode + " - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

                if (a.ToList().Count > 0)
                {
                    ConnectionList oCompanyFinal = a.First();
                    foreach (var item in _connlist.Where(x => x.isAvailable == "0" && x.number == oCompanyFinal.number && x.dbCode == dbCode))
                    {
                        item.isAvailable = "1";
                        item.baglantiNesnesininKullanilmayaBaslamaSaati = DateTime.Now;
                    }

                    logger.Info("ID: " + sayi + " " + "_connlist içerisindeki uygun db bulundu DbCode " + oCompanyFinal.oCompany.CompanyDB);
                    //writeToFile.writeToFile("_connlist içerisindeki uygun db bulundu DbCode : " + oCompanyFinal.oCompany.CompanyDB + " - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

                    return oCompanyFinal;
                }
                else
                {
                    logger.Info("ID: " + sayi + " " + "_connlist içerisindeki uygun db bulunamadı");
                    //writeToFile.writeToFile("_connlist içerisindeki uygun db bulunamadı - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);
                }
            }
            return null;
        }

        private ConnectionList ConnectCompany(string dbCode, int sayi = -1)
        {
            //if (_connlist.Count == 0)
            //{
            //oCompany.LicenseServer = System.Configuration.ConfigurationManager.AppSettings["LicenseServer"];
            //oCompany.Server = System.Configuration.ConfigurationManager.AppSettings["Server"];
            //oCompany.UserName = System.Configuration.ConfigurationManager.AppSettings["SAPUserName"];
            //oCompany.Password = System.Configuration.ConfigurationManager.AppSettings["SAPPassword"];
            ////string companyDB = "CountryCode" + CountryCode.ToString();
            //oCompany.CompanyDB = System.Configuration.ConfigurationManager.AppSettings["CompanyDB"];
            //oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            //oCompany.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;

            //WriteToFile writeToFile = new WriteToFile();

            string GUID = Guid.NewGuid().ToString();

            Logger logger = LogManager.GetCurrentClassLogger();

            logger.Info("ID: " + sayi + " " + "ConnectCompany methoduna geldi DbCode : " + dbCode);
            //writeToFile.writeToFile("ConnectCompany methoduna geldi DbCode : " + dbCode + "- ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            oCompany.LicenseServer = "10.10.227.14:40000";
            oCompany.Server = "10.10.227.14:30015";
            oCompany.UserName = "manager";
            oCompany.Password = "1234";
            oCompany.CompanyDB = dbCode;
            oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            oCompany.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;

            oCompany.Connect();

            logger.Info("ID: " + sayi + " " + "oCompany connect edildi bağlantı durumu : " + oCompany.Connected + " bağlantı hatası " + oCompany.GetLastErrorDescription());
            //writeToFile.writeToFile("oCompany connect edildi bağlantı durumu : " + oCompany.Connected + " bağlantı hatası " + oCompany.GetLastErrorDescription() + "- ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            _connlist.Add(new ConnectionList { oCompany = oCompany, isAvailable = oCompany.Connected ? "0" : "1", error = oCompany.GetLastErrorDescription(), number = 1, isConnected = oCompany.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID });

            logger.Info("ID: " + sayi + " " + "oCompany connliste eklendi ");
            //writeToFile.writeToFile("oCompany connliste eklendi - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            GUID = Guid.NewGuid().ToString();

            oCompany2.LicenseServer = "10.10.227.14:40000";
            oCompany2.Server = "10.10.227.14:30015";
            oCompany2.UserName = "manager";
            oCompany2.Password = "1234";
            oCompany2.CompanyDB = dbCode;
            oCompany2.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            oCompany2.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;

            oCompany2.Connect();

            logger.Info("ID: " + sayi + " " + "oCompany2 connect edildi bağlantı durumu : " + oCompany.Connected + " bağlantı hatası " + oCompany.GetLastErrorDescription());
            //writeToFile.writeToFile("oCompany2 connect edildi bağlantı durumu : " + oCompany2.Connected + " bağlantı hatası " + oCompany2.GetLastErrorDescription() + "- ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            _connlist.Add(new ConnectionList { oCompany = oCompany2, isAvailable = oCompany2.Connected ? "0" : "1", error = oCompany2.GetLastErrorDescription(), number = 2, isConnected = oCompany2.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID });

            logger.Info("ID: " + sayi + " " + "oCompany2 connliste eklendi ");

            //writeToFile.writeToFile("oCompany2 connliste eklendi - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);
            GUID = Guid.NewGuid().ToString();

            oCompany3.LicenseServer = "10.10.227.14:40000";
            oCompany3.Server = "10.10.227.14:30015";
            oCompany3.UserName = "manager";
            oCompany3.Password = "1234";
            oCompany3.CompanyDB = dbCode;
            oCompany3.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            oCompany3.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;

            oCompany3.Connect();

            logger.Info("ID: " + sayi + " " + "oCompany3 connect edildi bağlantı durumu : " + oCompany.Connected + " bağlantı hatası " + oCompany.GetLastErrorDescription());

            //writeToFile.writeToFile("oCompany3 connect edildi bağlantı durumu : " + oCompany3.Connected + " bağlantı hatası " + oCompany3.GetLastErrorDescription() + "- ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            _connlist.Add(new ConnectionList { oCompany = oCompany3, isAvailable = oCompany3.Connected ? "0" : "1", error = oCompany3.GetLastErrorDescription(), number = 3, isConnected = oCompany3.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID });

            logger.Info("ID: " + sayi + " " + "oCompany3 connliste eklendi ");

            //writeToFile.writeToFile("oCompany3 connliste eklendi - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);
            GUID = Guid.NewGuid().ToString();

            oCompany4.LicenseServer = "10.10.227.14:40000";
            oCompany4.Server = "10.10.227.14:30015";
            oCompany4.UserName = "manager";
            oCompany4.Password = "1234";
            oCompany4.CompanyDB = dbCode;
            oCompany4.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            oCompany4.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;

            oCompany4.Connect();


            logger.Info("ID: " + sayi + " " + "oCompany4 connect edildi bağlantı durumu : " + oCompany.Connected + " bağlantı hatası " + oCompany.GetLastErrorDescription());
            //writeToFile.writeToFile("oCompany4 connect edildi bağlantı durumu : " + oCompany4.Connected + " bağlantı hatası " + oCompany4.GetLastErrorDescription() + "- ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            _connlist.Add(new ConnectionList { oCompany = oCompany4, isAvailable = oCompany4.Connected ? "0" : "1", error = oCompany4.GetLastErrorDescription(), number = 4, isConnected = oCompany4.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID });

            logger.Info("ID: " + sayi + " " + "oCompany4 connliste eklendi ");
            //writeToFile.writeToFile("oCompany4 connliste eklendi - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            GUID = Guid.NewGuid().ToString();
            oCompany5.LicenseServer = "10.10.227.14:40000";
            oCompany5.Server = "10.10.227.14:30015";
            oCompany5.UserName = "manager";
            oCompany5.Password = "1234";
            oCompany5.CompanyDB = dbCode;
            oCompany5.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            oCompany5.language = SAPbobsCOM.BoSuppLangs.ln_Turkish_Tr;

            oCompany5.Connect();

            logger.Info("ID: " + sayi + " " + "oCompany5 connect edildi bağlantı durumu : " + oCompany.Connected + " bağlantı hatası " + oCompany.GetLastErrorDescription());
            //writeToFile.writeToFile("oCompany5 connect edildi bağlantı durumu : " + oCompany5.Connected + " bağlantı hatası " + oCompany5.GetLastErrorDescription() + "- ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            _connlist.Add(new ConnectionList { oCompany = oCompany5, isAvailable = oCompany5.Connected ? "0" : "1", error = oCompany5.GetLastErrorDescription(), number = 5, isConnected = oCompany5.Connected, dbCode = dbCode, baglantiNesnesininOlusturulmaSaati = DateTime.Now, ID = GUID });

            logger.Info("ID: " + sayi + " " + "oCompany5 connliste eklendi ");
            //writeToFile.writeToFile("oCompany5 connliste eklendi - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            var a = _connlist.Where(x => x.isAvailable == "0");

            logger.Info("ID: " + sayi + " " + "_connlist içerisindeki uygun dbler sorgulandı ");

            //writeToFile.writeToFile("_connlist içerisindeki uygun dbler sorgulandı - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            if (a.ToList().Count > 0)
            {
                ConnectionList ocom = a.First();
                foreach (var item in _connlist.Where(x => x.isAvailable == "0" && x.number == ocom.number && x.dbCode == dbCode))
                {
                    item.isAvailable = "1";
                }

                logger.Info("ID: " + sayi + " " + "_connlist içerisindeki uygun db bulundu DbCode : " + ocom.oCompany.CompanyDB);
                //writeToFile.writeToFile("_connlist içerisindeki uygun db bulundu DbCode : " + ocom.oCompany.CompanyDB + " - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);
                return ocom;
            }
            else
            {
                logger.Info("ID: " + sayi + " " + "_connlist içerisindeki uygun db bulunamadı.");
                //writeToFile.writeToFile("_connlist içerisindeki uygun db bulunamadı - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);
            }
            return null;
            //}
            //else
            //{
            //    var a = _connlist.Where(x => x.isAvailable == "0");
            //    if (a.ToList().Count > 0)
            //    {
            //        ConnectionList oCompanyFinal = a.First();
            //        foreach (var item in _connlist.Where(x => x.isAvailable == "0" && x.number == oCompanyFinal.number && x.dbCode == dbCode))
            //        {
            //            item.isAvailable = "1";
            //        }
            //        return oCompanyFinal;
            //    }
            //}

            return null;
            //if (errorCode != 0)
            //{
            //    Error = GetLastError();
            //    IsConnect = false;
            //}
            //return _oCompany;
        }

        public string GetLastError()
        {
            int err = 0;
            string exc = "";
            oCompany.GetLastError(out err, out exc);
            if (string.IsNullOrEmpty(exc))
                return "";
            else
                return err.ToString() + " " + exc;
        }

        public static string GetLastErrorStatic()
        {
            int err = 0;
            string exc = "";
            oCompany.GetLastError(out err, out exc);
            if (string.IsNullOrEmpty(exc))
                return "";
            else
                return err.ToString() + " " + exc;
        }

        public void DisConnect()
        {
            if (oCompany.Connected)
            {
                oCompanyService = null;
                oCompany.Disconnect();
                Error = "";
                IsConnect = false;
                oCompany = null;
                GC.Collect();
            }
        }

        public static void ReleaseConnection(int no, string dbCode, int sayi = -1)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            //WriteToFile writeToFile = new WriteToFile();
            //Random rastgele = new Random();

            logger.Info("ID: " + sayi + " " + "işlem yapılmadan önce :" + dbCode + " için uygun bağlantı sayısı:" + _connlist.Where(w => w.dbCode == dbCode && w.isAvailable == "0").Count().ToString());
            //writeToFile.writeToFile("işlem yapılmadan önce :" + dbCode + " için uygun bağlantı sayısı:" + _connlist.Where(w => w.dbCode == dbCode && w.isAvailable == "0").Count().ToString() + " - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);

            foreach (var item in _connlist.Where(w => w.number == no && w.dbCode == dbCode))
            {
                item.isAvailable = "0";
                item.baglantiNesnesininKullanilmayaBaslamaSaati = new DateTime();
            }

            logger.Info("ID: " + sayi + " " + "işlem yapıldıkan sonra :" + dbCode + " için uygun bağlantı sayısı:" + _connlist.Where(w => w.dbCode == dbCode && w.isAvailable == "0").Count().ToString());
            //writeToFile.writeToFile("işlem yapıldıkan sonra :" + dbCode + " için uygun bağlantı sayısı:" + _connlist.Where(w => w.dbCode == dbCode && w.isAvailable == "0").Count().ToString() + " - ID: " + sayi + " Tarih-Saat: " + DateTime.Now);
        }
    }
}