using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinFormMVP.Model.Entity;

namespace WinFormMVP.Model
{
    public interface IUser
    {
        IList<User> Users { get; }

        void AddItem(User user);
    }
}
