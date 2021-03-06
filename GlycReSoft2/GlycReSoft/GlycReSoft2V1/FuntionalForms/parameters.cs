﻿using System;
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
    public partial class ParametersForm : Form
    {
        /**
        Note: From left to right, the numbers in a *.para file means:
        Data Noise Threshold 0, Minimum Socre Threshold 1, Match Error (E_M) 2, Molecular Weight Lower Bound 3,
        Molecular Weight Upper Bound 4, Grouping Error (E_G) 5, Adduct Tolerance (E_A) 6, Minimum Number of Scans 7.
        **/
        public ParametersForm()
        {
            //First, read the Parameters from file.
            String ParaPath = Application.StartupPath + "\\Parameters.para";
            FileStream FS = new FileStream(ParaPath, FileMode.Open, FileAccess.Read);
            StreamReader ReadPara = new StreamReader(FS);
            String Line = ReadPara.ReadLine();
            ReadPara.Close();
            FS.Close();
            String[] Param = Line.Split(',');

            //Then, open the window and print the numbers in the textboxes
            InitializeComponent();
            numericUpDown2.Text = Param[0];
            numericUpDown1.Text = Param[1];
            numericUpDown3.Text = Param[2];
            numericUpDown4.Text = Param[3];
            numericUpDown5.Text = Param[4];
            numericUpDown6.Text = Param[5];
            numericUpDown7.Text = Param[6];
            numericUpDown8.Text = Param[7];

        }
        //This is the load button.
        private void button2_Click(object sender, EventArgs e)
        {
            this.LoadParameters();
        }
        //This is the save button.
        private void button3_Click(object sender, EventArgs e)
        {
            this.SaveParameters();
        }

        //This is the Reset to Default button.
        private void button1_Click(object sender, EventArgs e)
        {
            String ParaPath = Application.StartupPath + "\\Parameters.para";
            String DParaPath = Application.StartupPath + "\\parametersDefault.para";
            File.Copy(DParaPath, ParaPath, true);
            FileStream FS = new FileStream(ParaPath, FileMode.Open, FileAccess.Read);
            StreamReader ReadPara = new StreamReader(FS);
            String Line = ReadPara.ReadLine();
            FS.Close();
            String[] Param = Line.Split(',');
            numericUpDown2.Text = Param[0];
            numericUpDown1.Text = Param[1];
            numericUpDown3.Text = Param[2];
            numericUpDown4.Text = Param[3];
            numericUpDown5.Text = Param[4];
            numericUpDown6.Text = Param[5];
            numericUpDown7.Text = Param[6];
            numericUpDown8.Text = Param[7];
        }

        //This is the OK button. It applies the changes and close the window.
        private void button4_Click(object sender, EventArgs e)
        {
            this.ApplyParameters();
            this.Close();
        }

        public void ApplyParameters()
        {
            //First, read the 2 Parameters NOT on the Parameters form.
            String[] Param = new String[8];
            Param[0] = numericUpDown2.Text;
            Param[1] = numericUpDown1.Text;
            Param[2] = numericUpDown3.Text;
            Param[3] = numericUpDown4.Text;
            Param[4] = numericUpDown5.Text;
            Param[5] = numericUpDown6.Text;
            Param[6] = numericUpDown7.Text;
            Param[7] = numericUpDown8.Text;
            //Next, write them to the Parameters.para file.
            String ParaPath = Application.StartupPath + "\\Parameters.para";
            FileStream FS2 = new FileStream(ParaPath, FileMode.Create, FileAccess.Write);
            StreamWriter WritePara = new StreamWriter(FS2);
            WritePara.Write(Param[0]);
            for (int i = 1; i < 8; ++i)
            {
                WritePara.Write(',' + Param[i]);
            }
            WritePara.Flush();
            WritePara.Close();
            FS2.Close();
        }

        //This is used to save parameter data. It can be used by other classes.
        public void SaveParameters()
        {
            sFDPara.Filter = "para files (*.para)|*.para";
            sFDPara.ShowDialog();
        }
        private void sFDPara_FileOk_1(object sender, CancelEventArgs e)
        {
            //First, read the data:
            String ParaPath = Application.StartupPath + "\\Parameters.para";
            String[] Param = new String[8];
            Param[0] = numericUpDown2.Text;
            Param[1] = numericUpDown1.Text;
            Param[2] = numericUpDown3.Text;
            Param[3] = numericUpDown4.Text;
            Param[4] = numericUpDown5.Text;
            Param[5] = numericUpDown6.Text;
            Param[6] = numericUpDown7.Text;
            Param[7] = numericUpDown8.Text;

            //Second, save them to the file.
            String path = sFDPara.FileName;
            FileStream FS2 = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter WritePara = new StreamWriter(FS2);
            WritePara.Write(Param[0]);
            for (int i = 1; i < 8; ++i)
            {
                WritePara.Write(',' + Param[i]);
            }
            WritePara.Flush();
            WritePara.Close();
            FS2.Close();
        }

        //This loads parameter data. It can be used by another class.
        public void LoadParameters()
        {
            oFDPara.Filter = "para files (*.para)|*.para";
            oFDPara.ShowDialog();
        }
        private void oFDPara_FileOk(object sender, CancelEventArgs e)
        {
            //First, read the Parameters from file.
            String ParaPath = oFDPara.FileName;
            FileStream FS = new FileStream(ParaPath, FileMode.Open, FileAccess.Read);
            StreamReader ReadPara = new StreamReader(FS);
            String Line = ReadPara.ReadLine();
            FS.Close();
            String[] Param = Line.Split(',');

            //Then, open the window and print the numbers in the textboxes
            numericUpDown2.Text = Param[0];
            numericUpDown1.Text = Param[1];
            numericUpDown3.Text = Param[2];
            numericUpDown4.Text = Param[3];
            numericUpDown5.Text = Param[4];
            numericUpDown6.Text = Param[5];
            numericUpDown7.Text = Param[6];
            numericUpDown8.Text = Param[7];

            //Next, write them to the Parameters.para file.
            String ParaPath2 = Application.StartupPath + "\\Parameters.para";
            FileStream FS2 = new FileStream(ParaPath2, FileMode.Create, FileAccess.Write);
            StreamWriter WritePara = new StreamWriter(FS2);
            WritePara.Write(Param[0]);
            for (int i = 1; i < 8; ++i)
            {
                WritePara.Write(',' + Param[i]);
            }
            WritePara.Close();
            FS2.Close();
        }

        //This is a class to store parameter data.
        public class ParameterSettings
        {
            public Double DataNoiseTheshold { get; set; }
            public Double MinScoreThreshold { get; set; }
            public Double MatchErrorEM { get; set; }
            public Int32 MolecularWeightLowerBound { get; set; }
            public Int32 MolecularWeightUpperBound { get; set; }
            public Double GroupingErrorEG { get; set; }
            public Double AdductToleranceEA { get; set; }
            public Int32 MinScanNumber { get; set; }
        }

        //This is for other classes to read parameter data.
        public ParameterSettings GetParameters()
        {
            String ParaPath = Application.StartupPath + "\\Parameters.para";
            FileStream FS = new FileStream(ParaPath, FileMode.Open, FileAccess.Read);
            StreamReader ReadPara = new StreamReader(FS);
            String Line = ReadPara.ReadLine();
            FS.Close();
            String[] Param = Line.Split(',');
            ParameterSettings paradata = new ParameterSettings();
            paradata.DataNoiseTheshold = Convert.ToDouble(Param[0]);
            paradata.MinScoreThreshold = Convert.ToDouble(Param[1]);
            paradata.MatchErrorEM = Convert.ToDouble(Param[2]);
            paradata.MolecularWeightLowerBound = Convert.ToInt32(Param[3]);
            paradata.MolecularWeightUpperBound = Convert.ToInt32(Param[4]);
            paradata.GroupingErrorEG = Convert.ToDouble(Param[5]);
            paradata.AdductToleranceEA = Convert.ToDouble(Param[6]);
            paradata.MinScanNumber = Convert.ToInt32(Param[7]);
            return paradata;
        }

    }
}
