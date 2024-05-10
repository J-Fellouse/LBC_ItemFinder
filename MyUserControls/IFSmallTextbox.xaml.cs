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

namespace ItemFinder_WPF.MyUserControls
{
    /// <summary>
    /// Logique d'interaction pour IFSmallTextbox.xaml
    /// </summary>
    public partial class IFSmallTextbox : UserControl
    {
        bool firstTime = true;

        public IFSmallTextbox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder",
                                                                                                        typeof(string),
                                                                                                        typeof(IFSmallTextbox));
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderClearedProperty = DependencyProperty.Register("PlaceholderCleared",
                                                                                                        typeof(bool),
                                                                                                        typeof(IFSmallTextbox), new UIPropertyMetadata(false));
        public bool PlaceholderCleared
        {
            get { return (bool)GetValue(PlaceholderClearedProperty); }
            set { SetValue(PlaceholderClearedProperty, value); }
        }

        public string Text
        {
            get { return MyTextBox.Text; }
            set
            {
                Placeholder = null;
                PlaceholderCleared = true;
                MyTextBox.Text = value;
            }
        }

        private void MyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!firstTime) { return; }

            if (Placeholder != null)
            {
                MyTextBox.Text = "";
            }

            PlaceholderCleared = true;
            firstTime = false;
        }
    }
}
