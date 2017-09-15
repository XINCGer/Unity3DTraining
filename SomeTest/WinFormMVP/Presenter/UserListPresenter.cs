using System;
using System.Reflection;
using WinFormMVP.Model;
using WinFormMVP.View;

namespace WinFormMVP.Presenter
{
    public class UserListPresenter:IPresenter
    {
        private readonly IUser _model;
        private readonly IUserList _view;
        private readonly ApplicationFacade _facade = ApplicationFacade.Instance;

        public UserListPresenter(IUser model, IUserList view)
        {
            this._model = model;
            this._view = view;
            WireUpViewEvents();
        }

        public void ResponseNotification(string message)
        {
            switch (message)
            {
                case ApplicationFacade.USER_ADDED:
                    _view.Users = _model.Users;
                    break;
            }
        }

        private void WireUpViewEvents()
        {
            this._view.StartLoadEvent += new EventHandler(_view_UpdateUserList);
        }

        private void _view_UpdateUserList(object sender, EventArgs e)
        {
            _view.Users = _model.Users;
        }
    }
}
