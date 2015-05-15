using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlycReSoft.TandemGlycopeptidePipeline;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlycReSoft.UnitTests
{
    public class TestDirect
    {        
        public static ScriptManager Scripter = new ScriptManager();

        public static int Main()
        {

            new TandemGlycopeptideTests().TestBuildModelPipeline();

            //PythonProcessManager.Verbose = true;
            //Console.WriteLine("Starting test");
            //PythonProcessManager proc = new PythonProcessManager(
            //    Scripter.PythonExecutablePath,
            //    String.Format(
            //    Path.Combine(Scripter.ScriptRoot, "run_pipeline.py model-diagnostics --model-file {0} --method {1}"), TandemGlycopeptideTests.ModelCsvFile.QuoteWrap(), "full_random_forest")
            //    , Scripter.ScriptRoot);

            //proc.Start();
            //proc.WaitForExit();
            //Console.WriteLine(proc.GenerateDumpMessage());

            //ResultsRepresentation res = JsonConvert.DeserializeObject<ResultsRepresentation>(File.ReadAllText(@"C:\Users\jaklein\Dropbox\GlycomicsSandbox\GlycReSoft2_Devel\GlycReSoft2\GlycReSoftTandemGlycopeptidePipeline\TandemGlycoPeptidePipeline\python\test_data\MS1-matching-output 20131219_005.scored_fdr.json"));
            //foreach (JProperty prop in res.Metadata.Properties())
            //{
            //    Console.WriteLine("Property {0}", prop.Name);
            //}
            
            //JToken fdr = res.Metadata["fdr"];
            //List<FalseDiscoveryRateTestResult> fdrList = fdr.ToObject<List<FalseDiscoveryRateTestResult>>();
            //foreach (var test in fdrList)
            //{
            //    foreach (var key in test.Threshold.Keys)
            //        Console.WriteLine(key);
            //}
            
                     
            

            Console.WriteLine("Complete");

            Console.ReadLine();

            return 0;
        }

    }
}
