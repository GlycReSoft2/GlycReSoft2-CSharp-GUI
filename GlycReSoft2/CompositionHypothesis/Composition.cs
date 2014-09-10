using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GlycReSoft.CompositionHypothesis
{
    public interface IComposition
    {
        int ParseLine(String lineData, List<String> ElementNames, List<String>MoleculeNames, int start = 1);
        String CsvSerializeLine(List<String> ElementNames, List<String> MoleculeNames);        
    }

    public class GlycanComposition : IComposition
    {
        public Dictionary<string, int> ElementalComposition;
        public Dictionary<string, int> MolecularComposition;        
        public double MassWeight;
        public int AdductAmount;
        public string AdductReplaces;
        public string Annotation;

        /// <summary>
        /// Create an empty GlycanCompositions instance
        /// </summary>
        public GlycanComposition()
        {
            ElementalComposition = new Dictionary<string, int>();
            MolecularComposition = new Dictionary<string, int>();
            MassWeight = 0;
            AdductAmount = 0;
            AdductReplaces = "/0";
            Annotation = null;
        }

        public GlycanComposition(GlycanComposition other)
        {
            //Components are not reference types, and so they will be copied by value
            this.ElementalComposition = new Dictionary<string, int>(other.ElementalComposition);
            this.MolecularComposition = new Dictionary<string, int>(other.MolecularComposition);

            this.MassWeight = other.MassWeight;
            this.AdductAmount = other.AdductAmount;
            this.AdductReplaces = other.AdductReplaces;
            this.Annotation = other.Annotation;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Parse a line from the GlycanCompositions Hypothesis CSV to populate this object's fields
        /// </summary>
        /// <param name="compositionLine"></param>
        /// <param name="elementNames"></param>
        /// <param name="moleculeNames"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public int ParseLine(string compositionLine, List<string> elementNames, List<string> moleculeNames, int start = 1)
        {
            int index = start;
            String columnData = null;
            String[] lineData = compositionLine.Split(',');            
            //Extract MassWeight
            MassWeight = Convert.ToDouble(lineData[0]);
            for (; index < elementNames.Count + 1; index++)
            {
                columnData = lineData[index];
                //The index into the elementNames list is off by one since the 
                //Elements start at column 1 instead of 0
                this.ElementalComposition.Add(elementNames[index - 1], Convert.ToInt32(columnData));
            }
            index += 1;
            //Track offset made through the moleculeNames list
            int offset = 0;
            //Increment both counters each pass through the loop
            for (; offset < moleculeNames.Count; offset++, index++)
            {
                columnData = lineData[index];
                this.MolecularComposition.Add(moleculeNames[offset], Convert.ToInt32(columnData));
            }

            //Collect the one-off tailing columns
            this.AdductReplaces = lineData[index++];
            this.AdductAmount = Convert.ToInt32(lineData[index++]);

            return index;
        }

        public string GlycanCompositionTuple(List<string> moleculeNames)
        {
            string molecularCompositionTuple = "";
            foreach (string moleculeName in moleculeNames)
            {
                molecularCompositionTuple += this.MolecularComposition[moleculeName] + ";";
            }
            return molecularCompositionTuple;
        }

        public string CsvSerializeLine(List<string> elementNames, List<string> moleculeNames)
        {
            string repr = "";
            repr += Convert.ToString(MassWeight) + ',';
            foreach (string elementName in elementNames)
            {
                repr += ElementalComposition[elementName] + ',';
            }
            string molecularCompositionTuple = "[";
            foreach (string moleculeName in moleculeNames)
            {
                molecularCompositionTuple += this.MolecularComposition[moleculeName] + ";";
            }
            repr += molecularCompositionTuple.TrimEnd(';') + "],";
            foreach (string moleculeName in moleculeNames)
            {
                repr += MolecularComposition[moleculeName] + ',';
            }
            repr += AdductReplaces + ',';
            repr += AdductAmount;
            return repr;
        }
    }

    public class GlycopeptideComposition : GlycanComposition, IComposition
    {
        public string PeptideSequence;
        public string PeptideModification;
        public int MissedCleavages;
        public int GlycosylationCount;
        public int StartAA;
        public int EndAA;

        public GlycopeptideComposition()
            : base()
        {
            MissedCleavages = 0;
            GlycosylationCount = 0;
            StartAA = -1;
            EndAA = -1;
            PeptideSequence = null;
            PeptideModification = null;
        }

        public GlycopeptideComposition(GlycopeptideComposition other)
            : base(other as GlycanComposition)
        {
            this.MissedCleavages = other.MissedCleavages;
            this.GlycosylationCount = other.GlycosylationCount;
            this.StartAA = other.StartAA;
            this.EndAA = other.EndAA;
            this.PeptideSequence = other.PeptideSequence;
            this.PeptideModification = other.PeptideModification;
        }

        public GlycopeptideComposition(MSDigestPeptide peptide)
            : base()
        {
            this.PeptideSequence = peptide.Sequence;
            this.PeptideModification = peptide.Modifications;
            this.StartAA = peptide.StartAA;
            this.EndAA = peptide.EndAA;
            this.GlycosylationCount = peptide.numGly;
            this.MassWeight += peptide.Mass;
        }

        public GlycopeptideComposition(GlycanComposition glycanComposition, MSDigestPeptide peptide) : base(glycanComposition)
        {
            this.PeptideSequence = peptide.Sequence;
            this.PeptideModification = peptide.Modifications;
            this.StartAA = peptide.StartAA;
            this.EndAA = peptide.EndAA;
            this.GlycosylationCount = peptide.numGly;
            this.MassWeight += peptide.Mass;
        }

        public void AttachGlycan(GlycanComposition glycanComposition)
        {
            this.MassWeight += glycanComposition.MassWeight;
            this.GlycosylationCount++;
            foreach (KeyValuePair<string, int> kvp in glycanComposition.ElementalComposition)
            {
                if (!ElementalComposition.ContainsKey(kvp.Key))
                {
                    ElementalComposition[kvp.Key] = kvp.Value;
                }
                else
                {
                    ElementalComposition[kvp.Key] += kvp.Value; 
                }
            }
            foreach (KeyValuePair<string, int> kvp in glycanComposition.MolecularComposition)
            {
                if (!MolecularComposition.ContainsKey(kvp.Key))
                {
                    MolecularComposition[kvp.Key] = kvp.Value;
                }
                else
                {
                    MolecularComposition[kvp.Key] += kvp.Value;
                }
            }
            //Adduct information is a singleton despite there being multiple glycans per peptide. 
            //Is this actually correct chemically?
            this.AdductReplaces = glycanComposition.AdductReplaces;
            this.AdductAmount = glycanComposition.AdductAmount;
                

        }

        public new int ParseLine(string compositionLine, List<string> elementNames, List<string> moleculeNames, int start = 1)
        {
            int index = base.ParseLine(compositionLine, elementNames, moleculeNames, start);
            String[] lineData = compositionLine.Split(',');

            this.PeptideSequence = lineData[index++];
            this.PeptideModification = lineData[index++];
            this.MissedCleavages = Convert.ToInt32(lineData[index++]);
            this.GlycosylationCount = Convert.ToInt32(lineData[index++]);
            this.StartAA = Convert.ToInt32(lineData[index++]);
            this.EndAA = Convert.ToInt32(lineData[index++]);

            return index;
        }

        public new string CsvSerializeLine(List<string> elementNames, List<string> moleculeNames)
        {
            string repr = base.CsvSerializeLine(elementNames, moleculeNames);

            repr += ',' + PeptideSequence + ',';
            repr += PeptideModification + ',';
            repr += MissedCleavages + ',';
            repr += GlycosylationCount + ',';
            repr += StartAA + ',';
            repr += EndAA;

            return repr;
        }

    }
}
