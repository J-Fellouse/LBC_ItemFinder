using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemFinder_WPF.Backend.Enums;

namespace ItemFinder_WPF.Backend.Objects
{
    public class Ad
    {
        public string Name;
        public double Price;
        public State State;
        public uint SellerReviewCount;
        public double SellerReviewGrade;
        public string SellerName;
        public DateTime Date;
        public Uri Url;
        public bool IsDeliveryAvailable;
        public Location Location;
        public bool OngoingPurchase; //achat en cours
        public double Relevance;
        public string ImageUrl;
    }
}
