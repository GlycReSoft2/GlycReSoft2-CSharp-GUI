using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace GlycReSoft
{
    public class GetDeconData
    {
        List<DeconRow> DeconDATA = new List<DeconRow>();
        public List<DeconRow> getdata (String DeconData)
        {
            parameters parad = new parameters();
            parameters.para paradata = parad.getParameters();

            //try
           // {
                FileStream fileinput = new FileStream(DeconData, FileMode.Open, FileAccess.Read);
                StreamReader readdata = new StreamReader(fileinput);

                //The first line in the file contains the column names, we don't need it.
                readdata.ReadLine();
                while (readdata.Peek() >= 0)
                {
                    DeconRow Row = new DeconRow();
                    String Line = readdata.ReadLine();
                    String[] column = Line.Split(',');
                    Row.scan_num = Convert.ToInt32(column[0]);
                    Row.charge = Convert.ToInt32(column[1]);
                    Row.abundance = Convert.ToInt32(column[2]);
                    Row.mz = Convert.ToDouble(column[3]);
                    Row.fit = Convert.ToDouble(column[4]);
                    Row.average_mw = Convert.ToDouble(column[5]);
                    Row.monoisotopic_mw = Convert.ToDouble(column[6]);
                    Row.mostabundant_mw = Convert.ToDouble(column[7]);
                    Row.fwhm = Convert.ToDouble(column[8]);
                    Row.signal_noise = Convert.ToDouble(column[9]);
                    Row.mono_abundance = Convert.ToInt32(column[10]);
                    Double mp2a = Convert.ToDouble(column[11]);
                    Row.mono_plus2_abundance = Convert.ToInt32(mp2a);
                    //Flag maybe empty, so, special treatment.
                    if (column[12] == "")
                    { Row.flag = 0; }                    
                    else
                    { Row.flag = Convert.ToInt32(column[12]); }
                    if (Convert.ToInt32(column.Count()) == 14)
                        Row.interference_sore = Convert.ToDouble(column[13]);
                    else
                        Row.interference_sore = 0;
                    //Check if the data are within the boundaries of the Parameters
                    if (Row.abundance >= paradata.dataNoiseTheshold)
                    {
                        if (Row.monoisotopic_mw <= paradata.molecularWeightUpperBound)
                        {
                            if (Row.monoisotopic_mw >= paradata.molecularWeightLowerBound)
                            {
                                DeconDATA.Add(Row);
                            }
                        }
                    }
                }
                fileinput.Close();
           // }
          //  catch(Exception ex)
          //  {
          //      MessageBox.Show("Error: Could not read DeconTools Data file from disk. Original error: " + ex.Message);
         //   }
            return DeconDATA;
        }
        //checkFile class: Checks that each column in uploaded file conforms to proper data type
        public Boolean checkFile(String[] path)
        {
            Boolean FileWorks = true;
            try
            {
                foreach (string file in path)
                {
                    FileStream fileinput = new FileStream(file, FileMode.Open, FileAccess.Read);
                    StreamReader readdata = new StreamReader(fileinput);

                    readdata.ReadLine();
                    while (readdata.Peek() >= 0)
                    {
                        String Line = readdata.ReadLine();
                        String[] col = Line.Split(',');

                        //check each column in each row for expected data type


                        int tValue;
                        string tString = col[0];
                        double tdouble;
                        bool goodfile = int.TryParse(tString, out tValue);
                        if (goodfile)
                        {
                            tString = col[1];
                            goodfile = int.TryParse(tString, out tValue);
                            if (goodfile)
                            {
                                tString = col[2];
                                goodfile = int.TryParse(tString, out tValue);
                                if (goodfile)
                                {
                                    tString = col[3];
                                    goodfile = double.TryParse(tString, out tdouble);
                                    if (goodfile)
                                    {
                                        tString = col[4];
                                        goodfile = double.TryParse(tString, out tdouble);
                                        if (goodfile)
                                        {
                                            tString = col[5];
                                            goodfile = double.TryParse(tString, out tdouble);
                                            if (goodfile)
                                            {
                                                tString = col[6];
                                                goodfile = double.TryParse(tString, out tdouble);
                                                if (goodfile)
                                                {
                                                    tString = col[7];
                                                    goodfile = double.TryParse(tString, out tdouble);
                                                    if (goodfile)
                                                    {
                                                        tString = col[8];
                                                        goodfile = double.TryParse(tString, out tdouble);
                                                        if (goodfile)
                                                        {
                                                            tString = col[9];
                                                            goodfile = double.TryParse(tString, out tdouble);
                                                            if (goodfile)
                                                            {
                                                                tString = col[10];
                                                                goodfile = int.TryParse(tString, out tValue);
                                                                if (goodfile)
                                                                {
                                                                    tString = col[11];
                                                                    goodfile = double.TryParse(tString, out tdouble);

                                                                    if (goodfile)
                                                                    {
                                                                        tString = col[12];
                                                                        goodfile = int.TryParse(tString, out tValue);
                                                                        if (goodfile || string.IsNullOrEmpty(tString))
                                                                        {
                                                                            int colnumbers = col.Length;
                                                                            if (colnumbers >= 14)
                                                                            {
                                                                                tString = col[13];
                                                                                goodfile = double.TryParse(tString, out tdouble);
                                                                                if (goodfile)
                                                                                {
                                                                                    //If this point is reached, FileWorks = True and file will load successfully
                                                                                }
                                                                                else
                                                                                {
                                                                                    MessageBox.Show("Error: In column 14 'Interference_Score' type 'Double' is expected.");
                                                                                    FileWorks = false;
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            MessageBox.Show("Error: In column 13 'Flag' types 'Int' or 'Null' are expected.");
                                                                            FileWorks = false;
                                                                            break;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        MessageBox.Show("Error: In column 12 'Mono_Plus2_Abundance' type 'Integer' is expected.");
                                                                        FileWorks = false;
                                                                        break;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    MessageBox.Show("Error: In column 11 'Mono_Abundance' type 'Integer' is expected.");
                                                                    break;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("Error: In column 10 'Signal_Noise' types 'Integer' or 'Double' are expected.");
                                                                FileWorks = false;
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Error: In column 9 'FWHM' type 'Double' is expected.");
                                                            FileWorks = false;
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Error: In column 8 'MostAbundant_MW' type 'Double' is expected.");
                                                        FileWorks = false;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Error: In column 7 'Monoisotopic_MW' type 'Double' is expected.");
                                                    FileWorks = false;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Error: In column 6 'Average_MW' type 'Double' is expected.");
                                                FileWorks = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Error: In column 5 'Fit' type 'Double' is expected.");
                                            FileWorks = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Error: In column 4 'MZ' type 'Double' is expected.");
                                        FileWorks = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Error: In column 3 'Abundance' type 'Integer' is expected.");
                                    FileWorks = false;
                                    break;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Error: In column 2 'Charge' type 'Integer' is expected.");
                                FileWorks = false;
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error: In column 1 'Scan Num' type 'Integer' is expected.");
                            FileWorks = false;
                            break;
                        }
                    }
                    fileinput.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                FileWorks = false;
            }

            return FileWorks;
        }
    }

    // Each "DeconRow" variable stores one row from an isos file, so that a list of DeconRows can store a whole isos file. 
    public class DeconRow
    {
        public Int32 scan_num { get; set; }
        public Int32 charge { get; set; }
        public Int32 abundance { get; set; }
        public Double mz { get; set; }
        public Double fit { get; set; }
        public Double average_mw { get; set; }
        public Double monoisotopic_mw { get; set; }
        public Double mostabundant_mw { get; set; }
        public Double fwhm { get; set; }
        public Double signal_noise { get; set; }
        public Int32 mono_abundance { get; set; }
        public Int32 mono_plus2_abundance { get; set; }
        public Int32 flag { get; set; }
        public Double interference_sore { get; set; }
    }
}