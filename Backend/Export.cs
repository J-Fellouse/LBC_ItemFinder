using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ItemFinder_WPF.Backend.Objects;
using System.IO;

namespace ItemFinder_WPF.Backend
{
    public class Export
    {
        private string _path;
        private List<Ad> _ads;
        public Export(string path, List<Ad> ads)
        {
            _path = path;
            _ads = ads;
        }

        private ExportAd _convertSingleToExportAd(Ad ad)
        {
            ExportAd eAd = new ExportAd();

            eAd.Name = ad.Name;
            eAd.Price = ad.Price;
            eAd.Date = ad.Date.ToString();
            eAd.IsDeliveryAvailable = ad.IsDeliveryAvailable;
            eAd.OngoingPurchase = ad.OngoingPurchase;
            eAd.State = ad.State.ToString();
            eAd.City = ad.Location.City;
            eAd.ZipCode = (int)ad.Location.ZipCode;
            eAd.SellerName = ad.SellerName;
            eAd.SellerReviewCount = (int)ad.SellerReviewCount;
            eAd.SellerReviewGrade = ad.SellerReviewGrade;
            eAd.Url = ad.Url.OriginalString;

            return eAd;
        }

        private List<ExportAd> _convertAdsToExportAds(List<Ad> ads)
        {
            List<ExportAd> eAds = new List<ExportAd>();

            foreach(Ad ad in ads)
            {
                eAds.Add(_convertSingleToExportAd(ad));
            }

            return eAds;
        }

        public void save()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };
            using (FileStream fs = new FileStream(_path, FileMode.Create))
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(_convertAdsToExportAds(_ads));
            }
        }
    }
}
