using ItemFinder_WPF.Backend.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemFinder_WPF.Backend.RoutedPageArgs
{
    public class ResultsPageArgs
    {
        private Parameters _parameters { get; set; }

        public Parameters Parameters
        {
            get { return _parameters;}
            set { _parameters = value; }
        }

        public ResultsPageArgs(Parameters Parameters)
        {
            _parameters = Parameters;
        }
    }
}
