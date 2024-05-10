using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using ItemFinder_WPF.Backend.Enums;
using ItemFinder_WPF.Backend.Objects;

namespace ItemFinder_WPF.Backend
{
    public class Parser
    {
        private const string priceRegex = @"Prix: (.*) €.";
        private const string locationRegex = @"(.*?)\s(\d{5})";

        private UnparsedAd _unparsedAd;
        
        public Parser(UnparsedAd unparsedAd)
        {
            _unparsedAd = unparsedAd;
        }

        public Ad parseAd()
        {
            Ad ad = new Ad();
            ad.Name = _unparsedAd.Name;
            ad.Price = _parsePrice(_unparsedAd.Price);
            ad.Location = _parseLocation(_unparsedAd.Location);
            ad.State = _parseState(_unparsedAd.State);
            ad.Date = _parseDate(_unparsedAd.Date);
            ad.IsDeliveryAvailable = _parseIsDeliveryAvailable(_unparsedAd.IsDeliveryAvailable);
            ad.Url = _parseUri(_unparsedAd.Url);
            ad.SellerName = _unparsedAd.SellerName;
            ad.SellerReviewCount = _parseSellerReviewCount(_unparsedAd.SellerReviewCount);
            ad.SellerReviewGrade = _parseSellerReviewGrade(_unparsedAd.SellerReviewGrade);
            ad.OngoingPurchase = _parseOngoingPurchase(_unparsedAd.IsDeliveryAvailable);
            ad.ImageUrl = _unparsedAd.ImageUrl;

            return ad;
        }

        private double _parsePrice(string scrapedPrice)
        {
            var match = Regex.Match(scrapedPrice, priceRegex);
            double parsedPrice = 0;

            parsedPrice = double.Parse(match.Groups[1].Value);

            return parsedPrice;
        }

        private bool _parseOngoingPurchase(string scrapedDelivery)
        {
            if(scrapedDelivery == "Achat en cours")
            {
                return true;
            }
            return false;
        }

        private bool _parseIsDeliveryAvailable(string scrapedDelivery)
        {
            if(scrapedDelivery == string.Empty)
            {
                return false;
            }
            return true;
        }

        private State _parseState(string scrapedState)
        {
            switch(scrapedState)
            {
                case "Très bon état":
                    return State.VeryGood;
                    break;
                case "État satisfaisant":
                    return State.Satisfying;
                    break;
                case "Bon état":
                    return State.Good;
                    break;
                case "État neuf":
                    return State.BrandNew;
                    break;
                default:
                    return State.ForHardware;
                    break;
            }
        }

        private Uri _parseUri(string scrapedUri)
        {
            UriBuilder uriBuilder = new UriBuilder()
            {
                Scheme = "https",
                Host = "leboncoin.fr",
                Path = scrapedUri
            };

            return uriBuilder.Uri;
        }

        private DateTime _parseDate(string scrapedDate)
        {
            DateTime dt;
            dt = new DateTime(1970, 1, 1);

            if (scrapedDate != null)
            {
                if(scrapedDate != string.Empty)
                {
                    string dateString = scrapedDate;
                    dt = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }
            }

            return dt;
        }

        private uint _parseSellerReviewCount(string scrapedSellerReviewCount)
        {
            if(scrapedSellerReviewCount != string.Empty)
            {
                string parsedSellerReviewCount = string.Empty;
                parsedSellerReviewCount = scrapedSellerReviewCount.Replace("(", "").Replace(")", "");
                return uint.Parse(parsedSellerReviewCount);
            }
            else
            {
                return (uint)0;
            }
        }

        private double _parseSellerReviewGrade(string scrapedSellerReviewGrade)
        {
            if(scrapedSellerReviewGrade != string.Empty)
            {
                string newSellerReviewGrade = scrapedSellerReviewGrade.Replace(".", ",");
                return double.Parse(newSellerReviewGrade);
            }

            return (double)0;
        }

        private Location _parseLocation(string scrapedLocation)
        {
            Location location = new Location();

            var match = Regex.Match(scrapedLocation, locationRegex);
            string city = match.Groups[1].Value;
            uint zipCode = uint.Parse(match.Groups[2].Value);

            location.City = city;
            location.ZipCode = zipCode;

            return location;
        }
    }
}
