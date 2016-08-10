using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine
{
    /// <summary>
    /// Если будет личный кабинет то эта инфа пригодиться
    /// </summary>
    public class UserConfiguration
    {
        public string UserLogin = "";
        public string UserPassword = "";
        public int Amount;
        public int ID;

        public void Clear()
        {
            UserLogin = "";
            UserPassword = "";
            Amount = 0;
            ID = 0;
        }
    }
}
