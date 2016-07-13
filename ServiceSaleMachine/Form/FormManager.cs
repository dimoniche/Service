using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public static class FormManager
    {
        private static List<MyForm> forms { get; set; }
        public static ReadOnlyCollection<MyForm> Forms { get; private set; }

        // Общие параметры
        public static string AppCaptionName { get; set; }
        public static MyForm MainForm { get; set; }

        public static event VspFormErrorEventHandler CatchError;
        public static bool NeedNewSystemEnter { get; set; }

        // Журнал
        public static Log Log { get; set; }

        static FormManager()
        {
            NeedNewSystemEnter = false;
            forms = new List<MyForm>();
            Forms = new ReadOnlyCollection<MyForm>(forms);
        }

        internal static void RegForm(MyForm form)
        {
            if (!forms.Contains(form))
            {
                forms.Add(form);
                form.Load += form_Load;
                form.FormClosed += form_FormClosed;
            }
        }

        static void form_Load(object sender, EventArgs e)
        {
            MyForm form = (MyForm)sender;
        }

        static void form_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            MyForm form = (MyForm)sender;
            form.Load -= form_Load;
            form.FormClosed -= form_FormClosed;
            forms.Remove(form);

            if (MainForm == form)
            {
                MainForm = null;
            }
        }

        private static CreateFormResult<T> CreateForm<T>(FormParams formParams, bool showErrors) where T : MyForm, new()
        {
            try
            {
                T findedForm = null;
                for (int i = 0; i < Application.OpenForms.Count; i++)
                    if (Application.OpenForms[i].GetType() == typeof(T))
                    {
                        if (((T)Application.OpenForms[i]).IsSuch(formParams))
                        {
                            findedForm = (T)Application.OpenForms[i];
                            break;
                        }
                    }
                if (findedForm != null)
                {
                    if (findedForm.WindowState == FormWindowState.Minimized)
                        findedForm.WindowState = FormWindowState.Normal;
                    findedForm.Activate();
                    return new CreateFormResult<T>(findedForm, false);
                }

                T form = null;

                try
                {
                    {
                        form = new T();
                        form.Params = formParams;
                        form.LoadData();
                    }
                }
                catch
                {
                    if (form != null)
                        form.Dispose();
                    throw;
                }
                finally
                {
                }

                return new CreateFormResult<T>(form, true);
            }
            catch (Exception err)
            {
                CatchError?.Invoke(null, new FormErrorEventArgs(err));
                return null;
            }
        }

        private static object OpenForm<T>(FormParams Params) where T : MyForm, new()
        {
            try
            {
                CreateFormResult<T> createResult;
                if (MainForm != null) MainForm.Cursor = Cursors.WaitCursor;
                object resFromForm = null;

                if ((createResult = CreateForm<T>(Params, false)) != null)
                {
                    if (createResult.IsNewForm)
                        resFromForm = createResult.Form.ShowWithParams();
                }
                if (MainForm != null) MainForm.Cursor = Cursors.Default;
                return resFromForm;
            }
            catch
            {
                if (MainForm != null) MainForm.Cursor = Cursors.Default;

                return null;
            }
        }

        public static CreateFormResult<T> CreateForm<T>() where T : MyForm, new()
        {
            return CreateForm<T>(new FormParams(null, FormShowTypeEnum.Alone, FormReasonTypeEnum.Default, new object[0]), false);
        }

        public static CreateFormResult<T> CreateForm<T>(Form previousForm) where T : MyForm, new()
        {
            return CreateForm<T>(new FormParams(previousForm, FormShowTypeEnum.Alone, FormReasonTypeEnum.Default, new object[0]), false);
        }

        public static CreateFormResult<T> CreateForm<T>(Form previousForm, FormShowTypeEnum showType) where T : MyForm, new()
        {
            return CreateForm<T>(new FormParams(previousForm, showType, FormReasonTypeEnum.Default, new object[0]), false);
        }

        public static CreateFormResult<T> CreateForm<T>(Form previousForm, FormShowTypeEnum showType, FormReasonTypeEnum reasonType) where T : MyForm, new()
        {
            return CreateForm<T>(new FormParams(previousForm, showType, reasonType, new object[0]), false);
        }

        public static CreateFormResult<T> CreateForm<T>(Form previousForm, FormShowTypeEnum showType, FormReasonTypeEnum reasonType, params object[] objects) where T : MyForm, new()
        {
            return CreateForm<T>(new FormParams(previousForm, showType, reasonType, objects), false);
        }

        public static object OpenForm<T>() where T : MyForm, new()
        {
            return OpenForm<T>(new FormParams(null, FormShowTypeEnum.Alone, FormReasonTypeEnum.Default, new object[0]));
        }

        public static object OpenForm<T>(Form previousForm) where T : MyForm, new()
        {
            return OpenForm<T>(new FormParams(previousForm, FormShowTypeEnum.Alone, FormReasonTypeEnum.Default, new object[0]));
        }

        public static object OpenForm<T>(Form previousForm, FormShowTypeEnum showType) where T : MyForm, new()
        {
            return OpenForm<T>(new FormParams(previousForm, showType, FormReasonTypeEnum.Default, new object[0]));
        }

        public static object OpenForm<T>(Form previousForm, FormShowTypeEnum showType, FormReasonTypeEnum reasonType) where T : MyForm, new()
        {
            return OpenForm<T>(new FormParams(previousForm, showType, reasonType, new object[0]));
        }

        public static object OpenForm<T>(Form previousForm, FormShowTypeEnum showType, FormReasonTypeEnum reasonType, params object[] objects) where T : MyForm, new()
        {
            return OpenForm<T>(new FormParams(previousForm, showType, reasonType, objects));
        }
    }
}
