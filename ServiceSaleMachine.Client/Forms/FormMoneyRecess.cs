using System.Data;
using System.Linq;
using System.Windows.Forms;
using ServiceSaleMachine.Drivers;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class FormMoneyRecess : MyForm
    {
        FormResultData data;

        public FormMoneyRecess()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;
                }
            }

            data.drivers.ReceivedResponse += reciveResponse;

            // 
            moneySumm.Text = "Сумма денег в кассете: " + data.statistic.AllMoneySumm + " руб";
        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            switch (e.Message.Event)
            {
                case DeviceEvent.DropCassetteBillAcceptor:

                    break;
                case DeviceEvent.DropCassetteFullBillAcceptor:

                    break;
                case DeviceEvent.BillAcceptorError:

                    break;
                default:
                    // другие события
                    if(!((string)e.Message.Content).Contains("Drop Cassette out of position"))
                    {
                        // не выемка
                        data.stage = WorkerStateStage.EndDropCassette;
                        this.Close();
                    }
                    break;
            }
        }

        private void FormMoneyRecess_FormClosed(object sender, FormClosedEventArgs e)
        {
            // обнулим статистику
            data.statistic.AccountMoneySumm = 0;
            data.statistic.AllMoneySumm = 0;
            data.statistic.BarCodeMoneySumm = 0;
            data.statistic.CountBankNote = 0;
            data.statistic.ServiceMoneySumm = 0;

            data.drivers.ReceivedResponse -= reciveResponse;
            Params.Result = data;
        }
    }
}
