using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class InventoryGenEntry
    {
        public string CardCode { get; set; }

        public string DocDate { get; set; }

        public string DocDueDate { get; set; }

        public string Comments { get; set; }
        public List<InventoryGenEntryLines> InventoryGenEntryLines { get; set; }

    }

    public class InventoryGenEntryLines
    {
        public string ItemCode { get; set; }

        public double Quantity { get; set; }

        public string fromWhsCode { get; set; }

        public string toWhsCode { get; set; }

        public int BaseEntry { get; set; }

        public int BaseType { get; set; }

        public int BaseLine { get; set; }
        public string uretimdenGonderildi { get; set; }

        public string BinCode { get; set; }

        public string BinCode_from { get; set; }

        public string BinCode_to { get; set; }

        public List<InventoryGenEntryLinesBatch> InventoryGenEntryLinesBatch { get; set; }
        public string taslakGercek { get; set; }
    }

    public class InventoryGenEntryLinesBatch
    {
        public string BatchNumber { get; set; } 
        public double BatchQuantity { get; set; }
        public string DepoYeriId { get; set; }
        public string DepoYeriAdi { get; set; }
        public string HedefDepoYeriId { get; set; }
        public string HedefDepoYeriAdi { get; set; }
    }
}