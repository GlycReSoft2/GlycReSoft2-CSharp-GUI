using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycReSoft.CompositionHypothesis
{
    public class CompositionHypothesis
    {
        /// <summary>
        /// A list of objects describing masses and previousCompositions
        /// </summary>
        public List<IComposition> Compositions;
        /// <summary>
        /// A list of elements present in Compositions
        /// </summary>
        public List<String> ElementNames;
        /// <summary>
        /// A list of molecules present in Compositions
        /// </summary>
        public List<String> MoleculeNames;

        /// <summary>
        /// Create an empty CompositionHypothesis instance to fill
        /// </summary>
        public CompositionHypothesis()
        {
            Compositions = new List<IComposition>();
            ElementNames = new List<string>();
            MoleculeNames = new List<string>();
        }

        public override string ToString()
        {
            string repr = "";
            repr += JsonConvert.SerializeObject(this.ElementNames) + "\n";
            repr += JsonConvert.SerializeObject(this.MoleculeNames) + "\n";
            repr += "# of Compositions: " + Compositions.Count + "\n";
            repr += JsonConvert.SerializeObject(this.Compositions.GetRange(0,3)) + "\n";
            return repr;
        }

        /// <summary>
        /// A simplistic CSV Parser for extracting information from the header line.
        /// </summary>
        /// <param name="headerLine"></param>
        public void ParseHeader(String headerLine)
        {
            String[] columnNames = headerLine.Split(',');
            //Start from position 1 since column 0 is not relevant to the header.
            //This index will be used in multiple loops, so this for loop's init
            //is left empty.
            int index = 1;
            String headerText = null;
            //This loop will collect element names from the header line
            for (; index < columnNames.Length; index++)
            {
                headerText = columnNames[index];
                //If we have reached the end of the element names, exit this loop
                if (headerText == "Compositions")
                {
                    break;
                }
                ElementNames.Add(headerText);
            }

            index += 1; //Advance beyond GlycanCompositions to reach molecule names
            for (; index < columnNames.Length; index++)
            {
                headerText = columnNames[index];
                //If we have reached the end of the molecule names, exit this loop
                if (headerText == "Adduct/Replacement")
                {
                    break;
                }
                MoleculeNames.Add(headerText);
            }
            
        }


        /// <summary>
        /// Consume a StreamReader for a CompositionHypothesis CSV File
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        public void ParseCompositionHypothesisCsv<T>(StreamReader reader) where T : IComposition, new()
        {
            String line = reader.ReadLine();
            ParseHeader(line);
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                ParseComposition<T>(line);
            }
            reader.Close();
        }

        public void ParseCompositionHypothesisCsv<T>(String filePath) where T : IComposition, new()
        {
            StreamReader reader = new StreamReader(File.OpenRead(filePath));
            ParseCompositionHypothesisCsv<T>(reader);
        }

        public static CompositionHypothesis ParseCsv<T>(String filePath) where T : IComposition, new()
        {
            CompositionHypothesis hypothesis = new CompositionHypothesis();
            hypothesis.ParseCompositionHypothesisCsv<T>(filePath);
            return hypothesis;
        }

        /// <summary>
        /// Feed a line to the IComposition type and let it evaluate the line, and add the 
        /// populated IComposition object to the Compositions list. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compositionLine"></param>
        public void ParseComposition<T>(String compositionLine) where T : IComposition, new()
        {
            T obj = new T();
            obj.ParseLine(compositionLine, ElementNames, MoleculeNames);
            Compositions.Add(obj);
        }

        /// <summary>
        /// Writes the generic header line for a CompositionHypothesis CSV File. Includes headers 
        /// for both glycan and glycopeptide files since they are redundant formats
        /// </summary>
        /// <returns></returns>
        public string ComposeHeader()
        {
            string line = "Molecular Weight,";

            foreach (string elementName in ElementNames)
            {
                line += elementName + ',';
            }
            line += "Compositions,";
            foreach (string moleculeName in MoleculeNames)
            {
                line += moleculeName + ',';
            }
            line += "Adduct/Replacement,";
            line += "Adduct Amount,";
            line += "Peptide Sequence,";
            line += "Peptide Modification,";
            line += "Peptide Missed Cleavage Number,";
            line += "Number of Glycan Attachment to Peptide,";
            line += "Start AA,";
            line += "End AA";

            return line;
        }

        /// <summary>
        /// Write out the CompositionHypothesis object to CSV to the writer stream.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteCompositionHypothesisCsv(StreamWriter writer)
        {
            writer.WriteLine(ComposeHeader());
            foreach (IComposition composition in Compositions)
            {
                writer.WriteLine(composition.CsvSerializeLine(ElementNames, MoleculeNames));
            }
            writer.Close();
        }

        /// <summary>
        /// Write out the CompositionHypothesis object to CSV to the filePath file. 
        /// </summary>
        /// <param name="filePath"></param>
        public void WriteCompositionHypothesisCsv(String filePath)
        {
            StreamWriter writer = new StreamWriter(File.OpenWrite(filePath));
            WriteCompositionHypothesisCsv(writer);
        }

    }
}
