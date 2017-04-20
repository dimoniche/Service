namespace AirVitamin
{
    partial class FormError
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.errorData = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AppContinueButton = new System.Windows.Forms.Button();
            this.AppExitButton = new System.Windows.Forms.Button();
            this.labelControl1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // errorData
            // 
            this.errorData.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.errorData.Location = new System.Drawing.Point(0, 109);
            this.errorData.Name = "errorData";
            this.errorData.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.errorData.Size = new System.Drawing.Size(530, 298);
            this.errorData.TabIndex = 0;
            this.errorData.Text = "";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // AppContinueButton
            // 
            this.AppContinueButton.Location = new System.Drawing.Point(235, 74);
            this.AppContinueButton.Name = "AppContinueButton";
            this.AppContinueButton.Size = new System.Drawing.Size(120, 25);
            this.AppContinueButton.TabIndex = 2;
            this.AppContinueButton.Text = "Продолжить";
            this.AppContinueButton.UseVisualStyleBackColor = true;
            this.AppContinueButton.Click += new System.EventHandler(this.AppContinueButton_Click);
            // 
            // AppExitButton
            // 
            this.AppExitButton.Location = new System.Drawing.Point(361, 74);
            this.AppExitButton.Name = "AppExitButton";
            this.AppExitButton.Size = new System.Drawing.Size(157, 25);
            this.AppExitButton.TabIndex = 2;
            this.AppExitButton.Text = "Закрыть приложение";
            this.AppExitButton.UseVisualStyleBackColor = true;
            this.AppExitButton.Click += new System.EventHandler(this.AppExitButton_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.AutoSize = true;
            this.labelControl1.Location = new System.Drawing.Point(97, 23);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(220, 13);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "В работе приложения произошла ошибка.";
            // 
            // FormError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 407);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.AppExitButton);
            this.Controls.Add(this.AppContinueButton);
            this.Controls.Add(this.errorData);
            this.Name = "FormError";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormError";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox errorData;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button AppContinueButton;
        private System.Windows.Forms.Button AppExitButton;
        private System.Windows.Forms.Label labelControl1;
    }
}