using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Windows.Forms;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Linear;
using Accord.Statistics.Models.Regression.Fitting;

namespace GlycReSoft
{
    class unsupervisedLearning
    {
        List<DeconRow> DeconDATA = new List<DeconRow>();
        public List<groupingResults>[] run(OpenFileDialog FileLinks)
        {
            List<groupingResults>[] AllFinalResults = new List<groupingResults>[Convert.ToInt32(FileLinks.FileNames.Count())];
            Int32 Count = 0;
            //Each data file is treated separately, hence the for loop.
            foreach (String filename in FileLinks.FileNames)
            {
                //Get the parameters.
                parameters parameter = new parameters();
                parameters.para paradata = parameter.getParameters();

                //Perform the First and second grouping and getting data for the features by the Grouping function.
                List<groupingResults> LRLR = new List<groupingResults>();
                LRLR = Groupings(filename, paradata);


                //##############Logistic Regression####################
                Features fe = new Features();
                //current features
                Feature featureData = fe.readFeature();
                //default features
                String defaultpath = Application.StartupPath + "\\FeatureDefault.fea";
                Feature defaultData = fe.readFeature(defaultpath);
                
                //Features that will be used
                Feature finalfeatureData = new Feature();
                
                //Here are the beta values in logistic regression.
                finalfeatureData.Initial = featureData.Initial * 0.5 + defaultData.Initial * 0.5;
                finalfeatureData.numChargeStates = featureData.numChargeStates * 0.5 + defaultData.numChargeStates * 0.5;
                finalfeatureData.ScanDensity = featureData.ScanDensity * 0.5 + defaultData.ScanDensity * 0.5;
                finalfeatureData.numModiStates = featureData.numModiStates * 0.5 + defaultData.numModiStates * 0.5;
                finalfeatureData.totalVolume = featureData.totalVolume * 0.5 + defaultData.totalVolume * 0.5;
                finalfeatureData.ExpectedA = featureData.ExpectedA * 0.5 + defaultData.ExpectedA * 0.5;
                finalfeatureData.CentroidScan = featureData.CentroidScan * 0.5 + defaultData.CentroidScan * 0.5;
                finalfeatureData.numOfScan = featureData.numOfScan * 0.5 + defaultData.numOfScan * 0.5;
                finalfeatureData.avgSigNoise = featureData.avgSigNoise * 0.5 + defaultData.avgSigNoise * 0.5;


                //Generate scores.
                supervisedLearning sl = new supervisedLearning();
                AllFinalResults[Count] = sl.Scorings(LRLR, finalfeatureData, paradata);
                Count++;
            }
            return AllFinalResults;
        }

        public List<groupingResults>[] evaluate(OpenFileDialog FileLinks, Feature featureData)
        {
            List<groupingResults>[] AllFinalResults = new List<groupingResults>[Convert.ToInt32(FileLinks.FileNames.Count())];
            Int32 Count = 0;
            //Each data file is treated separately, hence the for loop.
            foreach (String filename in FileLinks.FileNames)
            {
                //Get the parameters.
                parameters parameter = new parameters();
                parameters.para paradata = parameter.getParameters();

                //Perform the First and second grouping and getting data for the features by the Grouping function.
                List<groupingResults> LRLR = new List<groupingResults>();
                LRLR = Groupings(filename, paradata);


                //Generate scores.
                supervisedLearning sl = new supervisedLearning();
                AllFinalResults[Count] = sl.Scorings(LRLR, featureData, paradata);
                Count++;
            }
            return AllFinalResults;
        }

        //this Grouping function performs the grouping.
        private List<groupingResults> Groupings(String filename, parameters.para paradata)
        {
            GetDeconData DeconDATA1 = new GetDeconData();
            List<DeconRow> sortedDeconData = new List<DeconRow>();
            sortedDeconData = DeconDATA1.getdata(filename);
            //First, sort the list descendingly by its abundance.
            sortedDeconData = sortedDeconData.OrderByDescending(a => a.abundance).ToList();
            //################Second, create a new list to store results from the first grouping.###############
            List<groupingResults> fgResults = new List<groupingResults>();
            groupingResults GR2 = new groupingResults();
            GR2.comphypo = new composition.comphypo();
            Int32 currentMaxBin = new Int32();
            currentMaxBin = 1;
            GR2.DeconRow = sortedDeconData[0];
            GR2.mostAbundant = true;
            GR2.numOfScan = 1;
            GR2.minScanNum = sortedDeconData[0].scan_num;
            GR2.maxScanNum = sortedDeconData[0].scan_num;
            GR2.ChargeStateList = new List<int>();
            GR2.ChargeStateList.Add(sortedDeconData[0].charge);
            GR2.avgSigNoiseList = new List<Double>();
            GR2.avgSigNoiseList.Add(sortedDeconData[0].signal_noise);
            GR2.avgAA2List = new List<double>();
            GR2.avgAA2List.Add(sortedDeconData[0].mono_abundance / (sortedDeconData[0].mono_plus2_abundance + 1));
            GR2.scanNumList = new List<Int32>();
            GR2.scanNumList.Add(sortedDeconData[0].scan_num);
            GR2.numModiStates = 1;
            GR2.totalVolume = sortedDeconData[0].abundance * sortedDeconData[0].fwhm;
            GR2.listAbun = new List<double>();
            GR2.listAbun.Add(sortedDeconData[0].abundance);
            GR2.listMonoMW = new List<double>();
            GR2.listMonoMW.Add(sortedDeconData[0].monoisotopic_mw);
            fgResults.Add(GR2);
            for (int j = 1; j < sortedDeconData.Count; j++)
            {
                for (int i = 0; i < fgResults.Count; i++)
                {
                    //Obtain grouping error. Note: its in ppm, so it needs to be multiplied by 0.000001.
                    Double GroupingError = fgResults[i].DeconRow.monoisotopic_mw * paradata.groupingErrorEG * 0.000001;
                    if ((sortedDeconData[j].monoisotopic_mw < (fgResults[i].DeconRow.monoisotopic_mw + GroupingError) && (sortedDeconData[j].monoisotopic_mw > (fgResults[i].DeconRow.monoisotopic_mw - GroupingError))))
                    {
                        if (fgResults[i].maxScanNum < sortedDeconData[j].scan_num)
                        {
                            fgResults[i].maxScanNum = sortedDeconData[j].scan_num;
                        }
                        else if (fgResults[i].minScanNum > sortedDeconData[j].scan_num)
                        {
                            fgResults[i].minScanNum = sortedDeconData[j].scan_num;
                        }
                        fgResults[i].numOfScan = fgResults[i].numOfScan + 1;
                        fgResults[i].scanNumList.Add(sortedDeconData[j].scan_num);
                        fgResults[i].totalVolume = fgResults[i].totalVolume + sortedDeconData[j].abundance * sortedDeconData[j].fwhm;
                        fgResults[i].ChargeStateList.Add(sortedDeconData[j].charge);
                        fgResults[i].avgSigNoiseList.Add(sortedDeconData[j].signal_noise);
                        fgResults[i].avgAA2List.Add(sortedDeconData[j].mono_abundance / (sortedDeconData[j].mono_plus2_abundance + 1));
                        fgResults[i].listAbun.Add(sortedDeconData[j].abundance);
                        fgResults[i].listMonoMW.Add(sortedDeconData[j].monoisotopic_mw);
                        break;
                    }

                    if (i == fgResults.Count - 1)
                    {
                        groupingResults GR = new groupingResults();
                        GR.comphypo = new composition.comphypo();
                        currentMaxBin = currentMaxBin + 1;
                        GR.DeconRow = sortedDeconData[j];
                        GR.mostAbundant = true;
                        GR.numOfScan = 1;
                        GR.minScanNum = sortedDeconData[j].scan_num;
                        GR.maxScanNum = sortedDeconData[j].scan_num;
                        GR.ChargeStateList = new List<int>();
                        GR.ChargeStateList.Add(sortedDeconData[j].charge);
                        GR.avgSigNoiseList = new List<Double>();
                        GR.avgSigNoiseList.Add(sortedDeconData[j].signal_noise);
                        GR.avgAA2List = new List<double>();
                        GR.avgAA2List.Add(sortedDeconData[j].mono_abundance / (sortedDeconData[j].mono_plus2_abundance + 1));
                        GR.scanNumList = new List<int>();
                        GR.scanNumList.Add(sortedDeconData[j].scan_num);
                        GR.numModiStates = 1;
                        GR.totalVolume = sortedDeconData[j].abundance * sortedDeconData[j].fwhm;
                        GR.listAbun = new List<double>();
                        GR.listAbun.Add(sortedDeconData[j].abundance);
                        GR.listMonoMW = new List<double>();
                        GR.listMonoMW.Add(sortedDeconData[j].monoisotopic_mw);
                        fgResults.Add(GR);
                    }
                }
            }
            //Lastly calculate the Average Weighted Abundance
            for (int y = 0; y < fgResults.Count(); y++)
            {
                Double sumofTopPart = 0;
                for (int z = 0; z < fgResults[y].listMonoMW.Count(); z++)
                {
                    sumofTopPart = sumofTopPart + fgResults[y].listMonoMW[z] * fgResults[y].listAbun[z];
                }
                fgResults[y].DeconRow.monoisotopic_mw = sumofTopPart / fgResults[y].listAbun.Sum();
            }

            //######################## Here is the second grouping for NH3. ################################
                fgResults = fgResults.OrderBy(o => o.DeconRow.monoisotopic_mw).ToList();
                for (int i = 0; i < fgResults.Count - 1; i++)
                {
                    if (fgResults[i].mostAbundant == true)
                    {
                        int numModStates = 1;
                        for (int j = i + 1; j < fgResults.Count; j++)
                        {
                            Double AdductTolerance = fgResults[i].DeconRow.monoisotopic_mw * paradata.adductToleranceEA * 0.000001;
                            if ((fgResults[i].DeconRow.monoisotopic_mw >= (fgResults[j].DeconRow.monoisotopic_mw - 17.02654911 * numModStates - AdductTolerance)) && (fgResults[i].DeconRow.monoisotopic_mw <= (fgResults[j].DeconRow.monoisotopic_mw - 17.02654911 * numModStates + AdductTolerance)))
                            {
                                //obtain max and min scan number
                                if (fgResults[i].maxScanNum < fgResults[j].maxScanNum)
                                {
                                    fgResults[i].maxScanNum = fgResults[j].maxScanNum;
                                }
                                else
                                {
                                    fgResults[i].maxScanNum = fgResults[i].maxScanNum;
                                }

                                if (fgResults[i].minScanNum > fgResults[j].minScanNum)
                                {
                                    fgResults[i].minScanNum = fgResults[j].minScanNum;
                                }
                                else
                                {
                                    fgResults[i].minScanNum = fgResults[i].minScanNum;
                                }
                                //numOfScan
                                fgResults[i].numOfScan = fgResults[i].numOfScan + fgResults[j].numOfScan;
                                fgResults[i].scanNumList.AddRange(fgResults[j].scanNumList);
                                //ChargeStateList
                                for (int h = 0; h < fgResults[j].ChargeStateList.Count; h++)
                                {
                                    fgResults[i].ChargeStateList.Add(fgResults[j].ChargeStateList[h]);
                                }
                                //avgSigNoiseList
                                for (int h = 0; h < fgResults[j].avgSigNoiseList.Count; h++)
                                {
                                    fgResults[i].avgSigNoiseList.Add(fgResults[j].avgSigNoiseList[h]);
                                }
                                //avgAA2List
                                for (int h = 0; h < fgResults[j].avgAA2List.Count; h++)
                                {
                                    fgResults[i].avgAA2List.Add(fgResults[j].avgAA2List[h]);
                                }
                                //numModiStates
                                numModStates++;
                                fgResults[i].numModiStates = fgResults[i].numModiStates + 1;
                                fgResults[j].mostAbundant = false;
                                //TotalVolume
                                fgResults[i].totalVolume = fgResults[i].totalVolume + fgResults[j].totalVolume;
                                if (fgResults[i].DeconRow.abundance < fgResults[j].DeconRow.abundance)
                                {
                                    fgResults[i].DeconRow = fgResults[j].DeconRow;
                                    numModStates = 1;
                                }
                            }
                            else if (fgResults[i].DeconRow.monoisotopic_mw < (fgResults[j].DeconRow.monoisotopic_mw - (17.02654911 + AdductTolerance * 2) * numModStates))
                            {
                                //save running time. Since the list is sorted, any other mass below won't match as an adduct.
                                break;
                            }
                        }
                    }
                }
            



            //Implement the scan number threshold
            fgResults = fgResults.OrderByDescending(a => a.numOfScan).ToList();
            Int32 scanCutOff = fgResults.Count() + 1;
            for (int t = 0; t < fgResults.Count(); t++)
            {
                if (fgResults[t].numOfScan < paradata.minScanNumber)
                {
                    scanCutOff = t;
                    break;
                }
            }
            if (scanCutOff != fgResults.Count() + 1)
            {
                fgResults.RemoveRange(scanCutOff, fgResults.Count() - scanCutOff);
            }

            for (int i = 0; i < fgResults.Count(); i++)
            {
                fgResults[i].Match = false;
            }


            //##############Last part, this is to calculate the feature data needed for logistic regression###################
            //Expected A and Centroid Scan Error need linear regression. The models are built here separately.
            //In the this model. output is the Y axis and input is X.
            SimpleLinearRegression AA2regression = new SimpleLinearRegression();
            List<double> aainput = new List<double>();
            List<double> aaoutput = new List<double>();
            //Centroid Scan Error
            List<double> ccinput = new List<double>();
            List<double> ccoutput = new List<double>();
            for (int i = 0; i < fgResults.Count; i++)
            {
                if (fgResults[i].avgAA2List.Average() != 0)
                {
                    aainput.Add(fgResults[i].DeconRow.monoisotopic_mw);
                    aaoutput.Add(fgResults[i].avgAA2List.Average());
                }
                if (fgResults[i].DeconRow.abundance > 250)
                {
                    ccoutput.Add(fgResults[i].scanNumList.Average());
                    ccinput.Add(fgResults[i].DeconRow.monoisotopic_mw);
                }
   
            }
            SimpleLinearRegression CSEregression = new SimpleLinearRegression();
            CSEregression.Regress(ccinput.ToArray(), ccoutput.ToArray());
            AA2regression.Regress(aainput.ToArray(), aaoutput.ToArray());


            //The remaining features and input them into the grouping results
            for (int i = 0; i < fgResults.Count; i++)
            {
                //ScanDensiy is: Number of scan divided by (max scan number – min scan number)
                Double ScanDensity = new Double();
                Int32 MaxScanNumber = fgResults[i].maxScanNum;
                Int32 MinScanNumber = fgResults[i].minScanNum;
                Double NumOfScan = fgResults[i].numOfScan;
                List<Int32> numChargeStatesList = fgResults[i].ChargeStateList.Distinct().ToList();
                Int32 numChargeStates = numChargeStatesList.Count;
                Double numModiStates = fgResults[i].numModiStates;
                if ((MaxScanNumber - MinScanNumber) != 0)
                    ScanDensity = NumOfScan / (MaxScanNumber - MinScanNumber + 15);
                else
                    ScanDensity = 0;
                //Use this scandensity for all molecules in this grouping.

                fgResults[i].numChargeStates = numChargeStates;
                fgResults[i].ScanDensity = ScanDensity;
                fgResults[i].numModiStates = numModiStates;
                fgResults[i].CentroidScanLR = CSEregression.Compute(fgResults[i].DeconRow.monoisotopic_mw);
                fgResults[i].CentroidScan = Math.Abs(fgResults[i].scanNumList.Average() - fgResults[i].CentroidScanLR);
                fgResults[i].ExpectedA = Math.Abs(fgResults[i].avgAA2List.Average() - AA2regression.Compute(fgResults[i].DeconRow.monoisotopic_mw));
                fgResults[i].avgSigNoise = fgResults[i].avgSigNoiseList.Average();
            }
            return fgResults;
        }

    }
}
