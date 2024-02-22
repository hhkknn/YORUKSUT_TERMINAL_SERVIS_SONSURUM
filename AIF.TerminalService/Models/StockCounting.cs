using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class StockCounting
    {
        public string SayimTarihi { get; set; }

        public string KullaniciId { get; set; }

        public string Aciklama { get; set; }

        public string VarolanSayimaDevamEdilmekIstendi { get; set; }

        public string BelgeNo { get; set; }

        public List<StockCountingLines> stockCountings { get; set; }

        public List<StockCountingParties> StockCountingParties { get; set; }
    }

    public class StockCountingLines
    {
        public string Barkod { get; set; }

        public string KalemKodu { get; set; }

        public string KalemTanimi { get; set; }

        public string DepoKodu { get; set; }

        public string DepoAdi { get; set; }

        public double Miktar { get; set; }

        public string OlcuBirimi { get; set; }

        public string DepoYeriId { get; set; }

        public string DepoYeriAdi { get; set; }
    }

    public class StockCountingParties
    {
        public string KalemKodu { get; set; }

        public string PartiNo { get; set; }

        public double Miktar { get; set; }

        public string DepoKodu { get; set; }
    }
}