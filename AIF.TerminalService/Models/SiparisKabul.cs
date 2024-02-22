using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class SiparisKabul
    {
        public string magaza { get; set; }
        public DateTime siparisTarihi { get; set; }
        public string durum { get; set; }
        public string siparisRefNo { get; set; }
        public DateTime terminTarihi { get; set; }
        public string subeId { get; set; }
        public string kaynakDepo { get; set; }
        public string hedefDepo { get; set; }
        public DateTime kabulTarihi { get; set; }
        public string durumNo { get; set; }
        public List<SiparisKabulDetay> siparisKabulDetays { get; set; }
    }

    public class SiparisKabulDetay
    {
        public string barkod { get; set; }
        public string urunKodu { get; set; }
        public string urunTanimi { get; set; }
        public double miktar { get; set; }
        public double onaylananMiktar { get; set; }
        public double kabulEdilenMiktar { get; set; }
        public double fiyat { get; set; }
        public string siraNumarasi { get; set; }

        //public DateTime siparisTarihi { get; set; }
    }
}
