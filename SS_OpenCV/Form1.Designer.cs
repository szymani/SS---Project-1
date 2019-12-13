namespace SS_OpenCV
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbFiltry = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbWeight = new System.Windows.Forms.TextBox();
            this.tbCoef8 = new System.Windows.Forms.TextBox();
            this.tbCoef9 = new System.Windows.Forms.TextBox();
            this.tbCoef7 = new System.Windows.Forms.TextBox();
            this.tbCoef5 = new System.Windows.Forms.TextBox();
            this.tbCoef6 = new System.Windows.Forms.TextBox();
            this.tbCoef4 = new System.Windows.Forms.TextBox();
            this.tbCoef2 = new System.Windows.Forms.TextBox();
            this.tbCoef3 = new System.Windows.Forms.TextBox();
            this.tbCoef1 = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(95, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filter";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 286);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Weight:";
            // 
            // lbFiltry
            // 
            this.lbFiltry.FormattingEnabled = true;
            this.lbFiltry.ItemHeight = 25;
            this.lbFiltry.Location = new System.Drawing.Point(100, 106);
            this.lbFiltry.Name = "lbFiltry";
            this.lbFiltry.Size = new System.Drawing.Size(247, 29);
            this.lbFiltry.TabIndex = 2;
            this.lbFiltry.SelectedIndexChanged += new System.EventHandler(this.ListBox1_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbWeight);
            this.groupBox1.Controls.Add(this.tbCoef8);
            this.groupBox1.Controls.Add(this.tbCoef9);
            this.groupBox1.Controls.Add(this.tbCoef7);
            this.groupBox1.Controls.Add(this.tbCoef5);
            this.groupBox1.Controls.Add(this.tbCoef6);
            this.groupBox1.Controls.Add(this.tbCoef4);
            this.groupBox1.Controls.Add(this.tbCoef2);
            this.groupBox1.Controls.Add(this.tbCoef3);
            this.groupBox1.Controls.Add(this.tbCoef1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(91, 176);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(334, 378);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Coeficients";
            // 
            // tbWeight
            // 
            this.tbWeight.Location = new System.Drawing.Point(162, 286);
            this.tbWeight.Name = "tbWeight";
            this.tbWeight.Size = new System.Drawing.Size(50, 31);
            this.tbWeight.TabIndex = 16;
            // 
            // tbCoef8
            // 
            this.tbCoef8.Location = new System.Drawing.Point(162, 179);
            this.tbCoef8.Name = "tbCoef8";
            this.tbCoef8.Size = new System.Drawing.Size(50, 31);
            this.tbCoef8.TabIndex = 15;
            // 
            // tbCoef9
            // 
            this.tbCoef9.Location = new System.Drawing.Point(229, 179);
            this.tbCoef9.Name = "tbCoef9";
            this.tbCoef9.Size = new System.Drawing.Size(50, 31);
            this.tbCoef9.TabIndex = 14;
            // 
            // tbCoef7
            // 
            this.tbCoef7.Location = new System.Drawing.Point(95, 179);
            this.tbCoef7.Name = "tbCoef7";
            this.tbCoef7.Size = new System.Drawing.Size(50, 31);
            this.tbCoef7.TabIndex = 13;
            // 
            // tbCoef5
            // 
            this.tbCoef5.Location = new System.Drawing.Point(162, 128);
            this.tbCoef5.Name = "tbCoef5";
            this.tbCoef5.Size = new System.Drawing.Size(50, 31);
            this.tbCoef5.TabIndex = 12;
            // 
            // tbCoef6
            // 
            this.tbCoef6.Location = new System.Drawing.Point(229, 128);
            this.tbCoef6.Name = "tbCoef6";
            this.tbCoef6.Size = new System.Drawing.Size(50, 31);
            this.tbCoef6.TabIndex = 11;
            // 
            // tbCoef4
            // 
            this.tbCoef4.Location = new System.Drawing.Point(95, 128);
            this.tbCoef4.Name = "tbCoef4";
            this.tbCoef4.Size = new System.Drawing.Size(50, 31);
            this.tbCoef4.TabIndex = 10;
            // 
            // tbCoef2
            // 
            this.tbCoef2.Location = new System.Drawing.Point(162, 76);
            this.tbCoef2.Name = "tbCoef2";
            this.tbCoef2.Size = new System.Drawing.Size(50, 31);
            this.tbCoef2.TabIndex = 9;
            // 
            // tbCoef3
            // 
            this.tbCoef3.Location = new System.Drawing.Point(229, 76);
            this.tbCoef3.Name = "tbCoef3";
            this.tbCoef3.Size = new System.Drawing.Size(50, 31);
            this.tbCoef3.TabIndex = 8;
            // 
            // tbCoef1
            // 
            this.tbCoef1.Location = new System.Drawing.Point(95, 76);
            this.tbCoef1.Name = "tbCoef1";
            this.tbCoef1.Size = new System.Drawing.Size(50, 31);
            this.tbCoef1.TabIndex = 6;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(112, 580);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(124, 54);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(278, 580);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(124, 54);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 675);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbFiltry);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Non-Uniform filter selection";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbFiltry;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox tbCoef1;
        private System.Windows.Forms.TextBox tbWeight;
        private System.Windows.Forms.TextBox tbCoef8;
        private System.Windows.Forms.TextBox tbCoef9;
        private System.Windows.Forms.TextBox tbCoef7;
        private System.Windows.Forms.TextBox tbCoef5;
        private System.Windows.Forms.TextBox tbCoef6;
        private System.Windows.Forms.TextBox tbCoef4;
        private System.Windows.Forms.TextBox tbCoef2;
        private System.Windows.Forms.TextBox tbCoef3;
        private System.Windows.Forms.Button btnCancel;
    }
}