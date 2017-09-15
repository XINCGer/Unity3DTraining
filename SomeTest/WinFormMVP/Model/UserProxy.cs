using System.Collections.Generic;
using WinFormMVP.Model.Entity;

namespace WinFormMVP.Model
{
    public class UserProxy:IUser
    {
        private readonly IList<User> _users = new List<User>();

        public UserProxy()
        {
            // generate some test data			
            AddItem(new User { Name = "Peter", Age = 29 });
        }

        /// <summary>
        /// Return data property cast to proper type
        /// </summary>
        public IList<User> Users
        {
            get { return _users; }
        }

        /// <summary>
        /// add an item to the data
        /// </summary>
        /// <param name="user"></param>
        public void AddItem(User user)
        {
            Users.Add(user);
        }

        /// <summary>
        /// update an item in the data
        /// </summary>
        /// <param name="user"></param>
        public void UpdateItem(User user)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Name.Equals(user.Name))
                {
                    Users[i] = user;
                    break;
                }
            }
        }

        /// <summary>
        /// delete an item in the data
        /// </summary>
        /// <param name="user"></param>
        public void DeleteItem(User user)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Name.Equals(user.Name))
                {
                    Users.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
