namespace WinFormMVP.View
{
    partial class UserAdd
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
            this.txbName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAge = new System.Windows.Forms.Label();
            this.txbAge = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txbName
            // 
            this.txbName.Location = new System.Drawing.Point(68, 8);
            this.txbName.Name = "txbName";
            this.txbName.Size = new System.Drawing.Size(100, 21);
            this.txbName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "name";
            // 
            // lblAge
            // 
            this.lblAge.AutoSize = true;
            this.lblAge.Location = new System.Drawing.Point(187, 11);
            this.lblAge.Name = "lblAge";
            this.lblAge.Size = new System.Drawing.Size(23, 12);
            this.lblAge.TabIndex = 2;
            this.lblAge.Text = "Age";
            // 
            // txbAge
            // 
            this.txbAge.Location = new System.Drawing.Point(234, 7);
            this.txbAge.Name = "txbAge";
            this.txbAge.Size = new System.Drawing.Size(100, 21);
            this.txbAge.TabIndex = 3;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(354, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Save";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // UserAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txbAge);
            this.Controls.Add(this.lblAge);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txbName);
            this.Name = "UserAdd";
            this.Size = new System.Drawing.Size(455, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txbName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblAge;
        private System.Windows.Forms.TextBox txbAge;
        private System.Windows.Forms.Button btnAdd;
    }
}
