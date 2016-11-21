namespace ServiceSaleMachine.Client
{
    partial class FormWaitClientGif
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
            this.ScreenSever = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ScreenSever)).BeginInit();
            this.SuspendLayout();
            // 
            // ScreenSever
            // 
            this.ScreenSever.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScreenSever.Location = new System.Drawing.Point(0, 0);
            this.ScreenSever.Name = "ScreenSever";
            this.ScreenSever.Size = new System.Drawing.Size(784, 561);
            this.ScreenSever.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ScreenSever.TabIndex = 0;
            this.ScreenSever.TabStop = false;
            this.ScreenSever.Click += new System.EventHandler(this.ScreenSever_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 50;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // FormWaitClientGif
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.ScreenSever);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormWaitClientGif";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormWaitClientGif";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWaitClientGif_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormWaitClientGif_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.ScreenSever)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ScreenSever;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}