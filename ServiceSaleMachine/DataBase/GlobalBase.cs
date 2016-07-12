using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine
{
    public static class GlobalDb
    {
        public static db GlobalBase;

        static GlobalDb()
        {
            GlobalBase = new db();
        }
    }
}
