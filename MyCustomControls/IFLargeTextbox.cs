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

namespace ItemFinder_WPF.MyCustomControls
{
    public class IFLargeTextbox : TextBox
    {
        static IFLargeTextbox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IFLargeTextbox), new FrameworkPropertyMetadata(typeof(IFLargeTextbox)));
        }

        public static readonly DependencyProperty PlaceholderEnabledProperty = DependencyProperty.Register("PlaceholderEnabled",
                                                                                                        typeof(bool),
                                                                                                        typeof(IFLargeTextbox));
        public bool PlaceholderEnabled
        {
            get { return (bool)GetValue(PlaceholderEnabledProperty); }
            set { SetValue(PlaceholderEnabledProperty, value); }

        }

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder",
                                                                                                        typeof(string),
                                                                                                        typeof(IFLargeTextbox));
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }

        }

        public static readonly DependencyProperty FocusedProperty = DependencyProperty.Register("Focused",
                                                                                                        typeof(bool),
                                                                                                        typeof(IFLargeTextbox));
        public bool Focused
        {
            get { return (bool)GetValue(FocusedProperty); }
            set { SetValue(FocusedProperty, value); }

        }
    }
}
