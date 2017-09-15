using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinFormMVP.Model;
using WinFormMVP.Presenter;
using WinFormMVP.View;

namespace WinFormMVP
{
    public partial class Form1 : Form
    {
        private readonly ApplicationFacade _facade = ApplicationFacade.Instance;

        public Form1()
        {
            InitializeComponent();

            IUser model = new UserProxy();
            IUserList viewList = this.userList1;
            _facade.Presenters.Add(new UserListPresenter(model, viewList));

            IUserAdd viewAdd = this.userAdd1;
            _facade.Presenters.Add(new UserAddPresenter(model, viewAdd));
        }


       
    }
}
