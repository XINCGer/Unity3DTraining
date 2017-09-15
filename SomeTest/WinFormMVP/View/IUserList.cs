using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormMVP.View
{
    public interface IUserList
    {
        object Users { set; }

        event EventHandler StartLoadEvent;
    }

}
