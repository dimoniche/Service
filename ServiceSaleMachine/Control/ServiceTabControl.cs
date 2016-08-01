using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public partial class ServiceTabControl : UserControl
    {
        public TabControl DeviceTab { get { return DevicetabControl; } set { DevicetabControl = value; } }
        public TextBox TextBoxRecognize { get { return textBoxRecognize; } set { textBoxRecognize = value; } }

        public delegate void ServiceClientResponseEventHandler(object sender);

        public event ServiceClientResponseEventHandler DeleteDevice;
        public event ServiceClientResponseEventHandler AddDevice;
        public event ServiceClientResponseEventHandler RecognizeLeave;

        public ServiceTabControl()
        {
            InitializeComponent();
        }

        void butaddDev_Click(object sender, System.EventArgs e)
        {
            AddDevice(sender);
        }

        private void butDelDev_Click(object sender, System.EventArgs e)
        {
            DeleteDevice(sender);
        }

        private void textBoxRecognize_Leave(object sender, System.EventArgs e)
        {
            RecognizeLeave(sender);
        }
    }
}
