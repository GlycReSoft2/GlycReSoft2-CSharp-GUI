using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GlycReSoft
{
    public partial class FormerResults : Form
    {
        private static List<ResultsGroup>[] AllFinalResults;
        public FormerResults()
        {
            InitializeComponent();
        }
        //Tag1##################################################################
        //Add Result Files Button
        public bool Multiselect { get; set; }
        private void button3_Click(object sender, EventArgs e)
        {
            oFDResults.Filter = "csv files (*.csv) tab delimited file (*.txt)|*.csv;*.txt";
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
            button2.Enabled = true;
            GroupingResults GR = new GroupingResults();
            List<ResultsGroup>[] ResultStore = new List<ResultsGroup>[oFDResults.FileNames.Count()];
            for (int i = 0; i < oFDResults.FileNames.Count(); i++)
            {
                ResultStore[i] = GR.ReadResultsFromFile(oFDResults.FileNames[i]);
            }
            AllFinalResults = ResultStore;
        }


        //Remove all files button
        private void button8_Click(object sender, EventArgs e)
        {
            oFDResults.FileName = String.Empty;
            richTextBox2.Text = String.Empty;
            button9.Enabled = false;
            button2.Enabled = false;
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
            DataTable DT = toDataTable(Ans, 0);
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

        //Tag2#####################################################################
        //Evaluate Results Button
        PleaseWait msgForm = new PleaseWait();
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
                msgForm.ShowDialog();//show the please wait box
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
            msgForm.Close();
            tabPage2.Show();
        }
        //List used to store data for the datagridview (data of TP rate and FP rate).
        List<DataTable> TF = new List<DataTable>();
        private void stuffToDo3()
        {

            TF.Clear();
            //Clean up the chart for new data.
            chart1.Invoke(new MethodInvoker(
            delegate
            {
                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                }
                chart1.Series.Clear();
            }));

            //This is the diagonal Black Line
            chart1.Invoke(new MethodInvoker(
             delegate
             {
                 chart1.Series.Add("Diagonal");
                 chart1.Series["Diagonal"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                 chart1.Series["Diagonal"].Color = Color.Black;
                 chart1.Series["Diagonal"].Points.AddXY(0, 0);
                 chart1.Series["Diagonal"].Points.AddXY(1, 1);

             }));
            if (radioButton1.Checked == true)
            {
                this.scoreBasedGraph();
            }
        }

        //comboBox control
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView2.DataSource = TF[comboBox2.SelectedIndex];
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
            this.drawGraph(AllFinalResults, "");

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
        private void drawGraph(List<ResultsGroup>[] TrueDATA, String status)
        {
            for (int i = 0; i < TrueDATA.Count(); i++)
            {
                chart1.Invoke(new MethodInvoker(
                delegate
                {
                    chart1.Series.Add(oFDResults.SafeFileNames[i] + status);
                    chart1.Series[oFDResults.SafeFileNames[i] + status].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    chart1.Series[oFDResults.SafeFileNames[i] + status].Points.AddXY(0, 0);
                    chart1.ChartAreas[0].AxisX.Title = "False Positive Rate";
                    chart1.ChartAreas[0].AxisY.Title = "True Positive Rate";
                }));

                DataTable store = new DataTable();
                store.Columns.Add("Cutoff Score", typeof(Double));
                store.Columns.Add("False Positive Rate", typeof(Double));
                store.Columns.Add("True Positive Rate", typeof(Double));
                store.TableName = oFDResults.SafeFileNames[i] + status;
                store.Rows.Add(1.001, 0, 0);
                List<ResultsGroup> True = TrueDATA[i].OrderByDescending(a => a.Score).ToList();

                Double TrueTotal = 0.000000000001;
                Double FalseTotal = 0.000000000001;


                for (int j = 0; j < True.Count; j++)
                {
                    if (True[j].Match == true)
                    {
                        TrueTotal = TrueTotal + True[j].Score;
                        //FalseTotal = FalseTotal + 1 - True[j].Score;
                    }
                    else
                    {
                        FalseTotal = FalseTotal + True[j].Score;
                        //TrueTotal = TrueTotal + 1 - True[j].Score;
                    }
                }
                Double TruePositive = 0.000000000001;
                Double FalsePositive = 0.000000000001;
                Double Step = 0.001;
                int NextStartj = 0;
                Double cutoff = 1;
                Boolean end = false;
                while (cutoff >= 0)
                {
                    for (int j = NextStartj; j < True.Count; j++)
                    {
                        if (Convert.ToDouble(True[j].Score) < (cutoff))
                        {
                            NextStartj = j;
                            break;
                        }
                        if (True[j].Match == true)
                        {
                            TruePositive = TruePositive + Convert.ToDouble(True[j].Score);
                            //FalsePositive = FalsePositive + 1 - Convert.ToDouble(True[j].Score);
                        }
                        else
                        {
                            //TruePositive = TruePositive + 1 - Convert.ToDouble(True[j].Score);
                            FalsePositive = FalsePositive + Convert.ToDouble(True[j].Score);
                        }
                        if (j == True.Count - 1)
                            end = true;
                    }

                    Double TrueRate = 1;
                    Double FalseRate = 1;
                    TrueRate = Math.Round((TruePositive) / (TrueTotal), 6);
                    FalseRate = Math.Round((FalsePositive) / (FalseTotal), 6);

                    chart1.Invoke(new MethodInvoker(
                    delegate
                    {
                        chart1.Series[oFDResults.SafeFileNames[i] + status].Points.AddXY(FalseRate, TrueRate);
                    }));

                    store.Rows.Add(cutoff, FalseRate, TrueRate);

                    //for some reason, if we simplay do minus 0.001 to cutoff for 4-5 times, it will gain lots of decimal points with 9s in them. So, we're doing it this way:
                    cutoff = Math.Round(cutoff - Step, 3);
                    if (end)
                        break;
                }
                TF.Add(store);
            }
        }


        //Other Functions
        //Function that turns the AllFinalResult data into DataTable
        private DataTable toDataTable(List<ResultsGroup>[] AFR, Int32 index)
        {
            //write.Write("Score,MassSpec MW,Compound Key,PPM Error,Hypothesis MW,#ofModificationStates,#ofCharges,#ofScans,Scan Density,Avg A:A+2 Error,A:A+2 Ratio,Total Volume,Signal to Noise Ratio,Centroid Scan Error,Centroid Scan,MaxScanNumber,MinScanNumber,C,H,N,O,S,P");
            List<string> elementIDs = new List<string>();
            List<string> molename = new List<string>();
            for (int i = 0; i < AFR[0].Count(); i++)
            {
                if (AFR[0][i].PredictedComposition.ElementNames.Count > 0)
                {
                    for (int j = 0; j < AFR[0][i].PredictedComposition.ElementNames.Count(); j++)
                    {
                        elementIDs.Add(AFR[0][i].PredictedComposition.ElementNames[j]);
                    }
                    for (int j = 0; j < AFR[0][i].PredictedComposition.MoleculeNames.Count(); j++)
                    {
                        molename.Add(AFR[0][i].PredictedComposition.MoleculeNames[j]);
                    }
                    break;
                }
            }
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

            int elementCount = 0;

            foreach (String name in elementIDs)
            {
                output.Columns.Add(name, typeof(Int32));                        
            }
            elementCount = elementIDs.Count();



            output.Columns.Add("Hypothesis MW", typeof(Double));
            int moleculeCount = 0;


            foreach (String name in molename)
            {
                output.Columns.Add(name, typeof(Int32));
            }
            moleculeCount = molename.Count();

            output.Columns.Add("Adduct/Replacement", typeof(String));
            output.Columns.Add("Adduct Amount", typeof(Int32));
            output.Columns.Add("PeptideModification", typeof(String));
            output.Columns.Add("PeptideMissedCleavage#", typeof(Int32));
            output.Columns.Add("#ofGlycanAttachmentToPeptide", typeof(Int32));
            output.Columns.Add("TotalVolumeSD",typeof(double));
            output.Columns.Add("RelativeTotalVolume", typeof(double));
            output.Columns.Add("RelativeTotalVolumeSD", typeof(double));
            for (int i = 0; i < current[0].ListOfOriginalTotalVolumes.Count(); i++)
            {
                output.Columns.Add("File" + Convert.ToString(i+1) + "_TotalVolume", typeof(Double));
            }

            foreach (ResultsGroup ch in current)
            {
                if (ch.PredictedComposition.MassWeight != 0)
                {
                    DataRow ab = output.NewRow();
                    Double MatchingError = 0;
                    if (ch.PredictedComposition.MassWeight != 0)
                        MatchingError = ((ch.DeconRow.MonoisotopicMassWeight - ch.PredictedComposition.MassWeight) / (ch.DeconRow.MonoisotopicMassWeight)) * 1000000;
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
                    ab[10] = ch.AvgAA2List.Average();
                    ab[11] = ch.TotalVolume;
                    ab[12] = ch.AvgSigNoise;
                    ab[13] = ch.CentroidScan;
                    ab[14] = ch.DeconRow.ScanNum;
                    ab[15] = ch.MaxScanNum;
                    ab[16] = ch.MinScanNum;
                    int sh = 17;
                    for (int i = 0; i < elementCount; i++)
                    {
                        ab[sh] = ch.PredictedComposition.ElementAmount[i];
                        sh++;
                    }
                    ab[sh] = ch.PredictedComposition.MassWeight;
                    sh++;
                    for (int s = 0; s < moleculeCount; s++)
                    {
                        ab[sh + s] = ch.PredictedComposition.eqCounts[s];
                    }
                    ab[sh + moleculeCount] = ch.PredictedComposition.AddRep;
                    ab[sh + moleculeCount + 1] = ch.PredictedComposition.AdductNum;
                    ab[sh + moleculeCount + 2] = ch.PredictedComposition.PepModification;
                    ab[sh + moleculeCount + 3] = ch.PredictedComposition.MissedCleavages;
                    ab[sh + moleculeCount + 4] = ch.PredictedComposition.NumGlycosylations;
                    ab[sh + moleculeCount + 5] = ch.TotalVolumeSD;
                    ab[sh + moleculeCount + 6] = ch.RelativeTotalVolume;
                    ab[sh + moleculeCount + 7] = ch.RelativeTotalVolumeSD;
                    for (int i = 0; i < ch.ListOfOriginalTotalVolumes.Count(); i++)
                    {
                        ab[sh + moleculeCount + 8 + i] = ch.ListOfOriginalTotalVolumes[i];
                    }
                    output.Rows.Add(ab);
                }
                else
                {
                    DataRow ab = output.NewRow();
                    Double MatchingError = 0;
                    if (ch.PredictedComposition.MassWeight != 0)
                        MatchingError = ((ch.DeconRow.MonoisotopicMassWeight - ch.PredictedComposition.MassWeight) / ch.DeconRow.MonoisotopicMassWeight) * 1000000;
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
                    ab[10] = ch.AvgAA2List.Average();
                    ab[11] = ch.TotalVolume;
                    ab[12] = ch.AvgSigNoise;
                    ab[13] = ch.CentroidScan;
                    ab[14] = ch.DeconRow.ScanNum;
                    ab[15] = ch.MaxScanNum;
                    ab[16] = ch.MinScanNum;
                    int sh = 17;
                    for (int i = 0; i < elementCount; i++)
                    {
                        ab[sh] = 0;
                        sh++;
                    }
                    ab[sh] = ch.PredictedComposition.MassWeight;
                    sh++;
                    for (int s = 0; s < moleculeCount; s++)
                    {
                        ab[sh + s] = 0;
                    }
                    ab[sh + moleculeCount] = "N/A";
                    ab[sh + moleculeCount + 1] = 0;
                    ab[sh + moleculeCount + 2] = "";
                    ab[sh + moleculeCount + 3] = 0;
                    ab[sh + moleculeCount + 4] = 0;
                    ab[sh + moleculeCount + 5] = ch.TotalVolumeSD;
                    ab[sh + moleculeCount + 6] = ch.RelativeTotalVolume;
                    ab[sh + moleculeCount + 7] = ch.RelativeTotalVolumeSD;
                    for (int i = 0; i < ch.ListOfOriginalTotalVolumes.Count(); i++)
                    {
                        ab[sh + moleculeCount + 8 + i] = ch.ListOfOriginalTotalVolumes[i];
                    }
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


    }
}
