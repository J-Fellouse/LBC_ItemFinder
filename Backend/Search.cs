using ItemFinder_WPF.Backend.Enums;
using ItemFinder_WPF.Backend.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemFinder_WPF.Backend
{
    public class Search
    {
        private Parameters _parameters;
        private bool _searchInTitleOnly;
        private uint _numberOfPages;
        private uint _averageDelayBetweenRequests;
        private Proxy _proxy;

        private Filter _filterDone;
        private int _numberOfFilteredResults;
        private int _numberOfGlobalResults;

        private IDictionary<State, string> stateToConditionString = new Dictionary<State, string>() {
            {State.Good, "Bon état" },
            {State.VeryGood, "Très bon état" },
            {State.BrandNew, "État neuf" },
            {State.Satisfying, "État satisfaisant" },
            {State.ForHardware, "Pour pièces" },
        };

        public Search(Parameters parameters, bool searchInTitleOnly, uint numberOfPages, uint averageDelayBetweenRequests, Proxy proxy = null)
        {
            _parameters = parameters;
            _searchInTitleOnly = searchInTitleOnly;
            _numberOfPages = numberOfPages;
            _averageDelayBetweenRequests = averageDelayBetweenRequests;
            _proxy = proxy;
        }

        public string getInformationString()
        {
            string searchStates = "Nombre de résulats avant filtrage : " + _numberOfGlobalResults.ToString() + "\n";
            searchStates = searchStates + "Nombre de résultats après filtrage : " + _numberOfFilteredResults.ToString() + "\n\n";

            foreach (State state in _parameters.ProductCondition)
            {
                searchStates = searchStates + "Moyenne pour " + stateToConditionString[state] + " : " + Math.Round(_filterDone.getStateMean(state)).ToString() + "€\n";
            }

            return searchStates;
        }

        public List<Ad> Launch(bool debug, string debugFile = null)
        {
            List<Ad> adList = new List<Ad>();

            if(debug && debugFile != null)
            {
                adList = JsonConvert.DeserializeObject<List<Ad>>(File.ReadAllText(debugFile));
            }
            else
            {
                Scraper scraper = new Scraper(_parameters.Query, _searchInTitleOnly, _numberOfPages, _averageDelayBetweenRequests, _proxy);
                adList = scraper.scrape();
            }

            _numberOfGlobalResults = adList.Count;

            return adList;
        }

        public List<Ad> ApplyFilters(List<Ad> ads)
        {
            Filters filters = new Filters();
            filters.SellerGradeFiltering = _parameters.SellerGradeFiltering;
            filters.BlacklistEnabled = _parameters.BlacklistEnabled;
            filters.MeanCalculation = _parameters.MeanCalculation;
            filters.ProductCondition = _parameters.ProductCondition;
            filters.AdSorting = _parameters.AdSorting;
            filters.MinimumSellerGrade = _parameters.MinimumSellerGrade;
            filters.DeliveryAvailable = _parameters.DeliveryAvailable;
            filters.MeanTolerance = new double[] { _parameters.MeanLowerTolerance / 100.0, _parameters.MeanUpperTolerance / 100.0 };
            filters.ExcludeOngoingPurchase = _parameters.ExcludeOngoingPurchase;
            filters.PersonalizedMean = _parameters.PersonalizedMean;
            filters.LocationEnabled = _parameters.LocationEnabled;
            filters.ZipCode = _parameters.ZipCode;
            filters.Blacklist = _parameters.Blacklist;
            filters.MeanToleranceEnabled = _parameters.MeanToleranceEnabled;

            Filter filter = new Filter(ads, filters);

            _filterDone = filter;

            List<Ad> filtered = filter.filter();

            _numberOfFilteredResults = filtered.Count;

            return filtered;
        }
    }
}
