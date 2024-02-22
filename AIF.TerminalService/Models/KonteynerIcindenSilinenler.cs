using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class KonteynerIcindenSilinenler
    {
        public int cekmeNo { get; set; }

        public int siparisNo { get; set; }

        public int siparisSatirNo { get; set; }

        public double miktar { get; set; }
        public string kaynak { get; set; }
        public string paletNo { get; set; }


    }
}