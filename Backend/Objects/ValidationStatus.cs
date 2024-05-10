using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemFinder_WPF.Backend.Objects
{
    public class ValidationStatus
    {
        private bool _validated;
        private string _message;

        public bool Validated
        {
            get { return _validated; }
            set { _validated = value; }
        }

        public string ErrorMessage
        {
            get { return _message; }
            set { _message = value; }
        }

        public ValidationStatus(bool Validated, string ErrorMessage = null) {
            _validated = Validated;
            _message = ErrorMessage;
        }
    }
}
