using ItemFinder_WPF.Backend;
using ItemFinder_WPF.Backend.Enums;
using ItemFinder_WPF.Backend.FrontendExporters;
using ItemFinder_WPF.Backend.Objects;
using ItemFinder_WPF.Backend.RoutedPageArgs;
using ItemFinder_WPF.MyCustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ItemFinder_WPF.MyPages
{
    /// <summary>
    /// Logique d'interaction pour SavedSearchesAndAlertsPage.xaml
    /// </summary>
    public partial class SavedSearchesAndAlertsPage : Page
    {
        List<FrontendSavedSearch> frontendSavedSearches = new List<FrontendSavedSearch>();
        List<Parameters> savedParameters = new List<Parameters>();

        SaveSearchHandler saveSearchHandler;

        

        public SavedSearchesAndAlertsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            frontendSavedSearches.Clear();

            saveSearchHandler = new SaveSearchHandler("Configuration/SavedSearches.json");
            if (saveSearchHandler.isConfigurationFile())
            {
                List<Parameters> parameters = saveSearchHandler.getEntries();
                savedParameters = parameters;

                FrontendSavedSearchesConverter frontendSavedSearchesConverter = new FrontendSavedSearchesConverter(parameters);
                frontendSavedSearches = frontendSavedSearchesConverter.frontendExport();

                savedSearchAndAlertsList.ItemsSource = "";
                savedSearchAndAlertsList.ItemsSource = frontendSavedSearches;
            }
        }

        private void DeleteSaved_Click(object sender, RoutedEventArgs e)
        {
            IFButton currentButton = (IFButton)sender;
            string uuid = currentButton.Tag.ToString();

            int index = frontendSavedSearches.IndexOf(frontendSavedSearches.Where(fsavedSearch => fsavedSearch.UUID == uuid).ToList()[0]);

            if (index != -1)
            {
                frontendSavedSearches.RemoveAt(index);
                savedParameters.RemoveAt(index);
            }

            saveSearchHandler.removeEntry(uuid);

            savedSearchAndAlertsList.ItemsSource = "";
            savedSearchAndAlertsList.ItemsSource = frontendSavedSearches;
        }

        private void SearchFromSaved_Click(object sender, RoutedEventArgs e)
        {
            IFButton currentButton = (IFButton)sender;
            string uuid = currentButton.Tag.ToString();

            int index = savedParameters.IndexOf(savedParameters.Where(param => param.UUID == uuid).ToList()[0]);

            if(index != -1)
            {
                Parameters parameters = savedParameters[index];

                NavigationService nav = NavigationService.GetNavigationService(this);
                ResultsPageArgs resultsPageArgs = new ResultsPageArgs(parameters);
                nav.Navigate(new ResultsPage(resultsPageArgs));
            }
        }

        private void ModifySaved_Click(object sender, RoutedEventArgs e)
        {
            IFButton currentButton = (IFButton)sender;
            string uuid = currentButton.Tag.ToString();

            int index = savedParameters.IndexOf(savedParameters.Where(param => param.UUID == uuid).ToList()[0]);

            if (index != -1)
            {
                Parameters parameters = savedParameters[index];

                NavigationService nav = NavigationService.GetNavigationService(this);
                HomePageArgs homePageArgs = new HomePageArgs(parameters);
                nav.Navigate(new HomePage(homePageArgs));
            }
        }
    }
}
