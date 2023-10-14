using SimpleSocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSocialNetwork.BLL.ViewModels.Account
{
    public class ChatViewModel
    {
        public User You { get; set; }

        public User ToWhom { get; set; }

        public List<Message> History { get; set; }

        public MessageViewModel NewMessage { get; set; }

        public ChatViewModel()
        {
            NewMessage = new MessageViewModel();
        }
    }
}
