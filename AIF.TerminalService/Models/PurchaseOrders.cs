using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class PurchaseOrders
    {
        public string CarCode { get; set; }

        public string WareHouse { get; set; }

        public string DocDate { get; set; }

        public string DocDueDate { get; set; }

        public List<PurchaseOrdersDetails> PurchaseOrdersDetails { get; set; }
    }

    public class PurchaseOrdersDetails
    {
        public string ItemCode { get; set; }

        public double Quantity { get; set; }
    }
}