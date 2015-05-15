using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace GlycReSoft
{
    class GroupingResults
    {
        public static Tuple<List<String>, List<String>> GetElementsAndMolecules(List<ResultsGroup> group)
        {
            List<String> Elements = new List<String>();
            List<String> Molecules = new List<String>();
            int i = 0;
            while (Elements.Count() < 1)
            {
                foreach (String elementName in group[i].PredictedComposition.ElementNames)
                {
                    Elements.Add(elementName);
                    Console.WriteLine(elementName);
                }
                foreach (String moleculeName in group[i].PredictedComposition.MoleculeNames)
                {
                    Molecules.Add(moleculeName);
                    Console.WriteLine(moleculeName);
                }
                i++;
            }
            return new Tuple<List<String>, List<String>>(Elements, Molecules);
        }

        //This is used to read a ResultFile
        public List<ResultsGroup> ReadResultsFromFile(String path)
        {
            //This code looks int, but its just repetitive code. Look for ext and you will understand.
            List<ResultsGroup> Ans = new List<ResultsGroup>();
            List<String> molnames = new List<String>();
            FileStream FS = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader read = new StreamReader(FS);
            String ext = Path.GetExtension(path).Replace(".", "");


            if (ext == "csv")
            {
                String header = read.ReadLine();
                String[] headers = header.Split(',');
                List<string> elementIDs = new List<string>();
                //This is another older form of data
                if (headers[5] != "Hypothesis MW")
                {
                    Boolean moreCompounds = true;
                    int i = 17;
                    while (moreCompounds)
                    {
                        if (headers[i] != "Hypothesis MW")
                        {
                            elementIDs.Add(headers[i]);
                            i++;
                        }
                        else
                        {
                            moreCompounds = false;
                            i++;
                        }
                    }
                    moreCompounds = true;
                    while (moreCompounds)
                    {
                        if (headers[i] != "Adduct/Replacement")
                        {
                            molnames.Add(headers[i]);
                            i++;
                        }
                        else
                            moreCompounds = false;
                    }
                    bool firstRow = true;
                    while (read.Peek() >= 0)
                    {
                        //Read data
                        String Line = read.ReadLine();
                        String[] Lines = Line.Split(',');
                        //initialize new gR object
                        ResultsGroup gR = new ResultsGroup();
                        DeconRow dR = new DeconRow();
                        CompositionHypothesisEntry cH = new CompositionHypothesisEntry();
                        gR.DeconRow = dR;
                        gR.PredictedComposition = cH;

                        //Input data
                        if (!String.IsNullOrEmpty(Lines[0]))
                        {
                            if (firstRow)
                            {
                                gR.PredictedComposition.ElementNames = elementIDs;
                                gR.PredictedComposition.MoleculeNames = molnames;
                                firstRow = false;
                            }
                            
                            gR.Score = Convert.ToDouble(Lines[0]);
                            gR.DeconRow.MonoisotopicMassWeight = Convert.ToDouble(Lines[1]);
                            gR.PredictedComposition.CompoundComposition = Lines[2];
                            if (String.IsNullOrEmpty(Lines[2]) || Lines[2] == "0")
                                gR.Match = false;
                            else
                                gR.Match = true;
                            gR.PredictedComposition.PepSequence = Lines[3];
                            gR.NumModiStates = Convert.ToDouble(Lines[5]);
                            gR.NumChargeStates = Convert.ToInt32(Lines[6]);
                            gR.NumOfScan = Convert.ToDouble(Lines[7]);
                            gR.ScanDensity = Convert.ToDouble(Lines[8]);
                            gR.ExpectedA = Convert.ToDouble(Lines[9]);
                            gR.AvgAA2List = new List<double>();
                            gR.AvgAA2List.Add(Convert.ToDouble(Lines[10]));
                            gR.TotalVolume = Convert.ToDouble(Lines[11]);
                            gR.AvgSigNoise = Convert.ToDouble(Lines[12]);
                            gR.CentroidScan = Convert.ToDouble(Lines[13]);
                            gR.DeconRow.ScanNum = Convert.ToInt32(Lines[14]);
                            gR.MaxScanNum = Convert.ToInt32(Lines[15]);
                            gR.MinScanNum = Convert.ToInt32(Lines[16]);
                            gR.PredictedComposition.eqCount = new Dictionary<string, int>();
                            int sh = 17;
                            for (int ele = 0; ele < elementIDs.Count(); ele++ )
                            {
                                gR.PredictedComposition.ElementAmount.Add(Convert.ToInt32(Lines[sh]));
                                sh++;
                            }
                            gR.PredictedComposition.MassWeight = Convert.ToDouble(Lines[sh]);
                            sh++;
                            List<int> eqCoun = new List<int>();
                            for (int j = 0; j < molnames.Count(); j++)
                            {
                                eqCoun.Add(Convert.ToInt32(Lines[sh + j]));
                            }
                            gR.PredictedComposition.eqCounts = eqCoun;
                            gR.PredictedComposition.AddRep = Lines[sh + molnames.Count()];
                            gR.PredictedComposition.AdductNum = Convert.ToInt32(Lines[sh + molnames.Count() + 1]);
                            gR.PredictedComposition.PepModification = Lines[sh + molnames.Count() + 2];
                            gR.PredictedComposition.MissedCleavages = Convert.ToInt32(Lines[sh + molnames.Count() + 3]);
                            gR.PredictedComposition.NumGlycosylations = Convert.ToInt32(Lines[sh + molnames.Count() + 4]);
                            gR.PredictedComposition.StartAA = Convert.ToInt32(Lines[sh + molnames.Count() + 5]);
                            gR.PredictedComposition.EndAA = Convert.ToInt32(Lines[sh + molnames.Count() + 6]);
                            if (Lines.Count() > sh + molnames.Count() + 7)
                            {
                                gR.PredictedComposition.ProteinID = Lines[sh + molnames.Count() + 7];
                            }
                            else
                            {
                                gR.PredictedComposition.ProteinID = "?";
                            }
                            Ans.Add(gR);
                        }
                    }
                }
                //older data format.
                else if (headers[3] == "PeptideSequence")
                {
                    Boolean moreCompounds = true;
                    int i = 24;
                    while (moreCompounds)
                    {
                        if (headers[i] != "Adduct/Replacement")
                        {
                            molnames.Add(headers[i]);
                            i++;
                        }
                        else
                            moreCompounds = false;
                    }
                    bool firstRow = true;
                    while (read.Peek() >= 0)
                    {
                        //Read data
                        String Line = read.ReadLine();
                        String[] Lines = Line.Split(',');
                        //initialize new gR object
                        ResultsGroup gR = new ResultsGroup();
                        DeconRow dR = new DeconRow();
                        CompositionHypothesisEntry cH = new CompositionHypothesisEntry();
                        gR.DeconRow = dR;
                        gR.PredictedComposition = cH;
                        if (firstRow)
                        {
                            gR.PredictedComposition.ElementNames.AddRange(new List<string> { "C", "H", "N", "O", "S", "P" });
                            gR.PredictedComposition.MoleculeNames = molnames;
                            firstRow = false;
                        }

                        //Input data
                        if (!String.IsNullOrEmpty(Lines[0]))
                        {                            
                            gR.Score = Convert.ToDouble(Lines[0]);
                            gR.DeconRow.MonoisotopicMassWeight = Convert.ToDouble(Lines[1]);
                            gR.PredictedComposition.CompoundComposition = Lines[2];
                            if (String.IsNullOrEmpty(Lines[2]) || Lines[2] == "0")
                                gR.Match = false;
                            else
                                gR.Match = true;
                            gR.PredictedComposition.PepSequence = Lines[3];
                            gR.PredictedComposition.MassWeight = Convert.ToDouble(Lines[5]);
                            gR.NumModiStates = Convert.ToDouble(Lines[6]);
                            gR.NumChargeStates = Convert.ToInt32(Lines[7]);
                            gR.NumOfScan = Convert.ToDouble(Lines[8]);
                            gR.ScanDensity = Convert.ToDouble(Lines[9]);
                            gR.ExpectedA = Convert.ToDouble(Lines[10]);
                            gR.AvgAA2List = new List<double>();
                            gR.AvgAA2List.Add(Convert.ToDouble(Lines[11]));
                            gR.TotalVolume = Convert.ToDouble(Lines[12]);
                            gR.AvgSigNoise = Convert.ToDouble(Lines[13]);
                            gR.CentroidScan = Convert.ToDouble(Lines[14]);
                            gR.DeconRow.ScanNum = Convert.ToInt32(Lines[15]);
                            gR.MaxScanNum = Convert.ToInt32(Lines[16]);
                            gR.MinScanNum = Convert.ToInt32(Lines[17]);
                            gR.PredictedComposition.eqCount = new Dictionary<string, int>();
                            for (int k = 18; k < 24; k++)
                            {
                                gR.PredictedComposition.ElementAmount.Add(Convert.ToInt32(Lines[k]));
                            }
                            List<int> eqCoun = new List<int>();
                            for (int j = 0; j < molnames.Count(); j++)
                            {
                                eqCoun.Add(Convert.ToInt32(Lines[24 + j]));
                            }
                            gR.PredictedComposition.eqCounts = eqCoun;
                            gR.PredictedComposition.AddRep = Lines[24 + molnames.Count()];
                            gR.PredictedComposition.AdductNum = Convert.ToInt32(Lines[24 + molnames.Count() + 1]);
                            gR.PredictedComposition.PepModification = Lines[24 + molnames.Count() + 2];
                            gR.PredictedComposition.MissedCleavages = Convert.ToInt32(Lines[24 + molnames.Count() + 3]);
                            gR.PredictedComposition.NumGlycosylations = Convert.ToInt32(Lines[24 + molnames.Count() + 4]);
                            Ans.Add(gR);
                        }
                    }
                }
                //This is supporting an older format of data. Today is Sept 2013, can be deleted after 1 year.
                else
                {
                    Boolean moreCompounds = true;
                    int i = 23;
                    while (moreCompounds)
                    {
                        if (headers[i] != "Adduct/Replacement")
                        {
                            molnames.Add(headers[i]);
                            i++;
                        }
                        else
                            moreCompounds = false;
                    }
                    bool firstRow = true;
                    while (read.Peek() >= 0)
                    {
                        //Read data
                        String Line = read.ReadLine();
                        String[] Lines = Line.Split(',');
                        //initialize new gR object
                        ResultsGroup gR = new ResultsGroup();
                        if (firstRow)
                        {
                            gR.PredictedComposition.ElementNames.AddRange(new List<string> { "C", "H", "N", "O", "S", "P" });
                            gR.PredictedComposition.MoleculeNames = molnames;
                            firstRow = false;
                        }
                        DeconRow dR = new DeconRow();
                        CompositionHypothesisEntry cH = new CompositionHypothesisEntry();
                        gR.DeconRow = dR;
                        gR.PredictedComposition = cH;
                        if (!String.IsNullOrEmpty(Lines[0]))
                        {
                            //Input data
                           
                            gR.Score = Convert.ToDouble(Lines[0]);
                            gR.DeconRow.MonoisotopicMassWeight = Convert.ToDouble(Lines[1]);
                            gR.PredictedComposition.CompoundComposition = Lines[2].Replace(",", ";");
                            if (String.IsNullOrEmpty(Lines[2]) || Lines[2] == "0")
                                gR.Match = false;
                            else
                                gR.Match = true;
                            gR.PredictedComposition.MassWeight = Convert.ToDouble(Lines[4]);
                            gR.NumModiStates = Convert.ToDouble(Lines[5]);
                            gR.NumChargeStates = Convert.ToInt32(Lines[6]);
                            gR.NumOfScan = Convert.ToDouble(Lines[7]);
                            gR.ScanDensity = Convert.ToDouble(Lines[8]);
                            gR.ExpectedA = Convert.ToDouble(Lines[9]);
                            gR.AvgAA2List = new List<double>();
                            gR.AvgAA2List.Add(Convert.ToDouble(Lines[10]));
                            gR.TotalVolume = Convert.ToDouble(Lines[11]);
                            gR.AvgSigNoise = Convert.ToDouble(Lines[12]);
                            gR.CentroidScan = Convert.ToDouble(Lines[13]);
                            gR.DeconRow.ScanNum = Convert.ToInt32(Lines[14]);
                            gR.MaxScanNum = Convert.ToInt32(Lines[15]);
                            gR.MinScanNum = Convert.ToInt32(Lines[16]);
                            gR.PredictedComposition.eqCount = new Dictionary<string, int>();
                            for (int k = 17; k < 23; k++)
                            {
                                gR.PredictedComposition.ElementAmount.Add(Convert.ToInt32(Lines[k]));
                            }
                            gR.PredictedComposition.eqCount.Add("A", Convert.ToInt32(Lines[23]));
                            gR.PredictedComposition.eqCount.Add("B", Convert.ToInt32(Lines[24]));
                            gR.PredictedComposition.eqCount.Add("C", Convert.ToInt32(Lines[25]));
                            gR.PredictedComposition.eqCount.Add("D", Convert.ToInt32(Lines[26]));
                            gR.PredictedComposition.eqCount.Add("E", Convert.ToInt32(Lines[27]));
                            gR.PredictedComposition.eqCount.Add("F", Convert.ToInt32(Lines[28]));
                            gR.PredictedComposition.eqCount.Add("G", Convert.ToInt32(Lines[29]));
                            gR.PredictedComposition.eqCount.Add("H", Convert.ToInt32(Lines[30]));
                            gR.PredictedComposition.eqCount.Add("I", Convert.ToInt32(Lines[31]));
                            gR.PredictedComposition.eqCount.Add("J", Convert.ToInt32(Lines[32]));
                            gR.PredictedComposition.eqCount.Add("K", Convert.ToInt32(Lines[33]));
                            gR.PredictedComposition.eqCount.Add("L", Convert.ToInt32(Lines[34]));
                            gR.PredictedComposition.eqCount.Add("M", Convert.ToInt32(Lines[35]));
                            gR.PredictedComposition.eqCount.Add("N", Convert.ToInt32(Lines[36]));
                            gR.PredictedComposition.eqCount.Add("O", Convert.ToInt32(Lines[37]));
                            gR.PredictedComposition.eqCount.Add("P", Convert.ToInt32(Lines[38]));
                            gR.PredictedComposition.eqCount.Add("Q", Convert.ToInt32(Lines[39]));
                            gR.PredictedComposition.AddRep = Lines[40];
                            gR.PredictedComposition.AdductNum = Convert.ToInt32(Lines[41]);
                            gR.PredictedComposition.PepSequence = Lines[42];
                            gR.PredictedComposition.PepModification = Lines[43];
                            gR.PredictedComposition.MissedCleavages = Convert.ToInt32(Lines[44]);
                            gR.PredictedComposition.NumGlycosylations = Convert.ToInt32(Lines[45]);
                            Ans.Add(gR);
                        }
                    }
                }
            }
            //This is gly1 data.
            else
            {
                String header = read.ReadLine();
                String[] headers = header.Split('\t');

                while (read.Peek() >= 0)
                {
                    //Read data
                    String Line = read.ReadLine();
                    String[] Lines = Line.Split('\t');
                    //initialize new gR object
                    ResultsGroup gR = new ResultsGroup();
                    DeconRow dR = new DeconRow();
                    CompositionHypothesisEntry cH = new CompositionHypothesisEntry();
                    gR.DeconRow = dR;
                    gR.PredictedComposition = cH;
                    if (!String.IsNullOrEmpty(Lines[0]))
                    {
                        //Input data
                        gR.PredictedComposition.MoleculeNames = molnames;
                        gR.Score = Convert.ToDouble(Lines[0]);
                        gR.DeconRow.MonoisotopicMassWeight = Convert.ToDouble(Lines[1]);
                        gR.PredictedComposition.CompoundComposition = Lines[2].Replace(",", ";");
                        if (String.IsNullOrEmpty(Lines[2]) || Lines[2] == "0")
                        {
                            gR.Match = false;
                            gR.PredictedComposition.MassWeight = 0;
                        }
                        else
                        {
                            gR.Match = true;
                            gR.PredictedComposition.MassWeight = Convert.ToDouble(Lines[4]);
                        }
                        gR.NumModiStates = Convert.ToDouble(Lines[5]);
                        gR.NumChargeStates = Convert.ToInt32(Lines[6]);
                        gR.NumOfScan = Convert.ToDouble(Lines[7]);
                        gR.ScanDensity = Convert.ToDouble(Lines[8]);
                        gR.ExpectedA = Convert.ToDouble(Lines[9]);
                        gR.AvgAA2List = new List<double>();
                        gR.AvgAA2List.Add(Convert.ToDouble(Lines[10]));
                        gR.TotalVolume = Convert.ToDouble(Lines[11]);
                        gR.AvgSigNoise = Convert.ToDouble(Lines[12]);
                        gR.CentroidScan = Convert.ToDouble(Lines[13]);
                        gR.DeconRow.ScanNum = Convert.ToInt32(Convert.ToDouble(Lines[14]));
                        Ans.Add(gR);
                    }
                }

            }
            return Ans;
        }

        //This is used to combine several groupingResults.
        public List<ResultsGroup> CombineResults(List<ResultsGroup>[] Results)
        {
            //Get molenames and elementIDs as usual, as first thing
            List<string> elementIDs = new List<string>();
            List<string> molename = new List<string>();
            for (int i = 0; i < Results[0].Count(); i++)
            {
                if (Results[0][i].PredictedComposition.ElementNames.Count > 0)
                {
                    for (int k = 0; k < Results[0][i].PredictedComposition.ElementNames.Count(); k++)
                    {
                        elementIDs.Add(Results[0][i].PredictedComposition.ElementNames[k]);
                    }
                    for (int k = 0; k < Results[0][i].PredictedComposition.MoleculeNames.Count(); k++)
                    {
                        molename.Add(Results[0][i].PredictedComposition.MoleculeNames[k]);
                    }
                    break;
                }
            }
            Double SumofTotalVolume = 0;
            List<ResultsGroup> storage = new List<ResultsGroup>();
            for (int i = 0; i < Results.Count(); i++)
            {
                for (int k = 0; k < Results[i].Count(); k++)
                {
                    Double[] AllTotalVolumes = new Double[Results.Count()];
                    Results[i][k].ListOfOriginalTotalVolumes = AllTotalVolumes;
                    Results[i][k].ListOfOriginalTotalVolumes[i] = Results[i][k].TotalVolume;
                    storage.Add(Results[i][k]);
                    SumofTotalVolume = SumofTotalVolume + Results[i][k].TotalVolume;
                }
            }
            List<ResultsGroup> FinalAns = new List<ResultsGroup>();
            storage = storage.OrderByDescending(a => a.PredictedComposition.MassWeight).ThenBy(b => b.PredictedComposition.CompoundComposition).ToList();
            int j = 0;
            while (j < storage.Count())
            {
                if (storage[j].PredictedComposition.MassWeight == 0)
                {
                    storage[j].RelativeTotalVolumeSD = 0;
                    storage[j].RelativeTotalVolume = storage[j].TotalVolume / SumofTotalVolume;
                    FinalAns.Add(storage[j]);
                    j++;
                    continue;
                }
                if (j == storage.Count() - 1)
                {
                    storage[j].RelativeTotalVolumeSD = 0;
                    storage[j].RelativeTotalVolume = storage[j].TotalVolume / SumofTotalVolume;
                    FinalAns.Add(storage[j]);
                    j++;
                    continue;
                }
                List<ResultsGroup> combiningRows = new List<ResultsGroup>();
                if (storage[j].PredictedComposition.CompoundComposition.Equals(storage[j + 1].PredictedComposition.CompoundComposition) && (storage[j].PredictedComposition.AdductNum == storage[j + 1].PredictedComposition.AdductNum) && storage[j].PredictedComposition.PepSequence.Equals(storage[j + 1].PredictedComposition.PepSequence))
                {
                    storage[j].RelativeTotalVolumeSD = 0;
                    storage[j].RelativeTotalVolume = storage[j].TotalVolume / SumofTotalVolume;
                    combiningRows.Add(storage[j]);
                    j++;
                    storage[j].RelativeTotalVolumeSD = 0;
                    storage[j].RelativeTotalVolume = storage[j].TotalVolume / SumofTotalVolume;
                    combiningRows.Add(storage[j]);
                    bool moreRows = true;
                    while (moreRows)
                    {
                        if (storage[j].PredictedComposition.CompoundComposition.Equals(storage[j + 1].PredictedComposition.CompoundComposition) && (storage[j].PredictedComposition.AdductNum == storage[j + 1].PredictedComposition.AdductNum) && storage[j].PredictedComposition.PepSequence.Equals(storage[j + 1].PredictedComposition.PepSequence))
                        {
                            j++;
                            storage[j].RelativeTotalVolumeSD = 0;
                            storage[j].RelativeTotalVolume = storage[j].TotalVolume / SumofTotalVolume;
                            combiningRows.Add(storage[j]);
                        }
                        else
                        {
                            moreRows = false;
                        }
                    }
                    j++;
                    FinalAns.Add(AverageResultsGroup(combiningRows));
                    continue;
                }
                else
                {
                    storage[j].RelativeTotalVolumeSD = 0;
                    storage[j].RelativeTotalVolume = storage[j].TotalVolume / SumofTotalVolume;
                    FinalAns.Add(storage[j]);
                    j++;
                    continue;
                }
            }
            FinalAns[0].PredictedComposition.ElementNames = elementIDs;
            FinalAns[0].PredictedComposition.MoleculeNames = molename;
            return FinalAns;
        }

        private ResultsGroup AverageResultsGroup(List<ResultsGroup> combiningRows)
        {
            List<Int32> NumChargeStates = new List<int>();
            List<Double> ScanDensity= new List<Double>() ;
            List<Double> NumModiStates= new List<Double>() ;
            List<Double> TotalVolume= new List<Double>() ;
            List<Double> ExpectedA= new List<Double>() ;
            List<Double> Score= new List<Double>() ;
            List<Double> CentroidScan= new List<Double>() ;
            List<Double> NumScan= new List<Double>() ;
            List<Double> AvgSigNoise= new List<Double>() ;
            //These are for calculating the features
            List<Int32> MaxScanNum= new List<int>() ;
            List<Int32> MinScanNum= new List<int>() ;
            List<Int32> ScanNums= new List<int>() ;
            List<Int32> ChargeStateList = new List<int>();
            List<Double> AvgSigNoiseList= new List<Double>() ;
            List<Double> CentroidScanLR= new List<Double>() ;
            List<Double> AvgAA2List= new List<Double>() ;
            List<Double> RelativeTotalVolume = new List<double>();


            List<Int32> scan_num = new List<Int32>();
            List<Int32> charge = new List<Int32>();
            List<Int32> abundance = new List<Int32>();
            List<Double> mz = new List<Double>();
            List<Double> fit = new List<Double>();
            List<Double> average_mw = new List<Double>();
            List<Double> monoisotopic_mw = new List<Double>();
            List<Double> mostabundant_mw = new List<Double>();
            List<Double> fwhm = new List<Double>();
            List<Double> signal_noise = new List<Double>();
            List<Int32> mono_abundance = new List<Int32>();
            List<Int32> mono_plus2_abundance = new List<Int32>();
            List<Int32> flag = new List<Int32>();
            List<Double> interference_sore = new List<Double>();

            ResultsGroup FinalAns = new ResultsGroup();
            FinalAns.ListOfOriginalTotalVolumes = combiningRows[0].ListOfOriginalTotalVolumes;

            for (int i = 0; i < combiningRows.Count(); i++)
            {
                NumChargeStates.Add(combiningRows[i].NumChargeStates);
                ScanDensity.Add(combiningRows[i].ScanDensity);
                NumModiStates.Add(combiningRows[i].NumModiStates);
                TotalVolume.Add(combiningRows[i].TotalVolume);
                ExpectedA.Add(combiningRows[i].ExpectedA);
                Score.Add(combiningRows[i].Score);
                CentroidScan.Add(combiningRows[i].CentroidScan);
                NumScan.Add(combiningRows[i].NumOfScan);
                AvgSigNoise.Add(combiningRows[i].AvgSigNoise);
                //These are for calculating the features
                MaxScanNum.Add(combiningRows[i].MaxScanNum);
                MinScanNum.Add(combiningRows[i].MinScanNum);
                CentroidScanLR.Add(combiningRows[i].CentroidScanLR);
                AvgAA2List.AddRange(combiningRows[i].AvgAA2List);
                RelativeTotalVolume.Add(combiningRows[i].RelativeTotalVolume);

                charge.Add(combiningRows[i].DeconRow.charge);
                scan_num.Add(combiningRows[i].DeconRow.ScanNum);
                abundance.Add(combiningRows[i].DeconRow.abundance);
                mz.Add(combiningRows[i].DeconRow.mz);
                fit.Add(combiningRows[i].DeconRow.fit);
                average_mw.Add(combiningRows[i].DeconRow.average_mw);
                monoisotopic_mw.Add(combiningRows[i].DeconRow.MonoisotopicMassWeight);
                mostabundant_mw.Add(combiningRows[i].DeconRow.mostabundant_mw);
                fwhm.Add(combiningRows[i].DeconRow.fwhm);
                signal_noise.Add(combiningRows[i].DeconRow.SignalNoiseRatio);
                mono_abundance.Add(combiningRows[i].DeconRow.MonoisotopicAbundance);
                mono_plus2_abundance.Add(combiningRows[i].DeconRow.MonoisotopicPlus2Abundance);
                flag.Add(combiningRows[i].DeconRow.flag);
                interference_sore.Add(combiningRows[i].DeconRow.interference_sore);
                for (int h = 0; h < combiningRows[i].ListOfOriginalTotalVolumes.Count(); h++)
                {
                    if (combiningRows[i].ListOfOriginalTotalVolumes[h] > FinalAns.ListOfOriginalTotalVolumes[h])
                    {
                        FinalAns.ListOfOriginalTotalVolumes[h] = combiningRows[i].ListOfOriginalTotalVolumes[h];
                    }
                }

            }

            
            FinalAns.DeconRow = new DeconRow();
            FinalAns.PredictedComposition = combiningRows[0].PredictedComposition;
            FinalAns.NumChargeStates = Convert.ToInt32(NumChargeStates.Average());
            FinalAns.ScanDensity = ScanDensity.Average();
            FinalAns.NumModiStates = NumModiStates.Average();
            FinalAns.TotalVolume = TotalVolume.Average();
            FinalAns.ExpectedA = ExpectedA.Average();
            FinalAns.Score = Score.Average();
            FinalAns.CentroidScan = CentroidScan.Average();
            FinalAns.NumOfScan = NumScan.Average();
            FinalAns.AvgSigNoise = AvgSigNoise.Average();
            FinalAns.MaxScanNum = Convert.ToInt32(MaxScanNum.Average());
            FinalAns.MinScanNum = Convert.ToInt32(MinScanNum.Average());
            FinalAns.CentroidScanLR = CentroidScanLR.Average();
            FinalAns.TotalVolumeSD = TotalVolume.StdDev();
            FinalAns.RelativeTotalVolumeSD = RelativeTotalVolume.StdDev();
            FinalAns.AvgAA2List = new List<double>();
            FinalAns.AvgAA2List.Add(AvgAA2List.Average());
            FinalAns.RelativeTotalVolume = RelativeTotalVolume.Average();

            FinalAns.DeconRow.ScanNum = Convert.ToInt32(scan_num.Average());
            FinalAns.DeconRow.abundance = Convert.ToInt32(abundance.Average());
            FinalAns.DeconRow.mz = mz.Average();
            FinalAns.DeconRow.fit = fit.Average();
            FinalAns.DeconRow.average_mw = average_mw.Average();
            FinalAns.DeconRow.MonoisotopicMassWeight = monoisotopic_mw.Average();
            FinalAns.DeconRow.mostabundant_mw = mostabundant_mw.Average();
            FinalAns.DeconRow.fwhm = fwhm.Average();
            FinalAns.DeconRow.SignalNoiseRatio = signal_noise.Average();
            FinalAns.DeconRow.MonoisotopicAbundance = Convert.ToInt32(mono_abundance.Average());
            FinalAns.DeconRow.MonoisotopicPlus2Abundance = Convert.ToInt32(mono_plus2_abundance.Average());
            FinalAns.DeconRow.flag = Convert.ToInt32(flag.Average());
            FinalAns.DeconRow.interference_sore = interference_sore.Average();

            return FinalAns;
        }


        public static StreamWriter WriteResultsToStream(StreamWriter writer, List<ResultsGroup> results, List<String> elementNames, List<String> moleculeNames)
        {
            String header = "Score,MassSpec MW,Compound Key,PeptideSequence,PPM Error,#ofAdduct,#ofCharges,#ofScans,ScanDensity,Avg A:A+2 Error,A:A+2 Ratio,Total Volume,Signal to Noise Ratio,Centroid Scan Error,Centroid Scan,MaxScanNumber,MinScanNumber";
            foreach(var element in elementNames){
                header += "," + element;
                Console.WriteLine(element);
            }
            header += ",Hypothesis MW";
            
            foreach (var name in moleculeNames)
            {
                header += ("," + name);
                Console.WriteLine(name);
            }

            header += ",Adduct/Replacement,Adduct Amount,PeptideModification,PeptideMissedCleavage#,#ofGlycanAttachmentToPeptide,StartAA,EndAA,ProteinID";
            ParametersForm pr = new ParametersForm();
            ParametersForm.ParameterSettings parameterInfo = pr.GetParameters();
            writer.WriteLine(header);
            for (int i = 0; i < results.Count; i++)
            {
                ResultsGroup result = results[i];
                DeconRow observed = result.DeconRow;
                CompositionHypothesisEntry hypothesis = result.PredictedComposition;
                Console.WriteLine(hypothesis);
                //If this is a prediction, emit in one format
                if (hypothesis.MassWeight != 0)
                {


                    double ppmError = ((observed.MonoisotopicMassWeight - hypothesis.MassWeight) / observed.MonoisotopicMassWeight);
                    ppmError *= 1000000;
                    writer.Write(result.Score + "," + observed.MonoisotopicMassWeight + "," +
                        hypothesis.CompoundComposition + "," + hypothesis.PepSequence + ","
                        + ppmError + "," + result.NumModiStates + "," + result.NumChargeStates + ","
                        + result.NumOfScan + "," + result.ScanDensity + "," + result.ExpectedA + ","
                        + (observed.MonoisotopicAbundance / (observed.MonoisotopicPlus2Abundance + 1)) + ","
                        + result.TotalVolume + "," + observed.SignalNoiseRatio + ","
                        + result.CentroidScan + "," + observed.ScanNum + ","
                        + result.MaxScanNum + "," + result.MinScanNum
                        );
                    for (int j = 0; j < elementNames.Count; j++)
                    {
                        writer.Write("," + hypothesis.ElementAmount[j]);
                    }
                    writer.Write("," + hypothesis.MassWeight);
                    for (int j = 0; j < moleculeNames.Count; j++)
                    {
                        writer.Write("," + hypothesis.eqCounts[j]);
                    }
                    writer.WriteLine("," + hypothesis.AddRep + "," + hypothesis.AdductNum + ","
                        + hypothesis.PepModification + "," + hypothesis.MissedCleavages + ","
                        + hypothesis.NumGlycosylations + "," + hypothesis.StartAA + ","
                        + hypothesis.EndAA + "," + hypothesis.ProteinID);

                }
                else
                {
                    writer.Write(result.Score + "," + result.DeconRow.MonoisotopicMassWeight + "," + 0 + "" + "," + "," + 0 + "," + result.NumModiStates + "," + result.NumChargeStates + "," + result.NumOfScan + "," + result.ScanDensity + "," + result.ExpectedA + "," + (result.DeconRow.MonoisotopicAbundance / (result.DeconRow.MonoisotopicPlus2Abundance + 1)) + "," + result.TotalVolume + "," + result.DeconRow.SignalNoiseRatio + "," + result.CentroidScan + "," + result.DeconRow.ScanNum + "," + result.MaxScanNum + "," + result.MinScanNum);
                    for (int s = 0; s < elementNames.Count(); s++)
                    {
                        writer.Write("," + 0);
                    }
                    writer.Write("," + 0);
                    for (int s = 0; s < moleculeNames.Count(); s++)
                    {
                        writer.Write("," + 0);
                    }
                    writer.WriteLine("," + "N/A" + "," + 0 + "," + "" + "," + 0 + "," + 0 + "," + 0 + "," + 0);
                }
            }

            writer.Flush();
            return writer;
        }
    
    }



    //Accessor Methods:####################################################################
    public class ResultsGroup
    {
        public ResultsGroup()
        {
            PredictedComposition = new CompositionHypothesisEntry();
        }
        public DeconRow DeconRow;
        public Boolean MostAbundant;
        public Boolean Match;
        public CompositionHypothesisEntry PredictedComposition;
        //The features
        public Int32 NumChargeStates;
        public Double ScanDensity;
        public Double NumModiStates;
        public Double TotalVolume;
        public Double ExpectedA;
        public Double Score;
        public Double CentroidScan;
        public Double NumOfScan;
        public Double AvgSigNoise;
        //These are for calculating the features
        public Int32 MaxScanNum;
        public Int32 MinScanNum;
        public List<Int32> ScanNumList;
        public List<Int32> ChargeStateList;
        public List<Double> AvgSigNoiseList;
        public Double CentroidScanLR;
        public List<Double> AvgAA2List;
        //These are special values for combining groupingResults
        public Double TotalVolumeSD;
        public Double RelativeTotalVolume;
        public Double RelativeTotalVolumeSD;
        public Double[] ListOfOriginalTotalVolumes;
        //They are for calculating the average weighted MW after groupings.
        public List<Double> ListMonoMassWeight;
        public List<Double> ListAbundance;
    }
}
