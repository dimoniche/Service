namespace AirVitamin
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
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxPriceService1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxBefore = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTimeService = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPriceService = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCaptionService = new System.Windows.Forms.TextBox();
            this.labCaptionService = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.butDelDev = new System.Windows.Forms.Button();
            this.butaddDev = new System.Windows.Forms.Button();
            this.textBoxPause = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.DevicetabControl = new System.Windows.Forms.TabControl();
            this.tabPageDevice = new System.Windows.Forms.TabPage();
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
            this.panel1.Size = new System.Drawing.Size(514, 591);
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
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(514, 591);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.textBoxPriceService1);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textBoxBefore);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.textBoxTimeService);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.textBoxPriceService);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.textBoxCaptionService);
            this.panel2.Controls.Add(this.labCaptionService);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.butDelDev);
            this.panel2.Controls.Add(this.butaddDev);
            this.panel2.Controls.Add(this.textBoxPause);
            this.panel2.Controls.Add(this.label20);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(508, 289);
            this.panel2.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(361, 155);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "руб";
            this.label8.Visible = false;
            // 
            // textBoxPriceService1
            // 
            this.textBoxPriceService1.Location = new System.Drawing.Point(255, 148);
            this.textBoxPriceService1.Name = "textBoxPriceService1";
            this.textBoxPriceService1.Size = new System.Drawing.Size(100, 20);
            this.textBoxPriceService1.TabIndex = 26;
            this.textBoxPriceService1.Text = "100";
            this.textBoxPriceService1.Visible = false;
            this.textBoxPriceService1.Leave += new System.EventHandler(this.textBoxPriceService1_Leave);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 155);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(147, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "Стоимость услуги (аккаунт)";
            this.label9.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(361, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "сек";
            // 
            // textBoxBefore
            // 
            this.textBoxBefore.Location = new System.Drawing.Point(255, 26);
            this.textBoxBefore.Name = "textBoxBefore";
            this.textBoxBefore.Size = new System.Drawing.Size(100, 20);
            this.textBoxBefore.TabIndex = 23;
            this.textBoxBefore.Text = "0";
            this.textBoxBefore.Leave += new System.EventHandler(this.textBoxBefore_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(175, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Пауза перед началом процедуры";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(361, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "сек";
            this.label4.Visible = false;
            // 
            // textBoxTimeService
            // 
            this.textBoxTimeService.Location = new System.Drawing.Point(255, 76);
            this.textBoxTimeService.Name = "textBoxTimeService";
            this.textBoxTimeService.Size = new System.Drawing.Size(100, 20);
            this.textBoxTimeService.TabIndex = 20;
            this.textBoxTimeService.Text = "0";
            this.textBoxTimeService.Visible = false;
            this.textBoxTimeService.Leave += new System.EventHandler(this.textBoxMaxTimeService_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Длительность услуги";
            this.label5.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(361, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "руб";
            this.label6.Visible = false;
            // 
            // textBoxPriceService
            // 
            this.textBoxPriceService.Location = new System.Drawing.Point(255, 123);
            this.textBoxPriceService.Name = "textBoxPriceService";
            this.textBoxPriceService.Size = new System.Drawing.Size(100, 20);
            this.textBoxPriceService.TabIndex = 17;
            this.textBoxPriceService.Text = "100";
            this.textBoxPriceService.Visible = false;
            this.textBoxPriceService.Leave += new System.EventHandler(this.textBoxPriceService_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 130);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(156, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Стоимость услуги (наличные)";
            this.label7.Visible = false;
            // 
            // textBoxCaptionService
            // 
            this.textBoxCaptionService.Location = new System.Drawing.Point(255, 99);
            this.textBoxCaptionService.Name = "textBoxCaptionService";
            this.textBoxCaptionService.Size = new System.Drawing.Size(133, 20);
            this.textBoxCaptionService.TabIndex = 14;
            this.textBoxCaptionService.Text = "До тренировки";
            this.textBoxCaptionService.Visible = false;
            this.textBoxCaptionService.Leave += new System.EventHandler(this.textBoxCaptionService_Leave);
            // 
            // labCaptionService
            // 
            this.labCaptionService.AutoSize = true;
            this.labCaptionService.Location = new System.Drawing.Point(3, 106);
            this.labCaptionService.Name = "labCaptionService";
            this.labCaptionService.Size = new System.Drawing.Size(93, 13);
            this.labCaptionService.TabIndex = 13;
            this.labCaptionService.Text = "Название услуги";
            this.labCaptionService.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(361, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "сек";
            // 
            // butDelDev
            // 
            this.butDelDev.Location = new System.Drawing.Point(166, 208);
            this.butDelDev.Name = "butDelDev";
            this.butDelDev.Size = new System.Drawing.Size(154, 23);
            this.butDelDev.TabIndex = 8;
            this.butDelDev.Text = "Удалить устройство";
            this.butDelDev.UseVisualStyleBackColor = true;
            this.butDelDev.Visible = false;
            this.butDelDev.Click += new System.EventHandler(this.butDelDev_Click);
            // 
            // butaddDev
            // 
            this.butaddDev.Location = new System.Drawing.Point(6, 208);
            this.butaddDev.Name = "butaddDev";
            this.butaddDev.Size = new System.Drawing.Size(154, 23);
            this.butaddDev.TabIndex = 8;
            this.butaddDev.Text = "Добавить устройство";
            this.butaddDev.UseVisualStyleBackColor = true;
            this.butaddDev.Visible = false;
            this.butaddDev.Click += new System.EventHandler(this.butaddDev_Click);
            // 
            // textBoxPause
            // 
            this.textBoxPause.Location = new System.Drawing.Point(255, 51);
            this.textBoxPause.Name = "textBoxPause";
            this.textBoxPause.Size = new System.Drawing.Size(100, 20);
            this.textBoxPause.TabIndex = 7;
            this.textBoxPause.Text = "0";
            this.textBoxPause.Leave += new System.EventHandler(this.textBoxRecognize_Leave);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 58);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(129, 13);
            this.label20.TabIndex = 6;
            this.label20.Text = "Пауза после процедуры";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.DevicetabControl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 298);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(508, 290);
            this.panel3.TabIndex = 1;
            // 
            // DevicetabControl
            // 
            this.DevicetabControl.Controls.Add(this.tabPageDevice);
            this.DevicetabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DevicetabControl.Location = new System.Drawing.Point(0, 0);
            this.DevicetabControl.Name = "DevicetabControl";
            this.DevicetabControl.SelectedIndex = 0;
            this.DevicetabControl.Size = new System.Drawing.Size(508, 290);
            this.DevicetabControl.TabIndex = 0;
            this.DevicetabControl.Visible = false;
            // 
            // tabPageDevice
            // 
            this.tabPageDevice.Location = new System.Drawing.Point(4, 22);
            this.tabPageDevice.Name = "tabPageDevice";
            this.tabPageDevice.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDevice.Size = new System.Drawing.Size(500, 264);
            this.tabPageDevice.TabIndex = 0;
            this.tabPageDevice.Text = "Устройство 1";
            this.tabPageDevice.UseVisualStyleBackColor = true;
            // 
            // ServiceTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ServiceTabControl";
            this.Size = new System.Drawing.Size(514, 591);
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
        private System.Windows.Forms.TextBox textBoxPause;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TabControl DevicetabControl;
        private System.Windows.Forms.TabPage tabPageDevice;
        private System.Windows.Forms.Button butaddDev;
        private System.Windows.Forms.Button butDelDev;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxPriceService;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxCaptionService;
        private System.Windows.Forms.Label labCaptionService;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTimeService;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxBefore;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxPriceService1;
        private System.Windows.Forms.Label label9;
    }
}
