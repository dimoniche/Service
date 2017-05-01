using System.Windows.Forms;

namespace AirVitamin
{
    public partial class DeviceTabControl : UserControl
    {
        public TextBox TextBoxIdDevice { get { return textBoxIdDevice; } set { textBoxIdDevice = value; } }
        public TextBox TextBoxNameDevice { get { return textBoxNameDevice; } set { textBoxNameDevice = value; } }

        public delegate void ServiceClientResponseEventHandler(object sender);

        public event ServiceClientResponseEventHandler IdDeviceLeave;
        public event ServiceClientResponseEventHandler NameDeviceLeave;

        public DeviceTabControl()
        {
            InitializeComponent();
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
