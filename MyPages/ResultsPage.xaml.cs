using ItemFinder_WPF.Backend;
using ItemFinder_WPF.Backend.Enums;
using ItemFinder_WPF.Backend.FrontendExporters;
using ItemFinder_WPF.Backend.Objects;
using ItemFinder_WPF.Backend.RoutedPageArgs;
using ItemFinder_WPF.MyCustomControls;
using ItemFinder_WPF.MyUserControls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ItemFinder_WPF.MyPages
{
    /// <summary>
    /// Logique d'interaction pour ResultsPage.xaml
    /// </summary>
    public partial class ResultsPage : Page
    {
        Parameters lastParameters = null;
        Parameters currentParameters = null;
        List<Ad> adToDisplay = null;
        List<Ad> globalAds = null; //all brute ads without any filters
        DispatcherTimer loadingTextTimer;
        int loadingTextCount = 0;

        string searchInformation = null;

        public ResultsPage(ResultsPageArgs pageArgs = null)
        {
            InitializeComponent();

            loadingTextTimer = new DispatcherTimer();
            loadingTextTimer.Interval = TimeSpan.FromMilliseconds(500);
            loadingTextTimer.Tick += loadingTextTimer_Tick;

            if (pageArgs != null)
            {
                currentParameters = pageArgs.Parameters;
            }
        }

        private void loadingTextTimer_Tick(object? sender, EventArgs e)
        {
            if(loadingTextCount > 2)
            {
                loadingTextCount = 0;
                LoadingText.Text = "Chargement";
            }
            else
            {
                LoadingText.Text = LoadingText.Text + ".";
                loadingTextCount++;
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private void IFButton_Click(object sender, RoutedEventArgs e)
        {
            IFButton currentButton = (IFButton)sender;
            OpenUrl(currentButton.Tag.ToString());
        }

        private void IFButton_Click_1(object sender, RoutedEventArgs e)
        {
            //Informations sur la recherche
            //MessageBox.Show(researchInformation);
            if(searchInformation != null)
            {
                MessageBox.Show(searchInformation, "Informations sur la recherche", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MessageBox.Show("Impossible d'afficher les informations sur la recherche si celle-ci n'a pas encore été effectuée", "Aucune recherche effectuée", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SortByRelevance_Checked(object sender, RoutedEventArgs e)
        {
            if (adToDisplay != null)
            {
                if (adToDisplay.Count > 0)
                {
                    adToDisplay = adToDisplay.OrderByDescending(fAd => fAd.Relevance).ToList();
                    updateList(adToDisplay);

                }
            }
        }

        private void SortByAscendingPrice_Checked(object sender, RoutedEventArgs e)
        {
            if (adToDisplay != null)
            {
                if (adToDisplay.Count > 0)
                {
                    adToDisplay = adToDisplay.OrderBy(fAd => fAd.Price).ToList();
                    updateList(adToDisplay);
                }
            }
        }

        private void SortByDescendingPrice_Checked(object sender, RoutedEventArgs e)
        {
            if (adToDisplay != null)
            {
                if (adToDisplay.Count > 0)
                {
                    adToDisplay = adToDisplay.OrderByDescending(fAd => fAd.Price).ToList();
                    updateList(adToDisplay);
                }
            }

        }

        private void SortByDate_Checked(object sender, RoutedEventArgs e)
        {
            if (adToDisplay != null)
            {
                if (adToDisplay.Count > 0)
                {
                    adToDisplay = adToDisplay.OrderByDescending(fAd => fAd.Date).ToList();
                    updateList(adToDisplay);
                }
            }
        }

        private void updateList(List<Ad> updatedAdList)
        {
            adList.ItemsSource = "";

            FrontendAdsConverter frontendAdsConverter = new FrontendAdsConverter(updatedAdList);
            List<FrontendAd> frontendAds = frontendAdsConverter.frontendExport();

            adList.ItemsSource = frontendAds;
        }

        private void updateSortRadioButtons()
        {
            switch(currentParameters.AdSorting)
            {
                case FilterBy.AscendingPrice:
                    SortByAscendingPrice.IsChecked = true;
                    break;
                case FilterBy.DescendingPrice:
                    SortByDescendingPrice.IsChecked = true;
                    break;
                case FilterBy.Recent:
                    SortByDate.IsChecked = true;
                    break;
                default:
                    SortByRelevance.IsChecked = true;
                    break;
            }
        }

        private void showLoadingAnimation()
        {
            LoadingAnimation.Visibility = Visibility.Visible;
            LoadingText.Visibility = Visibility.Visible;
            loadingTextTimer.Start();
        }

        private void hideLoadingAnimation()
        {
            LoadingAnimation.Visibility = Visibility.Hidden;
            LoadingText.Visibility = Visibility.Hidden;
            loadingTextTimer.Stop();
        }

        private void initPage()
        {
            if (currentParameters != null)
            {
                if (currentParameters != lastParameters)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        showLoadingAnimation();
                        updateSortRadioButtons();
                    }), DispatcherPriority.Background);


                    Search search = new Search(currentParameters, true, 2, 2500);
                    globalAds = search.Launch(false, "debug.json");

                    adToDisplay = search.ApplyFilters(globalAds);

                    searchInformation = search.getInformationString();

                    lastParameters = currentParameters;

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        updateList(adToDisplay);
                        hideLoadingAnimation();
                    }), DispatcherPriority.Background);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Thread pageInitialization = new Thread(initPage);
            pageInitialization.IsBackground = true;
            pageInitialization.Start();
        }
    }
}
