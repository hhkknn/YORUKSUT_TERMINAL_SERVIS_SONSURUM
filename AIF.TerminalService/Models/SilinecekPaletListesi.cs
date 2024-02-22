using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class SilinecekPaletler
    {
        public string paletNo { get; set; }
        public int siparisNo { get; set; }
        public int siparisSatirNo { get; set; }
        public double miktar { get; set; } //Bu alan Çekme Listesini Kalemlere gruplarken fifoya göre düşmek için alındı.
        public string cekmeNo { get; set; }
        public string kalemKodu { get; set; }
        public string kaynak { get; set; }
    }
}