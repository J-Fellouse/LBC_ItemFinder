using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemFinder_WPF.Backend.Objects
{
    public class ExportAd
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string State { get; set; }
        public int SellerReviewCount { get; set; }
        public double SellerReviewGrade { get; set; }
        public string SellerName { get; set; }
        public string Date { get; set; }
        public string Url { get; set; }
        public bool IsDeliveryAvailable { get; set; }
        public int ZipCode { get; set; }
        public string City { get; set; }
        public bool OngoingPurchase { get; set; } //achat en cours
    }
}
