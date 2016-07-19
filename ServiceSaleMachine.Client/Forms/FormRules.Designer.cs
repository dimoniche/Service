namespace ServiceSaleMachine.Client
{
    partial class FormRules
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
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.pbxRetToMain = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBoxEx1 = new ServiceSaleMachine.RichTextBoxEx();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxRetToMain)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.Controls.Add(this.pbxRetToMain, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 444);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(952, 89);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // pbxRetToMain
            // 
            this.pbxRetToMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbxRetToMain.Location = new System.Drawing.Point(3, 3);
            this.pbxRetToMain.Name = "pbxRetToMain";
            this.pbxRetToMain.Size = new System.Drawing.Size(136, 83);
            this.pbxRetToMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxRetToMain.TabIndex = 1;
            this.pbxRetToMain.TabStop = false;
            this.pbxRetToMain.Click += new System.EventHandler(this.pbxRetToMain_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.richTextBoxEx1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(952, 444);
            this.panel1.TabIndex = 3;
            // 
            // richTextBoxEx1
            // 
            this.richTextBoxEx1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxEx1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBoxEx1.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxEx1.Name = "richTextBoxEx1";
            this.richTextBoxEx1.ReadOnly = true;
            this.richTextBoxEx1.Size = new System.Drawing.Size(952, 444);
            this.richTextBoxEx1.TabIndex = 3;
            this.richTextBoxEx1.Text = "Это инструкция\nВ общем, нажимаете на кнопку, получаете результат.\nИ денежки не за" +
    "будьте!\n";
            // 
            // FormRules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 533);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tableLayoutPanel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRules";
            this.Text = "FormHelp";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormRules_FormClosed);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxRetToMain)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.PictureBox pbxRetToMain;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private RichTextBoxEx richTextBoxEx1;
    }
}