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
using MathParserTK;


namespace GlycReSoft
{
    public partial class Features : Form
    {
        public Features()
        {
            InitializeComponent();
        }
        //#####################################Generate Features part######################################################
        //This is the "Add Training set data Files" button.
        public bool Multiselect { get; set; }
        private void button12_Click(object sender, EventArgs e)
        {
            oFDTrain.Filter = "csv files (*.csv)|*.csv";
            oFDTrain.RestoreDirectory = true;
            oFDTrain.Multiselect = true;
            oFDTrain.ShowDialog();
        }
        private void oFDTrain_FileOk(object sender, CancelEventArgs e)
        {
            String fileInfo = "";
            Stream mystream = null;
            try
            {
                if ((mystream = oFDTrain.OpenFile()) != null)
                {
                    foreach (String file in oFDTrain.FileNames)
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
            richTextBox3.Font = new Font("Times New Roman", 12);
            richTextBox3.Text = fileInfo;
            button3.Enabled = true;
        }

        //"Clear file list" button.
        private void button11_Click(object sender, EventArgs e)
        {
            oFDTrain.FileName = String.Empty;
            oFDComposition.FileName = String.Empty;
            richTextBox3.Text = String.Empty;
            richTextBox1.Text = String.Empty;
            button1.Enabled = false;
            button3.Enabled = false;
        }

        //Load composition hypothesis button for generate feature.
        private void button3_Click(object sender, EventArgs e)
        {
            oFDComposition.Filter = "csv files (*.csv)|*.csv";
            oFDComposition.RestoreDirectory = true;
            oFDComposition.ShowDialog();
        }
        private void oFDComposition_FileOk(object sender, CancelEventArgs e)
        {
            String fileInfo = "";
            Stream mystream = null;
            try
            {
                if ((mystream = oFDComposition.OpenFile()) != null)
                {
                    String file = oFDComposition.FileName;
                    fileInfo += String.Format("{0}\n", file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            mystream.Close();
            richTextBox1.Text = String.Empty;
            richTextBox1.Font = new Font("Times New Roman", 12);
            richTextBox1.Text = fileInfo;
            button1.Enabled = true;
        }

        //Set Features Button
        PleaseWait msgForm = new PleaseWait();
        private void button1_Click(object sender, EventArgs e)
        {
            //This part is used to show the "Please Wait" box while running the code.
            //The codes for functions of this button are all inside the bw_doWork function.
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

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
            MessageBox.Show("Features are set.");

        }
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //Obtain Feature
            Feature Ans = new Feature();
            SupervisedLearner SL = new SupervisedLearner();
            CompositionHypothesisTabbedForm comp = new CompositionHypothesisTabbedForm();
            List<CompositionHypothesisEntry> comhy = comp.getCompHypo(oFDComposition.FileName);
            Ans = SL.obtainFeatures(oFDTrain, comhy);

            //Write Features to File
            String path = Application.StartupPath + "\\FeatureCurrent.fea";
            FileStream FS = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter Write1 = new StreamWriter(FS);
            Write1.Write(Ans.Initial + "," + Ans.numChargeStates + "," + Ans.ScanDensity + "," + Ans.numModiStates + "," + Ans.totalVolume + "," + Ans.ExpectedA + "," + Ans.CentroidScan + "," + Ans.numOfScan + "," + Ans.avgSigNoise);
            Write1.Flush();
            Write1.Close();
            FS.Close();
        }
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(Convert.ToString(e.Error));
            }
            //all background work has completed and we are going to close the waiting message
            msgForm.Close();
        }




        //Generate GlycanCompositions Hypothesis Button
        private void button5_Click(object sender, EventArgs e)
        {
            CompositionHypothesisTabbedForm opencompo = new CompositionHypothesisTabbedForm();
            opencompo.Show();
        }

        //OK button
        private void button16_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //###############################Evaluate Features part############################################

        // "Add Testing Data Set" Button
        private void button14_Click(object sender, EventArgs e)
        {
            oFDTest.Filter = "csv files (*.csv)|*.csv";
            oFDTest.RestoreDirectory = true;
            oFDTest.Multiselect = true;
            oFDTest.ShowDialog();
        }
        private void oFDTest_FileOk(object sender, CancelEventArgs e)
        {
            String fileInfo = "";
            Stream mystream = null;
            try
            {
                if ((mystream = oFDTest.OpenFile()) != null)
                {
                    foreach (String file in oFDTest.FileNames)
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
            richTextBox4.Font = new Font("Times New Roman", 12);
            richTextBox4.Text = fileInfo;
            button6.Enabled = true;

        }



        //Clear all Files button.
        private void button13_Click(object sender, EventArgs e)
        {
            richTextBox4.Text = String.Empty;
            richTextBox2.Text = String.Empty;
            richTextBox5.Text = String.Empty;

            button6.Enabled = false;
            button2.Enabled = false;
            button7.Enabled = false;
            button15.Enabled = false;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;

            oFDTest.FileName = String.Empty;
            oFDCompositionTest.FileName = String.Empty;
            oFDAddFeatureFiles.FileName = String.Empty;
            oFDcposTest.FileName = String.Empty;
        }

        //Load GlycanCompositions Hypothesis button.
        private void button6_Click(object sender, EventArgs e)
        {
            oFDCompositionTest.Filter = "csv files (*.csv)|*.csv";
            oFDCompositionTest.FilterIndex = 2;
            oFDCompositionTest.RestoreDirectory = true;
            oFDCompositionTest.ShowDialog();
        }
        private void oFDCompositionTest_FileOk(object sender, CancelEventArgs e)
        {
            String fileInfo = "";
            Stream mystream = null;
            try
            {
                if ((mystream = oFDCompositionTest.OpenFile()) != null)
                {
                    String file = oFDCompositionTest.FileName;
                    fileInfo += String.Format("{0}\n", file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            mystream.Close();
            richTextBox2.Text = String.Empty;
            richTextBox2.Font = new Font("Times New Roman", 12);
            richTextBox2.Text = fileInfo;
            button7.Enabled = true;
        }

        //Reset Feautres to Default button
        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure? Any unsaved features will be permanently deleted.","Reset Features", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    String defaultpath = Application.StartupPath + "\\FeatureDefault.fea";
                    String currentpath = Application.StartupPath + "\\FeatureCurrent.fea";
                    File.Copy(defaultpath, currentpath, true);
                }
                catch (Exception mainex)
                {
                    MessageBox.Show("Error in creating Features file. Error:" + mainex.Message);
                }
            }
        }

        //"Save Current Features to File" button
        private void button4_Click(object sender, EventArgs e)
        {
            sFDObtainFeatures.Filter = "fea files (*.fea)|*.fea";
            sFDObtainFeatures.ShowDialog();
        }
        private void sFDObtainFeatures_FileOk(object sender, CancelEventArgs e)
        {
            Feature Ans = readFeature();
            String path = sFDObtainFeatures.FileName;
            FileStream FS = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter Write1 = new StreamWriter(FS);
            Write1.Write(Ans.Initial + "," + Ans.numChargeStates + "," + Ans.ScanDensity + "," + Ans.numModiStates + "," + Ans.totalVolume + "," + Ans.ExpectedA + "," + Ans.CentroidScan + "," + Ans.numOfScan + "," + Ans.avgSigNoise);
            Write1.Flush();
            Write1.Close();
            FS.Close();
        }

        //"Load Features From File" button.
        private void button9_Click(object sender, EventArgs e)
        {
            oFDLoadFeature.Filter = "fea files (*.fea)|*.fea";
            oFDLoadFeature.ShowDialog();
        }
        private void oFDLoadFeature_FileOk(object sender, CancelEventArgs e)
        {
            //Read Feature from file
            Feature Ans = new Feature();
            String path = oFDLoadFeature.FileName;
            FileStream FS = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader read1 = new StreamReader(FS);
            String line = read1.ReadLine();
            String[] Lines = line.Split(',');
            Ans.Initial = Convert.ToDouble(Lines[0]);
            Ans.numChargeStates = Convert.ToDouble(Lines[1]);
            Ans.ScanDensity = Convert.ToDouble(Lines[2]);
            Ans.numModiStates = Convert.ToDouble(Lines[3]);
            Ans.totalVolume = Convert.ToDouble(Lines[4]);
            Ans.ExpectedA = Convert.ToDouble(Lines[5]);
            Ans.CentroidScan = Convert.ToDouble(Lines[6]);
            Ans.numOfScan = Convert.ToDouble(Lines[7]);
            Ans.avgSigNoise = Convert.ToDouble(Lines[8]);
            read1.Close();
            FS.Close();

            //Write feature to storage.
            String currentpath = Application.StartupPath + "\\FeatureCurrent.fea";
            FileStream FS2 = new FileStream(currentpath, FileMode.Create, FileAccess.Write);
            StreamWriter Write1 = new StreamWriter(FS2);
            Write1.Write(Ans.Initial + "," + Ans.numChargeStates + "," + Ans.ScanDensity + "," + Ans.numModiStates + "," + Ans.totalVolume + "," + Ans.ExpectedA + "," + Ans.CentroidScan + "," + Ans.numOfScan + "," + Ans.avgSigNoise);
            Write1.Flush();
            Write1.Close();
            FS.Close();
        }

        //Add Feature Files button
        private void button7_Click(object sender, EventArgs e)
        {
            oFDAddFeatureFiles.Filter = "fea files (*.fea)|*.fea";
            oFDAddFeatureFiles.RestoreDirectory = true;
            oFDAddFeatureFiles.ShowDialog();
        }
        private void oFDAddFeatureFiles_FileOk(object sender, CancelEventArgs e)
        {
            String fileInfo = "";
            Stream mystream = null;
            try
            {
                if ((mystream = oFDAddFeatureFiles.OpenFile()) != null)
                {
                    String file = oFDAddFeatureFiles.FileName;
                    fileInfo += String.Format("{0}\n", file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            mystream.Close();
            richTextBox5.Font = new Font("Times New Roman", 12);
            richTextBox5.Text = fileInfo;
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            button2.Enabled = true;
        }

        //Edit Parameters Button
        private void button10_Click(object sender, EventArgs e)
        {
            ParametersForm pd = new ParametersForm();
            pd.Show();
        }

        //Add composition file button
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
            richTextBox6.Font = new Font("Times New Roman", 12);
            richTextBox6.Text = fileInfo;
            button2.Enabled = true;
        }

        //Evaluate by Score radio button.
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            button15.Enabled = false;
            button2.Enabled = true;
        }
        //Evaluate by false data set radio button
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            button15.Enabled = true;
            button17.Enabled = true;
            Stream mystream = null;
            Boolean Enableit = false;
            try
            {
                if ((mystream = oFDcposTest.OpenFile()) != null)
                    Enableit = true;
                else
                    Enableit = false;
            }
            catch { }
            button2.Enabled = Enableit;
        }

        //Evaluate Features button.###############
        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
            comboBox1.Items.Clear();
            //This part is used to show the "Please Wait" box while running the code.
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork2);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted2);

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
        void bw_DoWork2(object sender, DoWorkEventArgs e)
        {
            stuffToDo2();
        }
        void bw_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            //all background work has completed and we are going to close the waiting message
            msgForm.Close();
        }
        //List used to store data for the datagridview (data of TP rate and FP rate).
        List<DataTable> TF = new List<DataTable>();
        private void stuffToDo2()
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
            else if (radioButton2.Checked == true)
            {
                this.FDSBasedGraph();
            }

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
        public List<Peptide> readtablim(String currentpath)
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





        //Tag2########################################################
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = TF[comboBox1.SelectedIndex];
        }




        //Other Functions#######################################################
        //This function loads the current feature and output it.
        public Feature readFeature()
        {
            Feature Ans = new Feature();
            try
            {
                String path = Application.StartupPath + "\\FeatureCurrent.fea";
                FileStream FS = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader read1 = new StreamReader(FS);
                String line = read1.ReadLine();
                String[] Lines = line.Split(',');
                Ans.Initial = Convert.ToDouble(Lines[0]);
                Ans.numChargeStates = Convert.ToDouble(Lines[1]);
                Ans.ScanDensity = Convert.ToDouble(Lines[2]);
                Ans.numModiStates = Convert.ToDouble(Lines[3]);
                Ans.totalVolume = Convert.ToDouble(Lines[4]);
                Ans.ExpectedA = Convert.ToDouble(Lines[5]);
                Ans.CentroidScan = Convert.ToDouble(Lines[6]);
                Ans.numOfScan = Convert.ToDouble(Lines[7]);
                Ans.avgSigNoise = Convert.ToDouble(Lines[8]);
                read1.Close();
                FS.Close();
            }
            catch
            {
                try
                {
                    String path = Application.StartupPath + "\\FeatureDefault.fea";
                    FileStream FS2 = new FileStream(path, FileMode.Open, FileAccess.Read);
                    StreamReader read2 = new StreamReader(FS2);
                    String line = read2.ReadLine();
                    String[] Lines = line.Split(',');
                    Ans.Initial = Convert.ToDouble(Lines[0]);
                    Ans.numChargeStates = Convert.ToDouble(Lines[1]);
                    Ans.ScanDensity = Convert.ToDouble(Lines[2]);
                    Ans.numModiStates = Convert.ToDouble(Lines[3]);
                    Ans.totalVolume = Convert.ToDouble(Lines[4]);
                    Ans.ExpectedA = Convert.ToDouble(Lines[5]);
                    Ans.CentroidScan = Convert.ToDouble(Lines[6]);
                    Ans.numOfScan = Convert.ToDouble(Lines[7]);
                    Ans.avgSigNoise = Convert.ToDouble(Lines[8]);
                    read2.Close();
                    FS2.Close();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Unable to load feature files. Error code:" + ex);
                }
            }

            return Ans;
        }
        //overrides if there is already a path.
        public Feature readFeature(String path)
        {
            Feature Ans = new Feature();
            try
            {
                FileStream FS = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader read1 = new StreamReader(FS);
                String line = read1.ReadLine();
                String[] Lines = line.Split(',');
                Ans.Initial = Convert.ToDouble(Lines[0]);
                Ans.numChargeStates = Convert.ToDouble(Lines[1]);
                Ans.ScanDensity = Convert.ToDouble(Lines[2]);
                Ans.numModiStates = Convert.ToDouble(Lines[3]);
                Ans.totalVolume = Convert.ToDouble(Lines[4]);
                Ans.ExpectedA = Convert.ToDouble(Lines[5]);
                Ans.CentroidScan = Convert.ToDouble(Lines[6]);
                Ans.numOfScan = Convert.ToDouble(Lines[7]);
                Ans.avgSigNoise = Convert.ToDouble(Lines[8]);
                read1.Close();
                FS.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Unable to load feature files. Error code:" + ex);
            }

            return Ans;
        }

        //This function draws the ROC curve by the TP and FP rates calculated from the score.
        private void scoreBasedGraph()
        {
            CompositionHypothesisTabbedForm comp = new CompositionHypothesisTabbedForm();
            SupervisedLearner SL = new SupervisedLearner();
            Feature Fea = readFeature(oFDAddFeatureFiles.FileName);
            //Create the list of random composition hypotesis for testing FDR. 
            //ObtainTrue Position data.
            List<CompositionHypothesisEntry> CH = comp.getCompHypo(oFDCompositionTest.FileName);
            List<ResultsGroup>[] TrueDATA = SL.EvaluateFeature(oFDTest, CH, Fea);

            this.drawGraph(TrueDATA, " Loaded Features");


            //Checkbox1 is default features.####################################
            List<ResultsGroup>[] DefaultFeature = new List<ResultsGroup>[oFDTest.FileNames.Count()];
            String path = Application.StartupPath + "\\FeatureDefault.fea";
            Feature DeFea = readFeature(path);
            if (checkBox1.Checked == true)
            {
                List<ResultsGroup>[] TrueDATADefault = SL.EvaluateFeature(oFDTest, CH, DeFea);

                this.drawGraph(TrueDATADefault, " Default Features");
            }


            //################################################
            //Checkbox2 is unsupervised Learning. It is a bit different from supervised learning, so it is hard-coded here.
            
            if (checkBox2.Checked == true)
            {
                UnsupervisedLearner UL = new UnsupervisedLearner();
                List<ResultsGroup>[] USLTrueDATA = UL.evaluate(oFDTest, Fea);
                //ROC curve needs match to perform, so we will use the match list from Supervised learning and apply them to USLDATA.
                for (int i = 0; i < oFDTest.FileNames.Count(); i++)
                {
                    USLTrueDATA[i] = USLTrueDATA[i].OrderByDescending(b => b.DeconRow.MonoisotopicMassWeight).ToList();
                    int USllasttruematch = 0;
                    for (int j = 0; j < TrueDATA[i].Count; j++)
                    {
                        if (TrueDATA[i][j].Match == true)
                        {
                            for (int k = USllasttruematch; k < USLTrueDATA[i].Count; k++)
                            {
                                if (USLTrueDATA[i][k].DeconRow.MonoisotopicMassWeight < TrueDATA[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USllasttruematch = k;
                                    break;
                                }
                                if (USLTrueDATA[i][k].DeconRow.MonoisotopicMassWeight == TrueDATA[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USLTrueDATA[i][k].Match = true;
                                    USLTrueDATA[i][k].PredictedComposition = TrueDATA[i][j].PredictedComposition;
                                    USllasttruematch = k + 1;
                                    break;
                                }
                                if (USLTrueDATA[i][k].DeconRow.MonoisotopicMassWeight > TrueDATA[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USLTrueDATA[i][k].Match = false;
                                }
                            }
                        }
                    }
                }

                //Now that both of the data got their matchs, draw the graph
                this.drawGraph(USLTrueDATA, " Unsupervised Learning + Loaded Features");
            }
            //#############################unsupervised learning part ends#################

            //Finally populate the Resulting datagridview and the combobox1

            comboBox1.Invoke(new MethodInvoker(delegate
            {
                for (int i = 0; i < TF.Count; i++)
                {
                    comboBox1.Items.Add(TF[i].TableName);
                }
                comboBox1.SelectedIndex = 0;
            }));
            dataGridView1.Invoke(new MethodInvoker(delegate
            {
                dataGridView1.DataSource = TF[0];
            }));
        }
        private void drawGraph(List<ResultsGroup>[] TrueDATA, String status)
        {
            for (int i = 0; i < TrueDATA.Count(); i++)
            {
                chart1.Invoke(new MethodInvoker(
                delegate
                {
                    chart1.Series.Add(oFDTest.SafeFileNames[i] + status);
                    chart1.Series[oFDTest.SafeFileNames[i] + status].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    chart1.Series[oFDTest.SafeFileNames[i] + status].Points.AddXY(0, 0);
                    chart1.ChartAreas[0].AxisX.Title = "False Positive Rate";
                    chart1.ChartAreas[0].AxisY.Title = "True Positive Rate";
                }));

                DataTable store = new DataTable();
                store.Columns.Add("Cutoff Score", typeof(Double));
                store.Columns.Add("False Positive Rate", typeof(Double));
                store.Columns.Add("True Positive Rate", typeof(Double));
                store.TableName = oFDTest.SafeFileNames[i] + status;
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
                        chart1.Series[oFDTest.SafeFileNames[i] + status].Points.AddXY(FalseRate, TrueRate);
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

        //This function draws the ROC curve by the TP and FP rates calculated from the false data set
        private void FDSBasedGraph()
        {
            CompositionHypothesisTabbedForm comp = new CompositionHypothesisTabbedForm();
            SupervisedLearner SL = new SupervisedLearner();
            Feature Fea = readFeature(oFDAddFeatureFiles.FileName);
            //Create the list of random composition hypotesis for testing FDR. 
            //ObtainTrue Position data.
            List<CompositionHypothesisEntry> CH = comp.getCompHypo(oFDCompositionTest.FileName);
            falseDataset fD = new falseDataset();
            List<CompositionHypothesisEntry> falseCH = fD.genFalse(oFDcposTest.FileName, CH, oFDPPMSD);
            List<ResultsGroup>[] FalseDATA = SL.EvaluateFeature(oFDTest, falseCH, Fea);

            this.drawGraph(FalseDATA, " Loaded Features", 0);


            //Checkbox1 is default features.####################################
            List<ResultsGroup>[] DefaultFeature = new List<ResultsGroup>[oFDTest.FileNames.Count()];
            String path = Application.StartupPath + "\\FeatureDefault.fea";
            Feature DeFea = readFeature(path);
            if (checkBox1.Checked == true)
            {
                List<ResultsGroup>[] FalseDATADefault = SL.EvaluateFeature(oFDTest, falseCH, DeFea);
                this.drawGraph(FalseDATADefault, " Default Features", 0);
            }

            //################################################
            //Checkbox2 is unsupervised Learning. It is a bit different from supervised learning, so it is hard-coded here.
            UnsupervisedLearner UL = new UnsupervisedLearner();
            if (checkBox2.Checked == true)
            {
                List<ResultsGroup>[] USLFalseDATA = UL.evaluate(oFDTest, Fea);
                //ROC curve needs match to perform, so we will use the match list from Supervised learning and apply them to USLDATA.
                for (int i = 0; i < oFDTest.FileNames.Count(); i++)
                {
                    FalseDATA[i] = FalseDATA[i].OrderByDescending(a => a.DeconRow.MonoisotopicMassWeight).ToList();
                    USLFalseDATA[i] = USLFalseDATA[i].OrderByDescending(b => b.DeconRow.MonoisotopicMassWeight).ToList();
                    int USllasttruematch = 0;
                    for (int j = 0; j < FalseDATA[i].Count; j++)
                    {
                        if (FalseDATA[i][j].Match == true)
                        {
                            for (int k = USllasttruematch; k < USLFalseDATA[i].Count; k++)
                            {
                                if (USLFalseDATA[i][k].DeconRow.MonoisotopicMassWeight < FalseDATA[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USllasttruematch = k;
                                    break;
                                }
                                if (USLFalseDATA[i][k].DeconRow.MonoisotopicMassWeight == FalseDATA[i][j].DeconRow.MonoisotopicMassWeight)
                                {
                                    USLFalseDATA[i][k].Match = true;
                                    USLFalseDATA[i][k].PredictedComposition = FalseDATA[i][j].PredictedComposition;
                                    USllasttruematch = k + 1;
                                    break;
                                }
                                if (USLFalseDATA[i][k].DeconRow.MonoisotopicMassWeight > FalseDATA[i][j].DeconRow.MonoisotopicMassWeight)
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

            comboBox1.Invoke(new MethodInvoker(delegate
            {
                for (int i = 0; i < TF.Count; i++)
                {
                    comboBox1.Items.Add(TF[i].TableName);
                }
                comboBox1.SelectedIndex = 0;
            }));
            dataGridView1.Invoke(new MethodInvoker(delegate
            {
                dataGridView1.DataSource = TF[0];
            }));
        }
        private void drawGraph(List<ResultsGroup>[] FalseDATA, String status, int isfalse)
        {
            for (int i = 0; i < FalseDATA.Count(); i++)
            {
                chart1.Invoke(new MethodInvoker(
                delegate
                {
                    chart1.Series.Add(oFDTest.SafeFileNames[i] + status);
                    chart1.Series[oFDTest.SafeFileNames[i] + status].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    chart1.Series[oFDTest.SafeFileNames[i] + status].Points.AddXY(0, 0);
                    chart1.ChartAreas[0].AxisX.Title = "False Positive Rate";
                    chart1.ChartAreas[0].AxisY.Title = "True Positive Rate";
                }));

                DataTable store = new DataTable();
                store.Columns.Add("Cutoff Score", typeof(Double));
                store.Columns.Add("False Positive Rate", typeof(Double));
                store.Columns.Add("True Positive Rate", typeof(Double));
                store.TableName = oFDTest.SafeFileNames[i] + status;
                store.Rows.Add(1.001, 0, 0);

                List<ResultsGroup> False = FalseDATA[i].OrderByDescending(b => b.Score).ToList();

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

                    chart1.Invoke(new MethodInvoker(
                    delegate
                    {
                        chart1.Series[oFDTest.SafeFileNames[i] + status].Points.AddXY(FalseRate, TrueRate);
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




    }



    public class Feature
    {
        public Double Initial;
        public Double numChargeStates;
        public Double ScanDensity;
        public Double numModiStates;
        public Double totalVolume;
        public Double ExpectedA;
        public Double CentroidScan;
        public Double numOfScan;
        public Double avgSigNoise;
    }
}
