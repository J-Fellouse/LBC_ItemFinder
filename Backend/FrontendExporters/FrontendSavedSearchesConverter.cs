using ItemFinder_WPF.Backend.Enums;
using ItemFinder_WPF.Backend.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ItemFinder_WPF.Backend.FrontendExporters
{
    public class FrontendSavedSearchesConverter
    {
        private List<Parameters> _parameters;

        private IDictionary<State, string> _stateToConditionString = new Dictionary<State, string>() {
            {State.Good, "Bon état" },
            {State.VeryGood, "Très bon état" },
            {State.BrandNew, "État neuf" },
            {State.Satisfying, "État satisfaisant" },
            {State.ForHardware, "Pour pièces" },

        };

        public FrontendSavedSearchesConverter(List<Parameters> parameters)
        {
            _parameters = parameters;
        }

        public List<FrontendSavedSearch> frontendExport()
        {
            List<FrontendSavedSearch> localFrontendSavedSearches = new List<FrontendSavedSearch>();

            foreach (Parameters parameter in _parameters)
            {
                FrontendSavedSearch frontendSavedSearch = new FrontendSavedSearch();

                frontendSavedSearch.UUID = parameter.UUID;
                frontendSavedSearch.Nom = parameter.Query;

                string tempEtats = "";

                foreach (State state in parameter.ProductCondition)
                {
                    tempEtats = tempEtats + _stateToConditionString[state] + ", ";
                }
                tempEtats = tempEtats.Substring(0, tempEtats.Length - 2);
                frontendSavedSearch.Etats = tempEtats;

                frontendSavedSearch.Livraison = "Livraison possible ou impossible";
                if (parameter.DeliveryAvailable)
                {
                    frontendSavedSearch.Livraison = "Uniquement si livraison";
                }

                frontendSavedSearch.EcartMoyenne = "Écart avec la moyenne : " + Convert.ToInt32(parameter.MeanLowerTolerance).ToString() + "% à " + Convert.ToInt32(Math.Abs(parameter.MeanUpperTolerance)).ToString() + "%";
                if(parameter.MeanToleranceEnabled == false)
                {
                    frontendSavedSearch.EcartMoyenne = "Exclusion selon l'écart à la moyenne désactivée";
                }

                frontendSavedSearch.CalculMoyenne = "Moyenne automatique";
                if (parameter.MeanCalculation == MeanCalculation.Personalized)
                {
                    frontendSavedSearch.CalculMoyenne = "Moyenne personalisée : " + parameter.PersonalizedMean.ToString() + "€";
                }

                frontendSavedSearch.Localisation = "Localisation : Toute la france";
                if (parameter.LocationEnabled)
                {
                    frontendSavedSearch.Localisation = "Localisation : " + parameter.ZipCode.ToString();
                }

                localFrontendSavedSearches.Add(frontendSavedSearch);
            }

            return localFrontendSavedSearches;
        }
    }
}
