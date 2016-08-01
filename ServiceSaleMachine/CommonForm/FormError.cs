using System;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public partial class FormError : MyForm
    {
        public static void TryShow(Form previousForm, Exception ex)
        {
            if (Application.OpenForms.OfType<FormError>().Count() > 0)
                return;
            FormManager.OpenForm<FormError>(previousForm, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Default, ex);
        }

        private bool showDetails = false;

        public FormError()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            if (Params.PreviousForm != null && Params.PreviousForm.WindowState != FormWindowState.Minimized)
                StartPosition = FormStartPosition.CenterParent;
            else
                StartPosition = FormStartPosition.CenterScreen;
            Text = FormManager.AppCaptionName;

            ServiceWin32.MessageBeep((uint)MessageBoxIcon.Error);
            if (Params.Objects.Length > 0 && Params.Objects[0] != null && Params.Objects[0].IsAssignableTo(typeof(Exception)))
            {
                if (Params.Objects[0].IsAssignableTo(typeof(UserException)))
                {
                    labelControl1.Text = ((UserException)Params.Objects[0]).Message;
                    errorData.Text = ((Exception)Params.Objects[0]).GetDebugInformation();
                }
                else
                {
                    errorData.Text = ((Exception)Params.Objects[0]).GetDebugInformation();
                }
            }
        }

        internal override bool IsSuch(FormParams otherFormParams)
        {
            return true;
        }

        private void AppContinueButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AppExitButton_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }
    }
}
