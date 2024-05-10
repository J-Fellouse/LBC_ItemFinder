using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using ItemFinder_WPF.Backend.Enums;
using ItemFinder_WPF.Backend.Objects;

namespace ItemFinder_WPF.Backend
{
    public class Filter
    {
        private List<Ad> _ads;
        private Filters _filters;
        private IDictionary<State, double> _stateMean = new Dictionary<State, double>();

        public Filter(List<Ad> ads, Filters filters)
        {
            _ads = ads;
            _filters = filters;
        }

        public double getStateMean(State state)
        {
            if(!_stateMean.ContainsKey(state))
            {
                return 0;
            }
            return _stateMean[state];
        }

        private double _calculateMean(List<Ad> adList)
        {
            List<double> prices = adList.Select(o => o.Price).ToList();
            return Convert.ToDouble(prices.Average());
        }

        public List<Ad> filter()
        {
            List<Ad> filteredAds = new List<Ad>();
            List<Ad> concatenatedStates = new List<Ad>();

            foreach (State selectedState in _filters.ProductCondition)
            {
                List<Ad> currentStateAds = _ads.Where(ad => ad.State == selectedState).ToList();
                if (currentStateAds != null)
                {
                    if (currentStateAds.Count > 0)
                    {
                        double currentStateMean;

                        if (_filters.MeanCalculation == MeanCalculation.Personalized)
                        {
                            currentStateMean = _filters.PersonalizedMean;
                        }
                        else
                        {
                            currentStateMean = _calculateMean(currentStateAds);
                        }

                        List<Ad> filteredCurrentStateAds = new List<Ad>();

                        filteredCurrentStateAds = currentStateAds;
                        
                        if(_filters.MeanToleranceEnabled == true)
                        {
                            filteredCurrentStateAds = new List<Ad>();
                            filteredCurrentStateAds = currentStateAds.Where(ad =>
                        (ad.Price > currentStateMean * (1 + _filters.MeanTolerance[0])) &&
                        (ad.Price < currentStateMean * (1 + _filters.MeanTolerance[1]))).ToList();
                        }

                        filteredCurrentStateAds.ForEach(ad => ad.Relevance = 100 - Math.Abs((currentStateMean - ad.Price) / currentStateMean) * 100);

                        _stateMean[selectedState] = currentStateMean;

                        concatenatedStates = concatenatedStates.Concat(filteredCurrentStateAds).ToList();
                    }
                }
            }

            if (concatenatedStates != null)
            {
                if(concatenatedStates.Count > 0)
                {
                    filteredAds = concatenatedStates.Where(ad =>
                    (_filters.SellerGradeFiltering == false || ad.SellerReviewGrade >= _filters.MinimumSellerGrade) &&
                    (_filters.DeliveryAvailable == false || ad.IsDeliveryAvailable == true) &&
                    (_filters.ExcludeOngoingPurchase == false || ad.OngoingPurchase == false) &&
                    (_filters.LocationEnabled == false || Convert.ToInt32(ad.Location.ZipCode / 1000) == Convert.ToInt32(_filters.ZipCode / 1000)) &&
                    (_filters.BlacklistEnabled == false || _filters.Blacklist.Any(ad.Name.ToLower().Contains) == false)).ToList();

                    switch(_filters.AdSorting)
                    {
                        case FilterBy.AscendingPrice:
                            filteredAds = filteredAds.OrderBy(ads => ads.Price).ToList();
                            break;
                        case FilterBy.DescendingPrice:
                            filteredAds = filteredAds.OrderByDescending(ads => ads.Price).ToList();
                            break;
                        case FilterBy.Recent:
                            filteredAds = filteredAds.OrderByDescending(ads => ads.Date).ToList();
                            break;
                        default:
                            filteredAds = filteredAds.OrderByDescending(ads => ads.Relevance).ToList();
                            break;
                    }
                }
            }
            return filteredAds;
        }
    }
}
