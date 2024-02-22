using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class Iade
    {
        public string CardCode { get; set; }

        public string WareHouse { get; set; }

        public string DocDate { get; set; }

        public string DocDueDate { get; set; }

        public string WayBillNo { get; set; }
        public string TaxDate { get; set; }
        public string Comments { get; set; }
        public string ShipToCode { get; set; }
        public List<IadeDetay> iadeDetays { get; set; }
    }

    public class IadeDetay
    {
        public string ItemCode { get; set; }

        public double Quantity { get; set; }

        public int BaseEntry { get; set; }

        public int BaseType { get; set; }

        public int BaseLine { get; set; } 

        public string WareHouse { get; set; }
        public string BinCode { get; set; }

        public int DocEntry { get; set; } 

        public int LineNum { get; set; }

        public List<IadeDetayParti> iadeDetayParti { get; set; }
    }

    public class IadeDetayParti
    {
        public int DocEntry { get; set; }

        public string BatchNumber { get; set; }

        public double BatchQuantity { get; set; }

        public string ItemCode { get; set; }

        public int LineNumber { get; set; }

        public int SAPLineNum { get; set; }

        public string DepoYeriId { get; set; }

        public string DepoYeriAdi { get; set; }
        public string BinCode { get; set; }

    }
}