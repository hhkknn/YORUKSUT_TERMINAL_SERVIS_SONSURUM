using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class Orders
    {
        public string CarCode { get; set; }

        public string WareHouse { get; set; }

        public string DocDate { get; set; }

        public string DocDueDate { get; set; }

        public string WayBillNo { get; set; }

        public string CarPlate { get; set; }

        public string DriverName { get; set; }

        public string PostType { get; set; }

        public double CarTemp { get; set; }

        public List<OrderDetails> OrderDetails { get; set; }
    }

    public class OrderDetails
    {
        public string ItemCode { get; set; }

        public double Quantity { get; set; }

        public int BaseEntry { get; set; }

        public int BaseType { get; set; }

        public int BaseLine { get; set; }

        public string WareHouse { get; set; }

        public string DepoyYeriId { get; set; }

        public List<OrderBatchList> BatchLists { get; set; }
    }

    public class OrderBatchList
    {
        public string BatchNumber { get; set; }

        public double BatchQuantity { get; set; }

        public string DepoyYeriId { get; set; }

        public string DepoyYeriAdi { get; set; }
    }
}