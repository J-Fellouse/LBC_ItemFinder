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
    /// Logique d'interaction pour MainTitle.xaml
    /// </summary>
    public partial class IFMainTitle : UserControl
    {
        public IFMainTitle()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
                                                                                                        typeof(string),
                                                                                                        typeof(IFMainTitle));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }

        }

        public static readonly DependencyProperty UnderlineProperty = DependencyProperty.Register("Underline",
                                                                                                        typeof(BitmapImage),
                                                                                                        typeof(IFMainTitle));
        public BitmapImage Underline
        {
            get { return (BitmapImage)GetValue(UnderlineProperty); }
            set { SetValue(UnderlineProperty, value); }

        }
    }
}
