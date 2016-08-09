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

        // максимальное время оказания услуги
        public int timework = 3;
        // время ознакомления с услугой и забор аксессуаров
        public int timeRecognize = 60;
        public string ServName;

        public MachineDrivers drivers;

        // статистика по деньгам
        public MoneyStatistic statistic;
        // статисткика использования устройств
        public DeviceStatistic dev_statistic;

        public FormResultData()
        {
            retLogin = "";
            retPassword = "";
            timework = 10;
            ServName = "";
        }
    }
}