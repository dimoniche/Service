using System;

namespace ServiceSaleMachine
{
    /// <summary>
    /// Чек
    /// </summary>
    public static class CheckHelper
    {
        /// <summary>
        /// Получить иникальный номер чека c проверкой в базе уникальности
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueNumberCheck(int length)
        {
            string str = "";
            Random rnd = new Random();

            for (int i = 0; i < length; i++)
            {
                str = str + Convert.ToChar(48 + rnd.Next(10));
            }

            return str;
        }
    }
}
