using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycReSoft.CompositionHypothesis
{
    public static class TestMain
    {
        static void Main(string[] args)
        {
            CompositionHypothesis glycanHypothesis = new CompositionHypothesis();
            String testGlycanCompositionFile = @"C:\Users\jaklein\Dropbox\GlycomicsSandbox\Test Data\Glycresoft glycan hypothesis.csv";

            glycanHypothesis.ParseCompositionHypothesisCsv<GlycanComposition>(testGlycanCompositionFile);
            Console.WriteLine(glycanHypothesis);

            String testGlycopeptideCompositionFile = @"C:\Users\jaklein\Dropbox\GlycomicsSandbox\Test Data\HA-USSR-Glycopeptide hypothesis.csv";
            CompositionHypothesis glycopeptideHypothesis = new CompositionHypothesis();
            glycopeptideHypothesis.ParseCompositionHypothesisCsv<GlycopeptideComposition>(testGlycopeptideCompositionFile);
            Console.WriteLine(glycopeptideHypothesis);

            String testMSDigestFile = @"C:\Users\jaklein\Dropbox\GlycomicsSandbox\Test Data\KK-USSR-digest-Prospector output.xml";
            MSDigestReport msdigest = MSDigestReport.Load(testMSDigestFile);

            foreach (MSDigestPeptide pept in msdigest.Peptides)
            {
                pept.numGly = 7;
            }
            Console.WriteLine(msdigest.Peptides.Count);
            Random rng = new Random(1);

            //Dictionary<int, bool> keepers = new Dictionary<int, bool>();
            //int nKeep = 100;

            //CompositionHypothesis glycanHypothesis2 = CompositionHypothesis.ParseCsv<GlycanComposition>(testGlycanCompositionFile);
            //glycanHypothesis2.Compositions = glycanHypothesis.Compositions.Take(nKeep).ToList();

            try
            {
                GlycopeptideCompositionHypothesisBuilder builder = new GlycopeptideCompositionHypothesisBuilder(glycanHypothesis, msdigest.Peptides);
                builder.BuildCompositionHypothesis();
                Console.WriteLine(builder.GlycopeptideComposition);
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine("\n\n\n!!!!!!!!!!!!!!!!Combinatorics exceeded memory size!", ex.Message, "\n\n\n!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
