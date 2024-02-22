using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class CekmeListesiOnaylama
    {
        public string BelgeNo { get; set; }
        public string MusteriKodu { get; set; }
        public List<CekmeListesiOnaylamaDetay> cekmeListesiOnaylamaDetays { get; set; }
        public List<CekmeListesiOnaylamaKoliDetay> cekmeListesiOnaylamaKoliDetays { get; set; }

    }
    public class CekmeListesiOnaylamaDetay
    {
        public int SatirNo { get; set; }
        public int SipNo { get; set; }
        public string KalemKodu { get; set; }
        public string KalemTanimi { get; set; }
        public double PlanlananMiktar { get; set; } 
        public string MuhatapKatalogNo { get; set; } 
        public string Barkod { get; set; } 
        public double Miktar { get; set; }
        public string PaletNo { get; set; }
        public string GridViewGorunenSira { get; set; }
        public string SiparisKarsilamaLineId { get; set; }
        public string satirKaynagi { get; set; }

    }
    public class CekmeListesiOnaylamaKoliDetay
    {
        public int siparisNumarasi { get; set; }
        public int sapSatirNo { get; set; }
        public double KoliAdedi { get; set; }
        public double KoliIciAdedi { get; set; }
        public double ToplamMiktar { get; set; }
        public string PaletNo { get; set; }
        public string KalemKodu { get; set; }
        public string satirKaynagi { get; set; }

    }
}
