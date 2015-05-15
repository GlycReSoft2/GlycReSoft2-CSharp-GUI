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
    class UnsupervisedLearner
    {
        List<DeconRow> DeconDATA = new List<DeconRow>();
        public List<ResultsGroup>[] run(OpenFileDialog FileLinks)
        {
            List<ResultsGroup>[] AllFinalResults = new List<ResultsGroup>[Convert.ToInt32(FileLinks.FileNames.Count())];
            Int32 Count = 0;
            //Each data file is treated separately, hence the for loop.
            foreach (String filename in FileLinks.FileNames)
            {
                //Get the Parameters.
                ParametersForm parameter = new ParametersForm();
                ParametersForm.ParameterSettings paradata = parameter.GetParameters();

                //Perform the First and second grouping and getting data for the features by the Grouping function.
                List<ResultsGroup> LRLR = new List<ResultsGroup>();
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
                SupervisedLearner sl = new SupervisedLearner();
                AllFinalResults[Count] = sl.Scorings(LRLR, finalfeatureData, paradata);
                Count++;
            }
            return AllFinalResults;
        }

        public List<ResultsGroup>[] evaluate(OpenFileDialog FileLinks, Feature featureData)
        {
            List<ResultsGroup>[] AllFinalResults = new List<ResultsGroup>[Convert.ToInt32(FileLinks.FileNames.Count())];
            Int32 Count = 0;
            //Each data file is treated separately, hence the for loop.
            foreach (String filename in FileLinks.FileNames)
            {
                //Get the Parameters.
                ParametersForm parameterForm = new ParametersForm();
                ParametersForm.ParameterSettings parameters = parameterForm.GetParameters();

                //Perform the First and second grouping and getting data for the features by the Grouping function.
                List<ResultsGroup> LRLR = new List<ResultsGroup>();
                LRLR = Groupings(filename, parameters);


                //Generate scores.
                SupervisedLearner sl = new SupervisedLearner();
                AllFinalResults[Count] = sl.Scorings(LRLR, featureData, parameters);
                Count++;
            }
            return AllFinalResults;
        }

        //this Grouping function performs the grouping.
        private List<ResultsGroup> Groupings(String filename, ParametersForm.ParameterSettings paradata)
        {
            GetDeconData DeconDATA1 = new GetDeconData();
            List<DeconRow> sortedDeconData = new List<DeconRow>();
            sortedDeconData = DeconDATA1.getdata(filename);
            //First, sort the list descendingly by its abundance.
            sortedDeconData = sortedDeconData.OrderByDescending(a => a.abundance).ToList();
            //################Second, create a new list to store results from the first grouping.###############
            List<ResultsGroup> fgResults = new List<ResultsGroup>();
            ResultsGroup GR2 = new ResultsGroup();
            GR2.PredictedComposition = new CompositionHypothesisEntry();
            Int32 currentMaxBin = new Int32();
            currentMaxBin = 1;
            GR2.DeconRow = sortedDeconData[0];
            GR2.MostAbundant = true;
            GR2.NumOfScan = 1;
            GR2.MinScanNum = sortedDeconData[0].ScanNum;
            GR2.MaxScanNum = sortedDeconData[0].ScanNum;
            GR2.ChargeStateList = new List<int>();
            GR2.ChargeStateList.Add(sortedDeconData[0].charge);
            GR2.AvgSigNoiseList = new List<Double>();
            GR2.AvgSigNoiseList.Add(sortedDeconData[0].SignalNoiseRatio);
            GR2.AvgAA2List = new List<double>();
            GR2.AvgAA2List.Add(sortedDeconData[0].MonoisotopicAbundance / (sortedDeconData[0].MonoisotopicPlus2Abundance + 1));
            GR2.ScanNumList = new List<Int32>();
            GR2.ScanNumList.Add(sortedDeconData[0].ScanNum);
            GR2.NumModiStates = 1;
            GR2.TotalVolume = sortedDeconData[0].abundance * sortedDeconData[0].fwhm;
            GR2.ListAbundance = new List<double>();
            GR2.ListAbundance.Add(sortedDeconData[0].abundance);
            GR2.ListMonoMassWeight = new List<double>();
            GR2.ListMonoMassWeight.Add(sortedDeconData[0].MonoisotopicMassWeight);
            fgResults.Add(GR2);
            for (int j = 1; j < sortedDeconData.Count; j++)
            {
                for (int i = 0; i < fgResults.Count; i++)
                {
                    //Obtain grouping error. Note: its in ppm, so it needs to be multiplied by 0.000001.
                    Double GroupingError = fgResults[i].DeconRow.MonoisotopicMassWeight * paradata.GroupingErrorEG * 0.000001;
                    if ((sortedDeconData[j].MonoisotopicMassWeight < (fgResults[i].DeconRow.MonoisotopicMassWeight + GroupingError) && (sortedDeconData[j].MonoisotopicMassWeight > (fgResults[i].DeconRow.MonoisotopicMassWeight - GroupingError))))
                    {
                        if (fgResults[i].MaxScanNum < sortedDeconData[j].ScanNum)
                        {
                            fgResults[i].MaxScanNum = sortedDeconData[j].ScanNum;
                        }
                        else if (fgResults[i].MinScanNum > sortedDeconData[j].ScanNum)
                        {
                            fgResults[i].MinScanNum = sortedDeconData[j].ScanNum;
                        }
                        fgResults[i].NumOfScan = fgResults[i].NumOfScan + 1;
                        fgResults[i].ScanNumList.Add(sortedDeconData[j].ScanNum);
                        fgResults[i].TotalVolume = fgResults[i].TotalVolume + sortedDeconData[j].abundance * sortedDeconData[j].fwhm;
                        fgResults[i].ChargeStateList.Add(sortedDeconData[j].charge);
                        fgResults[i].AvgSigNoiseList.Add(sortedDeconData[j].SignalNoiseRatio);
                        fgResults[i].AvgAA2List.Add(sortedDeconData[j].MonoisotopicAbundance / (sortedDeconData[j].MonoisotopicPlus2Abundance + 1));
                        fgResults[i].ListAbundance.Add(sortedDeconData[j].abundance);
                        fgResults[i].ListMonoMassWeight.Add(sortedDeconData[j].MonoisotopicMassWeight);
                        break;
                    }

                    if (i == fgResults.Count - 1)
                    {
                        ResultsGroup GR = new ResultsGroup();
                        GR.PredictedComposition = new CompositionHypothesisEntry();
                        currentMaxBin = currentMaxBin + 1;
                        GR.DeconRow = sortedDeconData[j];
                        GR.MostAbundant = true;
                        GR.NumOfScan = 1;
                        GR.MinScanNum = sortedDeconData[j].ScanNum;
                        GR.MaxScanNum = sortedDeconData[j].ScanNum;
                        GR.ChargeStateList = new List<int>();
                        GR.ChargeStateList.Add(sortedDeconData[j].charge);
                        GR.AvgSigNoiseList = new List<Double>();
                        GR.AvgSigNoiseList.Add(sortedDeconData[j].SignalNoiseRatio);
                        GR.AvgAA2List = new List<double>();
                        GR.AvgAA2List.Add(sortedDeconData[j].MonoisotopicAbundance / (sortedDeconData[j].MonoisotopicPlus2Abundance + 1));
                        GR.ScanNumList = new List<int>();
                        GR.ScanNumList.Add(sortedDeconData[j].ScanNum);
                        GR.NumModiStates = 1;
                        GR.TotalVolume = sortedDeconData[j].abundance * sortedDeconData[j].fwhm;
                        GR.ListAbundance = new List<double>();
                        GR.ListAbundance.Add(sortedDeconData[j].abundance);
                        GR.ListMonoMassWeight = new List<double>();
                        GR.ListMonoMassWeight.Add(sortedDeconData[j].MonoisotopicMassWeight);
                        fgResults.Add(GR);
                    }
                }
            }
            //Lastly calculate the Average Weighted Abundance
            for (int y = 0; y < fgResults.Count(); y++)
            {
                Double sumofTopPart = 0;
                for (int z = 0; z < fgResults[y].ListMonoMassWeight.Count(); z++)
                {
                    sumofTopPart = sumofTopPart + fgResults[y].ListMonoMassWeight[z] * fgResults[y].ListAbundance[z];
                }
                fgResults[y].DeconRow.MonoisotopicMassWeight = sumofTopPart / fgResults[y].ListAbundance.Sum();
            }

            //######################## Here is the second grouping for NH3. ################################
                fgResults = fgResults.OrderBy(o => o.DeconRow.MonoisotopicMassWeight).ToList();
                for (int i = 0; i < fgResults.Count - 1; i++)
                {
                    if (fgResults[i].MostAbundant == true)
                    {
                        int numModStates = 1;
                        for (int j = i + 1; j < fgResults.Count; j++)
                        {
                            Double AdductTolerance = fgResults[i].DeconRow.MonoisotopicMassWeight * paradata.AdductToleranceEA * 0.000001;
                            if ((fgResults[i].DeconRow.MonoisotopicMassWeight >= (fgResults[j].DeconRow.MonoisotopicMassWeight - 17.02654911 * numModStates - AdductTolerance)) && (fgResults[i].DeconRow.MonoisotopicMassWeight <= (fgResults[j].DeconRow.MonoisotopicMassWeight - 17.02654911 * numModStates + AdductTolerance)))
                            {
                                //obtain max and min scan number
                                if (fgResults[i].MaxScanNum < fgResults[j].MaxScanNum)
                                {
                                    fgResults[i].MaxScanNum = fgResults[j].MaxScanNum;
                                }
                                else
                                {
                                    fgResults[i].MaxScanNum = fgResults[i].MaxScanNum;
                                }

                                if (fgResults[i].MinScanNum > fgResults[j].MinScanNum)
                                {
                                    fgResults[i].MinScanNum = fgResults[j].MinScanNum;
                                }
                                else
                                {
                                    fgResults[i].MinScanNum = fgResults[i].MinScanNum;
                                }
                                //numOfScan
                                fgResults[i].NumOfScan = fgResults[i].NumOfScan + fgResults[j].NumOfScan;
                                fgResults[i].ScanNumList.AddRange(fgResults[j].ScanNumList);
                                //ChargeStateList
                                for (int h = 0; h < fgResults[j].ChargeStateList.Count; h++)
                                {
                                    fgResults[i].ChargeStateList.Add(fgResults[j].ChargeStateList[h]);
                                }
                                //avgSigNoiseList
                                for (int h = 0; h < fgResults[j].AvgSigNoiseList.Count; h++)
                                {
                                    fgResults[i].AvgSigNoiseList.Add(fgResults[j].AvgSigNoiseList[h]);
                                }
                                //avgAA2List
                                for (int h = 0; h < fgResults[j].AvgAA2List.Count; h++)
                                {
                                    fgResults[i].AvgAA2List.Add(fgResults[j].AvgAA2List[h]);
                                }
                                //numModiStates
                                numModStates++;
                                fgResults[i].NumModiStates = fgResults[i].NumModiStates + 1;
                                fgResults[j].MostAbundant = false;
                                //TotalVolume
                                fgResults[i].TotalVolume = fgResults[i].TotalVolume + fgResults[j].TotalVolume;
                                if (fgResults[i].DeconRow.abundance < fgResults[j].DeconRow.abundance)
                                {
                                    fgResults[i].DeconRow = fgResults[j].DeconRow;
                                    numModStates = 1;
                                }
                            }
                            else if (fgResults[i].DeconRow.MonoisotopicMassWeight < (fgResults[j].DeconRow.MonoisotopicMassWeight - (17.02654911 + AdductTolerance * 2) * numModStates))
                            {
                                //save running time. Since the list is sorted, any other mass below won't match as an adduct.
                                break;
                            }
                        }
                    }
                }
            



            //Implement the scan number threshold
            fgResults = fgResults.OrderByDescending(a => a.NumOfScan).ToList();
            Int32 scanCutOff = fgResults.Count() + 1;
            for (int t = 0; t < fgResults.Count(); t++)
            {
                if (fgResults[t].NumOfScan < paradata.MinScanNumber)
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
                if (fgResults[i].AvgAA2List.Average() != 0)
                {
                    aainput.Add(fgResults[i].DeconRow.MonoisotopicMassWeight);
                    aaoutput.Add(fgResults[i].AvgAA2List.Average());
                }
                if (fgResults[i].DeconRow.abundance > 250)
                {
                    ccoutput.Add(fgResults[i].ScanNumList.Average());
                    ccinput.Add(fgResults[i].DeconRow.MonoisotopicMassWeight);
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
                Int32 MaxScanNumber = fgResults[i].MaxScanNum;
                Int32 MinScanNumber = fgResults[i].MinScanNum;
                Double NumOfScan = fgResults[i].NumOfScan;
                List<Int32> numChargeStatesList = fgResults[i].ChargeStateList.Distinct().ToList();
                Int32 numChargeStates = numChargeStatesList.Count;
                Double numModiStates = fgResults[i].NumModiStates;
                if ((MaxScanNumber - MinScanNumber) != 0)
                    ScanDensity = NumOfScan / (MaxScanNumber - MinScanNumber + 15);
                else
                    ScanDensity = 0;
                //Use this scandensity for all molecules in this grouping.

                fgResults[i].NumChargeStates = numChargeStates;
                fgResults[i].ScanDensity = ScanDensity;
                fgResults[i].NumModiStates = numModiStates;
                fgResults[i].CentroidScanLR = CSEregression.Compute(fgResults[i].DeconRow.MonoisotopicMassWeight);
                fgResults[i].CentroidScan = Math.Abs(fgResults[i].ScanNumList.Average() - fgResults[i].CentroidScanLR);
                fgResults[i].ExpectedA = Math.Abs(fgResults[i].AvgAA2List.Average() - AA2regression.Compute(fgResults[i].DeconRow.MonoisotopicMassWeight));
                fgResults[i].AvgSigNoise = fgResults[i].AvgSigNoiseList.Average();
            }
            return fgResults;
        }

    }
}
