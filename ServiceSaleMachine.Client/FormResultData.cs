using AirVitamin.Drivers;
using System.Drawing.Text;

namespace AirVitamin.Client
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
        public int timework = 180;

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
        /// Тип инструкции 
        /// </summary>
        public InstructionEnum Instruction;

        /// <summary>
        /// статистика по деньгам
        /// </summary>
        public MoneyStatistic statistic;

        /// <summary>
        /// Глобальный лог приложения
        /// </summary>
        public Log log;

        /// <summary>
        /// об ошибке принтера уже сообщили
        /// </summary>
        public bool PrinterError = false;

        /// <summary>
        /// Ошибка приемника
        /// </summary>
        public bool BillError = false;

        /// <summary>
        /// Загруженные шрифты
        /// </summary>
        public PrivateFontCollection FontCollection;

        public FormResultData(Log log)
        {
            retLogin = "";
            retPassword = "";
            timework = 180;
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