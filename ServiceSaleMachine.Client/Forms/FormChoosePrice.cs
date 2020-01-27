using AirVitamin.Drivers;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static AirVitamin.Drivers.MachineDrivers;

namespace AirVitamin.Client
{
    public partial class FormChoosePrice : MyForm
    {
        FormResultData data;

        int Timeout = 0;

        int price = 100;
        int time = 30;

        public FormChoosePrice()
        {
            InitializeComponent();

            TimeOutTimer.Enabled = true;
            Timeout = 0;
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

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxMinus, Globals.DesignConfiguration.Settings.ButtonMinus);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxPlus, Globals.DesignConfiguration.Settings.ButtonPlus);

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxOplata, Globals.DesignConfiguration.Settings.ButtonOplata);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxMainMenu, "Menu_big.png");

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxClock, "Clock.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxMoney, "Money_blue.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxRubles, "Rub.png");

            Globals.DesignConfiguration.Settings.LoadPictureBox(pictureLogo, "Logo_O2.png");

            if (data.numberService == NumberServiceEnum.Before)
            {
                Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle, "Do_tren_ver.png");
            }
            else if (data.numberService == NumberServiceEnum.After)
            {
                Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle, "Posle_tren_ver.png");
            }
            else
            {
                Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle, "Vo_vremya_tren_ver.png");
            }

            time = data.serv.cost.rangeStart;
            price = data.serv.cost.getPriceAccountByAmount(time);
            TimeSpan span = new TimeSpan(0, time / 60, time % 60);

            LabelCounter.Font = new Font(data.FontCollection.Families[CustomFont.CeraRoundPro_Bold], 72, FontStyle.Bold);
            LabelCounter.ForeColor = Color.FromArgb(0, 158, 227);

            LabelMinute.Font = new Font(data.FontCollection.Families[CustomFont.CeraRoundPro_Bold], 72, FontStyle.Bold);
            LabelMinute.ForeColor = Color.FromArgb(0, 158, 227);

            LabelSmes.Font = new Font(data.FontCollection.Families[CustomFont.CeraRoundPro_Medium], 72, FontStyle.Regular);
            LabelSmes.ForeColor = Color.Gray;

            CountTime.Font = new Font(data.FontCollection.Families[CustomFont.CeraRoundPro_Medium], 72, FontStyle.Regular);
            CountTime.ForeColor = Color.Gray;
            CountTime.Text = span.ToString(@"mm\:ss");

            Price.Font = new Font(data.FontCollection.Families[CustomFont.CeraRoundPro_Medium], 72, FontStyle.Regular);
            Price.ForeColor = Color.Gray;
            Price.Text = price.ToString();

            data.drivers.ReceivedResponse += reciveResponse;
        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            if (data.log != null)
            {
                data.log.Write(LogMessageType.Debug, "CHOOSE SERVICE: Событие: " + e.Message.Content + ".");
            }

            switch (e.Message.Event)
            {
                case DeviceEvent.DropCassetteBillAcceptor:
                    {
                        data.stage = WorkerStateStage.DropCassettteBill;
                        this.Close();
                    }
                    break;
                case DeviceEvent.DropCassetteFullBillAcceptor:

                    break;
                case DeviceEvent.BillAcceptorError:

                    break;
                case DeviceEvent.ConnectBillError:
                    {
                        // нет связи с купюроприемником
                        data.stage = WorkerStateStage.ErrorBill;
                        this.Close();
                    }
                    break;
            }
        }

        private void TimeOutTimer_Tick(object sender, System.EventArgs e)
        {
            Timeout++;

            if (Globals.ClientConfiguration.Settings.timeout == 0)
            {
                Timeout = 0;
                return;
            }

            if (Timeout > Globals.ClientConfiguration.Settings.timeout * 60)
            {
                data.stage = WorkerStateStage.TimeOut;
                this.Close();
            }
        }

        private void FormChooseService1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void pBxMinus_Click(object sender, System.EventArgs e)
        {
            if (time <= data.serv.cost.rangeStart) return;

            time -= data.serv.cost.step;
            price = data.serv.cost.getPriceAccountByAmount(time);

            TimeSpan span = new TimeSpan(0, time / 60, time % 60);

            CountTime.Text = span.ToString(@"mm\:ss");
            Price.Text = price.ToString();
        }

        private void pBxPlus_Click(object sender, System.EventArgs e)
        {
            if (time >= data.serv.cost.rangeStop) return;

            time += data.serv.cost.step;
            price = data.serv.cost.getPriceAccountByAmount(time);

            TimeSpan span = new TimeSpan(0, time / 60, time % 60);

            CountTime.Text = span.ToString(@"mm\:ss");
            Price.Text = price.ToString();
        }

        private void pBxOplata_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.ChoosePay;
            this.Close();
        }

        private void pBxMainMenu_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.MainScreen;
            this.Close();
        }

        private void FormChoosePrice_FormClosed(object sender, FormClosedEventArgs e)
        {
            TimeOutTimer.Enabled = false;
            data.drivers.ReceivedResponse -= reciveResponse;

            // запомним выбранное время услуги
            data.timework = time;

            Params.Result = data;
        }
    }
}
