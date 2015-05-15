using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlycReSoft.TandemGlycopeptidePipeline;
using System.IO;


namespace GlycReSoft.UnitTests
{
    [TestClass]
    public class ExternalProcessControlTests
    {
        String ModelJsonFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "MS1-matching-output 20131219_005.model.json");

        [TestMethod]
        public void ExecuteArbitraryPythonDirect()
        {
            PythonProcessManager.Verbose = true;
            PythonProcessManager proc = new PythonProcessManager("python", " -c print('Hello')");
            Console.WriteLine("PYTHONPATH = {0}", proc.PythonPath);
            proc.Start();
            Console.WriteLine("Running");
            proc.WaitForExit();
        }

        [TestMethod]
        public void ExecuteScriptManagerHelpCall()
        {
            PythonProcessManager.Verbose = true;
            ScriptManager scripter = new ScriptManager();
            Assert.IsTrue(scripter.VerifyFileSystemTargets());
            Console.WriteLine("Running Dependency Checks");
            scripter.InstallPythonDependencies();
            
            
        }

        [TestMethod]
        public void ExecuteScriptManagerRunModelDiagnostics()
        {
            PythonProcessManager.Verbose = true;
            ScriptManager scripter = new ScriptManager();            
            Assert.IsTrue(scripter.VerifyFileSystemTargets());
            Console.WriteLine("Running Model Diagnostics");
            String diagnosticOutput = scripter.RunModelDiagnosticTask(ModelJsonFile, "full_random_forest");
            Assert.IsTrue(File.Exists(diagnosticOutput));
        }
    }
}
