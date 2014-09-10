using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Accord.Statistics;
using Accord.Statistics.Analysis;


namespace GlycReSoft
{
    public partial class HomeScreen : Form
    {
        public HomeScreen()
        {
                InitializeComponent();
        }


        //Tag1#################################################################################################

        //This is the button "Initialize Parameters." Clicking on it shows the "initializeParameters.cs" form
        private void button11_Click(object sender, EventArgs e)
        {
            (new Features()).Show();
        }
        //This is the Save Parameter button. Clicking on it shows saveFileDialog1.
        private void button14_Click(object sender, EventArgs e)
        {
            parameters savepara = new parameters();
            savepara.saveParameters();
        }

        //This is the lod dialog for load Parameters.
        private void button22_Click(object sender, EventArgs e)
        {
            parameters loadpara = new parameters();
            loadpara.loadParameters();
        }

        //Link to Decon Tools
        private void button10_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://omics.pnl.gov/software/DeconTools.php");
        }
        //About button
        private void button2_Click_1(object sender, EventArgs e)
        {
            AboutBox1 ab1 = new AboutBox1();
            ab1.ShowDialog();
        }
        //Combine Results
        private void button4_Click(object sender, EventArgs e)
        {
            FormerResults FR = new FormerResults();
            FR.ShowDialog();            
        }

        //This is the "Add LC/MS data file" button. It reads the decontools file and store them into variables
        //Multiselect method allows oFDDeconData to load multiple files at the same time.
        public bool Multiselect { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            oFDDeconData.Filter = "csv files (*.csv)|*.csv";
            oFDDeconData.FilterIndex = 2;
            oFDDeconData.RestoreDirectory = true;
            oFDDeconData.Multiselect = true;
            oFDDeconData.ShowDialog();

            //openFileDialog1_FileOk method takes care of stuffs after the user clicks "ok" in the openFileDialog.
        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            String fileInfo = "";
            GetDeconData gDD = new GetDeconData();
            Stream mystream = null;
            try
            {
                if ((mystream = oFDDeconData.OpenFile()) != null)
                {
                    if (gDD.checkFile(oFDDeconData.FileNames))
                    {
                        foreach (String file in oFDDeconData.FileNames)
                        {
                            fileInfo += String.Format("{0}\n", file);
                        }
                        richTextBox1.Font = new Font("Times New Roman", 12);
                        richTextBox1.Text = fileInfo;
                        button13.Enabled = true;
                        button12.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            mystream.Close();
        }

        //This is the "Remove all files" button.
        private void button6_Click(object sender, EventArgs e)
        {
            oFDDeconData.FileName = String.Empty;
            richTextBox1.Font = new Font("Times New Roman", 12);
            richTextBox1.Text = String.Empty;
            button12.Enabled = false;
            button3.Enabled = false;
            button13.Enabled = false;
        }

        //This is the "Unsupervised Learning" button.
        private void button12_Click(object sender, EventArgs e)
        {
            if (oFDDeconData != null)
            {
                tag1Result t1r = new tag1Result(oFDDeconData);
                t1r.Show();
            }
            else
            {
                MessageBox.Show("Error: no file is specified. Try to remove all files and add them again.");
            }
        }

        //This is the "Load GlycanCompositions Hypothesis" button
        private void button13_Click(object sender, EventArgs e)
        {
            oFDGenerator.Filter = "csv files (*.csv)|*.csv";
            oFDGenerator.FilterIndex = 2;
            oFDGenerator.RestoreDirectory = true;
            oFDGenerator.ShowDialog();
        }
        private void oFDGenerator_FileOk(object sender, CancelEventArgs e)
        {
            composition comp = new composition();
            button3.Enabled = true;
        }
        //This is the "Supervised Learning" Button. It performs supervised learning by calling the supervisedLearning class.
        private void button3_Click(object sender, EventArgs e)
        {
            if (oFDDeconData != null)
            {
                tag1Result t1r = new tag1Result(oFDDeconData, oFDGenerator.FileName);
                t1r.Show();
            }
            else
            {
                MessageBox.Show("Error: no file is specified. Try to remove all files and add them again.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            composition opencompo = new composition();
            opencompo.Show();
        }




        //This is the Apply Changes button on top of unsupervised learning. It applies the Parameters.
        private void button24_Click(object sender, EventArgs e)
        {
            parameters para = new parameters();
            para.applypara();
        }


        private void button19_Click(object sender, EventArgs e)
        {
            (new parameters()).Show();
        }

        //Help button
        private void button7_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(this, Application.StartupPath + "\\help.chm");
        }

        /// <summary>
        /// Launches the Tandem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalyzeTandemMSGlycopeptideLaunchButton_Click(object sender, EventArgs e)
        {
            var tandemMSUI = new GlycReSoft.TandemMSGlycopeptideGUI.TandemGlycopeptideAnalysis();
            tandemMSUI.Show();
        }




    }

}
