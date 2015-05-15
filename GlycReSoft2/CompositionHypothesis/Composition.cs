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
        public static Double WaterMass = 18.0105647;
        public Dictionary<string, int> ElementalComposition;
        public Dictionary<string, int> MolecularComposition;        
        public double MassWeight;
        public int AdductAmount;
        public string AdductReplaces;
        public string Annotation;
        public double AdductMass
        {
            get
            {
                return Convert.ToDouble(AdductReplaces.Split('/')[1]) * AdductAmount;
            }
        }
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
            foreach (string ele in elementNames)
            {
                Console.WriteLine(ele);
            }
            for (; index < elementNames.Count + 1; index++)
            {
                columnData = lineData[index];
                //The index into the elementNames list is off by one since the 
                //Elements start at column 1 instead of 0
                Console.WriteLine(index);
                Console.WriteLine(elementNames[index - 1]);
                Console.WriteLine(columnData);
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
                if (this.MolecularComposition.ContainsKey(moleculeName))
                {
                    molecularCompositionTuple += this.MolecularComposition[moleculeName] + ";";
                }
                else
                {
                    molecularCompositionTuple += "0;";
                }
                
            }
            molecularCompositionTuple = molecularCompositionTuple.Remove(molecularCompositionTuple.Length - 1);
            return "[" + molecularCompositionTuple + "]";
        }

        public string CsvSerializeLine(List<string> elementNames, List<string> moleculeNames)
        {
            string repr = "";
            repr += Convert.ToString(MassWeight) + ',';
            foreach (string elementName in elementNames)
            {
                if (ElementalComposition.ContainsKey(elementName))
                {
                    repr += Convert.ToString(ElementalComposition[elementName]) + ",";
                }
                else
                {
                    repr += "0,";
                }
            }
            repr += GlycanCompositionTuple(moleculeNames) + ",";
            foreach (string moleculeName in moleculeNames)
            {
                if (MolecularComposition.ContainsKey(moleculeName))
                {
                    repr += this.MolecularComposition[moleculeName] + ",";
                }
                else
                {
                    repr += "0,";
                }
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
        public string ProteinID;

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public GlycopeptideComposition()
            : base()
        {
            MissedCleavages = 0;
            GlycosylationCount = 0;
            StartAA = -1;
            EndAA = -1;
            PeptideSequence = null;
            PeptideModification = null;
            ProteinID = null;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public GlycopeptideComposition(GlycopeptideComposition other)
            : base(other as GlycanComposition)
        {
            this.MissedCleavages = other.MissedCleavages;
            this.GlycosylationCount = other.GlycosylationCount;
            this.StartAA = other.StartAA;
            this.EndAA = other.EndAA;
            this.PeptideSequence = other.PeptideSequence;
            this.PeptideModification = other.PeptideModification;
            this.ProteinID = other.ProteinID;
        }

        /// <summary>
        /// Peptide-Only Constructor
        /// </summary>
        /// <param name="peptide"></param>
        public GlycopeptideComposition(MSDigestPeptide peptide)
            : base()
        {
            this.PeptideSequence = peptide.Sequence;
            this.PeptideModification = peptide.Modifications;
            this.StartAA = peptide.StartAA;
            this.EndAA = peptide.EndAA;
            this.GlycosylationCount = peptide.NumGlycosylations;
            this.MassWeight += peptide.Mass;
            this.ProteinID = peptide.ProteinID;
        }

        /// <summary>
        /// Combination Cosntructor
        /// </summary>
        /// <param name="glycanComposition"></param>
        /// <param name="peptide"></param>
        public GlycopeptideComposition(GlycanComposition glycanComposition, MSDigestPeptide peptide) : base(glycanComposition)
        {
            this.PeptideSequence = peptide.Sequence;
            this.PeptideModification = peptide.Modifications;
            this.StartAA = peptide.StartAA;
            this.EndAA = peptide.EndAA;
            this.GlycosylationCount = peptide.NumGlycosylations;
            this.MassWeight += peptide.Mass - WaterMass - glycanComposition.AdductMass;
            MolecularComposition["Water"] -= 1;
            if (ElementalComposition.ContainsKey("O"))
            {
                ElementalComposition["O"] -= 1;
                ElementalComposition["H"] -= 2;
            }
            this.ProteinID = peptide.ProteinID;

            AdductAmount = 0;
            AdductReplaces = "";

        }


        /// <summary>
        /// Incorporate the components of a GlycanComposition object into
        /// this glycopeptide.
        /// </summary>
        /// <param name="glycanComposition"></param>
        public void AttachGlycan(GlycanComposition glycanComposition)
        {
            this.MassWeight += glycanComposition.MassWeight - WaterMass - glycanComposition.AdductMass;
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
            if (MolecularComposition.ContainsKey("Water"))
            {
                MolecularComposition["Water"] -= 1;
            }
            else
            {
                Console.WriteLine(JsonConvert.SerializeObject(this));
                throw new Exception("No water found to remove!");
            }
            if (ElementalComposition.ContainsKey("O"))
            {
                ElementalComposition["O"] -= 1;
                ElementalComposition["H"] -= 2;
            }
            //Adduct information is a singleton despite there being multiple glycans per peptide. 
            //Is this actually correct chemically?
            this.AdductReplaces = glycanComposition.AdductReplaces;
            this.AdductAmount = glycanComposition.AdductAmount;
                

        }

        /// <summary>
        /// Convert a line of a CSV file into a GlycopeptideComposition object given element and molecule labels.
        /// Parsing starts from column @start
        /// </summary>
        /// <param name="compositionLine"></param>
        /// <param name="elementNames"></param>
        /// <param name="moleculeNames"></param>
        /// <param name="start"></param>
        /// <returns></returns>
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
            if (lineData.Length > index)
            {
                this.ProteinID = lineData[index++];
            }
            else
            {
                this.ProteinID = "?";
            }
            

            return index;
        }

        /// <summary>
        /// Calls GlycanComposition's CsvSerializeLine function and then adds peptide-related features to the line
        /// </summary>
        /// <param name="elementNames"></param>
        /// <param name="moleculeNames"></param>
        /// <returns></returns>
        public new string CsvSerializeLine(List<string> elementNames, List<string> moleculeNames)
        {
            string repr = base.CsvSerializeLine(elementNames, moleculeNames);

            repr += "," + PeptideSequence + ",";
            repr += PeptideModification + ",";
            repr += MissedCleavages + ",";
            repr += GlycosylationCount + ",";
            repr += StartAA + ",";
            repr += EndAA + ",";
            repr += ProteinID;
            return repr;
        }

    }
}
