using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BUPIDClient
{
    /// <summary>
    /// A place to keep all the helper methods necessary to make using web requests in C#
    /// less cumbersome.
    /// </summary>
    public static class InternalHttpUtils
    {

        public static Byte[] NameValueToByteArray(NameValueCollection data)
        {
            var builder = new StringBuilder();

            foreach (String key in data.AllKeys)
            {
                builder.AppendFormat("{0}={1}&", key, HttpUtility.UrlEncode(data[key].ToString()));
            }
            builder.Remove(builder.Length - 1, 1); // remove the last '&'
            Byte[] dataBytes = Encoding.UTF8.GetBytes(builder.ToString());

            return dataBytes;
        }
    }
}
