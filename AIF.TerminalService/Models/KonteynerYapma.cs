using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class KonteynerYapma
    {
        public string KonteynerNumarasi { get; set; }
        public List<KonteynerYapmaDetay> konteynerYapmaDetays { get; set; }

    }
    public class KonteynerYapmaDetay
    {
        public string PaletNo { get; set; }
        public string Barkod { get; set; } 
        public string MuhatapKatalogNo { get; set; } 
        public string KalemKodu { get; set; }
        public string KalemTanimi { get; set; }
        public int siparisNo { get; set; }
        public int siparisSatirNo { get; set; }
        public double Quantity { get; set; }
        public string UrunKonteynereDahaOnceEklendi { get; set; }
        public string CekmeListesiNo { get; set; }
        public int koliMiktari { get; set; }
        public double netKilo { get; set; }
        public double brutKilo { get; set; }
        public string satirKaynagi { get; set; }

    }
}
