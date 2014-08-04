using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;


namespace GlycReSoft
{
    class Program
    {

        //These codes prevent two instances of Glycresoft 2 to be run at the same time. They're copied from http://stackoverflow.com/questions/184084/how-to-force-c-sharp-net-app-to-run-only-one-instance-in-windows
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);


        [STAThread]
        static void Main(string[] args)
        {
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "MyApplicationName", out createdNew))
            {
                if (createdNew)
                {
                    //If no other instances are running, open the software:
                    //Welcome Screen
                    WelcomeScreen wl = new WelcomeScreen();
                    Thread th = new Thread(new ThreadStart(wl.start));
                    th.Start();

                    //Reset Compositions to default:
                    try
                    {
                        String defaultpath = Application.StartupPath + "\\compositionsDefault.cpos";
                        String currentpath = Application.StartupPath + "\\compositionsCurrent.cpos";
                        File.Copy(defaultpath, currentpath, true);
                    }
                    catch (Exception mainex)
                    {
                        MessageBox.Show("Error in creating composition file. Error:" + mainex.Message);
                    }
                    System.Threading.Thread.Sleep(2000);
                    th.Abort();
                    //Run the Program.
                    Form1 mainForm = new Form1();
                    Application.Run(mainForm);
                }
                else
                {
                    //Another instance is running, don't open.
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
            }

        }
        
        //This is the welcome screen
        public class WelcomeScreen
        {
            Welcome wl = new Welcome();
            public void start()
            {
                wl.ShowDialog();
            }
            public void Close()
            {
                wl.Close();
            }
        }


        //This is the "please wait" message box.
        public class ShowPleaseWait
        {
            PleaseWait pw = new PleaseWait();
            public void Start()
            {
                pw.ShowDialog();
            }
            public void Stop()
            {
                pw.Close();
            }
        }
    }


    //http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp This function of randomizing a list is good, so I am using it. It is written by grenade.
    //start of grenade's code.
    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

    static class MyExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        // End of grenade's code
        //This is a standard deviation function from ehre http://stackoverflow.com/questions/2253874/linq-equivalent-for-standard-deviation
        public static double StdDev(this IEnumerable<double> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }

        public static MemoryStream ToStream(this string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}
