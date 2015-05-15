using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlycReSoft;
using System.IO;
using System.Xml;

namespace GlycReSoft.CompositionHypothesis
{

    /// <summary>
    /// Extract and represent relevant information from a ProteinProspector MS-Digest
    /// </summary>
    public class MSDigestReport
    {
        public MSDigestReportParameters Parameters;
        //public List<MSDigestDatabaseEntry> DatabaseEntries;
        public List<MSDigestPeptide> Peptides;
        public String[] ConstantModifications { 
            get{
                String[] results = Parameters.ConstantModifications.ConvertAll(x => x.Name).ToArray();
                return results;
            }
        }
        public String[] VariableModifications
        {
            get
            {
                String[] results = Parameters.VariableModifications.ConvertAll(x => x.Name).ToArray();
                return results;
            }
        }


        public static MSDigestReport Load(String filePath)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(File.ReadAllText(filePath));
            return new MSDigestReport(xml);
        }

        public MSDigestReport(XmlDocument data)
        {
            Parameters = new MSDigestReportParameters();
            Peptides = new List<MSDigestPeptide>();
            Parse(data);
        }

        public void Parse(XmlDocument data)
        {
            #region Parameter Initialization
            foreach (XmlNode mod in data.SelectNodes(".//parameters/const_mod"))
            {
                MSDigestModification constMod = MSDigestModification.FromURI(mod.InnerText);
                Parameters.ConstantModifications.Add(constMod);
            }
            foreach (XmlNode mod in data.SelectNodes(".//parameters/mod_AA"))
            {
                Parameters.VariableModifications.Add(MSDigestModification.FromURI(mod.InnerText));
            }
            Parameters.Enzyme = data.SelectSingleNode(".//parameters/enzyme").InnerText;
            Parameters.MissedCleavages = Convert.ToInt32(data.SelectSingleNode(".//parameters/missed_cleavages").InnerText);
            Parameters.UserProteinSequence = data.SelectSingleNode(".//parameters/user_protein_sequence").InnerText;
            #endregion

            //No Database extractions for now

            #region Peptide Extraction
            foreach (XmlNode peptideNode in data.SelectNodes(".//peptide"))
            {
                //Console.WriteLine("------------------------------");
                MSDigestPeptide peptideObj = new MSDigestPeptide();
                peptideObj.PeptideIndex = Convert.ToInt32(peptideNode.SelectSingleNode(".//protein_index").InnerText);
                peptideObj.Charge = Convert.ToInt32(peptideNode.SelectSingleNode(".//charge").InnerText);
                peptideObj.Mass = Convert.ToDouble(peptideNode.SelectSingleNode(".//mi_m_over_z").InnerText);
                String modStr = "";
                foreach (XmlNode mod in peptideNode.SelectNodes(".//modification"))
                {
                    //Console.WriteLine(mod.InnerText);
                    modStr += mod.InnerText + " ";
                }
                peptideObj.Modifications = modStr;
                peptideObj.StartAA = Convert.ToInt32(peptideNode.SelectSingleNode(".//start_aa").InnerText);
                peptideObj.EndAA = Convert.ToInt32(peptideNode.SelectSingleNode(".//end_aa").InnerText);
                peptideObj.NextAA = (peptideNode.SelectSingleNode(".//next_aa").InnerText);
                peptideObj.PreviousAA = (peptideNode.SelectSingleNode(".//previous_aa").InnerText);
                peptideObj.MissedCleavages = Convert.ToInt32(peptideNode.SelectSingleNode(".//missed_cleavages").InnerText);
                peptideObj.Sequence = (peptideNode.SelectSingleNode(".//database_sequence").InnerText);
                peptideObj.ProteinID = Parameters.UserProteinSequence;
                peptideObj.NumGlycosylations = PeptideUtilities.CountNGlycanSequons(peptideObj);
                //Console.WriteLine(peptideObj);

                Peptides.Add(peptideObj);
            }
            #endregion


        }

        
    }

    /// <summary>
    /// Currently unused
    /// </summary>
    public class MSDigestDatabaseEntry
    {

    }

    /// <summary>
    /// Store the parameters of the MS-Digest run for downstream analysis
    /// </summary>
    public class MSDigestReportParameters
    {
        /// <summary>
        /// The digestion enzyme
        /// </summary>
        public String Enzyme;
        /// <summary>
        /// Modifications that are always present
        /// </summary>
        public List<MSDigestModification> ConstantModifications;
        /// <summary>
        /// Modifications which may or may not be present
        /// </summary>
        public List<MSDigestModification> VariableModifications;
        /// <summary>
        /// Total number of missed cleavages allowed
        /// </summary>
        public int MissedCleavages;
        /// <summary>
        /// The protein sequence that was digested
        /// </summary>
        public String UserProteinSequence;

        public MSDigestReportParameters()
        {
            ConstantModifications = new List<MSDigestModification>();
            VariableModifications = new List<MSDigestModification>();
            Enzyme = "";
            MissedCleavages = 0;
            UserProteinSequence = "";
        }

    }

    /// <summary>
    /// Stores the Protein Prospector name for a modification
    /// </summary>
    public class MSDigestModification
    {
        /// <summary>
        /// Modification name as seen in Protein Prospector's MS-Digest form
        /// </summary>
        public String Name;

        public MSDigestModification(String name)
        {
            Name = name;
        }

        /// <summary>
        /// Modification names are URI encoded. This function decodes the name 
        /// and instantiates an instance of MSDigestModification on the spot
        /// </summary>
        /// <param name="uriEncodedName"></param>
        /// <returns></returns>
        public static MSDigestModification FromURI(String uriEncodedName)
        {
            var mod = new MSDigestModification(Uri.UnescapeDataString(uriEncodedName));
            return mod;
        }
    }

    /// <summary>
    /// An extension of the composition.PPMSD for use within this namespace
    /// that should be fully interoperable with PPMSD
    /// </summary>
    public class MSDigestPeptide
    {
        public Boolean Selected = false;
        public Int32 PeptideIndex = 0;
        public Double Mass = 0;
        public Int32 Charge = 0;
        public String Modifications = "";
        public Int32 StartAA = 0;
        public Int32 EndAA = 0;
        public Int32 MissedCleavages = 0;
        public String PreviousAA = "";
        public String Sequence = "----";
        public String NextAA = "";
        public Int32 NumGlycosylations = 0;
        public String ProteinID = "?";

        public MSDigestPeptide()
        {

        }

        public override string ToString()
        {
            return string.Format("<{0} ({1})>", this.Sequence, this.Modifications);
        }
    }


}
