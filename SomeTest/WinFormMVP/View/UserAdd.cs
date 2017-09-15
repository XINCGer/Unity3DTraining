using System;
using System.Windows.Forms;

namespace WinFormMVP.View
{
    public partial class UserAdd : UserControl, IUserAdd
    {
        public event EventHandler UserAddEvent;
        public string UserName
        {
            set { this.txbName.Text = value; }
            get { return this.txbName.Text; }
        }

        public string UserAge
        {
            set { this.txbAge.Text = value; }
            get { return this.txbAge.Text; }
        }

        public UserAdd()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
           if (UserAddEvent != null) UserAddEvent(this, e);
        }
    }
}
