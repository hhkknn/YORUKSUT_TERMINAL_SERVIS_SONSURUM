using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class GoodRecieptPO
    {
        public string CarCode { get; set; } 
        public string WareHouse { get; set; } 
        public string DocDate { get; set; } 
        public string DocDueDate { get; set; } 
        public string WayBillNo { get; set; }
        public string TaxDate { get; set; } 
        public List<GoodRecieptPODetails> GoodRecieptPODetails { get; set; }
    }

    public class GoodRecieptPODetails
    {
        public string ItemCode { get; set; }

        public double Quantity { get; set; }

        public int BaseEntry { get; set; }

        public int BaseType { get; set; }

        public int BaseLine { get; set; }

        public string WareHouse { get; set; }

        public string BinCode { get; set; }

        public List<GoodRecieptPOBatchList> BatchLists { get; set; }
    }

    public class GoodRecieptPOBatchList
    {
        public string BatchNumber { get; set; }

        public double BatchQuantity { get; set; }

        public string BinCode { get; set; } 
    }
}