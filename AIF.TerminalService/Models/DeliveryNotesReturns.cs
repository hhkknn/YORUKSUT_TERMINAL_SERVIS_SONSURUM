using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class DeliveryNotesReturns
    {
        public List<DeliveryNotesReturnDetails> DeliveryNotesReturnDetails { get; set; }

    }
    public class DeliveryNotesReturnDetails
    {
        public int DocEntry { get; set; }

        public double Quantity { get; set; }

        public string LineNum { get; set; }
    }
}
