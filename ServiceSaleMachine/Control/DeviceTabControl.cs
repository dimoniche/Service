using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public partial class DeviceTabControl : UserControl
    {
        public TextBox LimitTime { get { return textBoxLimitTime; } set { textBoxLimitTime = value; } }
        public TextBox TimeWork { get { return textBoxTimeWork; } set { textBoxTimeWork = value; } }
        public TextBox TextBoxIdDevice { get { return textBoxIdDevice; } set { textBoxIdDevice = value; } }
        public TextBox TextBoxNameDevice { get { return textBoxNameDevice; } set { textBoxNameDevice = value; } }


        public delegate void ServiceClientResponseEventHandler(object sender);

        public event ServiceClientResponseEventHandler LimitTimeLeave;
        public event ServiceClientResponseEventHandler TimeWorkLeave;
        public event ServiceClientResponseEventHandler IdDeviceLeave;
        public event ServiceClientResponseEventHandler NameDeviceLeave;

        public DeviceTabControl()
        {
            InitializeComponent();
        }

        private void textBoxTimeWork_Leave(object sender, System.EventArgs e)
        {
            TimeWorkLeave(sender);
        }

        private void textBoxLimitTime_Leave(object sender, System.EventArgs e)
        {
            LimitTimeLeave(sender);
        }

        private void textBoxIdDevice_Leave(object sender, System.EventArgs e)
        {
            IdDeviceLeave(sender);
        }

        private void textBoxNameDevice_Leave(object sender, System.EventArgs e)
        {
            NameDeviceLeave(sender);
        }
    }
}
