using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlycReSoft;
using GlycReSoft.TandemGlycopeptidePipeline;
using GlycReSoft.CompositionHypothesis;

namespace GlycReSoft.UnitTests
{
    /// <summary>
    /// Assumes that python is on the system path
    /// </summary>
    [TestClass]
    public class TandemGlycopeptideTests
    {
        static string FixtureFileDirectory = "TestFixtureFiles";

        String MS1MatchFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "MS1-matching-output 20131219_005.csv");
        String MS2DeconvolutionFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "YAML-input-for-MS2-20131219_005.mzML.results");
        String GlycosylationSitesFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "USSR-glycosylation site list.txt");
        String ProteinProspectorXmlFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "KK-USSR-digest-Prospector output.xml");

        String ModelCsvFile = Path.Combine(TandemGlycopeptideTests.FixtureFileDirectory, "MS1-matching-output 20131219_005.model.csv");

        [TestMethod]
        public void TestBuildModelPipeline()
        {            
            MSDigestReport msdigest = MSDigestReport.Load(ProteinProspectorXmlFile);

            AnalysisPipeline pipeline = new AnalysisPipeline(MS1MatchFile, GlycosylationSitesFile, MS2DeconvolutionFile,
                        null, null, 1e-5, 2e-5, msdigest.ConstantModifications, msdigest.VariableModifications);

            ResultsRepresentation result = pipeline.RunModelBuilder();
            Assert.IsInstanceOfType(result, typeof(ResultsRepresentation));           
        }

        [TestMethod]
        public void TestClassifyWithModelPipeline()
        {
            MSDigestReport msdigest = MSDigestReport.Load(ProteinProspectorXmlFile);

            AnalysisPipeline pipeline = new AnalysisPipeline(MS1MatchFile, GlycosylationSitesFile, MS2DeconvolutionFile, ModelCsvFile, null, 1e-5, 2e-5, msdigest.ConstantModifications, msdigest.VariableModifications);

            ResultsRepresentation result = pipeline.RunClassification();
            Assert.IsInstanceOfType(result, typeof(ResultsRepresentation)); 
        }

        [TestMethod]
        public void TestResultsRepresentation()
        {
            ResultsRepresentation result = ResultsRepresentation.ParseCsv(ModelCsvFile);
            Assert.IsInstanceOfType(result, typeof(ResultsRepresentation));           
        }
    }
}
