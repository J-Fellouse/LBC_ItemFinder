using ItemFinder_WPF.Backend;
using ItemFinder_WPF.Backend.Objects;
using ItemFinder_WPF.MyUserControls;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logique d'interaction pour BlacklistPage.xaml
    /// </summary>
    /// 

    //petit bug à résoudre sur la mise à jour de blacklist après avoir cliqué sur "Activer la liste noire" sur la page d'accueil, résolvable très facilement avec un
    //chargement dynamique dela blacklist dans la page d'accueil

    public partial class BlacklistPage : Page
    {
        string blacklistTextInFile = null;
        string currentBlacklistTextInTextbox = null;

        public BlacklistPage()
        {
            InitializeComponent();
        }

        private ValidationStatus ValidateBlacklist(IFMultilineTextbox control)
        {
            if (!string.IsNullOrEmpty(control.Text))
            {
                if (control.PlaceholderCleared)
                {
                    
                    return new ValidationStatus(true);
                }
            }
            return new ValidationStatus(false, "La liste noire ne peut pas être vide !");
        }

        private List<string> parseBlacklist(string blacklistText)
        {
            List<string> parsedBlacklist = blacklistText.Split(';').ToList();
            return parsedBlacklist;
        }

        private string unparseBlacklist(List<string> blacklistList)
        {
            string unparsedBlacklist = String.Join(";", blacklistList.ToArray());
            return unparsedBlacklist;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ValidationStatus blacklistValidateStatus = ValidateBlacklist(BlacklistTextbox);
            if(!blacklistValidateStatus.Validated && blacklistValidateStatus.ErrorMessage != null)
            {
                MessageBox.Show(blacklistValidateStatus.ErrorMessage, "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(blacklistTextInFile != BlacklistTextbox.Text)
            {
                SaveBlacklistHandler blacklistHandler = new SaveBlacklistHandler("Configuration/Blacklist.json", parseBlacklist(BlacklistTextbox.Text));
                blacklistHandler.handleNewSaveBlacklist();
                MessageBox.Show("Liste noire sauvegardée correctement !", "Sauvegarde réussie", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBox.Show("Veuillez effectuer un changement sur la liste noire antérieure pour la sauvegarder !", "Aucun changement detecté", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(BlacklistTextbox.Text != blacklistTextInFile)
            {
                SaveBlacklistHandler blacklistHandler = new SaveBlacklistHandler("Configuration/Blacklist.json");
                List<string> gatheredBlacklistWords = blacklistHandler.getEntries();

                if (gatheredBlacklistWords.Count > 0)
                {
                    BlacklistTextbox.Text = unparseBlacklist(gatheredBlacklistWords);
                    blacklistTextInFile = BlacklistTextbox.Text;
                }
            }
        }
    }
}
