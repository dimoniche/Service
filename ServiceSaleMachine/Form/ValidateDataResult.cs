using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public class ValidateDataResult
    {
        public bool IsValid { get; set; }
        public List<ValidateDataMessage> Messages { get; private set; }

        private ValidateDataResult()
        {
            Messages = new List<ValidateDataMessage>();
        }

        public ValidateDataResult(bool isValid) : this()
        {
            IsValid = isValid;
        }

        public ValidateDataResult(string message, MessageBoxIcon icon) : this()
        {
            Messages.Add(new ValidateDataMessage(message, icon));
        }
    }
}
