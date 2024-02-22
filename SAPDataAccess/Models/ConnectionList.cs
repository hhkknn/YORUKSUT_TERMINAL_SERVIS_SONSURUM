using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPDataAccess.Models
{
    public class ConnectionList
    {
        public SAPbobsCOM.Company oCompany { get; set; }
        public string isAvailable { get; set; }
        public string error { get; set; }
        public int number { get; set; }
        public bool isConnected { get; set; }
        public string dbCode { get; set; }
        public DateTime baglantiNesnesininOlusturulmaSaati { get; set; }
        public DateTime baglantiNesnesininKullanilmayaBaslamaSaati { get; set; }
        public string ID { get; set; }


    }
}