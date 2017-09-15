using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormMVP.View
{
    public interface IUserAdd
    {
        event EventHandler UserAddEvent;

        string UserName { get; set; }

        string UserAge { get; set; }

    }
}
