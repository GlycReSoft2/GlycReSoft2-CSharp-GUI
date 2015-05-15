using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Accord.Statistics.Analysis;
using MathParserTK;

namespace GlycReSoft
{

    public partial class MS1ResultsViewer : Form
    {
        //These are used to store the results, so that there is no need to rerun supervisedLearning repeatedly when you need data.
        private List<ResultsGroup>[] ResultGroups;
        private OpenFileDialog DeconData;
        private List<CompositionHypothesisEntry> CompositionHypothesisList;
        private List<string> ElementIDs = new List<string>();
        private List<string> MoleculeNames = new List<string>();
        PleaseWait WaitForm = new PleaseWait();

        //First method, run supervised learning with composition hypothesis.
        public MS1ResultsViewer(OpenFileDialog oFDDeconData, String ComHypoLink)
        {
            InitializeComponent();
            DeconData = oFDDeconData;
            CompositionHypothesisTabbedForm ct = new CompositionHypothesisTabbedForm();
            CompositionHypothesisList = ct.getCompHypo(ComHypoLink);

            //This part is used to show the "Please Wait" box while running the code.
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            //set lable and your waiting text in this form
            try
            {
                bw.RunWorkerAsync();//this will run the DoWork code at background thread
                WaitForm.ShowDialog();//show the please wait box
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            var elementsAndMolecules = GroupingResults.GetElementsAndMolecules(ResultGroups[0]);
            ElementIDs = elementsAndMolecules.Item1;
            MoleculeNames = elementsAndMolecules.Item2;
            //ElementIDs.Clear();
            //MoleculeNames.Clear();
            //for (int i = 0; i < ResultGroups[0].Count(); i++)
            //{
            //    if (ResultGroups[0][i].PredictedComposition.ElementNames.Count() > 0)
            //    {
            //        for (int j = 0; j < ResultGroups[0][i].PredictedComposition.ElementNames.Count(); j++)
            //        {
            //            ElementIDs.Add(ResultGroups[0][i].PredictedComposition.ElementNames[j]);
            //        }
            //        for (int j = 0; j < ResultGroups[0][i].PredictedComposition.MoleculeNames.Count(); j++)
            //        {
            //            MoleculeNames.Add(ResultGroups[0][i].PredictedComposition.MoleculeNames[j]);
            //        }
            //    }
            //}

            comboBox1.DataSource = oFDDeconData.FileNames;
            comboBox1.SelectedIndex = 0;
            VolumeHistogramFileSelector.DataSource = oFDDeconData.FileNames;
            VolumeHistogramFileSelector.SelectedIndex = 0;
            ResultsGridView.DataSource = ToDataTable(ResultGroups, 0);
            ResultsGridView.ReadOnly = true;
        }
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            RunSupervisedLearner();
        }
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(Convert.ToString(e.Error));
            }
            //all background work has completed and we are going to close the waiting message
            WaitForm.Close();
        }
        private void RunSupervisedLearner()
        {
            SupervisedLearner SL = new SupervisedLearner();
            ResultGroups = SL.Run(DeconData, CompositionHypothesisList);
        }

        //Second method, run unsupervised learning without composition hypothesis.
        public MS1ResultsViewer(OpenFileDialog oFDDeconData)
        {
            InitializeComponent();
            DeconData = oFDDeconData;

            //This part is used to show the "Please Wait" box while running the code.
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork2);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted2);

            //set lable and your waiting text in this form
            try
            {
                bw.RunWorkerAsync();//this will run the DoWork code at background thread
                WaitForm.ShowDialog();//show the please wait box
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            comboBox1.DataSource = oFDDeconData.FileNames;
            comboBox1.SelectedIndex = 0;
            VolumeHistogramFileSelector.DataSource = oFDDeconData.FileNames;
            VolumeHistogramFileSelector.SelectedIndex = 0;
            ResultsGridView.DataSource = ToDataTable(ResultGroups, 0);
            ResultsGridView.ReadOnly = true;

        }
        void bw_DoWork2(object sender, DoWorkEventArgs e)
        {
            RunUnsupervisedLearner();
        }
        void bw_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(Convert.ToString(e.Error));
            }
            //all background work has completed and we are going to close the waiting message
            WaitForm.Close();
        }
        private void RunUnsupervisedLearner()
        {
            UnsupervisedLearner UL = new UnsupervisedLearner();
            ResultGroups = UL.run(DeconData);
        }


        //Save All to File button:
        private void SaveAllResultsToFileButton_Click(object sender, EventArgs e)
        {
            DialogResult result = fBDGResults1.ShowDialog();
            if (result == DialogResult.OK)
            {
                String Path = fBDGResults1.SelectedPath;
                String[] Filenames = DeconData.SafeFileNames;
                for (int i = 0; i < ResultGroups.Count(); i++)
                {
                    //String currentPath = Path + "//ResultOf" + Filenames[i];
                    //FileStream FS = new FileStream(currentPath, FileMode.Create, FileAccess.Write);
                    //StreamWriter write = new StreamWriter(FS);
                    String currentPath2 = Path + "//ResultOf" + Filenames[i];
                    FileStream FS2 = new FileStream(currentPath2, FileMode.Create, FileAccess.Write);
                    StreamWriter write2 = new StreamWriter(FS2);
                    GroupingResults.WriteResultsToStream(write2, ResultGroups[i], ElementIDs, MoleculeNames);
                    //write2.Close();
                    ////Write the header
                    //write.Write("Score,MassSpec MW,Compound Key,PeptideSequence,PPM Error,#ofAdduct,#ofCharges,#ofScans,ScanDensity,Avg A:A+2 Error,A:A+2 Ratio,Total Volume,Signal to Noise Ratio,Centroid Scan Error,Centroid Scan,MaxScanNumber,MinScanNumber");
                    //int elementnamecount = 0;
                    //for (int u = 0; u < ResultGroups[i].Count; u++)
                    //{

                    //        foreach (String name in ElementIDs)
                    //        {
                    //            write.Write("," + name);
                    //        }
                    //        elementnamecount = ElementIDs.Count();
                    //        break;

                    //}
                    //write.Write(",Hypothesis MW");
                    //int molenamecount = 0;
                    //foreach (String name in MoleculeNames)
                    //{
                    //    write.Write("," + name);
                    //}
                    //molenamecount = MoleculeNames.Count();

                    //write.WriteLine(",Adduct/Replacement,Adduct Amount,PeptideModification,PeptideMissedCleavage#,#ofGlycanAttachmentToPeptide,StartAA,EndAA");
                    //ParametersForm pr = new ParametersForm();
                    //ParametersForm.ParameterSettings paradata = pr.GetParameters();
                    
                    ////Write the data
                    //for (int u = 0; u < ResultGroups[i].Count; u++)
                    //{
                    //    if (ResultGroups[i][u].PredictedComposition.MassWeight != 0)
                    //    {
                    //        Double MatchingError = 0;
                    //        if (ResultGroups[i][u].PredictedComposition.MassWeight != 0)
                    //        {
                    //            MatchingError = ((ResultGroups[i][u].DeconRow.MonoisotopicMassWeight - ResultGroups[i][u].PredictedComposition.MassWeight) / (ResultGroups[i][u].DeconRow.MonoisotopicMassWeight)) * 1000000;
                    //        }
                    //        write.Write(ResultGroups[i][u].Score + "," + ResultGroups[i][u].DeconRow.MonoisotopicMassWeight + "," + ResultGroups[i][u].PredictedComposition.CompoundComposition + "," + ResultGroups[i][u].PredictedComposition.PepSequence + "," + MatchingError + "," + ResultGroups[i][u].NumModiStates + "," + ResultGroups[i][u].NumChargeStates + "," + ResultGroups[i][u].NumOfScan + "," + ResultGroups[i][u].ScanDensity + "," + ResultGroups[i][u].ExpectedA + "," + (ResultGroups[i][u].DeconRow.MonoisotopicAbundance / (ResultGroups[i][u].DeconRow.MonoisotopicPlus2Abundance + 1)) + "," + ResultGroups[i][u].TotalVolume + "," + ResultGroups[i][u].DeconRow.SignalNoiseRatio + "," + ResultGroups[i][u].CentroidScan + "," + ResultGroups[i][u].DeconRow.ScanNum + "," + ResultGroups[i][u].MaxScanNum + "," + ResultGroups[i][u].MinScanNum);
                    //        for (int s = 0; s < elementnamecount; s++)
                    //        {
                    //            write.Write("," + ResultGroups[i][u].PredictedComposition.ElementAmount[s]);
                    //        }
                    //        write.Write("," + ResultGroups[i][u].PredictedComposition.MassWeight);
                    //        for (int s = 0; s < molenamecount; s++)
                    //        {
                    //            write.Write("," + ResultGroups[i][u].PredictedComposition.eqCounts[s]);
                    //        }
                    //        write.WriteLine("," + ResultGroups[i][u].PredictedComposition.AddRep + "," + ResultGroups[i][u].PredictedComposition.AdductNum + "," + ResultGroups[i][u].PredictedComposition.PepModification + "," + ResultGroups[i][u].PredictedComposition.MissedCleavages + "," + ResultGroups[i][u].PredictedComposition.NumGlycosylations + "," + ResultGroups[i][u].PredictedComposition.StartAA + "," + ResultGroups[i][u].PredictedComposition.EndAA);
                    //    }
                    //    else
                    //    {                            
                    //        write.Write(ResultGroups[i][u].Score + "," + ResultGroups[i][u].DeconRow.MonoisotopicMassWeight + "," + 0 + "" + "," + "," + 0 + "," + ResultGroups[i][u].NumModiStates + "," + ResultGroups[i][u].NumChargeStates + "," + ResultGroups[i][u].NumOfScan + "," + ResultGroups[i][u].ScanDensity + "," + ResultGroups[i][u].ExpectedA + "," + (ResultGroups[i][u].DeconRow.MonoisotopicAbundance / (ResultGroups[i][u].DeconRow.MonoisotopicPlus2Abundance + 1)) + "," + ResultGroups[i][u].TotalVolume + "," + ResultGroups[i][u].DeconRow.SignalNoiseRatio + "," + ResultGroups[i][u].CentroidScan + "," + ResultGroups[i][u].DeconRow.ScanNum + "," + ResultGroups[i][u].MaxScanNum + "," + ResultGroups[i][u].MinScanNum);
                    //        for (int s = 0; s < elementnamecount; s++)
                    //        {
                    //            write.Write("," + 0);
                    //        }
                    //        write.Write("," + 0);
                    //        for (int s = 0; s < molenamecount; s++)
                    //        {
                    //            write.Write("," + 0);
                    //        }
                    //        write.WriteLine("," + "N/A" + "," + 0 + "," + "" + "," + 0 + "," + 0 + "," + 0 + "," + 0);
                    //    }
                    //}
                    //write.Flush();
                    //write.Close();
                    //FS.Close();
                }
            }
        }

        //Controls for combobox1
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResultsGridView.DataSource = this.ToDataTable(ResultGroups, comboBox1.SelectedIndex);
        }




        //Tag2####################################################################################

        //Score radiobutton.
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            button15.Enabled = false;
            button2.Enabled = true;
        }
        //Randomly generated False Data Set radio button.
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            button15.Enabled = true;
            button17.Enabled = true;
            button2.Enabled = false;
        }

        //Add composition File button.
        private void button15_Click(object sender, EventArgs e)
        {
            oFDcposTest.Filter = "cpos files (*.cpos)|*.cpos";
            oFDcposTest.ShowDialog();
        }
        private void oFDcposTest_FileOk(object sender, CancelEventArgs e)
        {
            String fileInfo = "";
            Stream mystream = null;
            try
            {
                if ((mystream = oFDcposTest.OpenFile()) != null)
                {
                    String file = oFDcposTest.FileName;
                    fileInfo += String.Format("{0}\n", file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            mystream.Close();
            GlycanCompositionFileDisplay.Font = new Font("Times New Roman", 12);
            GlycanCompositionFileDisplay.Text = fileInfo;
            button2.Enabled = true;
        }

        //Load protein prospector MS-digest button.
        private void button17_Click(object sender, EventArgs e)
        {
            oFDPPMSD.Filter = "txt files (*.txt)|*.txt";
            oFDPPMSD.ShowDialog();
        }
        private void oFDPPMSD_FileOk(object sender, CancelEventArgs e)
        {
            String fileInfo = "";
            Stream mystream = null;
            try
            {
                if ((mystream = oFDPPMSD.OpenFile()) != null)
                {
                    String file = oFDPPMSD.FileName;
                    fileInfo += String.Format("{0}\n", file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read Protein Prospector MS-digest file from disk. Original error: " + ex.Message);
            }
            mystream.Close();
            richTextBox7.Font = new Font("Times New Roman", 12);
            richTextBox7.Text = fileInfo;
        }
        private List<Peptide> readtablim(String currentpath)
        {
            List<Peptide> data = new List<Peptide>();
            FileStream FS = new FileStream(currentpath, FileMode.Open, FileAccess.Read);
            StreamReader read = new StreamReader(FS);
            //skip title line:
            read.ReadLine();
            while (read.Peek() >= 0)
            {
                Peptide pp = new Peptide();
                String line = read.ReadLine();
                String[] Lines = line.Split('\t');
                pp.PeptideIndex = Convert.ToInt32(Lines[0]);
                pp.Mass = Convert.ToDouble(Lines[1]);
                pp.Charge = Convert.ToInt32(Lines[3]);
                pp.Modifications = Convert.ToString(Lines[4]);
                pp.StartAA = Convert.ToInt32(Lines[5]);
                pp.EndAA = Convert.ToInt32(Lines[6]);
                pp.MissedCleavages = Convert.ToInt32(Lines[7]);
                pp.PreviousAA = Convert.ToString(Lines[8]);
                pp.Sequence = Convert.ToString(Lines[9]);
                pp.NextAA = Convert.ToString(Lines[10]);
                data.Add(pp);
            }
            read.Close();
            FS.Close();

            return data;
        }




        //Evaluate Results Button
        private void button2_Click(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            //This part is used to show the "Please Wait" box while running the code.
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork3);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted3);

            //set lable and your waiting text in this form
            try
            {
                bw.RunWorkerAsync();//this will run the DoWork code at background thread
                WaitForm.ShowDialog();//show the please wait box
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        void bw_DoWork3(object sender, DoWorkEventArgs e)
        {
            stuffToDo3();
        }
        void bw_RunWorkerCompleted3(object sender, RunWorkerCompletedEventArgs e)
        {
            //all background work has completed and we are going to close the waiting message
            WaitForm.Close();
            tabPage2.Show();
        }
        //List used to store data for the datagridview (data of TP rate and FP rate).
        List<DataTable> TF = new List<DataTable>();
        private void stuffToDo3()
        {

            TF.Clear();
            //Clean up the chart for new data.
            ROCCurveChart.Invoke(new MethodInvoker(
            delegate
            {
                foreach (var series in ROCCurveChart.Series)
                {
                    series.Points.Clear();
                }
                ROCCurveChart.Series.Clear();
            }));

            //This is the diagonal Black Line
            ROCCurveChart.Invoke(new MethodInvoker(
             delegate
             {
                 ROCCurveChart.Series.Add("Diagonal");
                 ROCCurveChart.Series["Diagonal"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                 ROCCurveChart.Series["Diagonal"].Color = Color.Black;
                 ROCCurveChart.Series["Diagonal"].Points.AddXY(0, 0);
                 ROCCurveChart.Series["Diagonal"].Points.AddXY(1, 1);

             }));
            if (radioButton1.Checked == true)
            {
                this.scoreBasedGraph();
            }
            if (radioButton2.Checked == true)
            {
                this.DecoyDataROC();
            }

        }



        //This function draws the ROC curve by the TP and FP rates calculated from the score.
        private void scoreBasedGraph()
        {
            CompositionHypothesisTabbedForm comp = new CompositionHypothesisTabbedForm();

            Features FT = new Features();
            String currentpath = Application.StartupPath + "\\FeatureCurrent.fea";
            Feature Fea = FT.readFeature(currentpath);
            //Create the list of random composition hypotesis for testing FDR. 
            //ObtainTrue Position data.
            this.drawGraph(ResultGroups, " ");


            //Checkbox1 is default features.####################################
            List<ResultsGroup>[] DefaultFeature = new List<ResultsGroup>[DeconData.FileNames.Count()];
            String path = Application.StartupPath + "\\FeatureDefault.fea";
            Feature DeFea = FT.readFeature(path);
            if (checkBox1.Checked == true)
            {
                SupervisedLearner SL = new SupervisedLearner();
                List<ResultsGroup>[] TrueDATADefault = SL.EvaluateFeature(DeconData, CompositionHypothesisList, DeFea);

                this.drawGraph(TrueDATADefault, " Default Features");
            }


            //################################################
            //Checkbox2 is unsupervised Learning. It is a bit different from supervised learning, so it is hard-coded here.
            UnsupervisedLearner unsupervisedLearner = new UnsupervisedLearner();
            if (checkBox2.Checked == true)
            {
                List<ResultsGroup>[] USLTrueDATA = unsupervisedLearner.evaluate(DeconData, Fea);
                //ROC curve needs match to perform, so we will use the match list from Supervised learning and apply them to USLDATA.
                for (int i = 0; i < DeconData.FileNames.Count(); i++)
                {
                    ResultGroups[i] = ResultGroups[i].OrderByDescending(a => a.DeconRow.MonoisotopicMassWeight).ToList();
                    USLTrueDATA[i] = USLTrueDATA[i].OrderByDescending(b => b.DeconRow.MonoisotopicMassWeight).ToList();
                    int USllasttruematch = 0;
                    for (int j = 0; j < ResultGroups[i].Count; j++)
                    {
                        if (ResultGroups[i][j].Match == true)
                        {
                            for (int k = USllasttruematch; k < USLTrueDATA[i].Count; k++)
                            {
                                if (USLTrueDATA[i][k].DeconRow.MonoisotopicMassWeight < ResultGroups[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USllasttruematch = k;
                                    break;
                                }
                                if (USLTrueDATA[i][k].DeconRow.MonoisotopicMassWeight == ResultGroups[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USLTrueDATA[i][k].Match = true;
                                    USLTrueDATA[i][k].PredictedComposition = ResultGroups[i][j].PredictedComposition;
                                    USllasttruematch = k + 1;
                                    break;
                                }
                                if (USLTrueDATA[i][k].DeconRow.MonoisotopicMassWeight > ResultGroups[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USLTrueDATA[i][k].Match = false;
                                }
                            }
                        }
                    }
                }

                //Now that both of the data got their matchs, draw the graph
                this.drawGraph(USLTrueDATA, " Unsupervised Learning");
            }
            //#############################unsupervised learning part ends#################

            //Finally populate the Resulting datagridview and the combobox1

            comboBox2.Invoke(new MethodInvoker(delegate
            {
                for (int i = 0; i < TF.Count; i++)
                {
                    comboBox2.Items.Add(TF[i].TableName);
                }
                comboBox2.SelectedIndex = 0;
            }));
            dataGridView2.Invoke(new MethodInvoker(delegate
            {
                dataGridView2.DataSource = TF[0];
            }));
        }

        /// <summary>
        /// This looks sketchy. TODO - Investigate
        /// </summary>
        /// <param name="predictedData"></param>
        /// <param name="status"></param>
        private void drawGraph(List<ResultsGroup>[] predictedData, String status)
        {
            for (int i = 0; i < predictedData.Count(); i++)
            {
                ROCCurveChart.Invoke(new MethodInvoker(
                delegate
                {
                    ROCCurveChart.Series.Add(DeconData.SafeFileNames[i] + status);
                    ROCCurveChart.Series[DeconData.SafeFileNames[i] + status].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    ROCCurveChart.Series[DeconData.SafeFileNames[i] + status].Points.AddXY(0, 0);
                    ROCCurveChart.ChartAreas[0].AxisX.Title = "False Positive Rate";
                    ROCCurveChart.ChartAreas[0].AxisY.Title = "True Positive Rate";
                }));

                DataTable store = new DataTable();
                store.Columns.Add("Cutoff Score", typeof(Double));
                store.Columns.Add("False Positive Rate", typeof(Double));
                store.Columns.Add("True Positive Rate", typeof(Double));
                store.TableName = DeconData.SafeFileNames[i] + status;
                store.Rows.Add(1.001, 0, 0);
                List<ResultsGroup> predictedByScore = predictedData[i].OrderByDescending(a => a.Score).ToList();

                Double TrueTotal = 0.000000000001;
                Double FalseTotal = 0.000000000001;


                for (int j = 0; j < predictedByScore.Count; j++)
                {
                    if (predictedByScore[j].Match == true)
                    {
                        TrueTotal = TrueTotal + predictedByScore[j].Score;
                        //FalseTotal = FalseTotal + 1 - True[j].Score;
                    }
                    else
                    {
                        FalseTotal = FalseTotal + predictedByScore[j].Score;
                        //TrueTotal = TrueTotal + 1 - True[j].Score;
                    }
                }
                Double TruePositive = 0.000000000001;
                Double FalsePositive = 0.000000000001;
                Double stepSize = 0.001;
                int NextStartj = 0;
                Double cutoff = 1;
                Boolean end = false;
                while (cutoff >= 0)
                {
                    for (int j = NextStartj; j < predictedByScore.Count; j++)
                    {
                        if (Convert.ToDouble(predictedByScore[j].Score) < (cutoff))
                        {
                            NextStartj = j;
                            break;
                        }
                        if (predictedByScore[j].Match == true)
                        {
                            TruePositive = TruePositive + Convert.ToDouble(predictedByScore[j].Score);
                            //FalsePositive = FalsePositive + 1 - Convert.ToDouble(True[j].Score);
                        }
                        else
                        {
                            //TruePositive = TruePositive + 1 - Convert.ToDouble(True[j].Score);
                            FalsePositive = FalsePositive + Convert.ToDouble(predictedByScore[j].Score);
                        }
                        if (j == predictedByScore.Count - 1)
                            end = true;
                    }

                    Double TrueRate = 1;
                    Double FalseRate = 1;
                    TrueRate = Math.Round((TruePositive) / (TrueTotal), 6);
                    FalseRate = Math.Round((FalsePositive) / (FalseTotal), 6);

                    ROCCurveChart.Invoke(new MethodInvoker(
                    delegate
                    {
                        ROCCurveChart.Series[DeconData.SafeFileNames[i] + status].Points.AddXY(FalseRate, TrueRate);
                    }));

                    store.Rows.Add(cutoff, FalseRate, TrueRate);

                    cutoff = Math.Round(cutoff - stepSize, 3);
                    if (end)
                        break;
                }
                TF.Add(store);
            }
        }

        //This function draws the ROC curve by the TP and FP rates calculated from the false data set
        private void DecoyDataROC()
        {
            CompositionHypothesisTabbedForm comp = new CompositionHypothesisTabbedForm();
            SupervisedLearner SL = new SupervisedLearner();
            Features featuresMenu = new Features();
            String currentpath = Application.StartupPath + "\\FeatureCurrent.fea";
            Feature currentFeatures = featuresMenu.readFeature(currentpath);
            //Create the list of random composition hypotesis for testing FDR. 
            //ObtainTrue Position data.

            falseDataset fD = new falseDataset();
            List<CompositionHypothesisEntry> falseCH = fD.genFalse(oFDcposTest.FileName, CompositionHypothesisList, oFDPPMSD);
            List<ResultsGroup>[] decoyData = SL.EvaluateFeature(DeconData, falseCH, currentFeatures);

            this.drawGraph(decoyData, " ", 0);


            //Checkbox1 is default features.####################################
            List<ResultsGroup>[] DefaultFeature = new List<ResultsGroup>[DeconData.FileNames.Count()];
            String path = Application.StartupPath + "\\FeatureDefault.fea";
            Feature DeFea = featuresMenu.readFeature(path);
            if (checkBox1.Checked == true)
            {
                List<ResultsGroup>[] decoyDataDefault = SL.EvaluateFeature(DeconData, falseCH, DeFea);
                this.drawGraph(decoyDataDefault, " Default Features", 0);
            }

            //################################################
            //Checkbox2 is unsupervised Learning. It is a bit different from supervised learning, so it is hard-coded here.
            UnsupervisedLearner UL = new UnsupervisedLearner();
            if (checkBox2.Checked == true)
            {
                List<ResultsGroup>[] USLFalseDATA = UL.evaluate(DeconData, currentFeatures);
                //ROC curve needs match to perform, so we will use the match list from Supervised learning and apply them to USLDATA.
                for (int i = 0; i < DeconData.FileNames.Count(); i++)
                {
                    decoyData[i] = decoyData[i].OrderByDescending(a => a.DeconRow.MonoisotopicMassWeight).ToList();
                    USLFalseDATA[i] = USLFalseDATA[i].OrderByDescending(b => b.DeconRow.MonoisotopicMassWeight).ToList();
                    int USllasttruematch = 0;
                    for (int j = 0; j < decoyData[i].Count; j++)
                    {
                        if (decoyData[i][j].Match == true)
                        {
                            for (int k = USllasttruematch; k < USLFalseDATA[i].Count; k++)
                            {
                                if (USLFalseDATA[i][k].DeconRow.MonoisotopicMassWeight < decoyData[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USllasttruematch = k;
                                    break;
                                }
                                if (USLFalseDATA[i][k].DeconRow.MonoisotopicMassWeight == decoyData[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USLFalseDATA[i][k].Match = true;
                                    USLFalseDATA[i][k].PredictedComposition = decoyData[i][j].PredictedComposition;
                                    USllasttruematch = k + 1;
                                    break;
                                }
                                if (USLFalseDATA[i][k].DeconRow.MonoisotopicMassWeight > decoyData[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USLFalseDATA[i][k].Match = false;
                                }
                            }
                        }
                    }
                }

                //Now that both of the data got their matchs, draw the graph
                this.drawGraph(USLFalseDATA, " Unsupervised Learning", 0);
            }
            //#############################unsupervised learning part ends#################

            //Finally populate the Resulting datagridview and the combobox1

            comboBox2.Invoke(new MethodInvoker(delegate
            {
                for (int i = 0; i < TF.Count; i++)
                {
                    comboBox2.Items.Add(TF[i].TableName);
                }
                comboBox2.SelectedIndex = 0;
            }));
            dataGridView2.Invoke(new MethodInvoker(delegate
            {
                dataGridView2.DataSource = TF[0];
            }));
        }
        private void drawGraph(List<ResultsGroup>[] FalseData, String status, int isfalse)
        {
            for (int i = 0; i < FalseData.Count(); i++)
            {
                ROCCurveChart.Invoke(new MethodInvoker(
                delegate
                {
                    ROCCurveChart.Series.Add(DeconData.SafeFileNames[i] + status);
                    ROCCurveChart.Series[DeconData.SafeFileNames[i] + status].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    ROCCurveChart.Series[DeconData.SafeFileNames[i] + status].Points.AddXY(0, 0);
                    ROCCurveChart.ChartAreas[0].AxisX.Title = "False Positive Rate";
                    ROCCurveChart.ChartAreas[0].AxisY.Title = "True Positive Rate";
                }));

                DataTable store = new DataTable();
                store.Columns.Add("Cutoff Score", typeof(Double));
                store.Columns.Add("False Positive Rate", typeof(Double));
                store.Columns.Add("True Positive Rate", typeof(Double));
                store.TableName = DeconData.SafeFileNames[i] + status;
                store.Rows.Add(1.001, 0, 0);

                List<ResultsGroup> False = FalseData[i].OrderByDescending(b => b.Score).ToList();

                Double TrueTotal = 0.000000000001;
                Double FalseTotal = 0.000000000001;


                for (int j = 0; j < False.Count; j++)
                {
                    if (False[j].Match == true)
                    {
                        if (False[j].PredictedComposition.IsDecoy == false)
                            FalseTotal = FalseTotal + False[j].Score;
                        if (False[j].PredictedComposition.IsDecoy == true)
                            TrueTotal = TrueTotal + False[j].Score;
                    }
                }
                Double TruePositive = 0.000000000001;
                Double FalsePositive = 0.000000000001;
                Double Step = 0.001;
                int NextStartjf = 0;
                Double cutoff = 1;
                Boolean endTrue = false;
                Boolean endFalse = false;
                while (cutoff >= 0)
                {

                    for (int j = NextStartjf; j < False.Count; j++)
                    {
                        if (endFalse)
                            break;
                        if (Convert.ToDouble(False[j].Score) < (cutoff))
                        {
                            NextStartjf = j;
                            break;
                        }
                        if (False[j].Match == true)
                        {
                            if (False[j].PredictedComposition.IsDecoy == false)
                                FalsePositive = FalsePositive + False[j].Score;
                            if (False[j].PredictedComposition.IsDecoy == true)
                                TruePositive = TruePositive + False[j].Score;
                        }
                        else
 
                        if (j == False.Count - 1)
                            endFalse = true;
                    }
                    Double TrueRate = 1;
                    Double FalseRate = 1;
                    TrueRate = Math.Round((TruePositive) / (TrueTotal), 6);
                    FalseRate = Math.Round((FalsePositive) / (FalseTotal), 6);

                    ROCCurveChart.Invoke(new MethodInvoker(
                    delegate
                    {
                        ROCCurveChart.Series[DeconData.SafeFileNames[i] + status].Points.AddXY(FalseRate, TrueRate);
                    }));

                    store.Rows.Add(cutoff, FalseRate, TrueRate);

                    //for some reason, if we simplay do minus 0.001 to cutoff for 4-5 times, it will gain lots of decimal points with 9s in them. So, we're doing it this way:
                    cutoff = Math.Round(cutoff - Step, 3);
                    if (endTrue && endFalse)
                        break;
                }
                TF.Add(store);
            }

        }


        //Change the datagridview when the combobox selection is changed
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView2.DataSource = TF[comboBox2.SelectedIndex];
        }


        //Tag4################################################################
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawTotalVolumeHistogram();
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            DrawTotalVolumeHistogram();
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            DrawTotalVolumeHistogram();
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            DrawTotalVolumeHistogram();
        }




        //Tag3##################################################################
        //Add Result Files Button
        public bool Multiselect { get; set; }
        private void button3_Click(object sender, EventArgs e)
        {
            oFDResults.Filter = "csv files (*.csv)|*.csv";
            oFDResults.Multiselect = true;
            oFDResults.ShowDialog();
        }
        private void oFDResults_FileOk(object sender, CancelEventArgs e)
        {
            String fileInfo = "";
            Stream mystream = null;
            try
            {
                if ((mystream = oFDResults.OpenFile()) != null)
                {
                    foreach (String file in oFDResults.FileNames)
                    {
                        fileInfo += String.Format("{0}\n", file);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            mystream.Close();
            richTextBox2.Font = new Font("Times New Roman", 12);
            richTextBox2.Text = fileInfo;
            button9.Enabled = true;
        }

        //Remove all files button
        private void button8_Click(object sender, EventArgs e)
        {
            oFDResults.FileName = String.Empty;
            richTextBox2.Text = String.Empty;
            button9.Enabled = false;
        }

        //Generate Combined Result button#########################################
        private void button9_Click(object sender, EventArgs e)
        {
            GroupingResults GR = new GroupingResults();
            List<ResultsGroup>[] results = new List<ResultsGroup>[oFDResults.FileNames.Count()];
            for (int i = 0; i < oFDResults.FileNames.Count(); i++)
            {
                results[i] = GR.ReadResultsFromFile(oFDResults.FileNames[i]);
            }
            List<ResultsGroup> Answer = new List<ResultsGroup>();
            Answer = GR.CombineResults(results);
            List<ResultsGroup>[] Ans = { Answer };
            DataTable DT = ToDataTable(Ans, 0);
            dataGridView3.DataSource = DT;
        }

        //Save to File button
        private void button4_Click(object sender, EventArgs e)
        {
            sFDCR.Filter = "csv files (*.csv)|*.csv";
            sFDCR.ShowDialog();
        }
        private void sFDCR_FileOk(object sender, CancelEventArgs e)
        {
            saveDGV3ToFile(sFDCR.FileName);
        }



        //Other Functions
        //Function that turns the AllFinalResult data into DataTable
        private DataTable ToDataTable(List<ResultsGroup>[] AFR, Int32 index)
        {
            //write.Write("Score,MassSpec MW,Compound Key,PPM Error,Hypothesis MW,#ofModificationStates,#ofCharges,#ofScans,Scan Density,Avg A:A+2 Error,A:A+2 Ratio,Total Volume,Signal to Noise Ratio,Centroid Scan Error,Centroid Scan,MaxScanNumber,MinScanNumber,C,H,N,O,S,P");

            List<ResultsGroup> current = AFR[index];
            DataTable output = new DataTable();
            output.Columns.Add("Score", typeof(Double));
            output.Columns.Add("MassSpec MW", typeof(Double));
            output.Columns.Add("Compositions", typeof(String));
            output.Columns.Add("PeptideSequence", typeof(String));
            output.Columns.Add("PPM Error", typeof(Double));
            output.Columns.Add("#ofAdduct", typeof(Double));
            output.Columns.Add("#ofCharges", typeof(Double));
            output.Columns.Add("#ofScans", typeof(Double));
            output.Columns.Add("ScanDensity", typeof(Double));
            output.Columns.Add("Avg A:A+2 Error", typeof(Double));
            output.Columns.Add("A:A+2 Ratio", typeof(Double));
            output.Columns.Add("Total Volume", typeof(Double));
            output.Columns.Add("Signal to Noise Ratio", typeof(Double));
            output.Columns.Add("Centroid Scan Error", typeof(Double));
            output.Columns.Add("Centroid Scan", typeof(Double));
            output.Columns.Add("MaxScanNumber", typeof(Double));
            output.Columns.Add("MinScanNumber", typeof(Double));

            int ElementCount = 0;
            ElementCount = ElementIDs.Count();

            foreach (String name in ElementIDs)
            {
                output.Columns.Add(name, typeof(Int32));                        
            }


            output.Columns.Add("Hypothesis MW", typeof(Double));
            int moleculeCount = 0;
            moleculeCount = MoleculeNames.Count();
            foreach (String name in MoleculeNames)
            {
                output.Columns.Add(name, typeof(Int32));                
            }

            output.Columns.Add("Adduct/Replacement", typeof(String));
            output.Columns.Add("Adduct Amount", typeof(Int32));
            output.Columns.Add("PeptideModification", typeof(String));
            output.Columns.Add("PeptideMissedCleavage#", typeof(Int32));
            output.Columns.Add("#ofGlycanAttachmentToPeptide", typeof(Int32));
            output.Columns.Add("StartAA", typeof(Int32));
            output.Columns.Add("EndAA", typeof(Int32));
            output.Columns.Add("Protein ID", typeof(String));
            foreach (ResultsGroup ch in current)
            {
                if (ch.PredictedComposition.MassWeight != 0)
                {
                    DataRow ab = output.NewRow();
                    Double MatchingError = 0;
                    if (ch.PredictedComposition.MassWeight != 0)
                    {
                        MatchingError = ((ch.DeconRow.MonoisotopicMassWeight - ch.PredictedComposition.MassWeight)/(ch.DeconRow.MonoisotopicMassWeight)) * 1000000 ;
                    }
                    ab[0] = ch.Score;
                    ab[1] = ch.DeconRow.MonoisotopicMassWeight;
                    ab[2] = ch.PredictedComposition.CompoundComposition;
                    ab[3] = ch.PredictedComposition.PepSequence;
                    ab[4] = MatchingError;                    
                    ab[5] = ch.NumModiStates;
                    ab[6] = ch.NumChargeStates;
                    ab[7] = ch.NumOfScan;
                    ab[8] = ch.ScanDensity;
                    ab[9] = ch.ExpectedA;
                    ab[10] = (ch.DeconRow.MonoisotopicAbundance / (ch.DeconRow.MonoisotopicPlus2Abundance + 1));
                    ab[11] = ch.TotalVolume;
                    ab[12] = ch.DeconRow.SignalNoiseRatio;
                    ab[13] = ch.CentroidScan;
                    ab[14] = ch.DeconRow.ScanNum;
                    ab[15] = ch.MaxScanNum;
                    ab[16] = ch.MinScanNum;
                    int sh = 17;
                    for (int s = 0; s < ElementCount; s++)
                    {
                        ab[sh + s] = ch.PredictedComposition.ElementAmount[s];
                    }
                    ab[sh + ElementCount] = ch.PredictedComposition.MassWeight;
                    for (int s = 0; s < MoleculeNames.Count(); s++)
                    {
                        ab[sh + ElementCount + 1 + s] = ch.PredictedComposition.eqCounts[s];
                    }
                    ab[sh + ElementCount + 1 + moleculeCount] = ch.PredictedComposition.AddRep;
                    ab[sh + ElementCount + 1 + moleculeCount + 1] = ch.PredictedComposition.AdductNum;
                    ab[sh + ElementCount + 1 + moleculeCount + 2] = ch.PredictedComposition.PepModification;
                    ab[sh + ElementCount + 1 + moleculeCount + 3] = ch.PredictedComposition.MissedCleavages;
                    ab[sh + ElementCount + 1 + moleculeCount + 4] = ch.PredictedComposition.NumGlycosylations;
                    ab[sh + ElementCount + 1 + moleculeCount + 5] = ch.PredictedComposition.StartAA;
                    ab[sh + ElementCount + 1 + moleculeCount + 6] = ch.PredictedComposition.EndAA;
                    ab[sh + ElementCount + 1 + moleculeCount + 7] = ch.PredictedComposition.ProteinID;
                    output.Rows.Add(ab);
                }
                else
                {
                    DataRow ab = output.NewRow();
                    Double MatchingError = 0;
                    if (ch.PredictedComposition.MassWeight != 0)
                    {
                        MatchingError = ch.PredictedComposition.MassWeight - ch.DeconRow.MonoisotopicMassWeight;
                    }
                    ab[0] = ch.Score;
                    ab[1] = ch.DeconRow.MonoisotopicMassWeight;
                    ab[2] = ch.PredictedComposition.CompoundComposition;
                    ab[3] = ch.PredictedComposition.PepSequence;
                    ab[4] = MatchingError;
                    ab[5] = ch.NumModiStates;
                    ab[6] = ch.NumChargeStates;
                    ab[7] = ch.NumOfScan;
                    ab[8] = ch.ScanDensity;
                    ab[9] = ch.ExpectedA;
                    ab[10] = (ch.DeconRow.MonoisotopicAbundance / (ch.DeconRow.MonoisotopicPlus2Abundance + 1));
                    ab[11] = ch.TotalVolume;
                    ab[12] = ch.DeconRow.SignalNoiseRatio;
                    ab[13] = ch.CentroidScan;
                    ab[14] = ch.DeconRow.ScanNum;
                    ab[15] = ch.MaxScanNum;
                    ab[16] = ch.MinScanNum;
                    int sh = 17;
                    for (int s = 0; s < ElementCount; s++)
                    {
                        ab[sh + s] = 0;
                    }
                    ab[sh + ElementCount] = ch.PredictedComposition.MassWeight;
                    for (int s = 0; s < MoleculeNames.Count(); s++)
                    {
                        ab[sh + ElementCount + 1 + s] = 0;
                    }
                    ab[sh + ElementCount + 1 + moleculeCount] = "N/A";
                    ab[sh + ElementCount + 1 + moleculeCount + 1] = 0;
                    ab[sh + ElementCount + 1 + moleculeCount + 2] = "";
                    ab[sh + ElementCount + 1 + moleculeCount + 3] = 0;
                    ab[sh + ElementCount + 1 + moleculeCount + 4] = 0;
                    ab[sh + ElementCount + 1 + moleculeCount + 5] = 0;
                    ab[sh + ElementCount + 1 + moleculeCount + 6] = 0;
                    ab[sh + ElementCount + 1 + moleculeCount + 7] = "?";
                    output.Rows.Add(ab);
                }
            }
            return output;
        }

        //This function saves the datagridview3 (combine glycan data) to a csv file
        private void saveDGV3ToFile(String currentPath)
        {
            //First, prepare the writers.
            FileStream FS = new FileStream(currentPath, FileMode.Create, FileAccess.Write);
            StreamWriter write = new StreamWriter(FS);

            //Second,  loop from the datagridview and write to csv file.
            for (int j = 0; j < this.dataGridView3.ColumnCount; ++j)
            {
                String cell = this.dataGridView3.Columns[j].HeaderText;
                if (j == (this.dataGridView3.ColumnCount - 1))
                {
                    write.WriteLine(cell);
                }
                else
                {
                    write.Write(cell + ",");
                }
            }
            int iii = 0;
            int jjj = 0;
            try
            {
                for (int i = 0; i < this.dataGridView3.RowCount; ++i)
                {
                    iii = 1;
                    for (int j = 0; j < this.dataGridView3.ColumnCount; ++j)
                    {
                        jjj = j;
                        var cell = this.dataGridView3.Rows[i].Cells[j].Value;
                        if (j == (this.dataGridView3.ColumnCount - 1))
                        {
                            write.WriteLine(cell);
                        }
                        else
                        {
                            write.Write(cell + ",");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("c:" + Convert.ToString(iii) + " j:" + Convert.ToString(jjj) + " error " + ex);
            }

            write.Flush();
            write.Close();
            FS.Close();
        }

        //This function draws the percent total volume histogram, depending on the checkboxes.
        private void DrawTotalVolumeHistogram()
        {
            //This is matched compounds only.
            if (checkBox4.Checked == true)
            {
                foreach (var series in TotalVolumeHistogramChart.Series)
                {
                    series.Points.Clear();
                }
                TotalVolumeHistogramChart.Series.Clear();
                List<ResultsGroup> dataset = new List<ResultsGroup>();
                dataset = ResultGroups[VolumeHistogramFileSelector.SelectedIndex];
                dataset = dataset.OrderByDescending(a => a.PredictedComposition.MassWeight).ToList();
                //Getting total of total volumes
                Double TotalTV = 0;
                for (int i = 0; i < dataset.Count(); i++)
                {
                    TotalTV = TotalTV + dataset[i].TotalVolume;
                }
                List<Double> percentages = new List<Double>();
                List<string> rownames = new List<string>();
                //Limit number of data to n if checkBox 3 is checked. Else, no limit.
                if (checkBox3.Checked == true && dataset.Count() > numericUpDown1.Value)
                {
                    dataset = dataset.OrderByDescending(a => a.Score).ToList();
                    List<ResultsGroup> newdataset = new List<ResultsGroup>();
                    int count = 0;
                    int i7 = 0;
                    while (count < numericUpDown1.Value)
                    {
                        if (dataset[i7].Match == true)
                        {
                            newdataset.Add(dataset[i7]);
                            i7++;
                            count++;
                            continue;
                        }
                        i7++;
                        if (i7 >= dataset.Count())
                            break;
                    }
                    //output to chart
                    newdataset = newdataset.OrderByDescending(a => a.PredictedComposition.MassWeight).ToList();
                    for (int i = 0; i < newdataset.Count(); i++)
                    {
                        if (newdataset[i].PredictedComposition.MassWeight == 0)
                            break;
                        percentages.Add((newdataset[i].TotalVolume / TotalTV) * 100);
                        rownames.Add(Convert.ToString(newdataset[i].PredictedComposition.CompoundComposition));

                    }
                }
                else
                {
                    for (int i = 0; i < dataset.Count(); i++)
                    {
                        if (dataset[i].PredictedComposition.MassWeight == 0)
                            break;

                        percentages.Add((dataset[i].TotalVolume / TotalTV) * 100);
                        rownames.Add(Convert.ToString(dataset[i].PredictedComposition.CompoundComposition));

                    }
                }
                if (dataset.Count() != 0)
                {
                    String titleName = "";
                    dataset = dataset.OrderByDescending(a => a.PredictedComposition.CompoundComposition).ToList();
                    if (MoleculeNames.Count() == 0)
                    {
                        titleName = "GlycanCompositions: N/A";
                    }
                    else
                    {
                        titleName = "GlycanCompositions: ";
                        titleName = titleName + String.Join(", ", MoleculeNames);
                    }
                    TotalVolumeHistogramChart.Series.Add(DeconData.FileNames[VolumeHistogramFileSelector.SelectedIndex]);
                    TotalVolumeHistogramChart.ChartAreas[0].AxisX.Title = titleName;
                    for (int i = 0; i < percentages.Count(); i++)
                    {
                        TotalVolumeHistogramChart.Series[DeconData.FileNames[VolumeHistogramFileSelector.SelectedIndex]].Points.AddXY(rownames[i], percentages[i]);
                    }
                }
            }
            //This includes unmatched compounds:
            else
            {
                foreach (var series in TotalVolumeHistogramChart.Series)
                {
                    series.Points.Clear();
                }
                TotalVolumeHistogramChart.Series.Clear();
                List<ResultsGroup> dataset = new List<ResultsGroup>();
                dataset = ResultGroups[VolumeHistogramFileSelector.SelectedIndex];
                //Getting total of total volumes
                Double TotalTV = 0;
                for (int i = 0; i < dataset.Count(); i++)
                {
                    TotalTV = TotalTV + dataset[i].TotalVolume;
                }
                List<Double> percentages = new List<Double>();
                List<string> rownames = new List<string>();
                //Limit number of data to n if checkBox 3 is checked. Else, no limit.
                if (checkBox3.Checked == true && dataset.Count() > numericUpDown1.Value)
                {
                    dataset = dataset.OrderBy(a => a.Score).ToList();
                    List<ResultsGroup> newdataset = new List<ResultsGroup>();
                    int count = 0;
                    int i7 = 0;
                    while (count < numericUpDown1.Value)
                    {
                            newdataset.Add(dataset[i7]);
                            i7++;
                            count++;
                            continue;
                    }
                    //output to chart
                    newdataset = newdataset.OrderBy(a => a.DeconRow.ScanNum).ToList();
                    for (int i = 0; i < newdataset.Count(); i++)
                    {
                        percentages.Add((newdataset[i].TotalVolume / TotalTV) * 100);
                        rownames.Add(Convert.ToString(newdataset[i].DeconRow.ScanNum));

                    }
                }
                else
                {
                    dataset = dataset.OrderBy(a => a.DeconRow.ScanNum).ToList();
                    for (int i = 0; i < dataset.Count(); i++)
                    {
                        percentages.Add((dataset[i].TotalVolume / TotalTV) * 100);
                        rownames.Add(Convert.ToString(dataset[i].DeconRow.ScanNum));
                    }
                }
                String titleName = "Scan Number";
                TotalVolumeHistogramChart.Series.Add(DeconData.FileNames[VolumeHistogramFileSelector.SelectedIndex]);
                TotalVolumeHistogramChart.ChartAreas[0].AxisX.Title = titleName;
                for (int i = 0; i < percentages.Count(); i++)
                {
                    TotalVolumeHistogramChart.Series[DeconData.FileNames[VolumeHistogramFileSelector.SelectedIndex]].Points.AddXY(rownames[i], percentages[i]);
                }
            }
        }




    }


}
