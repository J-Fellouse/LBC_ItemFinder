using ItemFinder_WPF.Backend.Enums;
using ItemFinder_WPF.Backend.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ItemFinder_WPF.Backend.FrontendExporters
{
    public class FrontendAdsConverter
    {
        private List<Ad> _ads;

        private IDictionary<State, string> _stateToConditionString = new Dictionary<State, string>() {
            {State.Good, "Bon état" },
            {State.VeryGood, "Très bon état" },
            {State.BrandNew, "État neuf" },
            {State.Satisfying, "État satisfaisant" },
            {State.ForHardware, "Pour pièces" },

        };

        public FrontendAdsConverter(List<Ad> ads)
        {
            _ads = ads;
        }


        private string prettyDateDifferencePrint(DateTime date)
        {
            DateTime nowDate = DateTime.Now;
            TimeSpan Diff = nowDate - date;

            string output = "Posté il y a ";

            int tSeconds = (int)Diff.TotalSeconds;
            int tMinutes = (int)Diff.TotalMinutes;
            int tHours = (int)Diff.TotalHours;
            int tDays = (int)Diff.TotalDays;

            if (tSeconds < 60 && tSeconds > 0)
            {
                output = output + tSeconds + " seconde";
                if (tSeconds > 1)
                {
                    output = output + "s";
                }
            }

            if (tMinutes < 60 && tMinutes > 0)
            {
                output = output + tMinutes + " minute";
                if (tMinutes > 1)
                {
                    output = output + "s";
                }
            }

            if (tHours < 24 && tHours > 0)
            {
                output = output + tHours + " heure";
                if (tHours > 1)
                {
                    output = output + "s";
                }
            }

            if (tDays < 31 && tDays > 0)
            {
                output = output + tDays + " jour";
                if (tDays > 1)
                {
                    output = output + "s";
                }
            }

            if (tDays >= 31)
            {
                output = output + (tDays / 31).ToString() + " mois";
                int remainingDays = tDays % 31;

                if (remainingDays != 0)
                {
                    output = output + " et " + remainingDays.ToString() + " jour";
                    if (remainingDays > 1)
                    {
                        output = output + "s";
                    }
                }
            }

            return output;
        }

        private string removeIfTooLong(string input, int max)
        {
            if(input.Length >= max)
            {
                return input.Substring(0,max-3) + "...";
            }
            return input;
        }

        public List<FrontendAd> frontendExport()
        {
            List<FrontendAd> localFrontendAds = new List<FrontendAd>();

            foreach (Ad ad in _ads)
            {
                FrontendAd localfAd = new FrontendAd();

                localfAd.Nom = removeIfTooLong(ad.Name, 75);
                localfAd.Prix = ad.Price.ToString() + "€";
                localfAd.Date = prettyDateDifferencePrint(ad.Date);
                localfAd.UnparsedDate = ad.Date;
                localfAd.UnparsedPrix = ad.Price;
                localfAd.UnparsedRelevance = ad.Relevance;
                localfAd.Etat = _stateToConditionString[ad.State];
                localfAd.Ville = ad.Location.City + ", " + ad.Location.ZipCode.ToString();
                localfAd.Relevance = "Pertinence : " + Math.Round(ad.Relevance).ToString() + "%";
                localfAd.Livraison = "Livraison indisponible";
                localfAd.Link = ad.Url.ToString();

                if (ad.ImageUrl != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(ad.ImageUrl); ;
                    bitmapImage.EndInit();

                    localfAd.Image = bitmapImage;
                }
                else
                {
                    localfAd.Image = (BitmapImage)Application.Current.FindResource("BlankImage");
                }


                if (ad.IsDeliveryAvailable)
                {
                    localfAd.Livraison = "Livraison disponible";
                }

                localfAd.Vendeur = "Vendeur : " + removeIfTooLong(ad.SellerName,12);
                localfAd.Note = "Note : " + ad.SellerReviewGrade.ToString() + "/5";

                localFrontendAds.Add(localfAd);
            }

            return localFrontendAds;
        }
    }
}
