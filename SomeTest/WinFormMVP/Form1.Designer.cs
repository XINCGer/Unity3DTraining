namespace WinFormMVP
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
            this.userAdd1 = new WinFormMVP.View.UserAdd();
            this.userList1 = new WinFormMVP.View.UserList();
            this.SuspendLayout();
            // 
            // userAdd1
            // 
            this.userAdd1.Location = new System.Drawing.Point(42, 445);
            this.userAdd1.Name = "userAdd1";
            this.userAdd1.Size = new System.Drawing.Size(455, 35);
            this.userAdd1.TabIndex = 1;
            // 
            // userList1
            // 
            this.userList1.Location = new System.Drawing.Point(22, 12);
            this.userList1.Name = "userList1";
            this.userList1.Size = new System.Drawing.Size(698, 406);
            this.userList1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 517);
            this.Controls.Add(this.userAdd1);
            this.Controls.Add(this.userList1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private View.UserList userList1;
        private View.UserAdd userAdd1;
    }
}

