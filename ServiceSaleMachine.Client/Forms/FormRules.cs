﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormRules : MyForm
    {
        FormResultData data;

        public FormRules()
        {

            InitializeComponent();
            Globals.ClientConfiguration.Settings.LoadPictureBox(pbxRetToMain, Globals.ClientConfiguration.Settings.ButtonRetToMain);
            try
            {
                richTextBoxEx1.LoadFile(Globals.GetPath(PathEnum.Text) + "\\" +  Globals.HelpFileName, RichTextBoxStreamType.PlainText);
            }
            catch
            {
                richTextBoxEx1.Text = "Ошибка загрузки файла с инструкцией";
            }

        }

        public override void LoadData()
        {
            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;
                }
            }
            timer1.Enabled = true;
            this.WindowState = FormWindowState.Maximized;

        }

        private void FormRules_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        private void pbxRetToMain_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}