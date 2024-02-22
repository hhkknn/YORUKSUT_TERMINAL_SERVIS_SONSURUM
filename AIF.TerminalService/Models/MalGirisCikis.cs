using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class MalGirisCikis
    {
        public string girisMiCikisMi { get; set; }
        public int belgeNo { get; set; }
        public string durum { get; set; }
        public DateTime tarih { get; set; }
        public string malCikisNo { get; set; }
        public string referansNo { get; set; }
        public string aliciAdi { get; set; }
        public string aliciAdres { get; set; }
        public string postaKodu { get; set; }
        public string ilKodu { get; set; }
        public string ilAdi { get; set; }
        public string ilceKodu { get; set; }
        public string ilceAdi { get; set; }

        public string aliciSubeId { get; set; }
        public string belgeSubeId { get; set; }
        public string ref2 { get; set; }
        //public TasiyiciBilgileri tasiyiciBilgileri { get; set; }
        public List<MalGirisCikisDetay> malGirisCikisDetays { get; set; }

    }
    public class MalGirisCikisDetay
    {
        public string urunBarkodu { get; set; }
        public string urunKodu { get; set; }
        public string urunTanimi { get; set; }
        public double miktar { get; set; }
        public double belgeSatirMiktar { get; set; }

        public string sil { get; set; }
    }
}
