using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2NG_server
{
    public class Channel
    {
        public int ID;
        public string Name;
        public string Password;

        public bool Join(Cookies cookie, string Password)
        {
            if (this.Password != Password) return false;
            cookie.ChannelID = ID;
            return true;
        }
    }
}
