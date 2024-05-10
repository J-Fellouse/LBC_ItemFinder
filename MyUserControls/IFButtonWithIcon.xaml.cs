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
    /// Logique d'interaction pour IFButtonWithIcon.xaml
    /// </summary>
    public partial class IFButtonWithIcon : UserControl
    {
        public IFButtonWithIcon()
        {
            InitializeComponent();
            this.Cursor = Cursors.Hand;
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
                                                                                                        typeof(string),
                                                                                                        typeof(IFButtonWithIcon));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }

        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon",
                                                                                                        typeof(Geometry),
                                                                                                        typeof(IFButtonWithIcon));
        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }

        }
    }
}
