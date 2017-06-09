using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;
using System.Drawing;

namespace AirVitamin.Client
{
    public partial class FormProvideService : MyForm
    {
        FormResultData data;
        int Interval = 180;

        double height;
        double width;

        Pen pen;
        Graphics g;

        float startAngle = -90.0F;
        float sweepAngle = 360.0F;

        float step = 0;

        int timework = 0;

        public FormProvideService()
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

                    Interval = data.timework;
                }
            }

            // у флеша немного другой цвет фона
            BackColor = Color.FromArgb(212, 208, 199);

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxLogo, "Logo_O2.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxProvideServiceText, "Dishy_txt.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxReturnMainMenu, Globals.DesignConfiguration.Settings.ButtonRetToMain);

            data.drivers.control.SendOpenControl(data.numberCurrentDevice);
            data.drivers.ReceivedResponse += reciveResponse;

            step = 360f / Interval;
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
                    {
                        data.stage = WorkerStateStage.DropCassettteBill;
                        this.Close();
                    }
                    break;
                case DeviceEvent.DropCassetteFullBillAcceptor:

                    break;
                case DeviceEvent.BillAcceptorError:

                    break;
            }
        }

        private void FormProvideService1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // заканчиваем оказывать услугу
            data.drivers.control.SendCloseControl(data.numberCurrentDevice);
            data.realtimework = timework;

            Params.Result = data;
            timerService.Enabled = false;

            data.drivers.ReceivedResponse -= reciveResponse;
        }

        private void timerService_Tick(object sender, EventArgs e)
        {
            // наращиваем время работы
            timework++;

            if (Interval-- == 0)
            {
                // услугу оказали полностью
                data.stage = WorkerStateStage.EndService;
                Close();
            }
            else
            {
                g = Graphics.FromHwnd(tableLayoutPanel3.Handle);

                pen = new Pen(Color.Gainsboro, 20);
                g.DrawArc(pen, new Rectangle(15, 15, (int)(width - 30), (int)(height - 30)), startAngle, sweepAngle);

                startAngle += step;
                sweepAngle -= step;

                pen = new Pen(Color.FromArgb(0, 158, 227), 20);
                g.DrawArc(pen, new Rectangle(15, 15, (int)(width - 30), (int)(height - 30)), startAngle, sweepAngle);
            }

            ReasonEnum reason = CheckError.GetStatus(data);

            if (reason == ReasonEnum.FormClose)
            {
                this.Close();
            }
        }

        private void pBxStopService_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.Fail;
            Close();
        }

        private void ServiceText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pen = new Pen(Color.FromArgb(0, 158, 227), 20);
            g = Graphics.FromHwnd(tableLayoutPanel3.Handle);

            g.DrawArc(pen, new Rectangle(15,15, (int)(width - 30),(int)(height - 30)),45,90);
        }

        private void FormProvideService_Shown(object sender, EventArgs e)
        {
            height = baloonLayout.Height;
            width = baloonLayout.Width;

            double scale = (height / width) * 100;

            baloonLayout.ColumnStyles[0].Width = (float)(100.0 - scale) / 2;
            baloonLayout.ColumnStyles[1].Width = (float)(scale);
            baloonLayout.ColumnStyles[2].Width = (float)(100.0 - scale) / 2;

            height = panel5.Height;
            width = panel5.Width;

            g = Graphics.FromHwnd(tableLayoutPanel3.Handle);
            pen = new Pen(Color.FromArgb(0, 158, 227), 20);
            g.DrawArc(pen, new Rectangle(15, 15, (int)(width - 30), (int)(height - 30)), startAngle, sweepAngle);

            Balloon.Movie = Globals.GetPath(PathEnum.Flash) + "\\balloon.swf";
            Balloon.Zoom(40);

            timerService.Enabled = true;
        }
    }
}
