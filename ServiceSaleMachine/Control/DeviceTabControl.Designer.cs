namespace ServiceSaleMachine
{
    partial class DeviceTabControl
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
            this.textBoxNameDevice = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxIdDevice = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxLimitTime = new System.Windows.Forms.TextBox();
            this.textBoxTimeWork = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxNameDevice);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBoxIdDevice);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxLimitTime);
            this.panel1.Controls.Add(this.textBoxTimeWork);
            this.panel1.Controls.Add(this.label21);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(412, 177);
            this.panel1.TabIndex = 0;
            // 
            // textBoxNameDevice
            // 
            this.textBoxNameDevice.Location = new System.Drawing.Point(165, 39);
            this.textBoxNameDevice.Name = "textBoxNameDevice";
            this.textBoxNameDevice.Size = new System.Drawing.Size(100, 20);
            this.textBoxNameDevice.TabIndex = 18;
            this.textBoxNameDevice.Text = "0";
            this.textBoxNameDevice.Leave += new System.EventHandler(this.textBoxNameDevice_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Наименование устройства";
            // 
            // textBoxIdDevice
            // 
            this.textBoxIdDevice.Location = new System.Drawing.Point(165, 12);
            this.textBoxIdDevice.Name = "textBoxIdDevice";
            this.textBoxIdDevice.Size = new System.Drawing.Size(100, 20);
            this.textBoxIdDevice.TabIndex = 16;
            this.textBoxIdDevice.Text = "0";
            this.textBoxIdDevice.Leave += new System.EventHandler(this.textBoxIdDevice_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Номер устройства";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(287, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "мин";
            this.label2.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(287, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "мин";
            this.label1.Visible = false;
            // 
            // textBoxLimitTime
            // 
            this.textBoxLimitTime.Location = new System.Drawing.Point(165, 93);
            this.textBoxLimitTime.Name = "textBoxLimitTime";
            this.textBoxLimitTime.Size = new System.Drawing.Size(100, 20);
            this.textBoxLimitTime.TabIndex = 12;
            this.textBoxLimitTime.Text = "0";
            this.textBoxLimitTime.Visible = false;
            this.textBoxLimitTime.Leave += new System.EventHandler(this.textBoxLimitTime_Leave);
            // 
            // textBoxTimeWork
            // 
            this.textBoxTimeWork.Location = new System.Drawing.Point(165, 66);
            this.textBoxTimeWork.Name = "textBoxTimeWork";
            this.textBoxTimeWork.Size = new System.Drawing.Size(100, 20);
            this.textBoxTimeWork.TabIndex = 13;
            this.textBoxTimeWork.Text = "0";
            this.textBoxTimeWork.Visible = false;
            this.textBoxTimeWork.Leave += new System.EventHandler(this.textBoxTimeWork_Leave);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 100);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(140, 13);
            this.label21.TabIndex = 10;
            this.label21.Text = "Предельное время услуги";
            this.label21.Visible = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 73);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(116, 13);
            this.label20.TabIndex = 11;
            this.label20.Text = "Длительность услуги";
            this.label20.Visible = false;
            // 
            // DeviceTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "DeviceTabControl";
            this.Size = new System.Drawing.Size(412, 177);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxLimitTime;
        private System.Windows.Forms.TextBox textBoxTimeWork;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxIdDevice;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxNameDevice;
        private System.Windows.Forms.Label label3;
    }
}
