using ServiceSaleMachine.Drivers;

namespace ServiceSaleMachine.Client
{
    internal class FormResultData
    {
        public WorkerStateStage stage;
        public object result = null;

        public int numberService;

        public string retLogin;
        public string retPassword;

        public int timework;
        public string ServName;

        public MachineDrivers drivers;

        public FormResultData()
        {
            retLogin = "";
            retPassword = "";
            timework = 10;
            ServName = "";
        }
    }
}