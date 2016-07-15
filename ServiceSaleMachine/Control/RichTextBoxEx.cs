using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public partial class RichTextBoxEx : RichTextBox
    {
        public RichTextBoxEx()
        {
            HideCaret(this.Handle);
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool HideCaret(IntPtr hwnd);

        public bool HideCaret()
        {
            return HideCaret(this.Handle);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            HideCaret(this.Handle);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            HideCaret(this.Handle);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            HideCaret(this.Handle);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            HideCaret(this.Handle);
        }
    }
}
