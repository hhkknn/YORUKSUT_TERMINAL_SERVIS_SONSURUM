using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class MagazaMalGirisCikis
    {
        public int belgeNo { get; set; }
        public DateTime tarih { get; set; }
        public string girisMiCikisMi { get; set; }
        public string gondericiSube { get; set; }
        public string aliciSube { get; set; } 
    }
}
