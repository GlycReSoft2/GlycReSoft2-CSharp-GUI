using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlycReSoft;
using GlycReSoft.TandemGlycopeptidePipeline;
using GlycReSoft.CompositionHypothesis;
using Newtonsoft.Json;

namespace GlycReSoft.UnitTests
{
    /// <summary>
    /// Assumes that python is on the system path
    /// </summary>
    [TestClass]
    public class TandemGlycopeptideTests
    {
        public static string FixtureFileDirectory = "TestFixtureFiles";

        public static String MS1MatchFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "MS1-matching-output 20131219_005.csv");
        public static String MS2DeconvolutionFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "YAML-input-for-MS2-20131219_005.mzML.results");
        public static String GlycosylationSitesFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "USSR-glycosylation site list.txt");
        public static String ProteinProspectorXmlFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "KK-USSR-digest-Prospector output.xml");
        public static String ModelJsonFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "MS1-matching-output 20131219_005.model.json");

        [TestMethod]
        public void TestBuildModelPipeline()
        {            
            AnalysisPipeline pipeline = new AnalysisPipeline(MS1MatchFile, GlycosylationSitesFile, MS2DeconvolutionFile,
                        null, null, 1e-5, 2e-5, proteinProspectorXMLFilePath: ProteinProspectorXmlFile);
            ResultsRepresentation result = pipeline.RunModelBuilder();
            Assert.IsInstanceOfType(result, typeof(ResultsRepresentation));           
    
            
        }

        [TestMethod]
        public void TestClassifyWithModelPipeline()
        {
            AnalysisPipeline pipeline = new AnalysisPipeline(MS1MatchFile, GlycosylationSitesFile, MS2DeconvolutionFile, ModelJsonFile, null, 1e-5, 2e-5, proteinProspectorXMLFilePath: ProteinProspectorXmlFile);

            ResultsRepresentation result = pipeline.RunClassification();
            Assert.IsInstanceOfType(result, typeof(ResultsRepresentation)); 
        }

        [TestMethod]
        public void TestResultsRepresentation()
        {
            
            JsonSerializer serializer = new JsonSerializer();
            ResultsRepresentation result =  (ResultsRepresentation)serializer.Deserialize(File.OpenText(ModelJsonFile), typeof(ResultsRepresentation ));
            Assert.IsInstanceOfType(result, typeof(ResultsRepresentation));           
        }

        [TestMethod]
        public void TestModelDiagnostics()
        {
            ScriptManager scripter = new ScriptManager();
            String resultsPlotsPath = scripter.RunModelDiagnosticTask(ModelJsonFile, "full_random_forest");
            Assert.IsTrue(File.Exists(resultsPlotsPath));
        }

    }
}
