using ItemFinder_WPF.MyPages;
using ItemFinder_WPF.MyUserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ItemFinder_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IDictionary<IFIconButton, Page> buttonPages;

        IFIconButton currentSelected;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public MainWindow()
        {
            InitializeComponent();
            //Evenement lorsque le fenêtre est chargé
            MyMainWindow.Loaded += MyMainWindow_Loaded;

            //Evenements pour les boutons de switch de pages
            HomeButton.MouseLeftButtonUp += IFIconButton_MouseLeftButtonUp;
            ResultsButton.MouseLeftButtonUp += IFIconButton_MouseLeftButtonUp;
            SavedSearchesAndAlertsButton.MouseLeftButtonUp += IFIconButton_MouseLeftButtonUp;
            BlacklistButton.MouseLeftButtonUp += IFIconButton_MouseLeftButtonUp;
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Pour gérer le fait de pouvoir faire bouger la fenêtre en laissant enfoncer le clic gauche
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                ReleaseCapture();
                SendMessage(new WindowInteropHelper(MyMainWindow).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void IFIconButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Petite barre pour minimiser la fenêtre
            MyMainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
            MyMainWindow.WindowState = WindowState.Minimized;
            MyMainWindow.WindowStyle = WindowStyle.None;
        }

        private void IFIconButton_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            //Croix pour fermer la fenêtre
            System.Windows.Application.Current.Shutdown();
        }

        private void IFIconButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Quand un bouton est cliqué pour switch la page
            IFIconButton iconButton = (IFIconButton)sender;
            if(!iconButton.Selected)
            {
                MyFrame.Content = buttonPages[iconButton];
            }
        }

        private void startupConfiguration()
        {
            if(!Directory.Exists("Configuration"))
            {
                Directory.CreateDirectory("Configuration");
            }
        }

        private void MyMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            startupConfiguration();

            buttonPages = new Dictionary<IFIconButton, Page>() {
                {HomeButton, new HomePage() },
                {ResultsButton, new ResultsPage() },
                {SavedSearchesAndAlertsButton, new SavedSearchesAndAlertsPage() },
                {BlacklistButton, new BlacklistPage() }
            };

            currentSelected = HomeButton;
            MyFrame.Content = buttonPages[currentSelected];
        }

        private void MyFrame_ContentRendered(object sender, EventArgs e)
        {
            object loadedPage = MyFrame.Content;
            string currentName = loadedPage.GetType().Name;

            currentSelected.Selected = false;

            switch (currentName)
            {
                case "HomePage":
                    HomeButton.Selected = true;
                    currentSelected = HomeButton;
                    break;
                case "ResultsPage":
                    ResultsButton.Selected = true;
                    currentSelected = ResultsButton;
                    break;
                case "SavedSearchesAndAlertsPage":
                    SavedSearchesAndAlertsButton.Selected = true;
                    currentSelected = SavedSearchesAndAlertsButton;
                    break;
                case "BlacklistPage":
                    BlacklistButton.Selected = true;
                    currentSelected = BlacklistButton;
                    break;
                default:
                    HelpButton.Selected = true;
                    currentSelected = HelpButton;
                    break;
            }

            buttonPages[currentSelected] = (Page)loadedPage;
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

        private void HelpButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenUrl(System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + "Documentation/Documentation.pdf");
        }
    }
}
