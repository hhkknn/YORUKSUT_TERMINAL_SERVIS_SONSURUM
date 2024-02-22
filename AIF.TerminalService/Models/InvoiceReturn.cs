using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class InvoiceReturn
    {
        public List<InvoiceReturnDetails> InvoiceReturnDetails { get; set; }

    }
    public class InvoiceReturnDetails
    {
        public int DocEntry { get; set; }

        public double Quantity { get; set; }

        public string LineNum { get; set; }
    }
}