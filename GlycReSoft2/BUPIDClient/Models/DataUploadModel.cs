using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUPIDClient
{
    public class DataUploadModel
    {
        public String JobDescription;
        public String DataFilePath;

        public DataUploadModel(String desc, String data)
        {
            JobDescription = desc;
            DataFilePath = data;
        }

        public NameValueCollection ToNameValueCollection()
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("description", JobDescription);
            return nameValueCollection;
        }
    }

    [Serializable]
    public class DataUploadException : Exception
    {
        public DataUploadException() { }
        public DataUploadException(string message) : base(message) { }
        public DataUploadException(string message, Exception inner) : base(message, inner) { }
        protected DataUploadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class DataUploadNameNotUniqueException : DataUploadException
    {
        public DataUploadNameNotUniqueException() { }
        public DataUploadNameNotUniqueException(string message) : base(message) { }
        public DataUploadNameNotUniqueException(string message, Exception inner) : base(message, inner) { }
        protected DataUploadNameNotUniqueException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class DataUploadFileExceedsQuotaException : DataUploadException
    {
        public DataUploadFileExceedsQuotaException() { }
        public DataUploadFileExceedsQuotaException(string message) : base(message) { }
        public DataUploadFileExceedsQuotaException(string message, Exception inner) : base(message, inner) { }
        protected DataUploadFileExceedsQuotaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
