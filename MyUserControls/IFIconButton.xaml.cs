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
    /// Logique d'interaction pour IFIconButton.xaml
    /// </summary>
    public partial class IFIconButton : UserControl
    {
        public IFIconButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon",
                                                                                                        typeof(Geometry),
                                                                                                        typeof(IFIconButton));
        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }

        }

        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected",
                                                                                                        typeof(bool),
                                                                                                        typeof(IFIconButton));
        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }

        }

        public static readonly DependencyProperty GridBackgroundProperty = DependencyProperty.Register("GridBackground",
                                                                                                        typeof(Brush),
                                                                                                        typeof(IFIconButton));
        public Brush GridBackground
        {
            get { return (Brush)GetValue(GridBackgroundProperty); }
            set { SetValue(GridBackgroundProperty, value); }

        }
    }
}
