namespace AirVitamin
{
    public class MoneyStatistic
    {
        /// <summary>
        /// сумма принятых денег
        /// </summary>
        public int AllMoneySumm = 0;

        /// <summary>
        /// сумма денег на аккаунтах
        /// </summary>
        public int AccountMoneySumm = 0;

        /// <summary>
        ///  сумма денег на штрихкод-чеках
        /// </summary>
        public int BarCodeMoneySumm = 0;

        /// <summary>
        /// оказано услуг на сумму
        /// </summary>
        public int ServiceMoneySumm = 0;

        /// <summary>
        /// количество принятых банкнот
        /// </summary>
        public int CountBankNote = 0;

        public MoneyStatistic()
        {
        }
    }
}