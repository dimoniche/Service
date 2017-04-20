using System;

namespace AirVitamin
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

            int[] array = new int[length];

            for (int i = 0; i < length - 1; i++)
            {
                array[i] = rnd.Next(10);
            }

            int step1 = (array[0] + array[2] + array[4] + array[6] + array[8] + array[10]) * 3;
            int step2 = (array[1] + array[3] + array[5] + array[7] + array[9] + array[11]);
            int step3 = (step1 + step2) % 10;
            array[length - 1] = 10 - step3;

            if(array[length - 1] == 10)
            {
                array[length - 1] = 0;
            }

            for (int i = 0; i < length; i++)
            {
                str = str + Convert.ToChar(48 + array[i]);
            }

            return str;
        }
    }
}
