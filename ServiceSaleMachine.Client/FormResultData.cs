using ServiceSaleMachine.Drivers;

namespace ServiceSaleMachine.Client
{
    internal class FormResultData
    {
        public WorkerStateStage stage;

        /// <summary>
        /// Номер текущей выбранной услуги
        /// </summary>
        public int numberService;

        /// <summary>
        /// данные по текущей выбранной услуге
        /// </summary>
        public Service serv;

        public string retLogin;
        public string retPassword;

        /// <summary>
        /// ID текущего пользователя из базы
        /// </summary>
        public int CurrentUserId;

        /// <summary>
        /// максимальное время оказания услуги
        /// </summary>
        public int timework = 3;

        /// <summary>
        /// время ознакомления с услугой и забор аксессуаров
        /// </summary>
        public int timeRecognize = 60;

        /// <summary>
        /// Наименование услуги
        /// </summary>
        public string ServName;

        /// <summary>
        /// номер текущего устройства
        /// </summary>
        public int numberCurrentDevice = 1;

        /// <summary>
        /// Драйвера устройств
        /// </summary>
        public MachineDrivers drivers;

        /// <summary>
        /// статистика по деньгам
        /// </summary>
        public MoneyStatistic statistic;

        /// <summary>
        /// Глобальный лог приложения
        /// </summary>
        Log log;

        public FormResultData(Log log)
        {
            retLogin = "";
            retPassword = "";
            timework = 3;
            ServName = "";

            // Лог
            this.log = log;

            // инициализируем драйверы устройств
            drivers = new MachineDrivers(log);

            // прочтем из базы статистику
            statistic = GlobalDb.GlobalBase.GetMoneyStatistic();
        }
    }
}