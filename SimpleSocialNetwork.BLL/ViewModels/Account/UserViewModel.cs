using SimpleSocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSocialNetwork.BLL.ViewModels.Account
{
    public class UserViewModel
    {
        public User User { get; set; }
        public UserViewModel(User user) 
        {
            this.User = user;
        }
        public List<User> Friends { get; set; }
    }
}
