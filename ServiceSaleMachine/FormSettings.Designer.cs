namespace ServiceSaleMachine
{
    partial class FormSettings
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.DeviceSettings = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.cbxComPortMoney = new System.Windows.Forms.ComboBox();
            this.cbxComPortScanner = new System.Windows.Forms.ComboBox();
            this.cbxComPortPrinter = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.DeviceSettings.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.DeviceSettings);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(728, 496);
            this.tabControl1.TabIndex = 0;
            // 
            // DeviceSettings
            // 
            this.DeviceSettings.Controls.Add(this.tabControl2);
            this.DeviceSettings.Location = new System.Drawing.Point(4, 22);
            this.DeviceSettings.Name = "DeviceSettings";
            this.DeviceSettings.Padding = new System.Windows.Forms.Padding(3);
            this.DeviceSettings.Size = new System.Drawing.Size(720, 470);
            this.DeviceSettings.TabIndex = 0;
            this.DeviceSettings.Text = "Оборудование";
            this.DeviceSettings.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 74);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(3, 3);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(714, 464);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cbxComPortMoney);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(706, 438);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Купюроприемник";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cbxComPortScanner);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(706, 438);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "Сканер";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.cbxComPortPrinter);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(706, 438);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Принтер";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // cbxComPortMoney
            // 
            this.cbxComPortMoney.FormattingEnabled = true;
            this.cbxComPortMoney.Location = new System.Drawing.Point(79, 27);
            this.cbxComPortMoney.Name = "cbxComPortMoney";
            this.cbxComPortMoney.Size = new System.Drawing.Size(121, 21);
            this.cbxComPortMoney.TabIndex = 0;
            // 
            // cbxComPortScanner
            // 
            this.cbxComPortScanner.FormattingEnabled = true;
            this.cbxComPortScanner.Location = new System.Drawing.Point(42, 32);
            this.cbxComPortScanner.Name = "cbxComPortScanner";
            this.cbxComPortScanner.Size = new System.Drawing.Size(121, 21);
            this.cbxComPortScanner.TabIndex = 1;
            // 
            // cbxComPortPrinter
            // 
            this.cbxComPortPrinter.FormattingEnabled = true;
            this.cbxComPortPrinter.Location = new System.Drawing.Point(82, 27);
            this.cbxComPortPrinter.Name = "cbxComPortPrinter";
            this.cbxComPortPrinter.Size = new System.Drawing.Size(121, 21);
            this.cbxComPortPrinter.TabIndex = 1;
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 496);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormSettings";
            this.Text = "FormSettings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSettings_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.DeviceSettings.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage DeviceSettings;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ComboBox cbxComPortMoney;
        private System.Windows.Forms.ComboBox cbxComPortScanner;
        private System.Windows.Forms.ComboBox cbxComPortPrinter;
    }
}