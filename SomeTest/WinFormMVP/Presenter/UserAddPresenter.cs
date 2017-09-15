using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinFormMVP.Model;
using WinFormMVP.Model.Entity;
using WinFormMVP.View;

namespace WinFormMVP.Presenter
{
    public class UserAddPresenter:IPresenter
    {
        private readonly IUser _model;
        private readonly IUserAdd _view;
        private readonly ApplicationFacade _facade = ApplicationFacade.Instance;

        public UserAddPresenter(IUser model, IUserAdd view)
        {
            this._model = model;
            this._view = view;
            WireUpViewEvents();
        }

        private void WireUpViewEvents()
        {
            this._view.UserAddEvent += new EventHandler(_view_UserAdd);
        }

        private void _view_UserAdd(object sender, EventArgs e)
        {
            var user = new User()
                           {
                               Name = _view.UserName,
                               Age = Convert.ToInt32(_view.UserAge)
                           };

            _model.AddItem(user);
            _facade.SendNotification(ApplicationFacade.USER_ADDED);
        }

        #region Implementation of IPresenter

        public void ResponseNotification(string message)
        {
            
        }

        #endregion
    }
}
