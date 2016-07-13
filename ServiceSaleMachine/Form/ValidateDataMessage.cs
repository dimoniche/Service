using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public class ValidateDataMessage
    {
        public MessageBoxIcon Icon { get; private set; }
        public string Message { get; private set; }

        public ValidateDataMessage(string message)
        {
            Message = message;
            Icon = MessageBoxIcon.Information;
        }

        public ValidateDataMessage(string message, MessageBoxIcon icon)
        {
            Message = message;
            Icon = icon;
        }
    }
}
