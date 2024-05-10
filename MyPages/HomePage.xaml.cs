using ItemFinder_WPF.MyCustomControls;
using ItemFinder_WPF.MyUserControls;
using ItemFinder_WPF.Backend.Enums;
using ItemFinder_WPF.Backend.Objects;
using ItemFinder_WPF.Backend;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ItemFinder_WPF.Backend.RoutedPageArgs;

namespace ItemFinder_WPF.MyPages
{
    /// <summary>
    /// Logique d'interaction pour HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private Parameters _loadedSavedParameters = null;
        private bool modifySavedParameters = false;
        private NavigationService _nav;

        public HomePage(HomePageArgs pageArgs = null)
        {
            InitializeComponent();
            if(pageArgs != null)
            {
                modifySavedParameters = true;
                _loadedSavedParameters = pageArgs.Parameters;
            }
        }
        
        static IDictionary<IFCheckbox, State> checkBoxConditionStates;
        static IDictionary<IFRadiobutton, FilterBy> radioButtonFilterBy;
        static IDictionary<State, string> stateToConditionString = new Dictionary<State, string>() {
            {State.Good, "Bon état" },
            {State.VeryGood, "Très bon état" },
            {State.BrandNew, "État neuf" },
            {State.Satisfying, "État satisfaisant" },
            {State.ForHardware, "Pour pièces" },
        };

        private ValidationStatus ValidateSearchQuery(IFLargeTextboxIcon control)
        {
            if (!string.IsNullOrEmpty(control.Text))
            {
                if(control.PlaceholderCleared)
                {
                    return new ValidationStatus(true);
                }
            }
            return new ValidationStatus(false, "Le champ de recherche ne peut pas être vide !");
        }

        private ValidationStatus ValidateZipCode(MyUserControls.IFSmallTextbox control)
        {
            if (!control.IsEnabled) { return new ValidationStatus(true); }

                if (!string.IsNullOrEmpty(control.Text))
                {
                    if (control.PlaceholderCleared)
                    {
                        if (control.Text.Length == 5)
                        {
                            if (int.TryParse(control.Text, out _))
                            {
                                return new ValidationStatus(true);
                            }
                        }
                    }
                }
            return new ValidationStatus(false, "Le code postal saisi est invalide !");
        }

        private ValidationStatus ValidatePersonalizedMean(MyUserControls.IFSmallTextbox control)
        {
            if(!control.IsEnabled) {return new ValidationStatus(true); }

            if (!string.IsNullOrEmpty(control.Text))
            {
                if (control.PlaceholderCleared)
                {
                    if (int.TryParse(control.Text, out _))
                    {
                        return new ValidationStatus(true);
                    }
                }
            }
            return new ValidationStatus(false, "La moyenne personalisée saisie est invalide !");
        }

        private void tttt_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;

            ValidationStatus[] validationStatuses = new ValidationStatus[]
            {
                ValidateSearchQuery(SearchQuery), ValidatePersonalizedMean(PersonalizedMean), ValidateZipCode(ZipCode)
            };

            foreach (ValidationStatus status in validationStatuses)
            {
                if(!status.Validated && status.ErrorMessage != null)
                {
                    MessageBox.Show(status.ErrorMessage, "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                    error = true;
                }
            }

            if (error) { return; }

            Parameters parameters = new Parameters();
            parameters.Query = SearchQuery.Text;
            parameters.MeanLowerTolerance = MeanToleranceSlider.LowerValue;
            parameters.MeanUpperTolerance = MeanToleranceSlider.UpperValue;

            parameters.SellerGradeFiltering = (bool)EnableSellerRating.IsChecked;

            if (parameters.SellerGradeFiltering)
            {
                parameters.MinimumSellerGrade = SellerRating.Value;
            }

            parameters.DeliveryAvailable = (bool)EnableShipment.IsChecked;
            parameters.ExcludeOngoingPurchase = (bool)ExcludeOngoingPurchaseEnabled.IsChecked;
            List<State> localState = new List<State>();
            bool unselectedStates = true;

            uint count = 0;

            foreach (var item in checkBoxConditionStates)
            {
                if ((bool)item.Key.IsChecked)
                {
                    unselectedStates = false;
                    localState.Add(item.Value);
                    count++;
                }
            }

            if (unselectedStates)
            {
                localState = new List<State>() { State.Satisfying, State.ForHardware, State.BrandNew, State.VeryGood, State.Good };
            }

            parameters.ProductCondition = localState;

            parameters.AdSorting = FilterBy.Recent;
            if ((bool)SortByPertinence.IsChecked)
            {
                parameters.AdSorting = FilterBy.Relevance;
            }

            parameters.LocationEnabled = (bool)EnableLocation.IsChecked;
            if (parameters.LocationEnabled)
            {
                parameters.ZipCode = Convert.ToUInt32(ZipCode.Text);
            }

            parameters.MeanCalculation = MeanCalculation.Automatic;
            if ((bool)EnablePersonalizedMean.IsChecked)
            {
                parameters.MeanCalculation = MeanCalculation.Personalized;
                parameters.PersonalizedMean = Convert.ToInt32(PersonalizedMean.Text);
            }

            parameters.SaveSearch = (bool)SaveResearchEnabled.IsChecked;
            parameters.CreateAlert = (bool)CreateAlertEnabled.IsChecked;
            parameters.BlacklistEnabled = (bool)BlacklistEnabled.IsChecked;
            parameters.MeanToleranceEnabled = (bool)EnableMeanTolerance.IsChecked;

            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();

            if ((bool)SortByDate.IsChecked)
            {
                parameters.AdSorting = FilterBy.Recent;
            }
            else if((bool)SortByPertinence.IsChecked)
            {
                parameters.AdSorting = FilterBy.Relevance;
            }
            else if((bool)SortByPriceAscending.IsChecked)
            {
                parameters.AdSorting = FilterBy.AscendingPrice;
            }
            else
            {
                parameters.AdSorting = FilterBy.DescendingPrice;
            }

            if (parameters.BlacklistEnabled)
            {
                SaveBlacklistHandler saveBlacklistHandler = new SaveBlacklistHandler("Configuration/Blacklist.json");
                parameters.Blacklist = saveBlacklistHandler.getEntries();
            }

            if (modifySavedParameters)
            {
                parameters.UUID = _loadedSavedParameters.UUID;
                SaveSearchHandler saveSearchHandler = new SaveSearchHandler("Configuration/SavedSearches.json", parameters);
                saveSearchHandler.replaceEntry(_loadedSavedParameters.UUID);

                _nav.Navigate(new SavedSearchesAndAlertsPage());

                resetPageToDefault();

                return;
            }

            parameters.UUID = myuuidAsString;

            if ((bool)SaveResearchEnabled.IsChecked)
            {
                SaveSearchHandler saveSearchHandler = new SaveSearchHandler("Configuration/SavedSearches.json", parameters);
                saveSearchHandler.handleNewSaveSearch();
            }

            ResultsPageArgs resultsPageArgs = new ResultsPageArgs(parameters);
            _nav.Navigate(new ResultsPage(resultsPageArgs));
        }

        private void resetPageToDefault()
        {
            this.DataContext = null;
            modifySavedParameters = false;
            _loadedSavedParameters = null;

            SearchQuery.Text = null;
            SearchQuery.Placeholder = "Tapez ici vots mots clés...";
            SearchQuery.PlaceholderCleared = false;

            EnableSellerRating.IsChecked = false;
            SellerRating.Value = 0;
            SellerRating.Opacity = 0.7;
            SellerRating.IsEnabled = false;

            EnableLocation.IsChecked = false;
            ZipCode.IsEnabled = false;
            ZipCode.Text = null;
            ZipCode.Placeholder = "Ex : 75000";
            ZipCode.PlaceholderCleared = false;

            BrandNewState.IsChecked = false;
            GoodState.IsChecked = false;
            ForHardware.IsChecked = false;
            SatisfyingState.IsChecked = false;
            VeryGoodState.IsChecked = false;

            SortByPertinence.IsChecked = true;
            SortByDate.IsChecked = false;
            SortByPriceAscending.IsChecked = false;
            SortByPriceDescending.IsChecked = false;
            
            ExcludeOngoingPurchaseEnabled.IsChecked = false;

            EnableMeanTolerance.IsChecked = true;
            MeanToleranceSlider.IsEnabled = true;
            MeanToleranceSlider.Opacity = 1;
            MeanToleranceSlider.LowerValue = -50;
            MeanToleranceSlider.UpperValue = 50;

            EnableAutomaticMean.IsChecked = true;
            EnablePersonalizedMean.IsChecked = false;
            PersonalizedMean.IsEnabled = false;
            PersonalizedMean.Text = "Ex : 100";

            EnableShipment.IsChecked = false;

            SaveResearchEnabled.IsEnabled= true;
            SaveResearchEnabled.IsChecked = false;

            BlacklistEnabled.IsChecked = false;

            MainTitle.Text = "Que souhaitez vous rechercher ?";
            SearchButton.Content = "Rechercher";
        }

        private void EnableLocation_Checked(object sender, RoutedEventArgs e)
        {
            ZipCode.IsEnabled = true;
        }

        private void EnableLocation_Unchecked(object sender, RoutedEventArgs e)
        {
            ZipCode.IsEnabled = false;
        }

        private void EnableSellerRating_Checked(object sender, RoutedEventArgs e)
        {
            SellerRating.Opacity = 1;
            SellerRating.IsEnabled = true;
        }

        private void EnableSellerRating_Unchecked(object sender, RoutedEventArgs e)
        {
            SellerRating.Opacity = 0.7;
            SellerRating.IsEnabled = false;
        }

        private void EnablePersonalizedMean_Checked(object sender, RoutedEventArgs e)
        {
            PersonalizedMean.IsEnabled = true;
        }

        private void EnablePersonalizedMean_Unchecked(object sender, RoutedEventArgs e)
        {
           PersonalizedMean.IsEnabled = false;
        }

        private void CreateAlertEnabled_Checked(object sender, RoutedEventArgs e)
        {
            SaveResearchEnabled.IsChecked = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _nav = NavigationService.GetNavigationService(this);
            checkBoxConditionStates = new Dictionary<IFCheckbox, State>() {
                {BrandNewState, State.BrandNew},
                {VeryGoodState, State.VeryGood },
                {GoodState, State.Good },
                {SatisfyingState, State.Satisfying },
                {ForHardware, State.ForHardware }
            };

            if(modifySavedParameters)
            {
                SearchButton.Content = "Modifier";
                MainTitle.Text = "Que souhaitez vous modifier ?";
                SearchQuery.Text = _loadedSavedParameters.Query;
                BrandNewState.IsChecked = _loadedSavedParameters.ProductCondition.Contains(State.BrandNew);
                VeryGoodState.IsChecked = _loadedSavedParameters.ProductCondition.Contains(State.VeryGood);
                GoodState.IsChecked = _loadedSavedParameters.ProductCondition.Contains(State.Good);
                SatisfyingState.IsChecked = _loadedSavedParameters.ProductCondition.Contains(State.Satisfying);
                ForHardware.IsChecked = _loadedSavedParameters.ProductCondition.Contains(State.ForHardware);

                switch(_loadedSavedParameters.AdSorting)
                {
                    case FilterBy.AscendingPrice:
                        SortByPriceAscending.IsChecked = true;
                        break;
                    case FilterBy.DescendingPrice:
                        SortByPriceDescending.IsChecked = true;
                        break;
                    case FilterBy.Recent:
                        SortByDate.IsChecked = true;
                        break;
                    default:
                        SortByPertinence.IsChecked = true;
                        break;
                }

                EnableLocation.IsChecked = _loadedSavedParameters.LocationEnabled;
                if(_loadedSavedParameters.LocationEnabled)
                {
                    ZipCode.Text = _loadedSavedParameters.ZipCode.ToString();
                }

                ExcludeOngoingPurchaseEnabled.IsChecked = _loadedSavedParameters.ExcludeOngoingPurchase;

                EnableSellerRating.IsChecked = _loadedSavedParameters.SellerGradeFiltering;

                if(_loadedSavedParameters.SellerGradeFiltering)
                {
                    SellerRating.IsEnabled = true;
                    SellerRating.Opacity = 1;
                    SellerRating.Value = _loadedSavedParameters.MinimumSellerGrade;
                }

                EnableMeanTolerance.IsChecked = true;
                MeanToleranceSlider.LowerValue = _loadedSavedParameters.MeanLowerTolerance;
                MeanToleranceSlider.UpperValue = _loadedSavedParameters.MeanUpperTolerance;

                if (!_loadedSavedParameters.MeanToleranceEnabled)
                {
                    EnableMeanTolerance.IsChecked = false;
                    MeanToleranceSlider.LowerValue = -50;
                    MeanToleranceSlider.UpperValue = 50;
                }


                

                switch(_loadedSavedParameters.MeanCalculation)
                {
                    case MeanCalculation.Personalized:
                        EnablePersonalizedMean.IsChecked = true;
                        PersonalizedMean.Text = _loadedSavedParameters.PersonalizedMean.ToString();
                        break;
                    default:
                        EnableAutomaticMean.IsChecked = true;
                        break;
                }

                EnableShipment.IsChecked = _loadedSavedParameters.DeliveryAvailable;
                SaveResearchEnabled.IsChecked = true;
                SaveResearchEnabled.IsEnabled = false;
                BlacklistEnabled.IsChecked = _loadedSavedParameters.BlacklistEnabled;
            }
        }

        private void EnableMeanTolerance_Checked(object sender, RoutedEventArgs e)
        {
            if(MeanToleranceSlider != null)
            {
                MeanToleranceSlider.IsEnabled = true;
                MeanToleranceSlider.Opacity = 1;
            }
        }

        private void EnableMeanTolerance_Unchecked(object sender, RoutedEventArgs e)
        {
            if(MeanToleranceSlider != null)
            {
                MeanToleranceSlider.IsEnabled = false;
                MeanToleranceSlider.Opacity = 0.7;
            }
        }
    }
}
