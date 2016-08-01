namespace ServiceSaleMachine
{
    partial class ServiceTabControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxRecognize = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.DevicetabControl = new System.Windows.Forms.TabControl();
            this.tabPageDevice = new System.Windows.Forms.TabPage();
            this.butaddDev = new System.Windows.Forms.Button();
            this.butDelDev = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.DevicetabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(514, 335);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(514, 335);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.butDelDev);
            this.panel2.Controls.Add(this.butaddDev);
            this.panel2.Controls.Add(this.textBoxRecognize);
            this.panel2.Controls.Add(this.label20);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(508, 105);
            this.panel2.TabIndex = 0;
            // 
            // textBoxRecognize
            // 
            this.textBoxRecognize.Location = new System.Drawing.Point(166, 4);
            this.textBoxRecognize.Name = "textBoxRecognize";
            this.textBoxRecognize.Size = new System.Drawing.Size(100, 20);
            this.textBoxRecognize.TabIndex = 7;
            this.textBoxRecognize.Text = "0";
            this.textBoxRecognize.Leave += new System.EventHandler(this.textBoxRecognize_Leave);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 11);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(157, 13);
            this.label20.TabIndex = 6;
            this.label20.Text = "Длительность ознакомления";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.DevicetabControl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 114);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(508, 218);
            this.panel3.TabIndex = 1;
            // 
            // DevicetabControl
            // 
            this.DevicetabControl.Controls.Add(this.tabPageDevice);
            this.DevicetabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DevicetabControl.Location = new System.Drawing.Point(0, 0);
            this.DevicetabControl.Name = "DevicetabControl";
            this.DevicetabControl.SelectedIndex = 0;
            this.DevicetabControl.Size = new System.Drawing.Size(508, 218);
            this.DevicetabControl.TabIndex = 0;
            // 
            // tabPageDevice
            // 
            this.tabPageDevice.Location = new System.Drawing.Point(4, 22);
            this.tabPageDevice.Name = "tabPageDevice";
            this.tabPageDevice.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDevice.Size = new System.Drawing.Size(500, 192);
            this.tabPageDevice.TabIndex = 0;
            this.tabPageDevice.Text = "Устройство 1";
            this.tabPageDevice.UseVisualStyleBackColor = true;
            // 
            // butaddDev
            // 
            this.butaddDev.Location = new System.Drawing.Point(6, 41);
            this.butaddDev.Name = "butaddDev";
            this.butaddDev.Size = new System.Drawing.Size(154, 23);
            this.butaddDev.TabIndex = 8;
            this.butaddDev.Text = "Добавить устройство";
            this.butaddDev.UseVisualStyleBackColor = true;
            this.butaddDev.Click += new System.EventHandler(this.butaddDev_Click);
            // 
            // butDelDev
            // 
            this.butDelDev.Location = new System.Drawing.Point(166, 41);
            this.butDelDev.Name = "butDelDev";
            this.butDelDev.Size = new System.Drawing.Size(154, 23);
            this.butDelDev.TabIndex = 8;
            this.butDelDev.Text = "Удалить устройство";
            this.butDelDev.UseVisualStyleBackColor = true;
            this.butDelDev.Click += new System.EventHandler(this.butDelDev_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(272, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "сек";
            // 
            // ServiceTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ServiceTabControl";
            this.Size = new System.Drawing.Size(514, 335);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.DevicetabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBoxRecognize;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TabControl DevicetabControl;
        private System.Windows.Forms.TabPage tabPageDevice;
        private System.Windows.Forms.Button butaddDev;
        private System.Windows.Forms.Button butDelDev;
        private System.Windows.Forms.Label label1;
    }
}
