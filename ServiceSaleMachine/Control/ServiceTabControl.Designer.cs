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
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPriceService = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCaptionService = new System.Windows.Forms.TextBox();
            this.labCaptionService = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LightUrn = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.butDelDev = new System.Windows.Forms.Button();
            this.butaddDev = new System.Windows.Forms.Button();
            this.textBoxRecognize = new System.Windows.Forms.TextBox();
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
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(514, 591);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.textBoxPriceService);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.textBoxCaptionService);
            this.panel2.Controls.Add(this.labCaptionService);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.LightUrn);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.butDelDev);
            this.panel2.Controls.Add(this.butaddDev);
            this.panel2.Controls.Add(this.textBoxRecognize);
            this.panel2.Controls.Add(this.label20);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(508, 191);
            this.panel2.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(361, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "руб";
            // 
            // textBoxPriceService
            // 
            this.textBoxPriceService.Location = new System.Drawing.Point(255, 75);
            this.textBoxPriceService.Name = "textBoxPriceService";
            this.textBoxPriceService.Size = new System.Drawing.Size(100, 20);
            this.textBoxPriceService.TabIndex = 17;
            this.textBoxPriceService.Text = "100";
            this.textBoxPriceService.Leave += new System.EventHandler(this.textBoxPriceService_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Стоимость услуги";
            // 
            // textBoxCaptionService
            // 
            this.textBoxCaptionService.Location = new System.Drawing.Point(255, 51);
            this.textBoxCaptionService.Name = "textBoxCaptionService";
            this.textBoxCaptionService.Size = new System.Drawing.Size(133, 20);
            this.textBoxCaptionService.TabIndex = 14;
            this.textBoxCaptionService.Text = "До тренировки";
            this.textBoxCaptionService.Leave += new System.EventHandler(this.textBoxCaptionService_Leave);
            // 
            // labCaptionService
            // 
            this.labCaptionService.AutoSize = true;
            this.labCaptionService.Location = new System.Drawing.Point(3, 58);
            this.labCaptionService.Name = "labCaptionService";
            this.labCaptionService.Size = new System.Drawing.Size(93, 13);
            this.labCaptionService.TabIndex = 13;
            this.labCaptionService.Text = "Название услуги";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(361, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "сек";
            // 
            // LightUrn
            // 
            this.LightUrn.Location = new System.Drawing.Point(255, 27);
            this.LightUrn.Name = "LightUrn";
            this.LightUrn.Size = new System.Drawing.Size(100, 20);
            this.LightUrn.TabIndex = 11;
            this.LightUrn.Text = "0";
            this.LightUrn.Leave += new System.EventHandler(this.LightUrn_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Длительность подсветки урны";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(361, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "сек";
            // 
            // butDelDev
            // 
            this.butDelDev.Location = new System.Drawing.Point(166, 109);
            this.butDelDev.Name = "butDelDev";
            this.butDelDev.Size = new System.Drawing.Size(154, 23);
            this.butDelDev.TabIndex = 8;
            this.butDelDev.Text = "Удалить устройство";
            this.butDelDev.UseVisualStyleBackColor = true;
            this.butDelDev.Click += new System.EventHandler(this.butDelDev_Click);
            // 
            // butaddDev
            // 
            this.butaddDev.Location = new System.Drawing.Point(6, 109);
            this.butaddDev.Name = "butaddDev";
            this.butaddDev.Size = new System.Drawing.Size(154, 23);
            this.butaddDev.TabIndex = 8;
            this.butaddDev.Text = "Добавить устройство";
            this.butaddDev.UseVisualStyleBackColor = true;
            this.butaddDev.Click += new System.EventHandler(this.butaddDev_Click);
            // 
            // textBoxRecognize
            // 
            this.textBoxRecognize.Location = new System.Drawing.Point(255, 4);
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
            this.label20.Size = new System.Drawing.Size(204, 13);
            this.label20.TabIndex = 6;
            this.label20.Text = "Длительность подсветки расходников";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.DevicetabControl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 200);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(508, 388);
            this.panel3.TabIndex = 1;
            // 
            // DevicetabControl
            // 
            this.DevicetabControl.Controls.Add(this.tabPageDevice);
            this.DevicetabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DevicetabControl.Location = new System.Drawing.Point(0, 0);
            this.DevicetabControl.Name = "DevicetabControl";
            this.DevicetabControl.SelectedIndex = 0;
            this.DevicetabControl.Size = new System.Drawing.Size(508, 388);
            this.DevicetabControl.TabIndex = 0;
            // 
            // tabPageDevice
            // 
            this.tabPageDevice.Location = new System.Drawing.Point(4, 22);
            this.tabPageDevice.Name = "tabPageDevice";
            this.tabPageDevice.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDevice.Size = new System.Drawing.Size(500, 362);
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
        private System.Windows.Forms.TextBox textBoxRecognize;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TabControl DevicetabControl;
        private System.Windows.Forms.TabPage tabPageDevice;
        private System.Windows.Forms.Button butaddDev;
        private System.Windows.Forms.Button butDelDev;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox LightUrn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxPriceService;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxCaptionService;
        private System.Windows.Forms.Label labCaptionService;
    }
}
