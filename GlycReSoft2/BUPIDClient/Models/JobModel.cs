using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUPIDClient
{
    public enum JobState
    {
        New, Running, Complete
    }

    public class JobModel
    {
        public String JobName { get; set; }        
        public int JobID { get; set; }
        public JobState State { get; set; }
        public RunModel Run { get; set; }

        public JobModel(int id, string name) : this(id, name, JobState.New) {
        }

        public JobModel(int id, string name, JobState state, RunModel run = null)
        {
            JobID = id;
            JobName = name;
            State = state;
            Run = run;
        }

        public void AddRun(RunModel run)
        {
            Run = run;
            State = JobState.Running;
        }

        public void HasResults()
        {
            State = JobState.Complete;
        }

        public static List<JobModel> Parse(String serverSimpleFormatString)
        {
            List<JobModel> jobs = new List<JobModel>();
            String[] jobText = serverSimpleFormatString.Split('\n');
            foreach(String line in jobText){
                try
                {
                    if (line == "") continue;
                    String[] parts = line.Split('|');
                    int id = Convert.ToInt32(parts[0]);
                    string name = String.Join("|", parts.Skip(1));
                    jobs.Add(new JobModel(id, name));
                }
                catch (Exception ex)
                {
                    throw new JobModelParseException("An error occurred while parsing " + line, ex);
                }
            }
            return jobs;
        }
    }



    [Serializable]
    public class JobModelException : Exception
    {
        public JobModelException() { }
        public JobModelException(string message) : base(message) { }
        public JobModelException(string message, Exception inner) : base(message, inner) { }
        protected JobModelException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class JobModelParseException : JobModelException
    {
        public JobModelParseException() { }
        public JobModelParseException(string message) : base(message) { }
        public JobModelParseException(string message, Exception inner) : base(message, inner) { }
        protected JobModelParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }


    [Serializable]
    public class JobProcessRequestException : JobModelException
    {
        public JobProcessRequestException() { }
        public JobProcessRequestException(string message) : base(message) { }
        public JobProcessRequestException(string message, Exception inner) : base(message, inner) { }
        protected JobProcessRequestException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
