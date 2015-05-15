using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlycReSoft.Extensions;


namespace GlycReSoft.CompositionHypothesis
{
    public class GlycopeptideCompositionHypothesisBuilder
    {
        public CompositionHypothesis GlycanCompositions;
        public List<MSDigestPeptide> Peptides;
        public CompositionHypothesis GlycopeptideComposition;
        public Boolean KeepUnglycosylatedPeptides = false;

        public GlycopeptideCompositionHypothesisBuilder(CompositionHypothesis glycanCompositionHypothesis, List<MSDigestPeptide> peptides)
        {
            this.GlycanCompositions = glycanCompositionHypothesis;
            this.Peptides = peptides;
            this.GlycopeptideComposition = new CompositionHypothesis();
            GlycopeptideComposition.ElementNames = GlycanCompositions.ElementNames;
            GlycopeptideComposition.MoleculeNames = GlycanCompositions.MoleculeNames;
        }

        /// <summary>
        /// Generate all possible combinations of glycans and peptides within the constraints of number of glycosylations
        /// on a given peptide. Represent each combination as a GlycopeptideComposition.
        /// </summary>
        public void BuildCompositionHypothesis()
        {
            int peptideIter = 0;
            foreach (MSDigestPeptide peptide in Peptides)
            {
                peptideIter++;
                //Console.WriteLine("Working on Peptide {0}, {1}", peptideIter, peptide);
                //Creates the unglycosylated peptide composition row.
                GlycopeptideComposition rawPeptideComposition = new GlycopeptideComposition(peptide);
                int numGlycosylations = rawPeptideComposition.GlycosylationCount;                
                rawPeptideComposition.GlycosylationCount = 0;

                //If there were no glycosylations on this peptide to begin with, this step is done!
                if (numGlycosylations == 0)
                {
                    if (KeepUnglycosylatedPeptides)
                    {
                        GlycopeptideComposition.Compositions.Add(rawPeptideComposition);
                    }
                    continue;
                }                
                //This is a combinations with replacement problem, meaning that the number of choices with each iteration does not shrink and order does not matter.
                //Since convergent glycan compositions cannot be distinguished, we may be able to reduce the total number generated. 
                //Console.WriteLine("[{4}] This peptide has {0} (K) glycosylation sites, and there are {1} (N) glycans to choose from. Combinations = {2} new objects without convergent compositions. Current Memory Size: {3} ", numGlycosylations, GlycanCompositions.Compositions.Count, Numerics.Combinations(GlycanCompositions.Compositions.Count, numGlycosylations), GC.GetTotalMemory(false), peptideIter++);
                //Represent the iterative increase in glycosylation as a set of queues. Each possible glycoform
                //is generated incrementally from previously generated set of compositions. We only need the set of 
                //compositions we created from the last iteration to build the next iteration.
                //I use a Queue here because C#'s List API doesn't have a notion of get-and-remove, and removing
                Queue<GlycopeptideComposition> previousCompositions = new Queue<GlycopeptideComposition>();
                Queue<GlycopeptideComposition> nextCompositions = new Queue<GlycopeptideComposition>();                
                previousCompositions.Enqueue(rawPeptideComposition);
                for (int depthIter = 0; depthIter < numGlycosylations; depthIter++)
                {
                    Dictionary<string, bool> glycanCompositionCache = new Dictionary<string, bool>();
                    int lengthOfPrevious = previousCompositions.Count;
                    while (previousCompositions.Count > 0)
                    {
                        GlycopeptideComposition template = previousCompositions.Dequeue();
                        GlycopeptideComposition.Compositions.Add(template);
                        foreach (GlycanComposition glycan in this.GlycanCompositions.Compositions)
                        {
                            GlycopeptideComposition instance = new GlycopeptideComposition(template);
                            instance.AttachGlycan(glycan);
                            string glycanTuple = instance.GlycanCompositionTuple(GlycopeptideComposition.MoleculeNames);
                            if (glycanCompositionCache.ContainsKey(glycanTuple))
                            {
                                continue;
                            }
                            else
                            {
                                glycanCompositionCache.Add(glycanTuple, true);
                            }
                            nextCompositions.Enqueue(instance);
                        }
                    }
                    int lengthOfNext = nextCompositions.Count;
                    Console.WriteLine("Length of run: {0:N}", lengthOfNext);
                    previousCompositions = nextCompositions;
                    nextCompositions = new Queue<GlycopeptideComposition>();                    
                    //Console.WriteLine("[{5}]{0} * {1} = {2}. Generated {3}. Saved {6}%\nMemory Size: {4}", lengthOfPrevious, GlycanCompositions.Compositions.Count,                        lengthOfPrevious * GlycanCompositions.Compositions.Count, lengthOfNext, GC.GetTotalMemory(false), depthIter, (1 - (lengthOfNext / (double)(lengthOfPrevious * GlycanCompositions.Compositions.Count))) * 100);
                }
                //After the last pass through the loop, previousComposition still holds all of the most recently generated values,
                //so we need to add them to the total list.
                GlycopeptideComposition.Compositions.AddRange(previousCompositions);
            }
            Console.WriteLine("Build complete.");
        }
    }
}
