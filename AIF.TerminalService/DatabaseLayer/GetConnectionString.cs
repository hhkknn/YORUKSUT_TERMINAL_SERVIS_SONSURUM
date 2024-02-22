using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.DatabaseLayer
{
    public class GetConnectionString
    {
        private string constring = "";// ConfigurationManager.ConnectionStrings["SAP"].ConnectionString;

        public string getConnectionString(string dbName = "", string mKod = "")
        {
            if (dbName != "")
            {
                if (mKod != "")
                {
                    if (mKod == "10TRMN")
                    {
                        constring = "Application Name=AIF.TERMINAL;Data Source=SAPSRV;Initial Catalog=" + dbName + ";User ID=sa; Password=Qaz1Wsx2";
                    }
                    else if (mKod == "30TRMN")
                    {
                        constring = "Application Name=AIF.TERMINAL;Data Source=ANATOLYA-SAP\\SAPB1;Initial Catalog=" + dbName + ";User ID=sa; Password=Eropa2018!";
                    }
                    else if (mKod == "70TRMN")
                    {
                        constring = "Application Name=AIF.TERMINAL;Data Source=ZWTKISTAZSP01;Initial Catalog=" + dbName + ";User ID=sa; Password=qawsEDRF!!!!";
                    }
                    else if (mKod == "20TRMN")
                    {
                        constring = "Application Name=AIF.TERMINAL;Data Source=SAP;Initial Catalog=" + dbName + ";User ID=sa; Password=Yoruk@1234";
                    }
                }
            }
            else
            {
                if (mKod != "")
                {
                    if (mKod == "10TRMN")
                    {
                        constring = "Application Name=AIF.TERMINAL;Data Source=SAPSRV;Initial Catalog=" + dbName + ";User ID=sa; Password=Qaz1Wsx2";
                    }
                    else if (mKod == "30TRMN")
                    {
                        constring = "Application Name=AIF.TERMINAL;Data Source=ANATOLYA-SAP\\SAPB1;Initial Catalog=" + dbName + ";User ID=sa; Password=Eropa2018!"; //bilgiler değiştirilecek
                    }
                    else if (mKod == "70TRMN")
                    {
                        constring = "Application Name=AIF.TERMINAL;Data Source=ZWTKISTAZSP01;Initial Catalog=" + dbName + ";User ID=sa; Password=qawsEDRF!!!!";
                    }
                    else if (mKod == "20TRMN")
                    {
                        constring = "Application Name=AIF.TERMINAL;Data Source=SAP;Initial Catalog=" + dbName + ";User ID=sa; Password=Yoruk@1234";
                    }
                }
            }

            return constring;
        }
    }
}