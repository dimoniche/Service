using ServiceSaleMachine.Drivers;

namespace ServiceSaleMachine.Client
{
    internal class FormResultData
    {
        public WorkerStateStage stage;
        public object result = null;

        public int numberService;
        public Service serv;

        public string retLogin;
        public string retPassword;
        // ID текущего пользователя из базы
        public int CurrentUserId;

        // максимальное время оказания услуги
        public int timework = 3;
        // время ознакомления с услугой и забор аксессуаров
        public int timeRecognize = 60;
        public string ServName;

        // номер текущего устройства
        public int numberCurrentDevice = 1;

        public MachineDrivers drivers;

        // статистика по деньгам
        public MoneyStatistic statistic;

        public FormResultData()
        {
            retLogin = "";
            retPassword = "";
            timework = 10;
            ServName = "";
        }
    }
}