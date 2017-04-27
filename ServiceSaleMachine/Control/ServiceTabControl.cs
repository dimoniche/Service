using System.Windows.Forms;

namespace AirVitamin
{
    public partial class ServiceTabControl : UserControl
    {
        public TabControl DeviceTab { get { return DevicetabControl; } set { DevicetabControl = value; } }
        public TextBox TextBoxRecognize { get { return textBoxRecognize; } set { textBoxRecognize = value; } }
        public TextBox TextBoxLightUrn { get { return LightUrn; } set { LightUrn = value; } }

        public TextBox TextBoxCaptionService { get { return textBoxCaptionService; } set { textBoxCaptionService = value; } }
        public TextBox TextBoxPriceService { get { return textBoxPriceService; } set { textBoxPriceService = value; } }
        public TextBox TextBoxMaxTimeService { get { return textBoxMaxTimeService; } set { textBoxMaxTimeService = value; } }

        public delegate void ServiceClientResponseEventHandler(object sender);

        public event ServiceClientResponseEventHandler DeleteDevice;
        public event ServiceClientResponseEventHandler AddDevice;
        public event ServiceClientResponseEventHandler RecognizeLeave;
        public event ServiceClientResponseEventHandler LightUrnLeave;
        public event ServiceClientResponseEventHandler CaptionServiceLeave;
        public event ServiceClientResponseEventHandler PriceServiceLeave;
        public event ServiceClientResponseEventHandler MaxTimeServiceLeave;

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

        private void LightUrn_Leave(object sender, System.EventArgs e)
        {
            LightUrnLeave(sender);
        }

        private void textBoxCaptionService_Leave(object sender, System.EventArgs e)
        {
            CaptionServiceLeave(sender);
        }

        private void textBoxPriceService_Leave(object sender, System.EventArgs e)
        {
            PriceServiceLeave(sender);
        }

        private void textBoxMaxTimeService_Leave(object sender, System.EventArgs e)
        {
            MaxTimeServiceLeave(sender);
        }
    }
}
