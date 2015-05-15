using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUPIDClient
{
    public class UserLoginModel
    {
        public String UserName;
        public String Password;
        
        public UserLoginModel(String uname, String pword)
        {
            this.UserName = uname;
            this.Password = pword;
        }        

    }

    [Serializable]
    public class UserLoginException : Exception
    {
        public UserLoginException() { }
        public UserLoginException(string message) : base(message) { }
        public UserLoginException(string message, Exception inner) : base(message, inner) { }
        protected UserLoginException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
