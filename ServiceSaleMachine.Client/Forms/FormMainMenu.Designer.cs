namespace ServiceSaleMachine.Client
{
    partial class FormMainMenu
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
            this.panel3 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.TimeOutTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pBxBegin = new System.Windows.Forms.PictureBox();
            this.pBxPhilosof = new System.Windows.Forms.PictureBox();
            this.pBxInstruction = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBxBegin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBxPhilosof)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBxInstruction)).BeginInit();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(678, 25);
            this.panel3.TabIndex = 3;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // TimeOutTimer
            // 
            this.TimeOutTimer.Interval = 1000;
            this.TimeOutTimer.Tick += new System.EventHandler(this.TimeOutTimer_Tick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.pBxBegin, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.pBxPhilosof, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.pBxInstruction, 0, 5);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(138, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 7;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(400, 460);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(678, 466);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // pBxBegin
            // 
            this.pBxBegin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pBxBegin.Location = new System.Drawing.Point(3, 72);
            this.pBxBegin.Name = "pBxBegin";
            this.pBxBegin.Size = new System.Drawing.Size(394, 63);
            this.pBxBegin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBxBegin.TabIndex = 0;
            this.pBxBegin.TabStop = false;
            this.pBxBegin.Click += new System.EventHandler(this.pbxStart_Click);
            // 
            // pBxPhilosof
            // 
            this.pBxPhilosof.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pBxPhilosof.Location = new System.Drawing.Point(3, 198);
            this.pBxPhilosof.Name = "pBxPhilosof";
            this.pBxPhilosof.Size = new System.Drawing.Size(394, 63);
            this.pBxPhilosof.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBxPhilosof.TabIndex = 1;
            this.pBxPhilosof.TabStop = false;
            this.pBxPhilosof.Click += new System.EventHandler(this.pBxPhilosof_Click);
            // 
            // pBxInstruction
            // 
            this.pBxInstruction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pBxInstruction.Location = new System.Drawing.Point(3, 324);
            this.pBxInstruction.Name = "pBxInstruction";
            this.pBxInstruction.Size = new System.Drawing.Size(394, 63);
            this.pBxInstruction.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBxInstruction.TabIndex = 2;
            this.pBxInstruction.TabStop = false;
            this.pBxInstruction.Click += new System.EventHandler(this.pbxHelp_Click);
            // 
            // FormMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 491);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMainMenu";
            this.Text = "";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMainMenu_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMainMenu_KeyDown);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pBxBegin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBxPhilosof)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBxInstruction)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer TimeOutTimer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pBxBegin;
        private System.Windows.Forms.PictureBox pBxPhilosof;
        private System.Windows.Forms.PictureBox pBxInstruction;
    }
}