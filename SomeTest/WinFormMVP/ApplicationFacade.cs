using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinFormMVP.Model;
using WinFormMVP.Presenter;
using WinFormMVP.View;

namespace WinFormMVP
{
    public class ApplicationFacade
    {
        #region Notification name constants
        public const string USER_ADDED = "userAdded";


        private static ApplicationFacade _mInstance;
        private static readonly object MStaticSyncRoot = new object();

     
        public readonly IList<IPresenter> Presenters = new List<IPresenter>();

        #endregion

        #region Accessors

        /// <summary>
        /// Facade Singleton Factory method.  This method is thread safe.
        /// </summary>
        public new static ApplicationFacade Instance
        {
            get
            {
                if (_mInstance == null)
                {
                    lock (MStaticSyncRoot)
                    {
                        if (_mInstance == null) _mInstance = new ApplicationFacade();
                    }
                }

                return _mInstance;
            }
        }

        public void SendNotification(string message)
        {
            foreach (var presenter in Presenters)
            {
                presenter.ResponseNotification(message);
            }
        }
      
        #endregion
    }
}
