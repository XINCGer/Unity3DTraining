using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace WinFormMVP.View
{
    public partial class UserList : UserControl, IUserList
    {
        public event EventHandler StartLoadEvent;

        public object Users
        {
            set
            {
                this.dataGridView1.DataSource = null;
                this.dataGridView1.DataSource = value;
            }
        }

        public UserList()
        {
            InitializeComponent();
        }

        private void UserList_Load(object sender, System.EventArgs e)
        {
            if (StartLoadEvent != null) StartLoadEvent(sender, e);
        }
    }
}
