using ItemFinder_WPF.Backend.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemFinder_WPF.Backend.RoutedPageArgs
{
    public class HomePageArgs
    {
        private Parameters _parameters { get; set; }

        public Parameters Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public HomePageArgs(Parameters Parameters)
        {
            _parameters = Parameters;
        }
    }
}
