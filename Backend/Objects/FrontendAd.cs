using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ItemFinder_WPF.Backend.Objects
{
    public class FrontendAd
    {
        public string Nom { get; set; }
        public string Livraison { get; set; }
        public string Prix { get; set; }
        public string Etat { get; set; }
        public string Date { get; set; }
        public string DepuisMaintenant { get; set; }
        public string Ville { get; set; }
        public string Link { get; set; } 
        public string Note { get; set; }
        public string Vendeur { get; set; }
        public string Relevance { get; set; }
        public BitmapImage Image { get; set; }

        //Pour filtrer selons ces critères
        public DateTime UnparsedDate { get; set; }
        public double UnparsedPrix { get; set; }
        public double UnparsedRelevance { get; set; }
    }
}
