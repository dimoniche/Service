using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AirVitamin
{
    public partial class MyForm : Form
    {
        public FormParams Params { get; internal set; }

        public bool IsDataLoaded { get; protected set; }
        public bool IsSaveFailed { get; protected set; }

        public MyForm()
        {
            InitializeComponent();

            // Регистрация формы в менеджере форм
            FormManager.RegForm(this);
        }

        public void FormLogWrite(LogMessageType messageType, string message)
        {
            if (FormManager.Log != null)
            {
                FormManager.Log.Write(messageType, message);
            }
        }

        public void FormLogWrite(LogMessageType messageType, string message, string content)
        {
            if (FormManager.Log != null)
            {
                FormManager.Log.Write(messageType, message, content);
            }
        }

        public void FormLogWrite(LogMessageType messageType, string message, Exception error)
        {
            if (FormManager.Log != null)
            {
                FormManager.Log.Write(messageType, message, error);
            }
        }

        public virtual void LoadData()
        {

        }

        internal virtual bool IsSuch(FormParams otherFormParams)
        {
            if (otherFormParams == null) throw new ArgumentNullException("OtherFormParams");

            // Есть смысл сравнивать только с окнами, открывающимся не в диалоговом режиме
            if (Params != null && otherFormParams.ShowType != FormShowTypeEnum.Dialog && otherFormParams.ReasonType != FormReasonTypeEnum.New)
            {
                if (Params.ReasonType == otherFormParams.ReasonType && Params.Objects.Length == otherFormParams.Objects.Length)
                {
                    bool objectsEquals = true;
                    for (int i = 0; i < Params.Objects.Length; i++)
                        if (!object.Equals(Params.Objects[i], otherFormParams.Objects[i]))
                        {
                            objectsEquals = false;
                            break;
                        }
                    if (objectsEquals) return true;
                }
            }
            return false;
        }

        public virtual object ShowWithParams()
        {
            if (Params == null) throw new ArgumentNullException("Params");

            if (Params.ShowType == FormShowTypeEnum.Dialog)
            {
                ShowDialog(Params.PreviousForm);
                Dispose();
            }
            else
            {
                if (Params.ShowType == FormShowTypeEnum.Mdi)
                {
                    MdiParent = Params.PreviousForm;
                }
                Show();
                Activate();
            }
            return Params.Result;
        }

        public void ShowMessages(ValidateDataResult validateDataResult)
        {
            if (validateDataResult.Messages.Count > 0)
            {
                MessageBoxIcon icon = validateDataResult.Messages.Min(c => c.Icon);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < validateDataResult.Messages.Count; i++)
                {
                    if (i < validateDataResult.Messages.Count - 1)
                        stringBuilder.Append(validateDataResult.Messages[i].Message + "\r\n");
                    else
                        stringBuilder.Append(validateDataResult.Messages[i].Message);
                }
                MessageBox.Show(this, stringBuilder.ToString(), FormManager.AppCaptionName, MessageBoxButtons.OK, icon);
            }
        }
    }
}
