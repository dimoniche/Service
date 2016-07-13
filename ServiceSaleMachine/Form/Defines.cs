using System;
using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public class CreateFormResult<T> where T : MyForm, new()
    {
        public T Form { get; private set; }
        public bool IsNewForm { get; private set; }

        public CreateFormResult(T form, bool isNewForm)
        {
            this.Form = form;
            this.IsNewForm = isNewForm;
        }
    }

    public enum FormShowTypeEnum
    {
        Alone = 0,
        Mdi = 1,
        Dialog = 2,
    }

    public enum FormReasonTypeEnum
    {
        Default = 0,
        New = 1,
        Modify,
    }

    public class FormParams
    {
        public Form PreviousForm { get; set; }
        public FormShowTypeEnum ShowType { get; set; }
        public FormReasonTypeEnum ReasonType { get; set; }

        public object Result { get; set; }
        public object[] Objects { get; set; }

        public FormParams(Form previousForm, FormShowTypeEnum openReason, FormReasonTypeEnum reasonType, params object[] objects)
        {
            PreviousForm = previousForm;
            ShowType = openReason;
            ReasonType = reasonType;
            Objects = objects;
        }
    }

    public delegate void VspFormErrorEventHandler(object sender, FormErrorEventArgs e);
    public class FormErrorEventArgs : EventArgs
    {
        public Exception Error { get; private set; }
        public FormErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
