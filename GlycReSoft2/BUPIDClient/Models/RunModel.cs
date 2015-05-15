using System;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BUPIDClient
{
    public class RunModel
    {
        public String Key;
        public String Name;
        public String JobID;
        public String ID;
        public String Date;

        /// <summary>
        /// Parse Constructor
        /// </summary>
        /// <param name="run"></param>
        public RunModel(string run)
        {
            string[] fields = run.Split('|');
            ID = fields[0];
            Date = fields[1];
            Key = fields[2];
            JobID = fields[3];
            Name = fields[4];
        }

        /// <summary>
        /// Fully specified Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="jobId"></param>
        /// <param name="id"></param>
        /// <param name="date"></param>
        public RunModel(string key, string name, string jobId, string id, string date)
        {
            Key = key;
            Name = name;
            ID = id;
            JobID = jobId;
            Date = date;
        }

        public override String ToString()
        {
            return String.Format("{0}@{1}[{2}, {3}]", Name, Date, Key, ID);
        }

        /// <summary>
        /// Parses zero or more runs from given string.
        /// </summary>
        /// <param name="responseString"></param>
        /// <returns></returns>
        public static List<RunModel> Parse(string responseString)
        {
            List<RunModel> results = new List<RunModel>();
            string[] runs = responseString.Split('\n');
            foreach (String run in runs)
            {
                try
                {
                    if (run.Length == 0) continue;
                    var runModel = new RunModel(run);
                    results.Add(runModel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return results;
        }
    }
}
