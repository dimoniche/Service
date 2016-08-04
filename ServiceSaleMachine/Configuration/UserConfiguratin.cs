using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine
{
    public class UserConfiguration
    {
        public string UserLogin = "";
        public string UserPassword = "";
        public int Amount;

        public void Clear()
        {
            UserLogin = "";
            UserPassword = "";
            Amount = 0;
        }
    }
}
