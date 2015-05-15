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
    public class SupervisedLearner
    {
        List<DeconRow> DeconData = new List<DeconRow>();
        public List<ResultsGroup>[] Run(OpenFileDialog FileLinks, List<CompositionHypothesisEntry> comhyp)
        {
            //Initialize storage variables.
            List<ResultsGroup>[] AllFinalResults = new List<ResultsGroup>[Convert.ToInt32(FileLinks.FileNames.Count())];
            Int32 Count = 0;

            //Each data file is treated separately, hence the for loop.
            foreach (String filename in FileLinks.FileNames)
            {
                //Get the Parameters.
                ParametersForm parameter = new ParametersForm();
                ParametersForm.ParameterSettings paradata = parameter.GetParameters();

                //Perform the First and second grouping, matching and getting data for the features by the Grouping function.
                Double Mas = new Double();
                Mas = adductMass(comhyp);
                List<ResultsGroup> LRLR = new List<ResultsGroup>();
                LRLR = Groupings(filename, paradata, Mas, comhyp);

                //Error prevention
                if (LRLR.Count == 1)
                {
                    MessageBox.Show("There is no match between the hypothesis and the data. Unable to generate results from the file:" + filename);
                    List<ResultsGroup> FinalResult = LRLR;
                    AllFinalResults[Count] = FinalResult;
                    Count++;
                    continue;
                }

                //##############Logistic Regression####################
                Feature featureData = FitLogisticRegression(LRLR);

                //Generate scores.
                AllFinalResults[Count] = Scorings(LRLR, featureData, paradata);
                Count++;
            }
            return AllFinalResults;
        }

        //This is used by the Features class to evaluate the features
        public List<ResultsGroup>[] EvaluateFeature(OpenFileDialog FileLinks, List<CompositionHypothesisEntry> comhyp, Feature dfeatureData)
        {
            //Initialize storage variables.
            List<ResultsGroup>[] AllFinalResults = new List<ResultsGroup>[Convert.ToInt32(FileLinks.FileNames.Count())];
            Int32 Count = 0;
            //Each data file is treated separately, hence the for loop.
            foreach (String filename in FileLinks.FileNames)
            {
                //Get the Parameters.
                ParametersForm parameter = new ParametersForm();
                ParametersForm.ParameterSettings paradata = parameter.GetParameters();

                //Perform the First and second grouping, matching and getting data for the features by the Grouping function.
                Double Mas = new Double();
                Mas = adductMass(comhyp);
                List<ResultsGroup> LRLR = new List<ResultsGroup>();
                LRLR = Groupings(filename, paradata, Mas, comhyp);

                //Error prevention
                if (LRLR.Count == 1)
                {
                    MessageBox.Show("There is no match between the hypothesis and the data. Unable to generate results from the file:" + filename);
                    List<ResultsGroup> FinalResult = LRLR;
                    AllFinalResults[Count] = FinalResult;
                    Count++;
                    continue;
                }

                //##############Logistic Regression####################
                Feature featureData = FitLogisticRegression(LRLR);

                //Generate scores.
                AllFinalResults[Count] = Scorings(LRLR, featureData, paradata);
                Count++;
            }
            return AllFinalResults;
        }

        //This is used by the Feature Class to generate Features
        public Feature obtainFeatures(OpenFileDialog FileLinks, List<CompositionHypothesisEntry> comhyp)
        {
            List<Double> Ini = new List<Double>();
            List<Double> nCS = new List<Double>();
            List<Double> SD = new List<Double>();
            List<Double> nMS = new List<Double>();
            List<Double> tV = new List<Double>();
            List<Double> EA = new List<Double>();
            List<Double> CS = new List<Double>();
            List<Double> NS = new List<Double>();
            List<Double> SN = new List<Double>();

            //Each data file is treated separately, hence the for loop.
            foreach (String filename in FileLinks.FileNames)
            {
                //Get the Parameters.
                ParametersForm parameter = new ParametersForm();
                ParametersForm.ParameterSettings paradata = parameter.GetParameters();

                //Perform the First and second grouping, matching and getting data for the features by the Grouping function.
                Double Mas = new Double();
                Mas = adductMass(comhyp);
                List<ResultsGroup> LRLR = new List<ResultsGroup>();
                LRLR = Groupings(filename, paradata, Mas, comhyp);

                //Error prevention
                if (LRLR.Count == 1)
                {
                    MessageBox.Show("There is no match between the hypothesis and the data. Unable to generate results from the file:" + filename);
                    continue;
                }

                //##############Logistic Regression####################
                //Perform logistic regression to get the Parameters
                Feature featureData = new Feature();
                featureData = FitLogisticRegression(LRLR);
                Ini.Add(featureData.Initial);
                nCS.Add(featureData.numChargeStates);
                SD.Add(featureData.ScanDensity);
                nMS.Add(featureData.numModiStates);
                tV.Add(featureData.totalVolume);
                EA.Add(featureData.ExpectedA);
                CS.Add(featureData.CentroidScan);
                NS.Add(featureData.numOfScan);
                SN.Add(featureData.avgSigNoise);

            }
            // Get the average of all features.
            Feature finalans = new Feature();
            finalans.Initial = Ini.Average();
            finalans.numChargeStates = nCS.Average();
            finalans.ScanDensity = SD.Average();
            finalans.numModiStates = nMS.Average();
            finalans.totalVolume = tV.Average();
            finalans.ExpectedA = EA.Average();
            finalans.CentroidScan = CS.Average();
            finalans.numOfScan = NS.Average();
            finalans.avgSigNoise = SN.Average();
            return finalans;
        }

        //this "Grouping" function performs the grouping.
        private List<ResultsGroup> Groupings(String filename, ParametersForm.ParameterSettings modelParameters, Double Mas, List<CompositionHypothesisEntry> comhyp)
        {
            GetDeconData DeconDATA1 = new GetDeconData();

            List<string> elementIDs = new List<string>();
            List<string> molename = new List<string>();
            for (int i = 0; i < comhyp.Count(); i++ )
            {
                if (comhyp[i].ElementNames.Count > 0)
                {
                    for (int j = 0; j < comhyp[i].ElementNames.Count(); j++)
                    {
                        elementIDs.Add(comhyp[i].ElementNames[j]);
                    }
                    for (int j = 0; j < comhyp[i].MoleculeNames.Count(); j++)
                    {
                        molename.Add(comhyp[i].MoleculeNames[j]);
                    }
                    break;
                }
            }
            List<DeconRow> sortedDeconData = new List<DeconRow>();;
            sortedDeconData = DeconDATA1.getdata(filename);
            //First, sort the list descendingly by its abundance.
            sortedDeconData = sortedDeconData.OrderByDescending(a => a.abundance).ToList();
            //################Second, create a new list to store results from the first grouping.###############
            List<ResultsGroup> fgResults = new List<ResultsGroup>();
            ResultsGroup GR2 = new ResultsGroup();
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
                    Double GroupingError = fgResults[i].DeconRow.MonoisotopicMassWeight * modelParameters.GroupingErrorEG * 0.000001;
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

            //######################## Here is the second grouping. ################################
            fgResults = fgResults.OrderBy(o => o.DeconRow.MonoisotopicMassWeight).ToList();
            if (Mas != 0)
            {
                for (int i = 0; i < fgResults.Count - 1; i++)
                {
                    if (fgResults[i].MostAbundant == true)
                    {
                        int numModStates = 1;
                        for (int j = i + 1; j < fgResults.Count; j++)
                        {
                            Double AdductTolerance = fgResults[i].DeconRow.MonoisotopicMassWeight * modelParameters.AdductToleranceEA * 0.000001;
                            if ((fgResults[i].DeconRow.MonoisotopicMassWeight >= (fgResults[j].DeconRow.MonoisotopicMassWeight - Mas * numModStates - AdductTolerance)) && (fgResults[i].DeconRow.MonoisotopicMassWeight <= (fgResults[j].DeconRow.MonoisotopicMassWeight - Mas * numModStates + AdductTolerance)))
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
                            else if (fgResults[i].DeconRow.MonoisotopicMassWeight < (fgResults[j].DeconRow.MonoisotopicMassWeight - (Mas + AdductTolerance * 2) * numModStates))
                            {
                                //save running time. Since the list is sorted, any other mass below won't match as an adduct.
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < fgResults.Count; i++)
                {
                    fgResults[i].NumModiStates = 0;
                }
            }
            List<ResultsGroup> sgResults = new List<ResultsGroup>();
            //Implement the scan number threshold
            fgResults = fgResults.OrderByDescending(a => a.NumOfScan).ToList();
            Int32 scanCutOff = fgResults.Count() + 1;
            for (int t = 0; t < fgResults.Count(); t++)
            {
                if (fgResults[t].NumOfScan < modelParameters.MinScanNumber)
                {
                    scanCutOff = t;
                    break;
                }
            }
            if (scanCutOff != fgResults.Count() + 1)
            {
                fgResults.RemoveRange(scanCutOff, fgResults.Count() - scanCutOff);
            }

            //############# This is the matching part. It matches the composition hypothesis with the grouped decon data.############
            String[] MolNames = new String[17];

            //These numOfMatches and lists are used to fit the linear regression model for Expect A: A+2. They are put here to decrease the already-int running time.
            Int32 numOfMatches = new Int32();
            List<Double> moleWeightforA = new List<Double>();
            List<Double> AARatio = new List<Double>();
            //Used to obtain all available bins for centroid scan error.
            //Read the other lines for compTable data.
            fgResults = fgResults.OrderByDescending(a => a.DeconRow.MonoisotopicMassWeight).ToList();
            comhyp = comhyp.OrderByDescending(b => b.MassWeight).ToList();
            bool hasMatch = false;
            int lastMatch = 0;
            for (int j = 0; j < fgResults.Count; j++)
            {
                if (fgResults[j].MostAbundant == true)
                {
                    lastMatch = lastMatch - 4;
                    if (lastMatch < 0)
                        lastMatch = 0;
                    for (int i = lastMatch; i < comhyp.Count; i++)
                    {

                        Double MatchingError = comhyp[i].MassWeight * modelParameters.MatchErrorEM * 0.000001;
                        if ((fgResults[j].DeconRow.MonoisotopicMassWeight <= (comhyp[i].MassWeight + MatchingError)) && (fgResults[j].DeconRow.MonoisotopicMassWeight >= (comhyp[i].MassWeight - MatchingError)))
                        {
                            ResultsGroup GR = new ResultsGroup();
                            GR = matchPassbyValue(fgResults[j], comhyp[i]);
                            sgResults.Add(GR);
                            //Stuffs for feature
                            numOfMatches++;
                            moleWeightforA.Add(fgResults[j].DeconRow.MonoisotopicMassWeight);
                            AARatio.Add(fgResults[j].AvgAA2List.Average());
                            lastMatch = i + 1;
                            hasMatch = true;
                            continue;
                        }
                        //Since the data is sorted, there are no more matches below that row, break it.
                        if (fgResults[j].DeconRow.MonoisotopicMassWeight > (comhyp[i].MassWeight + MatchingError))
                        {
                            if (hasMatch == false)
                            {
                                ResultsGroup GR = new ResultsGroup();
                                CompositionHypothesisEntry comhypi = new CompositionHypothesisEntry();
                                GR = fgResults[j];
                                GR.Match = false;
                                GR.PredictedComposition = comhypi;
                                sgResults.Add(GR);
                                lastMatch = i;
                                break;
                            }
                            else
                            {
                                hasMatch = false;
                                break;
                            }
                        }
                    }
                }
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
            if (numOfMatches > 3)
            {
                for (int i = 0; i < sgResults.Count; i++)
                {
                    if (sgResults[i].Match == true)
                    {
                        if (sgResults[i].AvgAA2List.Average() != 0)
                        {
                            aainput.Add(sgResults[i].DeconRow.MonoisotopicMassWeight);
                            aaoutput.Add(sgResults[i].AvgAA2List.Average());
                        }
                        if (sgResults[i].DeconRow.abundance > 250)
                        {
                            ccoutput.Add(sgResults[i].DeconRow.ScanNum);
                            ccinput.Add(sgResults[i].DeconRow.MonoisotopicMassWeight);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < sgResults.Count; i++)
                {
                    if (sgResults[i].AvgAA2List.Average() != 0)
                    {
                        aainput.Add(sgResults[i].DeconRow.MonoisotopicMassWeight);
                        aaoutput.Add(sgResults[i].AvgAA2List.Average());
                    }
                    if (sgResults[i].DeconRow.abundance > 250)
                    {
                        ccoutput.Add(sgResults[i].ScanNumList.Average());
                        ccinput.Add(sgResults[i].DeconRow.MonoisotopicMassWeight);
                    }
                }
            }
            SimpleLinearRegression CSEregression = new SimpleLinearRegression();
            CSEregression.Regress(ccinput.ToArray(), ccoutput.ToArray());
            AA2regression.Regress(aainput.ToArray(), aaoutput.ToArray());


            //The remaining features and input them into the grouping results
            for (int i = 0; i < sgResults.Count; i++)
            {
                //ScanDensiy is: Number of scan divided by (max scan number – min scan number)
                Double ScanDensity = new Double();
                Int32 MaxScanNumber = sgResults[i].MaxScanNum;
                Int32 MinScanNumber = sgResults[i].MinScanNum;
                Double NumOfScan = sgResults[i].NumOfScan;
                List<Int32> numChargeStatesList = sgResults[i].ChargeStateList.Distinct().ToList();
                Int32 numChargeStates = numChargeStatesList.Count;
                Double numModiStates = sgResults[i].NumModiStates;
                if ((MaxScanNumber - MinScanNumber) != 0)
                    ScanDensity = NumOfScan / (MaxScanNumber - MinScanNumber + 15);
                else
                    ScanDensity = 0;
                //Use this scandensity for all molecules in this grouping.

                sgResults[i].NumChargeStates = numChargeStates;
                sgResults[i].ScanDensity = ScanDensity;
                sgResults[i].NumModiStates = numModiStates;
                sgResults[i].CentroidScanLR = CSEregression.Compute(sgResults[i].DeconRow.MonoisotopicMassWeight);
                sgResults[i].CentroidScan = Math.Abs(sgResults[i].ScanNumList.Average() - sgResults[i].CentroidScanLR);
                sgResults[i].ExpectedA = Math.Abs(sgResults[i].AvgAA2List.Average() - AA2regression.Compute(sgResults[i].DeconRow.MonoisotopicMassWeight));
                sgResults[i].AvgSigNoise = sgResults[i].AvgSigNoiseList.Average();
            }
            for (int i = 0; i < sgResults.Count(); i++ )
            {
                sgResults[i].PredictedComposition.ElementNames.Clear();
                sgResults[i].PredictedComposition.MoleculeNames.Clear();

                if (i == sgResults.Count() - 1)
                {
                    sgResults[0].PredictedComposition.ElementNames = elementIDs;
                    sgResults[0].PredictedComposition.MoleculeNames = molename;
                }
            }
            return sgResults;
        }

        //Used by matching part to prevent pass by reference.
        private ResultsGroup matchPassbyValue(ResultsGroup input1, CompositionHypothesisEntry comhypo)
        {
            ResultsGroup storage = new ResultsGroup();
            //Pass by value, I only way I know we can do this is to pass them one by one. Yes, it is troublesome.
            storage.DeconRow = input1.DeconRow;
            storage.MostAbundant = input1.MostAbundant;
            storage.NumChargeStates = input1.NumChargeStates;
            storage.ScanDensity = input1.ScanDensity;
            storage.NumModiStates = input1.NumModiStates;
            storage.TotalVolume = input1.TotalVolume;
            storage.ExpectedA = input1.ExpectedA;
            storage.CentroidScan = input1.CentroidScan;
            storage.NumOfScan = input1.NumOfScan;
            storage.AvgSigNoise = input1.AvgSigNoise;
            storage.MaxScanNum = input1.MaxScanNum;
            storage.MinScanNum = input1.MinScanNum;
            storage.ScanNumList = input1.ScanNumList;
            storage.ChargeStateList = input1.ChargeStateList;
            storage.AvgSigNoiseList = input1.AvgSigNoiseList;
            storage.CentroidScanLR = input1.CentroidScanLR;
            storage.AvgAA2List = input1.AvgAA2List;

            storage.PredictedComposition = comhypo;
            storage.Match = true;
            return storage;
        }

        //This class performs logistic regression from data obtained from the Groupings Function
        private Feature FitLogisticRegression(List<ResultsGroup> LRLR)
        {
            int numofMatches = 0;
            //now, put LRLR into a table of arrays so that the regression function can read it.
            Double[][] inputs = new Double[LRLR.Count][];
            Double[] output = new Double[LRLR.Count];
            for (int i = 0; i < LRLR.Count; i++)
            {
                inputs[i] = new Double[] { Convert.ToDouble(LRLR[i].NumChargeStates), Convert.ToDouble(LRLR[i].ScanDensity), Convert.ToDouble(LRLR[i].NumModiStates), Convert.ToDouble(LRLR[i].TotalVolume), Convert.ToDouble(LRLR[i].ExpectedA), Convert.ToDouble(LRLR[i].CentroidScan), Convert.ToDouble(LRLR[i].NumOfScan),  Convert.ToDouble(LRLR[i].AvgSigNoise) };
                output[i] = Convert.ToDouble(LRLR[i].Match);
                if (LRLR[i].Match == true)
                    numofMatches++;
            }

            if (numofMatches < 10)
            {
                Features FeaturesMenu = new Features();
                Feature defaultFeatures = FeaturesMenu.readFeature();
                MessageBox.Show("Warning: there are less than 10 matches. Currently Loaded Features will be used instead.");
                return defaultFeatures;
            }

            //Perform logistic regression to get the Parameters
            LogisticRegression regression = new LogisticRegression(inputs: 8);
            var results = new IterativeReweightedLeastSquares(regression);
            double delta = 0;
            do
            {
                // Perform an iteration
                delta = results.Run(inputs, output);

            } while (delta > 0.001);

            Feature answer = new Feature();
            //Here are the beta values in logistic regression.
            answer.Initial = regression.Coefficients[0];
            answer.numChargeStates = regression.Coefficients[1];
            answer.ScanDensity = regression.Coefficients[2];
            answer.numModiStates = regression.Coefficients[3];
            answer.totalVolume = regression.Coefficients[4];
            answer.ExpectedA = regression.Coefficients[5];
            answer.CentroidScan = regression.Coefficients[6];
            answer.numOfScan = regression.Coefficients[7];
            answer.avgSigNoise = regression.Coefficients[8];
            return answer;
        }

        private List<ResultsGroup> balanceMatch(List<ResultsGroup> LRLR)
        {
            List<ResultsGroup> MatchList = new List<ResultsGroup>();
            List<ResultsGroup> noMatchList = new List<ResultsGroup>();
            for (int i = 0; i < LRLR.Count; i++)
            {
                if (LRLR[i].Match == true)
                {
                    MatchList.Add(LRLR[i]);
                }
                else
                {
                    noMatchList.Add(LRLR[i]);
                }
            }
            //Keep match: unmatch ratio to 1:ratio
            double ratio = 2;



            //for every 1 match, there can at most be 2 non-match, and vice versa
            Double MNMratio = Convert.ToDouble(MatchList.Count())/Convert.ToDouble(noMatchList.Count());
            if (MNMratio <= ratio && MNMratio >= 1/ratio)
                //the ratio is good, ending the function.
                return LRLR;

            //need less match in the data
            if (MNMratio > ratio)
            {
                //all nomatch will be in the new data, randomly pick match to fit in. (Reuse the nomatch list to save time and memory)
                //This is the amount of match we will add into LRLR.
                Int32 numneeded = Convert.ToInt32(noMatchList.Count() * ratio);

                List<int> rangeone = Enumerable.Range(0, MatchList.Count).ToList();
                rangeone.Shuffle();

                for (int i = 0; i < numneeded; i++)
                {
                    noMatchList.Add(MatchList[rangeone[i]]);
                }
                return noMatchList;
            }

            //need less no match in the data
            if (MNMratio < 1/ratio)
            {
                //all omatch will be in the new data, randomly pick nomatch to fit in. (Reuse the match list to save time and memory)
                Int32 numneeded = Convert.ToInt32(MatchList.Count() * ratio);
                List<int> rangeone = Enumerable.Range(0, noMatchList.Count).ToList();
                rangeone.Shuffle();

                for (int i = 0; i < numneeded; i++)
                {
                    MatchList.Add(noMatchList[rangeone[i]]);
                }
                return MatchList;
            }

            //We should never arrive here, but when it happens....
            return LRLR;
        }
        //balanceMatch function uses this to randomly pick out items in list.
        static Random rnd = new Random();

        //This runs the linear regression and generate score for the grouping results
        public List<ResultsGroup> Scorings(List<ResultsGroup> LRLR, Feature featureData, ParametersForm.ParameterSettings paradata)
        {
            //Now, load current features from the software, if it doesn't exist, use default features.
            Features fea = new Features();
            Feature dfeatureData = fea.readFeature();
            String defaultpath = Application.StartupPath + "\\FeatureDefault.fea";
            Feature defaultData = fea.readFeature(defaultpath);
            Double initial = featureData.Initial * 0.9 + dfeatureData.Initial * 0.05 + defaultData.Initial * 0.05;
            Double bnumChargeStates = featureData.numChargeStates * 0.9 + dfeatureData.numChargeStates * 0.05 + defaultData.numChargeStates * 0.05;
            Double bScanDensity = featureData.ScanDensity * 0.9 + dfeatureData.ScanDensity * 0.05 + defaultData.ScanDensity * 0.05;
            Double bnumModiStates = featureData.numModiStates * 0.9 + dfeatureData.numModiStates * 0.05 + defaultData.numModiStates * 0.05;
            Double btotalVolume = featureData.totalVolume * 0.9 + dfeatureData.totalVolume * 0.05 + defaultData.totalVolume * 0.05;
            Double bExpectedA = featureData.ExpectedA * 0.9 + dfeatureData.totalVolume * 0.05 + defaultData.totalVolume * 0.05;
            Double bCentroid = featureData.CentroidScan * 0.9 + dfeatureData.CentroidScan * 0.05 + defaultData.CentroidScan * 0.05;
            Double bnumOfScan = featureData.numOfScan * 0.9 + dfeatureData.numOfScan * 0.05 + defaultData.numOfScan * 0.05;
            Double bavgSigNoise = featureData.avgSigNoise * 0.9 + dfeatureData.avgSigNoise * 0.05 + defaultData.avgSigNoise * 0.05;
            
            if (dfeatureData.Initial != defaultData.Initial)
            {
            //Here are the beta values in logistic regression. 0.75 is from default, 0.25 is from calculation.
                 initial = featureData.Initial * 0.7 + dfeatureData.Initial * 0.2 + defaultData.Initial * 0.1;
                 bnumChargeStates = featureData.numChargeStates * 0.7 + dfeatureData.numChargeStates * 0.2 + defaultData.numChargeStates * 0.1;
                 bScanDensity = featureData.ScanDensity * 0.7 + dfeatureData.ScanDensity * 0.2 + defaultData.ScanDensity * 0.1;
                 bnumModiStates = featureData.numModiStates * 0.7 + dfeatureData.numModiStates * 0.2 + defaultData.numModiStates * 0.1;
                 btotalVolume = featureData.totalVolume * 0.7 + dfeatureData.totalVolume * 0.2 + defaultData.totalVolume * 0.1;
                 bExpectedA = featureData.ExpectedA * 0.7 + dfeatureData.totalVolume * 0.2 + defaultData.totalVolume * 0.1;
                 bCentroid = featureData.CentroidScan * 0.7 + dfeatureData.CentroidScan * 0.2 + defaultData.CentroidScan * 0.1;
                 bnumOfScan = featureData.numOfScan * 0.7 + dfeatureData.numOfScan * 0.2 + defaultData.numOfScan * 0.1;
                 bavgSigNoise = featureData.avgSigNoise * 0.7 + dfeatureData.avgSigNoise * 0.2 + defaultData.avgSigNoise * 0.1;
            }


            Double e = Math.E;
            try
            {
                //Now calculate the scores for each of them.
                Double scoreInput = new Double();
                Double Score = new Double();
                for (int o = 0; o < LRLR.Count; o++)
                {                    
                    scoreInput = (initial + bnumChargeStates * Convert.ToDouble(LRLR[o].NumChargeStates) + bScanDensity * Convert.ToDouble(LRLR[o].ScanDensity) + bnumModiStates * Convert.ToDouble(LRLR[o].NumModiStates) + btotalVolume * Convert.ToDouble(LRLR[o].TotalVolume) + bExpectedA * Convert.ToDouble(LRLR[o].ExpectedA) + bCentroid * Convert.ToDouble(LRLR[o].CentroidScan) + bnumOfScan * Convert.ToDouble(LRLR[o].NumOfScan) + bavgSigNoise * Convert.ToDouble(LRLR[o].AvgSigNoise));
                    Double store = Math.Pow(e, (-1 * scoreInput));
                    Score = 1 / (1 + store);
                    if (Score >= 0.5)
                    {
                        store = Math.Pow(e, (-0.6 * scoreInput));
                        Score = (0.8512 / (1 + store)) + 0.1488;
                    }
                    else
                    {
                        store = Math.Pow(e, (-0.6 * scoreInput -0.3));
                        Score = 1 / (1 + store);
                    }

                    LRLR[o].Score = Score;
                }
                //Implement score threshold
                LRLR = LRLR.OrderByDescending(a => a.Score).ToList();



                if (LRLR[0].Score + LRLR[1].Score + LRLR[2].Score > 2.94)
                {
                    scoreInput = (initial + bnumChargeStates * Convert.ToDouble(LRLR[0].NumChargeStates) + bScanDensity * Convert.ToDouble(LRLR[0].ScanDensity) + bnumModiStates * Convert.ToDouble(LRLR[0].NumModiStates) + btotalVolume * Convert.ToDouble(LRLR[0].TotalVolume) + bExpectedA * Convert.ToDouble(LRLR[0].ExpectedA) + bCentroid * Convert.ToDouble(LRLR[0].CentroidScan) + bnumOfScan * Convert.ToDouble(LRLR[0].NumOfScan) + bavgSigNoise * Convert.ToDouble(LRLR[0].AvgSigNoise));
                    scoreInput = scoreInput + (initial + bnumChargeStates * Convert.ToDouble(LRLR[1].NumChargeStates) + bScanDensity * Convert.ToDouble(LRLR[1].ScanDensity) + bnumModiStates * Convert.ToDouble(LRLR[1].NumModiStates) + btotalVolume * Convert.ToDouble(LRLR[1].TotalVolume) + bExpectedA * Convert.ToDouble(LRLR[1].ExpectedA) + bCentroid * Convert.ToDouble(LRLR[1].CentroidScan) + bnumOfScan * Convert.ToDouble(LRLR[1].NumOfScan) + bavgSigNoise * Convert.ToDouble(LRLR[1].AvgSigNoise));
                    scoreInput = scoreInput + (initial + bnumChargeStates * Convert.ToDouble(LRLR[2].NumChargeStates) + bScanDensity * Convert.ToDouble(LRLR[2].ScanDensity) + bnumModiStates * Convert.ToDouble(LRLR[2].NumModiStates) + btotalVolume * Convert.ToDouble(LRLR[2].TotalVolume) + bExpectedA * Convert.ToDouble(LRLR[2].ExpectedA) + bCentroid * Convert.ToDouble(LRLR[2].CentroidScan) + bnumOfScan * Convert.ToDouble(LRLR[2].NumOfScan) + bavgSigNoise * Convert.ToDouble(LRLR[2].AvgSigNoise));
                    scoreInput = scoreInput / 3;
                    Double n = -2.9444389791664404600090274318879 / scoreInput;
                    for (int o = 0; o < LRLR.Count; o++)
                    {
                        if (LRLR[o].Score >= 0.57444251681)
                        {
                            scoreInput = (initial + bnumChargeStates * Convert.ToDouble(LRLR[o].NumChargeStates) + bScanDensity * Convert.ToDouble(LRLR[o].ScanDensity) + bnumModiStates * Convert.ToDouble(LRLR[o].NumModiStates) + btotalVolume * Convert.ToDouble(LRLR[o].TotalVolume) + bExpectedA * Convert.ToDouble(LRLR[o].ExpectedA) + bCentroid * Convert.ToDouble(LRLR[o].CentroidScan) + bnumOfScan * Convert.ToDouble(LRLR[o].NumOfScan) + bavgSigNoise * Convert.ToDouble(LRLR[o].AvgSigNoise));
                            Double store = Math.Pow(e, (n* scoreInput));
                            Score = (0.8512 / (1 + store)) + 0.1488;
                            LRLR[o].Score = Score;
                        }
                    }
                }



                Int32 scoreCutOff = LRLR.Count() + 1;
                for (int t = 0; t < LRLR.Count(); t++)
                {
                    if (LRLR[t].Score < paradata.MinScoreThreshold)
                    {
                        scoreCutOff = t;
                        break;
                    }
                }
                if (scoreCutOff != LRLR.Count() + 1)
                {
                    LRLR.RemoveRange(scoreCutOff, LRLR.Count() - scoreCutOff);
                }
            }
            catch
            {
                for (int o = 0; o < LRLR.Count; o++)
                {
                    LRLR[o].Score = 0;
                }
            }

            return LRLR;
        }

        //This class calculates delta adduct mass.
        private Double adductMass(List<CompositionHypothesisEntry> comhyp)
        {
            String adduct = "";
            String replacement = "";
            try
            {
                String AddRep = comhyp[1].AddRep;
                String[] AddandRep = AddRep.Split('/');
                adduct = AddandRep[0];
                replacement = AddandRep[1];
            }
            catch 
            {
                adduct = "0";
                replacement = "0";
            }

            //Regex for the capital letters
            string regexpattern = @"[A-Z]{1}[a-z]?[a-z]?\d?";
            MatchCollection adducts = Regex.Matches(adduct, regexpattern);
            MatchCollection replacements = Regex.Matches(replacement, regexpattern);
            //Regex for the element;
            string elementregexpattern = "[A-Z]{1}[a-z]?[a-z]?";
            //Regex for the number of the element.
            string numberregexpattern = "[0-9]*$";
            Double adductmass = 0;
            Double replacementmass = 0;
            PeriodicTable pTable = new PeriodicTable();
            //Console.WriteLine("Adduct String {0}, {1} Adduct Matches {1}", adduct, adducts);
            //For each element in adducts, add up their masses.
            foreach (Match add in adducts)
            {
                String ad = Convert.ToString(add);
                String element = Convert.ToString(Regex.Match(ad, elementregexpattern));
                String snumber = Convert.ToString(Regex.Match(ad, numberregexpattern));
                Int32 number = 0;
                if (snumber == String.Empty)
                {
                    number = 1;
                }
                else
                {
                    number = Convert.ToInt32(snumber);
                }

                adductmass = adductmass + number * pTable.getMass(element);
            }
            //For each element in replacements, add up their masses.
            //Console.WriteLine("Replacement String {0}, {1} Replacement Matches {1}", replacement, replacements);
            foreach (Match rep in replacements)
            {
                String el = Convert.ToString(rep);
                String element = Convert.ToString(Regex.Match(el, elementregexpattern));
                String snumber = Convert.ToString(Regex.Match(el, numberregexpattern));
                Int32 number = 0;
                if (snumber == String.Empty)
                {
                    number = 1;
                }
                else
                {
                    number = Convert.ToInt32(Regex.Match(el, numberregexpattern));
                }
                replacementmass = replacementmass + number * pTable.getMass(element);
            }

            //Finally, subtract them and obtain delta mass.
            Double dMass = adductmass - replacementmass;
            return dMass;
        }


    }

}