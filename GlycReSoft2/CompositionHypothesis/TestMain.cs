using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycReSoft.CompositionHypothesis
{
    public static class TestMain
    {
        public static void TestGlycanCompositionHypothesis()
        {
            String testGlycanCompositionFile = @"TestData\Glycresoft glycan hypothesis.csv";
            CompositionHypothesis glycanHypothesis = new CompositionHypothesis();
            glycanHypothesis.ParseCompositionHypothesisCsv<GlycanComposition>(testGlycanCompositionFile);
            Console.WriteLine(glycanHypothesis);
        }
        public static void TestGlycoPeptideHypothesis()
        {
            String testGlycopeptideCompositionFile = @"TestData\HA-USSR-Glycopeptide hypothesis.csv";
            CompositionHypothesis glycopeptideHypothesis = new CompositionHypothesis();
            glycopeptideHypothesis.ParseCompositionHypothesisCsv<GlycopeptideComposition>(testGlycopeptideCompositionFile);
            Console.WriteLine(glycopeptideHypothesis);
        }
        public static void TestBuildGlycoPeptideHypothesis()
        {
            String testGlycanCompositionFile = @"TestData\Glycresoft glycan hypothesis.csv";
            CompositionHypothesis glycanHypothesis = new CompositionHypothesis();
            glycanHypothesis.ParseCompositionHypothesisCsv<GlycanComposition>(testGlycanCompositionFile);
            String testMSDigestFile = @"TestData\KK-USSR-digest-Prospector output.xml";
            MSDigestReport msdigest = MSDigestReport.Load(testMSDigestFile);
            int counter = 0;
            foreach (MSDigestPeptide pep in msdigest.Peptides)
            {
                counter += pep.NumGlycosylations;
                if (counter > 30)
                {
                    pep.NumGlycosylations = 0;
                }
            }
            try
            {
                GlycopeptideCompositionHypothesisBuilder builder = new GlycopeptideCompositionHypothesisBuilder(glycanHypothesis, msdigest.Peptides);
                Console.WriteLine("Building Hypothesis");
                builder.BuildCompositionHypothesis();
                Console.WriteLine(builder.GlycopeptideComposition);
                builder.GlycopeptideComposition.WriteCompositionHypothesisCsv("TestData/TestOutputHypothesis.csv");
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine("\n\n\n!!!!!!!!!!!!!!!!Combinatorics exceeded memory size!", ex.Message, "\n\n\n!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
        }
        static void Main(string[] args)
        {

            TestGlycanCompositionHypothesis();
            TestGlycoPeptideHypothesis();
            TestBuildGlycoPeptideHypothesis();
            
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
