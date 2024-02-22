using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class WaybillReturn
    {
        public string CardCode { get; set; }

        public string WareHouse { get; set; }

        public string DocDate { get; set; }

        public string DocDueDate { get; set; }

        public string WayBillNo { get; set; }
        public string TaxDate { get; set; }
        public string Comments { get; set; }
        public string ShipToCode { get; set; }
        public List<WaybillReturnDetails> WaybillReturnDetails { get; set; }
    }

    public class WaybillReturnDetails
    {
        public string ItemCode { get; set; }

        public double Quantity { get; set; }

        public int BaseEntry { get; set; }

        public int BaseType { get; set; }

        public int BaseLine { get; set; }

        public string WareHouse { get; set; }

        public List<WaybillReturnBatchList> BatchLists { get; set; }
    }

    public class WaybillReturnBatchList
    {
        public string BatchNumber { get; set; }

        public double BatchQuantity { get; set; }
        public string DepoYeriId { get; set; }
        public string DepoYeriAdi { get; set; }
    }
}