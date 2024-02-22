using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class InventoryGenExit
    {
        public string DocDate { get; set; }

        public string Comments { get; set; }

        public List<InventoryGenExitLines> InventoryGenExitLines { get; set; }

    }

    public class InventoryGenExitLines
    {
        public string ItemCode { get; set; }

        public double Quantity { get; set; }

        public string WareHouse { get; set; }

        public int BaseEntry { get; set; }

        public int BaseType { get; set; }

        public int BaseLine { get; set; }

        public string BinCode { get; set; }

        public List<InventoryGenExitLinesBatch> InventoryGenExitLinesBatch { get; set; }
    }

    public class InventoryGenExitLinesBatch
    {
        public string BatchNumber { get; set; }

        public double BatchQuantity { get; set; }

        public string DepoYeriId { get; set; }

        public string DepoYeriAdi { get; set; }
    }
}