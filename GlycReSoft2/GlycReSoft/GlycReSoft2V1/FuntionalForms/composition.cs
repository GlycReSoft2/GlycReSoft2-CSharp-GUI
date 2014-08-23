using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathParserTK;
using System.Text.RegularExpressions;
using System.Linq;
using System.Resources;
using GlycReSoft.CompositionHypothesis;

namespace GlycReSoft
{
    public partial class composition : Form
    {
        private static DataTable theComhypoOnTab2;
        public composition()
        {
            InitializeComponent();
            //Initialize the additional rules table.
            DataGridViewTextBoxColumn Formula = new DataGridViewTextBoxColumn();
            Formula.HeaderText = "Formula";
            Formula.Name = "Formula";
            Formula.ValueType = typeof(String);
            this.dataGridView3.Columns.Add(Formula);
            DataGridViewComboBoxColumn Relationship = new DataGridViewComboBoxColumn();
            Relationship.HeaderText = "Relationship";
            Relationship.Name = "Relationship";
            Relationship.Items.AddRange("=", ">", "<", "≥", "≤", "≠");
            this.dataGridView3.Columns.Add(Relationship);
            DataGridViewTextBoxColumn constraint = new DataGridViewTextBoxColumn();
            constraint.HeaderText = "constraint";
            constraint.Name = "constraint";
            constraint.ValueType = typeof(String);
            this.dataGridView3.Columns.Add(constraint);

            //Populate the tables.
            //Get the path of the source file.
            String currentpath = Application.StartupPath + "\\compositionsCurrent.cpos";
            this.populateForm(currentpath);
            periodicTable PT = new periodicTable();
            List<string> elements = PT.getElements();
            List<string> elementsInHeader = new List<string>();
            for (int j = 2; j < this.dataGridView1.ColumnCount -2; ++j)
            {
                elementsInHeader.Add(dataGridView1.Columns[j].HeaderText);
            }
            foreach (string ele in elements)
            {
                bool eleNotInTable = true;
                foreach (string element in elementsInHeader)
                {
                    if (element == ele)
                    {
                        eleNotInTable = false;
                        comboBox2.Items.Add(ele);
                    }
                }
                if (eleNotInTable)
                    comboBox1.Items.Add(ele);
            }
            try
            {
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception compoex)
            {
                MessageBox.Show("Error in populating Composition Table. Error:" + compoex);
            }
        }

        //TAG1##############################################################################################################################

        //This is the add element button. It adds an element column into the comp table.
        private void button11_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                return;
            }
            else
            {
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.ReadOnly = true;
                }  
                DataGridViewColumn DGC = new DataGridViewColumn();
                DGC.HeaderText = comboBox1.SelectedItem.ToString();
                DGC.CellTemplate = new DataGridViewTextBoxCell();
                DGC.ValueType = typeof(Int32);
                dataGridView1.Columns.Insert(dataGridView1.ColumnCount - 2, DGC);
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Cells[dataGridView1.ColumnCount - 3].Value = 0;
                }
                dataGridView1.Columns[dataGridView1.ColumnCount - 3].SortMode = DataGridViewColumnSortMode.Automatic;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                comboBox2.Items.Add(comboBox1.SelectedItem.ToString());
                comboBox1.Items.Remove(comboBox1.SelectedItem.ToString()); 
            }
            //Then to update datasource of datagridview1
            //First, get the path to the file.
            String currentpath = Application.StartupPath + "\\compositionsCurrent.cpos";
            //Second, write to the file
            saveCompoToFile(currentpath);
            //Then reload the file to update datasource
            dataGridView1.DataSource = null;
            dataGridView1.ColumnCount = 0;
            dataGridView1.RowCount = 0;
            populateForm(currentpath);
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.ReadOnly = false;
            }  
        }
        //This is the remove element button. It removes an element column from the comp table.
        private void button12_Click(object sender, EventArgs e)
        {
            if (comboBox2.Items.Count > 1)
            {
                if (comboBox2.SelectedItem == null)
                {
                    return;
                }
                else
                {
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                    {
                        col.ReadOnly = true;
                    }  
                    DataTable DT = (DataTable)(dataGridView1.DataSource);
                    DT.Columns.Remove(comboBox2.SelectedItem.ToString());
                    comboBox1.Items.Add(comboBox2.SelectedItem.ToString());
                    comboBox2.Items.Remove(comboBox2.SelectedItem.ToString());
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                    {
                        col.ReadOnly = false;
                    }  
                }
            }
            else
            {
                MessageBox.Show("Number of Elements cannot be zero.");
            }
        }

        //This is the Reset button. It returns the table to default.
        private void button3_Click(object sender, EventArgs e)
        {
            //First, delete the former data and return them to default.
            String defaultpath = Application.StartupPath + "\\compositionsDefault.cpos";
            String currentpath = Application.StartupPath + "\\compositionsCurrent.cpos";
            dataGridView1.DataSource = null;
            dataGridView1.ColumnCount = 0;
            dataGridView1.RowCount = 0;
            try
            {
                //Here, it is returning currentpath to default value.
                File.Copy(defaultpath, currentpath, true);
            }
            catch (Exception mainex)
            {
                MessageBox.Show("Error in creating composition file in composition(). Error: " + mainex.Message);
            }

            //Second, reset the datagridview's data.

            this.populateForm(currentpath);
        }

        //This is the Load button. It loads a cpos file and use it as the composition table.
        private void button1_Click(object sender, EventArgs e)
        {
            oFDComposition.Filter = "cpos files (*.cpos)|*.cpos";
            oFDComposition.ShowDialog();
        }
        private void oFDComposition_FileOk(object sender, CancelEventArgs e)
        {
            //First, get the path to the file.
            String currentpath = oFDComposition.FileName;
            //Second, populate the Form.
            composition comp = new composition();
            this.populateForm(currentpath);
        }

        //This is the save button. It saves the table to a cpos file.
        private void button2_Click(object sender, EventArgs e)
        {
            sFDComposition.Filter = "cpos files (*.cpos)|*.cpos";
            sFDComposition.ShowDialog();
        }
        private void sFDComposition_FileOk(object sender, CancelEventArgs e)
        {
            //First, get the path to the file.
            String currentPath = sFDComposition.FileName;
            //Second, write the data to the user's designated composition file.
            saveCompoToFile(currentPath);
            //After that, write the data to the composition file that the program uses.
            String currentpath2 = Application.StartupPath + "\\compositionsCurrent.cpos";
            saveCompoToFile(currentpath2);
        }

        //This is the Apply Changes button.
        private void button4_Click(object sender, EventArgs e)
        {
            //In int, this button writes the data to the composition file that the program uses.
            //First, get the path to the file.
            String currentpath = Application.StartupPath + "\\compositionsCurrent.cpos";

            //Second, write to the file
            saveCompoToFile(currentpath);
        }

        //This is the "Generate Hypothesis" button. Clicking this button will switch to tag2 and display the Composition Hypothesis on the screen.
        private void button8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
            this.generateHypo();
        }

        //TAG2####################################################################################################################################
        //This class save the data in datagridview2 to a csv file.
        private void button6_Click(object sender, EventArgs e)
        {
            sFDCompoHypo.Filter = "csv files (*.csv)|*.csv";
            sFDCompoHypo.ShowDialog();
        }
        private void sFDCompoHypo_FileOk(object sender, CancelEventArgs e)
        {
            String currentPath = sFDCompoHypo.FileName;
            saveCompoHypo(currentPath);
        }

        //Tag3 Glycopeptide hypothesis generation#################################################################################################
        private void button17_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://prospector.ucsf.edu/prospector/cgi-bin/msform.cgi?form=msdigest");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            oFDPPMSD.Filter = "tab delimited file (*.txt)|*.txt|XML file (*.xml)|*.xml";
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
                    fileInfo += String.Format("{0}\n", oFDPPMSD.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            mystream.Close();
            richTextBox5.Font = new Font("Times New Roman", 12);
            richTextBox5.Text = fileInfo;
            button5.Enabled = true;
            button7.Enabled = true;
            button18.Enabled = true;
            button20.Enabled = true;
            populateGPForm(oFDPPMSD.FileName);
        }

        // This is the Clear Button
        private void button15_Click(object sender, EventArgs e)
        {
            button5.Enabled = false;
            button7.Enabled = false;
            button18.Enabled = false;
            button20.Enabled = false;
            richTextBox5.Text = String.Empty;
            oFDPPMSD.FileName = String.Empty;
            if (this.dataGridView4.DataSource != null)
            {
                this.dataGridView4.DataSource = null;
            }
            else
            {
                this.dataGridView4.Rows.Clear();
            }
        }

        //Load composition hypothesis button
        private void button10_Click(object sender, EventArgs e)
        {
            oFDPPComHypo.Filter = "csv files (*.csv)|*.csv";
            oFDPPComHypo.ShowDialog();
        }
        private void oFDPPComHypo_FileOk(object sender, CancelEventArgs e)
        {
            String fileInfo = "";
            Stream mystream = null;
            try
            {
                if ((mystream = oFDPPComHypo.OpenFile()) != null)
                {
                    fileInfo += String.Format("{0}\n", oFDPPComHypo.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            mystream.Close();
            richTextBox1.Font = new Font("Times New Roman", 12);
            richTextBox1.Text = fileInfo;
        }

        //Clear button of the composition hypothesis box
        private void button9_Click(object sender, EventArgs e)
        {
            oFDPPComHypo.FileName = String.Empty;
            richTextBox1.Text = String.Empty;
        }

        //Select All button
        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView4.Rows.Count; i ++)
            {
                dataGridView4.Rows[i].Cells[0].Value = 1;
            }
        }
        //Clear all selctions button
        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView4.Rows.Count; i++)
            {
                dataGridView4.Rows[i].Cells[0].Value = 0;
            }
        }


        //Load button
        private void button21_Click(object sender, EventArgs e)
        {
            oFDGP.Filter =  "GPe files (*.GPe)|*.GPe";
            oFDGP.ShowDialog();
        }
        private void oFDGP_FileOk(object sender, CancelEventArgs e)
        {
            loadGPfromFile(oFDGP.FileName);
            button5.Enabled = true;
            button7.Enabled = true;
            button18.Enabled = true;
            button20.Enabled = true;
        }

        //Save current work button
        private void button20_Click(object sender, EventArgs e)
        {
            sFDGP.Filter = "GPe files (*.GPe)|*.GPe";
            sFDGP.ShowDialog();
        }
        private void sFDGP_FileOk(object sender, CancelEventArgs e)
        {
            saveGPToFile(sFDGP.FileName);
        }


        //Generate Glycopeptide Hypothesis button.
        private void button18_Click(object sender, EventArgs e)
        {
            Stream mystream = null;
            try
            {
                if ((mystream = oFDPPComHypo.OpenFile()) != null)
                {
                    this.generateGPCompHypo(oFDPPComHypo.FileName);
                    tabControl1.SelectedTab = tabPage2;
                }
            }
            catch
            {
                this.generateGPCompHypo();
                tabControl1.SelectedTab = tabPage2;
            }

        }

        //Other Functions#######################################################################################################################
        //This function is used to populate the generator in tag1 by the file from the indicated filepath.
        private void populateForm(String currentpath)
        {
            //table is used to store the data before they are put onto the composition table.
            DataTable table = new DataTable();
            this.dataGridView3.DataSource = null;
            try
            {
                FileStream reading = new FileStream(currentpath, FileMode.Open, FileAccess.Read);
                StreamReader readcompo = new StreamReader(reading);
                //Read the first line to set the column names:
                String firstline = readcompo.ReadLine();
                String[] columnnames = firstline.Split(',');
                table.Columns.Add(columnnames[0], typeof(String));
                table.Columns.Add(columnnames[1], typeof(String));
                for (int i = 2; i < columnnames.Count() - 2; i++)
                {
                    table.Columns.Add(columnnames[i], typeof(Int32));
                }
                table.Columns.Add(columnnames[columnnames.Count() - 2], typeof(String));
                table.Columns.Add(columnnames[columnnames.Count() - 1], typeof(String));
                while (readcompo.Peek() >= 0)
                {
                    String Line = readcompo.ReadLine();
                    String[] eachentry = Line.Split(',');
                    if (eachentry[0] == "Modification%^&!@#*()iop")
                        break;
                    DataRow DR = table.NewRow();
                    DR[0] = eachentry[0];
                    DR[1] = eachentry[1];
                    for (int i = 2; i < eachentry.Count() - 2; i++)
                    {
                        DR[i] = eachentry[i];
                    }
                    DR[eachentry.Count() - 2] = eachentry[eachentry.Count() - 2];
                    DR[eachentry.Count() - 1] = eachentry[eachentry.Count() - 1];
                    table.Rows.Add(DR);
                }

                //Populate the Modification Table
                String modiLine = readcompo.ReadLine();
                String[] modiEntry = modiLine.Split(',');
                this.textBox35.Text = modiEntry[0];
                this.textBox44.Text = modiEntry[1];
                this.textBox46.Text = modiEntry[2];
                this.textBox45.Text = modiEntry[3];

                //Populate the additional rules table
                if (dataGridView3.Rows.Count > 0)
                {
                    dataGridView3.Rows.Clear();
                }
                while (readcompo.Peek() >= 0)
                {
                    String Line = readcompo.ReadLine();
                    String[] eachentry = Line.Split(',');
                    if (eachentry[2] != "")
                        dataGridView3.Rows.Add(eachentry[0], eachentry[1], eachentry[2]);
                }

                readcompo.Close();
                reading.Close();
            }
            catch (Exception compoex)
            {
                MessageBox.Show("Error in loading Composition Table. Error:" + compoex);
            }
            try
            {
                dataGridView1.DataSource = table;

            }
            catch (Exception compoex)
            {
                MessageBox.Show("Error in populating Composition Table. Error:" + compoex);
            }
        }

        //This function is used to save tag1 generator data to a cpos file in the indicated filepath.
        private void saveCompoToFile(String currentPath)
        {
            //First, prepare the writers.
            FileStream FS = new FileStream(currentPath, FileMode.Create, FileAccess.Write);
            StreamWriter write = new StreamWriter(FS);

            //Second,  loop from the datagridview and write to csv file.
            for (int j = 0; j < this.dataGridView1.ColumnCount; ++j)
            {
                String cell = this.dataGridView1.Columns[j].HeaderText;
                if (j == (this.dataGridView1.ColumnCount - 1))
                {
                    write.WriteLine(cell);
                }
                else
                {
                    write.Write(cell + ",");
                }
            }


            for (int i = 0; i < this.dataGridView1.RowCount; ++i)
            {
                for (int j = 0; j < this.dataGridView1.ColumnCount; ++j)
                {
                    var cell = this.dataGridView1.Rows[i].Cells[j].Value;
                    if (j == (this.dataGridView1.ColumnCount - 1))
                    {
                        write.WriteLine(cell);
                    }
                    else
                    {
                        write.Write(cell + ",");
                    }
                }
            }
            //Then, write the Modification data
            write.WriteLine("Modification%^&!@#*()iop,");
            write.WriteLine(this.textBox35.Text + "," + this.textBox44.Text + "," + this.textBox46.Text + "," + this.textBox45.Text);

            //Finally, the additional rules table
            for (int i = 0; i < this.dataGridView3.RowCount; ++i)
            {
                for (int j = 0; j < this.dataGridView3.ColumnCount; ++j)
                {
                    String newcell = "";
                    try
                    {
                        newcell = this.dataGridView3.Rows[i].Cells[j].Value.ToString();
                    }
                    catch (Exception)
                    {
                        newcell = "";
                    }
                    if (j == (this.dataGridView3.ColumnCount - 1))
                    {
                        write.WriteLine(newcell);
                    }
                    else
                    {
                        write.Write(newcell + ",");
                    }
                }
            }

            write.Flush();
            write.Close();
            FS.Close();
        }

        //This function is used to save tag3's datagridview4 into a file.
        private void saveGPToFile(String currentPath)
        {
            //First, prepare the writers.
            FileStream FS = new FileStream(currentPath, FileMode.Create, FileAccess.Write);
            StreamWriter write = new StreamWriter(FS);

            //Second,  loop from the datagridview and write to csv file.
            for (int j = 0; j < this.dataGridView4.ColumnCount; ++j)
            {
                String cell = this.dataGridView4.Columns[j].HeaderText;
                if (j == (this.dataGridView4.ColumnCount - 1))
                {
                    write.WriteLine(cell);
                }
                else
                {
                    write.Write(cell + ",");
                }
            }


            for (int i = 0; i < this.dataGridView4.RowCount; ++i)
            {
                for (int j = 0; j < this.dataGridView4.ColumnCount; ++j)
                {
                    try
                    {
                        //The boolean value in the selected column is NOT writing "True" into the file when the value is true. This creates a problem when we load the file. So it is handled manually.
                        if (j != 0)
                        {
                            String cell = this.dataGridView4.Rows[i].Cells[j].Value.ToString();

                            if (j == (this.dataGridView4.ColumnCount - 1))
                            {
                                write.WriteLine(cell);
                            }
                            else
                            {
                                write.Write(cell + ",");
                            }
                        }
                        else
                        {
                            String cell = "";
                            if (dataGridView4.Rows[i].Cells[j].Value.ToString() == "1")
                            {
                                cell = "True";
                            }
                            else
                            {
                                cell = "False";
                            }
                            if (j == (this.dataGridView4.ColumnCount - 1))
                            {
                                write.WriteLine(cell);
                            }
                            else
                            {
                                write.Write(cell + ",");
                            }

                        }
                    }
                    catch
                    {
                    }
                }
            }
            write.Flush();
            write.Close();
            FS.Close();
        }
        //?This function si used to load a file to tag3's datagridview4.
        private void loadGPfromFile(String currentPath)
        {
            dataGridView4.Rows.Clear();
            FileStream FS = new FileStream(currentPath, FileMode.Open, FileAccess.Read);
            StreamReader read = new StreamReader(FS);
            //This is the variable for the Adduct/Replacement column.
            read.ReadLine();
            while (read.Peek() >= 0)
            {
                String Line = read.ReadLine();
                String[] Lines = Line.Split(',');
                dataGridView4.Rows.Add(Convert.ToBoolean(Lines[0]), Convert.ToDouble(Lines[1]), Lines[2], Convert.ToInt32(Lines[3]), Convert.ToInt32(Lines[4]), Lines[5], Lines[6], Lines[7]);

            }
        }

        //This function is used to save composition hypothesis from datagridview2 to a file located in the filelink given.
        private void saveCompoHypo(String currentPath)
        {
            //First, prepare the writers.
            FileStream FS3 = new FileStream(currentPath, FileMode.Create, FileAccess.Write);
            StreamWriter write3 = new StreamWriter(FS3);

            //Second,  loop from the datagridview and write to csv file.
            for (int j = 0; j < this.dataGridView2.ColumnCount; ++j)
            {
                String cell = this.dataGridView2.Columns[j].HeaderText;
                if (j == (this.dataGridView2.ColumnCount - 1))
                {
                    write3.WriteLine(cell);
                }
                else
                {
                    write3.Write(cell + ",");
                }
            }


            for (int i = 0; i < this.dataGridView2.RowCount - 1; ++i)
            {
                for (int j = 0; j < this.dataGridView2.ColumnCount; ++j)
                {
                    String cell = this.dataGridView2.Rows[i].Cells[j].Value.ToString();
                    if (j == (this.dataGridView2.ColumnCount - 1))
                    {
                        write3.WriteLine(cell);
                    }
                    else
                    {
                        write3.Write(cell + ",");
                    }
                }
            }
            write3.Flush();
            write3.Close();
            FS3.Close();
        }

        //This function reads the generate composition page and save it to a variable of class "generator".
        public generatorData getGenerator()
        {
            //compotable is used to store the data from the composition table before they are used.
            List<compTable> compotable = new List<composition.compTable>();
            //arTable is used to store the data from additional rules table before they're used.
            List<arTable> arTable = new List<composition.arTable>();
            //GD is used to store all the data that will be returned.
            generatorData GD = new generatorData();

            String currentpath = Application.StartupPath + "\\compositionsCurrent.cpos";
            try
            {
                FileStream reading = new FileStream(currentpath, FileMode.Open, FileAccess.Read);
                StreamReader readcompo = new StreamReader(reading);
                //Read the first line to skip the column names:
                String Line1 = readcompo.ReadLine();
                String[] headers = Line1.Split(',');
                List<string> elementIDs = new List<string>();
                bool moreElements = true;
                int sh= 2;
                while (moreElements)
                {
                    if (headers[sh] != "Lower Bound")
                    {
                        elementIDs.Add(headers[sh]);
                        sh++;
                    }
                    else
                        moreElements = false;
                }

                bool firstrow = true;
                //Read the other lines for compTable data.
                while (readcompo.Peek() >= 0)
                {
                    compTable compTable = new compTable();
                    if (firstrow)
                    {
                        compTable.elementIDs = elementIDs;
                        firstrow = false;
                    }
                    String Line = readcompo.ReadLine();
                    String[] eachentry = Line.Split(',');
                    //When next line is the Modification line, it breaks.
                    if (eachentry[0] == "Modification%^&!@#*()iop")
                        break;
                    compTable.Letter = eachentry[0];
                    compTable.Molecule = eachentry[1];
                    for (int i = 2; i < elementIDs.Count + 2; i++)
                    {
                        compTable.elementAmount.Add(Convert.ToInt32(eachentry[i]));
                    }
                    List<String> bounds = new List<String>();
                    bounds.Add(eachentry[elementIDs.Count + 2]);
                    bounds.Add(eachentry[elementIDs.Count + 3]);
                    compTable.Bound = bounds;
                    compotable.Add(compTable);
                }
                //Send compotable data to GD                
                GD.comTable = compotable;
                //Populate the Modification Table
                String modiLine = readcompo.ReadLine();
                //Send Modification data to GD
                GD.Modification = modiLine.Split(',');

                //Populate the additional rules table
                while (readcompo.Peek() >= 0)
                {
                    String Line = readcompo.ReadLine();
                    String[] eachentry = Line.Split(',');
                    if (eachentry[0] != "" && eachentry[2] != "")
                    {
                        arTable areTable = new arTable();
                        areTable.Formula = eachentry[0];
                        areTable.Relationship = eachentry[1];
                        areTable.Constraint = Convert.ToString(eachentry[2]);
                        arTable.Add(areTable);
                    }
                }
                //Send arTable data to GD.
                GD.aTable = arTable;

                readcompo.Close();
                reading.Close();
            }
            catch (Exception compoex)
            {
                MessageBox.Show("Error in loading Composition Table. Error:" + compoex);
            }
            return GD;
        }

        //This function reads a composition hypothesis file, get its data and return a list of comphypo.
        public List<comphypo> getCompHypo(String currentPath)
        {
            //This is the list for storing the answer.
            List<comphypo> compotable = new List<comphypo>();

            try
            {
                FileStream reading = new FileStream(currentPath, FileMode.Open, FileAccess.Read);
                StreamReader readcompo = new StreamReader(reading);
                //Read the first line to skip the column names:
                String head = readcompo.ReadLine();
                String[] headers = head.Split(',');
                List<string> molename = new List<string>();
                List<string> elementIDs = new List<string>();
                int h = 1;
                while (headers[h] != "Compositions")
                {
                    Console.WriteLine(headers[h]);
                    elementIDs.Add(headers[h]);
                    h++;
                }
                h++;
                while (headers[h] != "Adduct/Replacement")
                {
                    Console.WriteLine(headers[h]);
                    molename.Add(headers[h]);
                    h++;
                }
                bool firstrow = true;
                //Read the other lines for compTable data.
                while (readcompo.Peek() >= 0)
                {
                    String Line = readcompo.ReadLine();
                    String[] eachentry = Line.Split(',');
                    if (eachentry.Count() < 2)
                        break;
                    if (string.IsNullOrEmpty(eachentry[0]))
                        break;
                    //comhyp is used to store the data that will be put into the list, compotable.
                    comphypo comhyp = new comphypo();
                    comhyp.TrueOrFalse = true;
                    if (firstrow)
                    {
                        comhyp.elementIDs = elementIDs;
                        comhyp.MoleNames = molename;
                        firstrow = false;
                    }
                    comhyp.MW = Convert.ToDouble(eachentry[0]);
                    int i = 1;
                    bool moreElements = true;
                    while (moreElements)
                    {
                        if (headers[i] != "Compositions")
                        {
                            comhyp.elementAmount.Add(Convert.ToInt32(eachentry[i]));
                            i++;
                        }
                        else
                            moreElements = false;
                    }
                    comhyp.compoundCompo = Convert.ToString(eachentry[i]);
                    i++;
                    bool moreCompounds = true;
                    List<int> eqCoun = new List<int>();
                    while (moreCompounds)
                    {
                        if (headers[i] != "Adduct/Replacement")
                        {
                            if (!String.IsNullOrEmpty(eachentry[i]))
                                eqCoun.Add(Convert.ToInt32(eachentry[i]));
                            else
                                eqCoun.Add(0);
                            i++;
                        }
                        else
                            moreCompounds = false;
                    }
                    comhyp.eqCounts = eqCoun;
                    comhyp.AddRep = Convert.ToString(eachentry[i]);
                    comhyp.AdductNum = Convert.ToInt32(eachentry[i + 1]);
                    if (eachentry.Count() > (i + 2))
                    {
                        comhyp.PepSequence = eachentry[i + 2];
                        comhyp.PepModification = eachentry[i + 3];
                        comhyp.MissedCleavages = Convert.ToInt32(eachentry[i + 4]);
                        comhyp.numGly = Convert.ToInt32(eachentry[i + 5]);
                        comhyp.StartAA = Convert.ToInt32(eachentry[i + 6]);
                        comhyp.EndAA = Convert.ToInt32(eachentry[i + 7]);
                    }
                    else
                    {
                        comhyp.PepSequence = "";
                        comhyp.PepModification = "";
                        comhyp.MissedCleavages = 0;
                        comhyp.numGly = 0;
                        comhyp.StartAA = 0;
                        comhyp.EndAA = 0;
                    }
                    compotable.Add(comhyp);
                }

                readcompo.Close();
                reading.Close();
            }
            catch (Exception compoex)
            {
                MessageBox.Show("Error in loading Composition Hypothesis File. Error:" + compoex);
            }
            return compotable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reading"></param>
        /// <returns></returns>
        public List<comphypo> getCompHypoFromStream(MemoryStream reading)
        {
            Console.WriteLine("---getCompHypoFromStream---");
            List<comphypo> compotable = new List<comphypo>();
                StreamReader readcompo = new StreamReader(reading);
                //Read the first line to skip the column names:
                String head = readcompo.ReadLine();
                String[] headers = head.Split(',');
                List<string> molename = new List<string>();
                List<string> elementIDs = new List<string>();
                int h = 1;
                while (headers[h] != "Compositions")
                {
                    elementIDs.Add(headers[h]);
                    h++;
                }
                h++;
                while (headers[h] != "Adduct/Replacement")
                {
                    molename.Add(headers[h]);
                    h++;
                }
                bool firstrow = true;
                //Read the other lines for compTable data.
                while (readcompo.Peek() >= 0)
                {
                    String Line = readcompo.ReadLine();
                    String[] eachentry = Line.Split(',');
                    if (eachentry.Count() < 2)
                        break;
                    if (string.IsNullOrEmpty(eachentry[0]))
                        break;
                    //comhyp is used to store the data that will be put into the list, compotable.
                    comphypo comhyp = new comphypo();
                    comhyp.TrueOrFalse = true;
                    if (firstrow)
                    {
                        comhyp.elementIDs = elementIDs;
                        comhyp.MoleNames = molename;
                        firstrow = false;
                    }
                    comhyp.MW = Convert.ToDouble(eachentry[0]);
                    int i = 1;
                    bool moreElements = true;
                    while (moreElements)
                    {
                        if (headers[i] != "Compositions")
                        {
                            comhyp.elementAmount.Add(Convert.ToInt32(eachentry[i]));
                            i++;
                        }
                        else
                            moreElements = false;
                    }
                    comhyp.compoundCompo = Convert.ToString(eachentry[i]);
                    i++;
                    bool moreCompounds = true;
                    List<int> eqCoun = new List<int>();
                    while (moreCompounds)
                    {
                        if (headers[i] != "Adduct/Replacement")
                        {
                            if (!String.IsNullOrEmpty(eachentry[i]))
                                eqCoun.Add(Convert.ToInt32(eachentry[i]));
                            else
                                eqCoun.Add(0);
                            i++;
                        }
                        else
                            moreCompounds = false;
                    }
                    comhyp.eqCounts = eqCoun;
                    comhyp.AddRep = Convert.ToString(eachentry[i]);
                    comhyp.AdductNum = Convert.ToInt32(eachentry[i + 1]);
                    if (eachentry.Count() > (i + 2))
                    {
                        comhyp.PepSequence = eachentry[i + 2];
                        comhyp.PepModification = eachentry[i + 3];
                        comhyp.MissedCleavages = Convert.ToInt32(eachentry[i + 4]);
                        comhyp.numGly = Convert.ToInt32(eachentry[i + 5]);
                        comhyp.StartAA = Convert.ToInt32(eachentry[i + 6]);
                        comhyp.EndAA = Convert.ToInt32(eachentry[i + 7]);
                    }
                    else
                    {
                        comhyp.PepSequence = "";
                        comhyp.PepModification = "";
                        comhyp.MissedCleavages = 0;
                        comhyp.numGly = 0;
                        comhyp.StartAA = 0;
                        comhyp.EndAA = 0;
                    }
                    compotable.Add(comhyp);
                }

                readcompo.Close();
                reading.Close();            

            return compotable;
        }        



        //This function is used by glycopeptide tag to populate the datagridview after adding a file.
        private void populateGPForm (String currentpath)
        {
            string extension = Path.GetExtension(currentpath);
            List<PPMSD> data = new List<PPMSD>();
            //This is tab delimited file.
            if (extension == ".txt")
            {
                data = readtablim(currentpath);
            }
            //This is XML File implemented by JK
            else if (extension == ".xml")
            {
                MSDigestReport report = MSDigestReport.Load(currentpath);
                Console.WriteLine(report.Peptides.Count);
                foreach (MSDigestPeptide pep in report.Peptides)
                {
                    //Console.WriteLine(pep.Sequence);
                }
                data = report.Peptides.ConvertAll(x => new PPMSD(x));
            }
            Console.WriteLine(data.Count);
            foreach (PPMSD x in data)
            {
                Console.WriteLine(x);
            }        
            periodicTable pt = new periodicTable();
            richTextBox6.Text = getSequence(data);
            foreach (PPMSD pp in data)
            {
                dataGridView4.Rows.Add(1, pp.Mass - (pp.Charge * pt.getMass("H+")), pp.Modifications, pp.StartAA, pp.EndAA, pp.MissedCleavages, pp.Sequence, "0");
            }

        }
        //This function reads tab delimited file.
        private List<PPMSD> readtablim(String currentpath)
        {
            List<PPMSD> data = new List<PPMSD>();
            FileStream FS = new FileStream(currentpath, FileMode.Open, FileAccess.Read );
            StreamReader read = new StreamReader(FS);
            //skip title line:
            read.ReadLine();
            while (read.Peek() >= 0)
            {
                PPMSD pp = new PPMSD();
                String line = read.ReadLine();
                String[] Lines = line.Split('\t');
                pp.number = Convert.ToInt32(Lines[0]);
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
        //This function reads in a list of PPMSD and returns the full sequence
        public String getSequence(List<PPMSD> data)
        {
            int startAA = 1;
            String sequence = "";
            data = data.OrderBy(a => a.StartAA).ToList();
            foreach (PPMSD pp in data)
            {
                if (pp.StartAA == startAA)
                {
                    sequence = sequence + pp.Sequence;
                    startAA = pp.EndAA + 1;
                }                
            }
            return sequence;
        }

        //This function reads datagridview4 and obtain its data.
        private List<PPMSD> obtainPP()
        {
            List<PPMSD> PP = new List<PPMSD>();
            for (int i = 0; i < dataGridView4.Rows.Count; i++)
            {
                //if (Convert.ToInt32(dataGridView4.Rows[c].Cells[0].Value) == 0)
                   // continue;
                PPMSD pp = new PPMSD();
                pp.selected = Convert.ToBoolean((dataGridView4.Rows[i].Cells[0].Value));
                pp.Mass = Convert.ToDouble(dataGridView4.Rows[i].Cells[1].Value);
                pp.Modifications = Convert.ToString(dataGridView4.Rows[i].Cells[2].Value);
                pp.StartAA = Convert.ToInt32(dataGridView4.Rows[i].Cells[3].Value);
                pp.EndAA = Convert.ToInt32(dataGridView4.Rows[i].Cells[4].Value);
                pp.MissedCleavages = Convert.ToInt32(dataGridView4.Rows[i].Cells[5].Value);
                pp.Sequence = Convert.ToString(dataGridView4.Rows[i].Cells[6].Value);
                pp.numGly = Convert.ToInt32(dataGridView4.Rows[i].Cells[7].Value);
                PP.Add(pp);
            }
            return PP;
        }

        //This function is used by the glycopeptide generator to generate hypothesis and put it into datagridview.
        private static String comhypopathStore;
        private void generateGPCompHypo(String comhypopath)
        {
            comhypopathStore = comhypopath;
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

            dataGridView2.DataSource = theComhypoOnTab2;
            button6.Enabled = true;
        }
        void bw_DoWork2(object sender, DoWorkEventArgs e)
        {
            stuffToDo2(comhypopathStore);
        }
        void bw_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(Convert.ToString(e.Error));
            }
            //all background work has completed and we are going to close the waiting message
            msgForm.Close();
        }
        private void stuffToDo2(string comhypopath)
        {
            List<comphypo> CHy = getCompHypo(comhypopath);
            List<string> elementIDs = new List<string>();
            List<string> molename = new List<string>();
            for (int j = 0; j < CHy.Count(); j++)
            {
                if (CHy[j].elementIDs.Count > 0)
                {
                    for (int i = 0; i < CHy[j].elementIDs.Count(); i++)
                    {
                        elementIDs.Add(CHy[j].elementIDs[i]);
                    }
                    for (int i = 0; i < CHy[j].MoleNames.Count(); i++)
                    {
                        molename.Add(CHy[j].MoleNames[i]);
                    }
                    break;
                }
            }

            String AddRep = CHy[0].AddRep;

            int indexH = 0;
            int indexO = 0;
            int indexWater = 0;
            try
            {
                indexH = elementIDs.IndexOf("H");
                indexO = elementIDs.IndexOf("O");
                indexWater = molename.IndexOf("Water");
                if (indexWater < 0)
                {
                    throw new Exception("No Water!");
                }
            }
            catch
            {
                MessageBox.Show("Your composition hypothesis contains a compound without Water. Job terminated.");
                return;
            }
            List<PPMSD> PP = obtainPP();
            List<comphypo> Ans = new List<comphypo>();
            //Ans.AddRange(CHy);
            periodicTable PT = new periodicTable();
            for (int i = 0; i < PP.Count; i++)
            {
                if (PP[i].selected)
                {
                    Int32 Count = Convert.ToInt32(PP[i].numGly);
                    List<comphypo> Temp = new List<comphypo>();
                    comphypo temp = new comphypo();
                    //First line:
                    temp.compoundCompo = "";
                    temp.AdductNum = 0;
                    temp.AddRep = "";
                    temp.MW = PP[i].Mass;
                    for (int s = 0; s < CHy[0].eqCounts.Count; s++)
                    {
                        temp.eqCounts.Add(0);
                    }
                    for (int s = 0; s < CHy[0].elementAmount.Count; s++)
                    {
                        temp.elementAmount.Add(0);
                    }
                    //columns for glycopeptides
                    temp.PepModification = PP[i].Modifications;
                    temp.PepSequence = PP[i].Sequence;
                    temp.MissedCleavages = PP[i].MissedCleavages;
                    temp.StartAA = PP[i].StartAA;
                    temp.EndAA = PP[i].EndAA;
                    temp.numGly = 0;
                    Temp.Add(temp);
                    for (int j = 0; j < Count; j++)
                    {
                        List<comphypo> Temp2 = new List<comphypo>();
                        for (int k = 0; k < Temp.Count(); k++)
                        {
                            //need to reread the file and get new reference, because c# keeps passing by reference which creates a problem.
                            List<comphypo> CH = getCompHypo(comhypopath);
                            for (int l = 0; l < CH.Count(); l++)
                            {
                                comphypo temp2 = new comphypo();
                                temp2 = CH[l];
                                temp2.numGly = Temp[k].numGly + 1;
                                temp2.PepModification = Temp[k].PepModification;
                                temp2.PepSequence = Temp[k].PepSequence;
                                temp2.MissedCleavages = Temp[k].MissedCleavages;
                                temp2.StartAA = Temp[k].StartAA;
                                temp2.EndAA = Temp[k].EndAA;
                                List<string> forsorting = new List<string>();
                                forsorting.Add(Temp[k].compoundCompo);
                                forsorting.Add(temp2.compoundCompo);
                                forsorting = forsorting.OrderBy(a => a).ToList();
                                temp2.compoundCompo = forsorting[0] + forsorting[1];
                                temp2.AdductNum = temp2.AdductNum + Temp[k].AdductNum;
                                for (int s = 0; s < temp2.eqCounts.Count; s++)
                                {
                                    temp2.eqCounts[s] = temp2.eqCounts[s] + Temp[k].eqCounts[s];
                                }
                                for (int s = 0; s < temp2.elementAmount.Count; s++)
                                {
                                    temp2.elementAmount[s] = temp2.elementAmount[s] + Temp[k].elementAmount[s];
                                }
                                for (int ui = 0; ui < molename.Count(); ui++)
                                {
                                    if (molename[ui] == "Water")
                                    {
                                        if (temp2.eqCounts[ui] > 0)
                                        {
                                            temp2.eqCounts[ui] = temp2.eqCounts[ui] - 1;
                                        }
                                        break;
                                    }
                                }
                                #region Modified by JK
                                //temp2.elementAmount[indexH] = temp2.elementAmount[indexH] - 2;
                                //temp2.elementAmount[indexO] = temp2.elementAmount[indexO] - 1;
                                //if (temp2.elementAmount[indexO] < 0)
                                //    temp2.elementAmount[indexO] = 0;
                                //if (temp2.elementAmount[indexH] < 0)
                                //    temp2.elementAmount[indexH] = 0;

                                /* These fields are not present in the Database-generated hypothesis,
                                 * but they are not appropriately error-checked when computed earlier.
                                 * This bandaid should let existing files work while letting Database-
                                 * generated ones through as well. This function is very difficult to 
                                 * trace in and would benefit from rewriting in the future.
                                 */
                                if ((indexH > 0) && (indexO > 0))
                                {
                                    temp2.elementAmount[indexH] = temp2.elementAmount[indexH] - 2;
                                    temp2.elementAmount[indexO] = temp2.elementAmount[indexO] - 1;
                                    if (temp2.elementAmount[indexO] < 0)
                                        temp2.elementAmount[indexO] = 0;
                                    if (temp2.elementAmount[indexH] < 0)
                                        temp2.elementAmount[indexH] = 0;
                                }
                                //else
                                //{
                                //    temp2.elementAmount[indexH] = 0;
                                //    temp2.elementAmount[indexO] = 0;
                                //}
                                #endregion
                                //Hard coded removal of extra water from neutral Charge glycan. 
                                temp2.MW = temp2.MW + Temp[k].MW - PT.getMass("H") * 2 - PT.getMass("O");
                                Temp2.Add(temp2);
                            }
                        }
                        Temp.AddRange(Temp2);
                    }
                    Ans.AddRange(Temp);
                }
            }
            //Remove Duplicates from CHy
            Ans = Ans.OrderBy(a => a.MW).ToList();
            CHy.Clear();
            for (int i = 0; i < Ans.Count() - 1; i++)
            {
                bool thesame = false;
                bool equal = (Ans[i].eqCounts.Count == Ans[i + 1].eqCounts.Count) && new HashSet<int>(Ans[i].eqCounts).SetEquals(Ans[i + 1].eqCounts);
                if (Ans[i].PepSequence == Ans[i + 1].PepSequence && equal)
                {
                    if (Ans[i].AdductNum == Ans[i + 1].AdductNum && Ans[i].PepModification == Ans[i + 1].PepModification)
                    {
                        thesame = true;
                    }
                }
                if (!thesame)
                    CHy.Add(Ans[i]);
            }
            Console.WriteLine("Ans Length {0}", Ans.Count());
            //Enter elementID and MoleNames into each rows
            CHy.Add(Ans[Ans.Count() - 1]);
            for (int i = 0; i < CHy.Count(); i++)
            {
                CHy[i].elementIDs.Clear();
                CHy[i].MoleNames.Clear();

                if (i == CHy.Count() - 1)
                {
                    CHy[0].elementIDs = elementIDs;
                    CHy[0].MoleNames = molename;
                }
            }

            //Obtain the Name of the adduct molecules:           
            generatorData GD = new generatorData();
            GD.Modification = AddRep.Split('/');

            //Send to generate DataTable
            Console.WriteLine(CHy[0]);
            theComhypoOnTab2 = genDT(CHy, GD);

        }
        
        //override, if a composition hyposthesis file isn't loaded
        private void generateGPCompHypo()
        {
            List<PPMSD> PP = obtainPP();
            List<comphypo> Ans = new List<comphypo>();
            for (int i = 0; i < PP.Count; i++)
            {
                if (PP[i].selected)
                {
                    List<comphypo> Temp = new List<comphypo>();
                    comphypo temp = new comphypo();
                    //First line:
                    temp.compoundCompo = "";
                    temp.AdductNum = 0;
                    temp.AddRep = "";
                    temp.MW = PP[i].Mass;
                    //columns for glycopeptides
                    temp.PepModification = PP[i].Modifications;
                    temp.PepSequence = PP[i].Sequence;
                    temp.MissedCleavages = PP[i].MissedCleavages;
                    temp.StartAA = PP[i].StartAA;
                    temp.EndAA = PP[i].EndAA;
                    temp.numGly = 0;
                    Ans.Add(temp);
                }
            }
            String composition = "0";
            foreach (comphypo ch in Ans)
            {
                ch.compoundCompo = composition;
            }


            DataTable DT = genDT(Ans);

            dataGridView2.DataSource = DT;
            button6.Enabled = true;
        }


        //#################################################generateHypo class is big and it deserves a block######################################
        //This function generates the Composition Hypothesis and print it on dataGridView2.
        PleaseWait msgForm = new PleaseWait();
        private void generateHypo()
        {
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
            //Print the hypothesis to the datagridview.
            dataGridView2.DataSource = theComhypoOnTab2;
            button6.Enabled = true;
        }
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            stuffToDo();
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
        private void stuffToDo()
        {
            //Obtain Table
            generatorData GD = getGenerator();
            List<comphypo> UltFinalAns = genHypo(GD);
            
            Console.WriteLine(UltFinalAns[0]);
            
            theComhypoOnTab2 = genDT(UltFinalAns, GD);


        }
        public List<comphypo> genHypo(generatorData GD)
        {
            //If there are letters in the lower bounds and upper bounds section, add those rules into the additional table in the GD variable.
            GD = obtainNewGD(GD);

            //Convert all letters in Bounds to numbers, so that we can calcuate them
            List<compTable> Table = convertLetters(GD);
            
            //Now, all bounds in Table only consists of numbers, but they're still not calculated. (e.g. 2+3+4) So, calculate them.
            List<compTable> CoTa0 = calnumbers(Table);
            //Now, in those bounds, replace them with numbers from all ranges. (ex. if a bound has numbers (2,6,8), replace it with (2,3,4,5,6,7,8) for computations). Afterall, they are bounds.
            CoTa0 = expendBound(CoTa0);
            //Now that all of the bounds are numbers, we can create the composition hypothesis with them, by combining their combinations.
            List<comphypo> Ans = calComHypo(CoTa0);
            //Then limit them with the rules from the additional rules table.
            Ans = checkAddRules(Ans, GD);
            //Check if there is any unwanted rows, which have negative values.
            Ans = checkNegative(Ans);
            //Add the adducts into each row data.
            Ans = addAdducts(Ans, GD);
            //Final Check, if any of the values are negative (ex. a negative amount of S or C), remove that row.
            Ans = checkNegative(Ans);
            //Lastly transfer data from eqCount to molnames and eqCounts, to decrease number of columns needed
            Ans = cleanColumns(Ans, GD);
            //Really, this is the last step. Lastly, remove columns of all zero element counts.
            Ans = removeZeroElements(Ans);

            //If "Is is a Glycan?" box is checked, add one water to add compounds automatically.
            if (checkBox1.Checked == true)
                Ans = addWater(Ans);

            return Ans;
        }

        //This puts comhypo data into a datatable.
        private DataTable genDT(List<comphypo> UltFinalAns, generatorData GD)
        {
            //Console.WriteLine("---genDT---");
            DataTable hypo = new DataTable();
            List<string> elementIDs = new List<string>();
            List<string> molname = new List<string>();
            for (int j = 0; j < UltFinalAns.Count(); j++)
            {
                if (UltFinalAns[j].elementIDs.Count > 0)
                {
                    for (int i = 0; i < UltFinalAns[j].elementIDs.Count(); i++)
                    {
                        elementIDs.Add(UltFinalAns[j].elementIDs[i]);
                    }
                    for (int i = 0; i < UltFinalAns[j].MoleNames.Count(); i++)
                    {
                        molname.Add(UltFinalAns[j].MoleNames[i]);
                    }
                    break;
                }
            }

            hypo.Columns.Add("Molecular Weight", typeof(Double));
            for (int i = 0; i < elementIDs.Count(); i++)
            {
                hypo.Columns.Add(elementIDs[i], typeof(Int32));
            }
            #region Debugging
            //Console.WriteLine("---Molecule Names---");
            //foreach (string s in molname)
            //{
            //    Console.WriteLine(s);
            //}
            ////Console.WriteLine("---Element IDs---");
            //foreach (string s in elementIDs)
            //{
            //    Console.WriteLine(s);
            //}
            #endregion
            hypo.Columns.Add("Compositions", typeof(String));
            for (int i = 0; i < UltFinalAns[0].eqCounts.Count(); i++)
            {
                Console.WriteLine(i);
                hypo.Columns.Add(molname[i], typeof(Int32));
            }

            hypo.Columns.Add("Adduct/Replacement", typeof(String));
            hypo.Columns.Add("Adduct Amount", typeof(Int32));
            hypo.Columns.Add("Peptide Sequence", typeof(String));
            hypo.Columns.Add("Peptide Modification", typeof(String));
            hypo.Columns.Add("Peptide Missed Cleavage Number", typeof(Int32));
            hypo.Columns.Add("Number of Glycan Attachment to Peptide", typeof(Int32));
            hypo.Columns.Add("Start AA", typeof(Int32));
            hypo.Columns.Add("End AA", typeof(Int32));
            
            //This is the variable for the Adduct/Replacement column.
            String AddReplace = GD.Modification[0] + "/" + GD.Modification[1];
            for (int s = 0; s < UltFinalAns.Count(); s++)
            {
                DataRow ab = hypo.NewRow();
                String composition = "";
                if (UltFinalAns[s].eqCounts.Count() == 0)
                    composition = "0";
                else
                {
                    composition = "[" + UltFinalAns[s].eqCounts[0];
                    for (int i = 1; i < UltFinalAns[s].eqCounts.Count(); i++)
                    {
                        composition = composition + ";" + UltFinalAns[s].eqCounts[i];
                    }
                    composition = composition + "]";
                }
                ab[0] = UltFinalAns[s].MW;
                int sh = 1;
                for (int i = 0; i < elementIDs.Count(); i++)
                {
                   ab[sh + i] = UltFinalAns[s].elementAmount[i];
                }
                ab[sh + elementIDs.Count()] = composition;
                for (int i = 0; i < UltFinalAns[s].eqCounts.Count(); i++)
                {
                    ab[sh + elementIDs.Count() + 1 + i] = UltFinalAns[s].eqCounts[i];
                }
                ab["Adduct/Replacement"] = AddReplace;
                ab["Adduct Amount"] = UltFinalAns[s].AdductNum;
                ab["Peptide Sequence"] = UltFinalAns[s].PepSequence;
                ab["Peptide Modification"] = UltFinalAns[s].PepModification;
                ab["Peptide Missed Cleavage Number"] = UltFinalAns[s].MissedCleavages;
                ab["Number of Glycan Attachment to Peptide"] = UltFinalAns[s].numGly;
                ab["Start AA"] = UltFinalAns[s].StartAA;
                ab["End AA"] = UltFinalAns[s].EndAA;
                hypo.Rows.Add(ab);
            }
            for (int i = 0; i < hypo.Columns.Count; i++)
            {
                hypo.Columns[i].SetOrdinal(i);
            }
            return hypo;
        }
        //override for the glycopeptide hypothesis class, when there is no generatordata.
        private DataTable genDT(List<comphypo> UltFinalAns)
        {
            DataTable hypo = new DataTable();
            hypo.Columns.Add("Molecular Weight", typeof(Double));
            hypo.Columns.Add("Compositions", typeof(String));
            hypo.Columns.Add("Adduct/Replacement", typeof(String));
            hypo.Columns.Add("Adduct Amount", typeof(Int32));
            hypo.Columns.Add("Peptide Sequence", typeof(String));
            hypo.Columns.Add("Peptide Modification", typeof(String));
            hypo.Columns.Add("Peptide Missed Cleavage Number", typeof(Int32));
            hypo.Columns.Add("Number of Glycan Attachment to Peptide", typeof(Int32));
            hypo.Columns.Add("Start AA", typeof(Int32));
            hypo.Columns.Add("End AA", typeof(Int32));

            //This is the variable for the Adduct/Replacement column.
            String AddReplace = "0/0";

            UltFinalAns = UltFinalAns.Distinct().ToList();
            foreach (comphypo ch in UltFinalAns)
            {
                String composition = "0";
                hypo.Rows.Add(ch.MW, composition, AddReplace, ch.AdductNum, ch.PepSequence, ch.PepModification, ch.MissedCleavages, ch.numGly, ch.StartAA, ch.EndAA);
            }
            return hypo;
        }


        //genHypo runs these following functions one by one to do its task://///////////////////////////////////////////////////////
        private generatorData obtainNewGD(generatorData GD)
        {
            List<compTable> Table = new List<compTable>();
            Table.AddRange(GD.comTable);
            Boolean hasLetter = false;
            for (int i = 0; i < Table.Count(); i++)
            {
                String bound = Table[i].Bound[0];
                foreach (char j in bound)
                {
                    if (char.IsUpper(j))
                    {
                        hasLetter = true;
                    }
                }
                if (hasLetter == true)
                {
                    hasLetter = false;
                    arTable AT = new arTable();
                    AT.Constraint = Table[i].Letter;
                    AT.Relationship = "≤";
                    AT.Formula = bound;
                    GD.aTable.Add(AT);
                }
                String bound2 = Table[i].Bound[1];
                foreach (char j in bound2)
                {
                    if (char.IsUpper(j))
                    {
                        hasLetter = true;
                    }
                }
                if (hasLetter == true)
                {
                    hasLetter = false;
                    arTable AT = new arTable();
                    AT.Constraint = Table[i].Letter;
                    AT.Relationship = "≥";
                    AT.Formula = bound2;
                    GD.aTable.Add(AT);
                }
                
            }
            return GD;

        }
        public List<compTable> convertLetters(generatorData tGD)
        {
            generatorData GD = new generatorData();
            GD = tGD;
            List<compTable> Table = new List<compTable>();
            Table.AddRange(GD.comTable);
            Boolean MoreLetters = true;
            //Use the cleanbound function to clean up all letters in the bounds.
            while (MoreLetters)
            {
                MoreLetters = false;
                for (int i = 0; i < Table.Count; i++)
                {
                    Table[i].Bound = cleanBounds(Table[i].Bound, tGD);
                }
                for (int i = 0; i < Table.Count; i++)
                {
                    foreach (String bound in Table[i].Bound)
                    {
                        foreach (char j in bound)
                        {
                            if (char.IsUpper(j))
                            {
                                MoreLetters = true;
                            }
                        }
                    }
                }
            }
            return Table;
        }
        public List<compTable> calnumbers(List<compTable> Table)
        {
            List<compTable> CoTa0 = new List<compTable>();
            String oneBound = "";
            foreach (compTable j in Table)
            {
                List<String> CoTatwo = new List<String>();
                foreach (String bound in j.Bound)
                {
                    oneBound = "";
                    MathParser Pa = new MathParser();
                    oneBound = Convert.ToString(Pa.Parse(bound, false));
                    CoTatwo.Add(oneBound);
                }
                j.Bound.Clear();
                //While we're at it, we'll remove the duplicate bounds also.
                j.Bound.AddRange(CoTatwo.Distinct().ToList());
            }
            CoTa0 = Table;
            return CoTa0;
        }
        private List<compTable> expendBound(List<compTable> CoTa0)
        {
            foreach (compTable j in CoTa0)
            {
                Int32 LBound = Convert.ToInt32(j.Bound[0]);
                Int32 UBound = Convert.ToInt32(j.Bound[0]);
                List<String> CoTatwo = new List<String>();
                foreach (String bound in j.Bound)
                {
                    if (Convert.ToInt32(bound) < LBound)
                        LBound = Convert.ToInt32(bound);
                    if (Convert.ToInt32(bound) > UBound)
                        UBound = Convert.ToInt32(bound);
                }
                for (Int32 i = LBound; i <= UBound; i++)
                {
                    CoTatwo.Add(Convert.ToString(i));
                }
                j.Bound.Clear();
                //While we're at it, we'll remove the duplicate bounds also.
                j.Bound.AddRange(CoTatwo.Distinct().ToList());
            }
            return CoTa0;
        }
        private List<comphypo> calComHypo(List<compTable> CoTa)
        {
            List<comphypo> Ans = new List<comphypo>();
            List<string> elementIDs = new List<string>();
            for (int j = 0; j < CoTa.Count(); j++ )
            {
                if (CoTa[j].elementIDs.Count > 0)
                {
                    for (int i = 0; i < CoTa[j].elementIDs.Count(); i++)
                    {
                        elementIDs.Add(CoTa[j].elementIDs[i]);
                    }
                    break;
                }
            }


            Double MW = new Double();
            foreach (compTable j in CoTa)
            {
                List<comphypo> tempAns = new List<comphypo>();
                foreach (String k in j.Bound)
                {
                    Int32 boundNumber = Convert.ToInt32(k);
                    //Append this molecule to the other compositions.
                    if (Ans.Count != 0)
                    {
                        for (int l = 0; l < Ans.Count; l++)
                        {
                            comphypo comphypoAns = new comphypo();
                            foreach (var item in Ans[l].eqCount)
                            {
                                comphypoAns.eqCount.Add(item.Key, item.Value);
                            }
                            comphypoAns.eqCount[j.Letter] = boundNumber;                            
                            for (int sh = 0; sh < Ans[l].elementAmount.Count(); sh++)
                            {
                                comphypoAns.elementAmount.Add(boundNumber * j.elementAmount[sh] + Ans[l].elementAmount[sh]);
                            }
                            MW = getcompMass(j, elementIDs) * boundNumber;
                            comphypoAns.MW = MW + Ans[l].MW;
                            tempAns.Add(comphypoAns);
                        }
                    }
                    else
                    {
                        comphypo anothercomphypoAns = new comphypo();
                        anothercomphypoAns.eqCount.Add("A", 0);
                        anothercomphypoAns.eqCount.Add("B", 0);
                        anothercomphypoAns.eqCount.Add("C", 0);
                        anothercomphypoAns.eqCount.Add("D", 0);
                        anothercomphypoAns.eqCount.Add("E", 0);
                        anothercomphypoAns.eqCount.Add("F", 0);
                        anothercomphypoAns.eqCount.Add("G", 0);
                        anothercomphypoAns.eqCount.Add("H", 0);
                        anothercomphypoAns.eqCount.Add("I", 0);
                        anothercomphypoAns.eqCount.Add("J", 0);
                        anothercomphypoAns.eqCount.Add("K", 0);
                        anothercomphypoAns.eqCount.Add("L", 0);
                        anothercomphypoAns.eqCount.Add("M", 0);
                        anothercomphypoAns.eqCount.Add("N", 0);
                        anothercomphypoAns.eqCount.Add("O", 0);
                        anothercomphypoAns.eqCount.Add("P", 0);
                        anothercomphypoAns.eqCount.Add("Q", 0);
                        //Add this molecule to the list
                        anothercomphypoAns.eqCount[j.Letter] = boundNumber;
                        for (int sh = 0; sh < j.elementAmount.Count(); sh++)
                        {
                            anothercomphypoAns.elementAmount.Add(boundNumber * j.elementAmount[sh]);
                        }
                        MW = boundNumber * this.getcompMass(j, elementIDs);
                        anothercomphypoAns.MW = MW;
                        tempAns.Add(anothercomphypoAns);
                    }

                }
                Ans.Clear();
                Ans.AddRange(tempAns);
            }
            //Lastly, remove the repeated rows
            Ans = Ans.OrderByDescending(a => a.MW).ToList();
            List<comphypo> Answer = new List<comphypo>();
            int startrow = 1;
            int endrow = 3;
            if (Ans.Count() > endrow)
                endrow = Ans.Count();
            Boolean OK = true;
            for (int i = 0; i < (Ans.Count())-1; i++)
            {
                startrow = i + 1;

                endrow = startrow + 3;
                if (Ans.Count() < endrow)
                    endrow = Ans.Count();
                for (int j = startrow; j < endrow; j++)
                {
                    if (Ans[i].eqCount.SequenceEqual(Ans[j].eqCount))
                    {
                        OK = false;
                        continue;
                    }
                }
                if (OK)
                    Answer.Add(Ans[i]);
                OK = true;
            }
            Answer.Add(Ans[Ans.Count()-1]);
            for (int i = 0; i < Answer.Count(); i++)
            {
                Answer[i].elementIDs.Clear();

                if (i == Answer.Count() - 1)
                    Answer[0].elementIDs = elementIDs;
            }
            return Answer;
        }
        private List<comphypo> checkAddRules(List<comphypo> Ans, generatorData GD)
        {
            List<comphypo> CHy = new List<comphypo>();
            List<string> elementIDs = new List<string>();
            for (int j = 0; j < Ans.Count(); j++)
            {
                if (Ans[j].elementIDs.Count > 0)
                {
                    for (int i = 0; i < Ans[j].elementIDs.Count(); i++)
                    {
                        elementIDs.Add(Ans[j].elementIDs[i]);
                    }
                    break;
                }
            }

            for (int i = 0; i < CHy.Count(); i++)
            {
                CHy[i].elementIDs.Clear();
                CHy[i].MoleNames.Clear();

                if (i == CHy.Count() - 1)
                {
                    CHy[0].elementIDs = elementIDs;
                }
            }
            Boolean RowGood = true;
            try
            {
                if (GD.aTable.Count != 0)
                {
                    for (int j = 0; j < Ans.Count; j++)
                    {
                        for (int i = 0; i < GD.aTable.Count(); i++)
                        {
                            //MessageBox.Show(Convert.ToString(c));
                            String equation = translet(Ans[j], GD.aTable[i]);
                            String Relationship = GD.aTable[i].Relationship;
                            String Constraint = GD.aTable[i].Constraint;
                            Constraint = converConstraints(Ans[j],Constraint);
                            //Five relationships
                            //First one
                            if (Relationship == "=")
                            {
                                MathParser parser = new MathParser();
                                Double solution = parser.Parse(equation, false);
                                //only 3 situation exists
                                //Equals to Even
                                if (Constraint == "even")
                                {
                                    if (solution % 2 != 0)
                                    {
                                        RowGood = false;
                                    }
                                    continue;
                                }
                                //Equals to Odd
                                if (Constraint == "odd")
                                {
                                    if (solution % 2 != 1)
                                    {
                                        RowGood = false;
                                    }
                                    continue;
                                }
                                //Equals to a number
                                MathParser parser2 = new MathParser();
                                Double solution2 = parser2.Parse(Constraint, false);
                                if (solution != solution2)
                                {
                                    RowGood = false;
                                }
                                continue;
                            }
                            //Second one 
                            else if (Relationship == ">")
                            {
                                MathParser parser = new MathParser();
                                Double solution = parser.Parse(equation, false);
                                //only 1 situation exists
                                //Equals to a number
                                MathParser parser2 = new MathParser();
                                Double solution2 = parser2.Parse(Constraint, false);
                                if (solution <= solution2)
                                {
                                    RowGood = false;
                                }
                                continue;
                            }
                            //Third one
                            else if (Relationship == "<")
                            {
                                MathParser parser = new MathParser();
                                Double solution = parser.Parse(equation, false);
                                //only 1 situation exists
                                //Equals to a number
                                MathParser parser2 = new MathParser();
                                Double solution2 = parser2.Parse(Constraint, false);
                                if (solution >= solution2)
                                {
                                    RowGood = false;
                                }
                                continue;
                            }
                            //Forth one
                            else if (Relationship == "≥")
                            {
                                MathParser parser = new MathParser();
                                Double solution = parser.Parse(equation, false);
                                //only 1 situation exists
                                //Equals to a number
                                MathParser parser2 = new MathParser();
                                Double solution2 = parser2.Parse(Constraint, false);
                                if (solution < solution2)
                                {
                                    RowGood = false;
                                }
                                continue;
                            }
                            //Fifth one
                            else if (Relationship == "≤")
                            {
                                MathParser parser = new MathParser();
                                Double solution = parser.Parse(equation, false);
                                //only 1 situation exists
                                //Equals to a number
                                MathParser parser2 = new MathParser();
                                Double solution2 = parser2.Parse(Constraint, false);
                                if (solution > solution2)
                                {
                                    RowGood = false;
                                }
                                continue;
                            }
                            //Last one
                            else if (Relationship == "≠")
                            {
                                MathParser parser = new MathParser();
                                Double solution = parser.Parse(equation, false);
                                //only 1 situation exists
                                //Equals to a number
                                MathParser parser2 = new MathParser();
                                Double solution2 = parser2.Parse(Constraint, false);
                                if (solution == solution2)
                                {
                                    RowGood = false;
                                }
                                continue;
                            }
                        }
                        if (RowGood)
                            CHy.Add(Ans[j]);
                        else
                            RowGood = true;
                    }
                }
                else
                {
                    CHy.AddRange(Ans);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Additional Rule Table. Please refer to the help section and check your input. Error Code:" + ex);
            }

            for (int i = 0; i < CHy.Count(); i++)
            {
                CHy[i].elementIDs.Clear();
                CHy[i].MoleNames.Clear();

                if (i == CHy.Count() - 1)
                {
                    CHy[0].elementIDs = elementIDs;
                }
            }
            return CHy;
        }
        public String converConstraints(comphypo CH, String Constraint)
        {
            String newConstraint = "";
            //Use the cleanbound function to clean up all letters in the bounds.
                foreach (char i in Constraint)
                {
                    if (char.IsUpper(i))
                    {
                        newConstraint = newConstraint + "(" + CH.eqCount[Convert.ToString(i)] + ")";
                    }
                    else
                    {
                        newConstraint = newConstraint + Convert.ToString(i);
                    }
                }
            return newConstraint;
        }
        private List<comphypo> addAdducts(List<comphypo> CHy, generatorData GD)
        {
            List<string> elementIDs = new List<string>();
            List<string> molname = new List<string>();
            for (int j = 0; j < CHy.Count(); j++ )
            {
                if (CHy[j].elementIDs.Count > 0)
                {
                    for (int i = 0; i < CHy[j].elementIDs.Count(); i++)
                    {
                        elementIDs.Add(CHy[j].elementIDs[i]);
                    }
                    for (int i = 0; i < CHy[j].MoleNames.Count(); i++)
                    {
                        molname.Add(CHy[j].MoleNames[i]);
                    }
                    break;
                }
            }

            Double adductMas = adductMass(GD);
            Int32 adductLB = new Int32();
            Int32 adductUB = new Int32();
            try
            {
                adductLB = Convert.ToInt32(GD.Modification[2]);
                adductUB = Convert.ToInt32(GD.Modification[3]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lower bound and Upper bound in the Modification list must be integers. Error:" + ex);
                this.Close();
            }
            adductcomp adc = getAdductCompo(GD);

            //update elementID list
            for (int i = 0; i < adc.elementIDs.Count(); i++)
            {
                if (!(elementIDs.Any(a => a.Contains(adc.elementIDs[i]))))
                {
                    elementIDs.Add(adc.elementIDs[i]);
                    foreach (comphypo CH in CHy)
                    {
                        CH.elementAmount.Add(0);
                    }
                }
            }

            List<comphypo> supFinalAns = new List<comphypo>();
            for (int i = 0; i < CHy.Count(); i++)
            {
                if (adductLB != 0)
                {
                    comphypo temp = new comphypo();
                    temp.elementAmount = CHy[i].elementAmount;
                    temp.AdductNum = 0;
                    temp.eqCount = CHy[i].eqCount;
                    temp.MW = CHy[i].MW;
                    supFinalAns.Add(temp);
                }

                for (int j = adductLB; j <= adductUB; j++)
                {
                    comphypo temp = new comphypo();
                    for (int k = 0; k < CHy[i].elementAmount.Count(); k++)
                    {
                        temp.elementAmount.Add(CHy[i].elementAmount[k]);
                    }
                    for (int l = 0; l < adc.elementAmount.Count(); l++)
                    {
                        temp.elementAmount[elementIDs.IndexOf(adc.elementIDs[l])] = CHy[i].elementAmount[elementIDs.IndexOf(adc.elementIDs[l])] + j * adc.elementAmount[l];
                     }
                    temp.AdductNum = j;
                    temp.eqCount = CHy[i].eqCount;
                    temp.MW = CHy[i].MW + j * adductMas;
                    supFinalAns.Add(temp);
                }
            }
            for (int i = 0; i < supFinalAns.Count(); i++)
            {
                supFinalAns[i].elementIDs.Clear();
                supFinalAns[i].MoleNames.Clear();

                if (i == supFinalAns.Count() - 1)
                {
                    supFinalAns[0].elementIDs = elementIDs;
                    supFinalAns[0].MoleNames = molname;
                }
            }

            return supFinalAns;
        }
        private List<comphypo> checkNegative(List<comphypo> supFinalAns)
        {
            List<comphypo> UltFinalAns = new List<comphypo>();
            for (int i = 0; i < supFinalAns.Count; i++)
            {
                Boolean hasnegative = false;
                int sumOfElements = 0;
                for (int j = 0; j < supFinalAns[i].elementAmount.Count; j++ )
                {
                    sumOfElements = sumOfElements + supFinalAns[i].elementAmount[j];
                    if (supFinalAns[i].elementAmount[j] < 0)
                    {
                        hasnegative = true;
                    }
                }
                //If all add up to zero, well, delete the line also.
                if (sumOfElements == 0)
                    hasnegative = true;
                if (hasnegative == false)
                {
                    //Finally, need to initialize the glycopeptide variables to prevent nullreference error;
                    supFinalAns[i].numGly = 0;
                    supFinalAns[i].PepSequence = "";
                    supFinalAns[i].PepModification = "";
                    supFinalAns[i].MissedCleavages = 0;
                    supFinalAns[i].StartAA = 0;
                    supFinalAns[i].EndAA = 0;
                    UltFinalAns.Add(supFinalAns[i]);
                }
            }
            return UltFinalAns;
        }
        private List<comphypo> cleanColumns(List<comphypo> Ans, generatorData GD)
        {
            List<string> molnames = new List<string>();
            List<int> index = new List<int>();
            if (Ans.FindIndex(item => item.eqCount["A"] > 0) >= 0)
                index.Add(0);
            if (Ans.FindIndex(item => item.eqCount["B"] > 0) >= 0)
                index.Add(1);
            if (Ans.FindIndex(item => item.eqCount["C"] > 0) >= 0)
                index.Add(2);
            if (Ans.FindIndex(item => item.eqCount["D"] > 0) >= 0)
                index.Add(3);
            if (Ans.FindIndex(item => item.eqCount["E"] > 0) >= 0)
                index.Add(4);
            if (Ans.FindIndex(item => item.eqCount["F"] > 0) >= 0)
                index.Add(5);
            if (Ans.FindIndex(item => item.eqCount["G"] > 0) >= 0)
                index.Add(6);
            if (Ans.FindIndex(item => item.eqCount["H"] > 0) >= 0)
                index.Add(7);
            if (Ans.FindIndex(item => item.eqCount["I"] > 0) >= 0)
                index.Add(8);
            if (Ans.FindIndex(item => item.eqCount["J"] > 0) >= 0)
                index.Add(9);
            if (Ans.FindIndex(item => item.eqCount["K"] > 0) >= 0)
                index.Add(10);
            if (Ans.FindIndex(item => item.eqCount["L"] > 0) >= 0)
                index.Add(11);
            if (Ans.FindIndex(item => item.eqCount["M"] > 0) >= 0)
                index.Add(12);
            if (Ans.FindIndex(item => item.eqCount["N"] > 0) >= 0)
                index.Add(13);
            if (Ans.FindIndex(item => item.eqCount["O"] > 0) >= 0)
                index.Add(14);
            if (Ans.FindIndex(item => item.eqCount["P"] > 0) >= 0)
                index.Add(15);
            if (Ans.FindIndex(item => item.eqCount["Q"] > 0) >= 0)
                index.Add(16);
            Dictionary<Int32, String> toLetter = new Dictionary<Int32, String>();
            toLetter.Add(0, "A");
            toLetter.Add(1, "B");
            toLetter.Add(2, "C");
            toLetter.Add(3, "D");
            toLetter.Add(4, "E");
            toLetter.Add(5, "F");
            toLetter.Add(6, "G");
            toLetter.Add(7, "H");
            toLetter.Add(8, "I");
            toLetter.Add(9, "J");
            toLetter.Add(10, "K");
            toLetter.Add(11, "L");
            toLetter.Add(12, "M");
            toLetter.Add(13, "N");
            toLetter.Add(14, "O");
            toLetter.Add(15, "P");
            toLetter.Add(16, "Q");

            for (int i = 0; i < index.Count(); i++)
            {
                molnames.Add(GD.comTable[index[i]].Molecule);
            }
            for (int i = 0; i < Ans.Count(); i++)
            {
                List<int> eqCountstemp = new List<int>();
                for (int j = 0; j < index.Count(); j++)
                {
                    int value = new int();
                    if (Ans[i].eqCount.TryGetValue(toLetter[index[j]], out value))
                    {
                        eqCountstemp.Add(Convert.ToInt32(value));
                    }
                    else
                        eqCountstemp.Add(0);

                }
                Ans[i].eqCounts = eqCountstemp;
            }
            for (int i = 0; i < Ans.Count(); i++)
            {
                if (Ans[i].elementIDs.Count() > 0)
                {
                    Ans[i].MoleNames = molnames;
                    break;
                }
            }
            return Ans;
        }
        private List<comphypo> addWater(List<comphypo> Ans)
        {
            periodicTable PT = new periodicTable();
            List<string> elementIDs = new List<string>();
            List<string> molname = new List<string>();
            for (int j = 0; j < Ans.Count(); j++)
            {
                if (Ans[j].elementIDs.Count > 0)
                {
                    for (int i = 0; i < Ans[j].elementIDs.Count(); i++)
                    {
                        elementIDs.Add(Ans[j].elementIDs[i]);
                    }
                    for (int i = 0; i < Ans[j].MoleNames.Count(); i++)
                    {
                        molname.Add(Ans[j].MoleNames[i]);
                    }
                    break;
                }
            }

            string H = "H";
            string O = "O";
            string Water = "Water";
            if (!elementIDs.Any(H.Contains))
            {
                elementIDs.Add(H);
                foreach (comphypo CH in Ans)
                {
                    CH.elementAmount.Add(0);
                }
            }
            if (!elementIDs.Any(O.Contains))
            {
                elementIDs.Add(O);
                foreach (comphypo CH in Ans)
                {
                    CH.elementAmount.Add(0);
                }
            }
            if (!molname.Any(Water.Contains))
            {
                molname.Add(Water);
                foreach (comphypo CH in Ans)
                {
                    CH.eqCounts.Add(0);
                }
            }
            foreach (comphypo CH in Ans)
            {

                CH.elementAmount[elementIDs.IndexOf("H")] = CH.elementAmount[elementIDs.IndexOf("H")] + 2;
                CH.elementAmount[elementIDs.IndexOf("O")] = CH.elementAmount[elementIDs.IndexOf("O")] + 1;

                CH.MW = CH.MW + PT.getMass("H") * 2 + PT.getMass("O");
                CH.eqCounts[molname.IndexOf("Water")] = CH.eqCounts[molname.IndexOf("Water")] + 1;
            }
            for (int i = 0; i < Ans.Count(); i++)
            {
                Ans[i].elementIDs.Clear();
                Ans[i].MoleNames.Clear();
 
                if (i == Ans.Count() - 1)
                {
                    Ans[0].elementIDs = elementIDs;
                    Ans[0].MoleNames = molname;
                }
            }

            return Ans;
        }
        private List<comphypo> removeZeroElements(List<comphypo> Ans)
        {
            List<string> elementIDs = new List<string>();
            List<string> molname = new List<string>();
            for (int j = 0; j < Ans.Count(); j++)
            {
                if (Ans[j].elementIDs.Count > 0)
                {
                    for (int i = 0; i < Ans[j].elementIDs.Count(); i++)
                    {
                        elementIDs.Add(Ans[j].elementIDs[i]);
                    }
                    for (int i = 0; i < Ans[j].MoleNames.Count(); i++)
                    {
                        molname.Add(Ans[j].MoleNames[i]);
                    }
                    break;
                }
            }


            List<bool> NonZeroRows = new List<bool>();
            foreach (var i in Ans[0].elementAmount)
            {
                NonZeroRows.Add(false);
            }
            foreach (comphypo CH in Ans)
            {
                for (int i = 0; i < CH.elementAmount.Count(); i++ )
                {
                    if (CH.elementAmount[i] != 0)
                    {
                        NonZeroRows[i] = true;
                    }
                }
            }
            List<int> NonZeroRowsIndex = new List<int>();
            for (int i = 0; i < NonZeroRows.Count(); i++)
            {
                if (NonZeroRows[i])
                    NonZeroRowsIndex.Add(i);
            }
            for (int j = 0; j < Ans.Count(); j++ )
            {
                List<int> elementAmount = new List<int>();
                for (int i = 0; i < NonZeroRowsIndex.Count(); i++)
                {
                    elementAmount.Add(Ans[j].elementAmount[NonZeroRowsIndex[i]]);
                }
                Ans[j].elementAmount = elementAmount;                
            }
            List<string> elementIDs2 = new List<string>();

            for (int i = 0; i < Ans.Count(); i++)
            {
                Ans[i].elementIDs.Clear();
                Ans[i].MoleNames.Clear();

                if (i == Ans.Count() - 1)
                {
                    for (int j = 0; j < NonZeroRowsIndex.Count(); j++)
                    {
                        elementIDs2.Add(elementIDs[NonZeroRowsIndex[j]]);
                    }
                    Ans[0].elementIDs = elementIDs2;
                    Ans[0].MoleNames = molname;
                }
            }


            return Ans;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //This function is used by ConvertLetters to clean the letters in the lower and upper bound areas. It only clean it once, so if you want to clear the area of all letters, put this into a loop.
        private List<String> cleanBounds(List<String> Bounds, generatorData GD)
        {

            List<String> FinalLS = new List<String>();
            List<String> Ans = new List<String>();
            foreach (String bound in Bounds)
            {
                foreach (char i in bound)
                {
                    if (char.IsUpper(i))
                    {
                        if (FinalLS.Count == 0)
                        {
                            foreach (String bou in getBound(i, GD.comTable))
                            {
                                FinalLS.Add("(" + bou + ")");
                            }
                        }
                        else
                        {
                            List<String> someLS = new List<String>();
                            foreach (String bou in getBound(i, GD.comTable))
                            {
                                for (int j = 0; j < FinalLS.Count; j++)
                                {
                                    someLS.Add(FinalLS[j] + "(" + bou + ")");
                                }
                            }
                            FinalLS.Clear();
                            FinalLS = someLS;
                        }
                    }
                    else
                    {
                        if (FinalLS.Count == 0)
                        {
                            FinalLS.Add(Convert.ToString(i));
                        }
                        else
                        {
                            List<String> someLS = new List<String>();
                            for (int j = 0; j < FinalLS.Count; j++)
                            {
                                someLS.Add(FinalLS[j] + i);
                            }
                            FinalLS.Clear();
                            FinalLS = someLS;
                        }
                    }
                }
                Ans.AddRange(FinalLS);
                FinalLS.Clear();
            }
            return Ans;
        }

        //This function is used by generateHypo to get the mass of a composition in the composition table.
        public Double getcompMass(compTable CT, List<string> elementIDs)
        {
            Double Mass = 0;
            periodicTable PT = new periodicTable();
            for (int i = 0; i < CT.elementAmount.Count(); i++)
            {
                Mass = Mass + CT.elementAmount[i] * PT.getMass(elementIDs[i]);
            }
            return Mass;
        }

        //This class helps the generateHypo class by outputting bounds of a molecule by its letter name.
        private List<String> getBound(char letter, List<compTable> CT)
        {
            List<String> Bounds = new List<String>();
            foreach (compTable cT in CT)
            {
                if (Convert.ToChar(cT.Letter) == letter)
                {
                    Bounds = cT.Bound;
                    break;
                }
            }
            if (Bounds.Count == 0)
            {
                MessageBox.Show("A character is not readable in the Lower Bounds or Upper Bounds. Please check your input.");
                return Bounds;
            }
            return Bounds;
        }

        //This class helps the generateHypo classin the Additional Rules section by translating letters in the artable into numbers.
        private String translet(comphypo one, arTable ar)
        {
            String Ans = "";
            foreach (char i in ar.Formula)
            {
                if (char.IsUpper(i))
                {
                    try
                    {
                        Ans = Ans + Convert.ToString(one.eqCount[Convert.ToString(i)]);
                    }
                    catch
                    {
                        MessageBox.Show("Invalid letter in the additional rules table.");
                    }
                }
                else
                {
                    Ans = Ans + Convert.ToString(i);
                }
            }
            return Ans;
        }

        //This class calculates delta adduct mass.
        public Double adductMass(composition.generatorData GD)
        {
            String adduct = GD.Modification[0];
            String replacement = GD.Modification[1];
            //Regex for the capital letters
            string regexpattern = @"[A-Z]{1}[a-z]?[a-z]?\d?";
            MatchCollection adducts = Regex.Matches(adduct, regexpattern);
            MatchCollection replacements = Regex.Matches(replacement, regexpattern);
            //Regex for the element;
            string elementregexpattern = @"[A-Z]{1}[a-z]?[a-z]?(\(([^)]*)\))?";
            //Regex for the number of the element.
            string numberregexpattern = @"\d+";
            Double adductmass = 0;
            Double replacementmass = 0;
            periodicTable pTable = new periodicTable();
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
            foreach (Match el in replacements)
            {
                String element = Convert.ToString(Regex.Match(Convert.ToString(el), elementregexpattern));
                String snumber = Convert.ToString(Regex.Match(Convert.ToString(el), numberregexpattern));
                Int32 number = 0;
                if (snumber == String.Empty)
                {
                    number = 1;
                }
                else
                {
                    number = Convert.ToInt32(snumber);
                }
                replacementmass = replacementmass + number * pTable.getMass(element);
            }

            //Finally, subtract them and obtain delta mass.
            Double dMass = adductmass - replacementmass;
            return dMass;
        }

        //This class outputs the elemental composition change from the adduct and replacement.
        private adductcomp getAdductCompo(composition.generatorData GD)
        {
            String adduct = GD.Modification[0];
            String replacement = GD.Modification[1];
            //Regex for the capital letters
            string regexpattern = @"([A-Z]{1}[a-z]?[a-z]?){1}(\({1}\d+\){1})?\d?";
            MatchCollection adducts = Regex.Matches(adduct, regexpattern);
            MatchCollection replacements = Regex.Matches(replacement, regexpattern);
            //Regex for the element;
            string elementregexpattern = @"([A-Z]{1}[a-z]?[a-z]?){1}(\({1}\d+\){1})?";
            //Regex for the number of the element.
            string numberregexpattern = @"\d+$";

            adductcomp adc = new adductcomp();

            //For each element in adducts.
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
                try
                {
                    adc.elementAmount[adc.elementIDs.IndexOf(element)] = adc.elementAmount[adc.elementIDs.IndexOf(element)] + number;
                }
                catch
                {
                    adc.elementIDs.Add(element);
                    adc.elementAmount.Add(number);
                }

            }
            //For each element in replacements.
            foreach (Match ele in replacements)
            {
                String el = Convert.ToString(ele);
                String element = Convert.ToString(Regex.Match(el, elementregexpattern));
                String snumber = Convert.ToString(Regex.Match(el, numberregexpattern));
                Int32 number = 0;
                if (snumber == String.Empty)
                {
                    number = 1;
                }
                else
                {
                    number = Convert.ToInt32(snumber);
                }

                try
                {
                    adc.elementAmount[adc.elementIDs.IndexOf(element)] = adc.elementAmount[adc.elementIDs.IndexOf(element)] - number;
                }
                catch
                {
                    adc.elementIDs.Add(element);
                    adc.elementAmount.Add(number);
                }
            }
            //Finally, subtract them and obtain the answer.

            return adc;
        }

        //Accessor Methods:#####################################################################################
        //These are the classes that help store variables from the datagridviews in tag1.
        public class compTable
        {
            public compTable()
            {
                elementIDs = new List<string>();
                elementAmount = new List<int>();
            }
            public String Letter;
            public String Molecule;
            //element ID and amount are used to record the element compositions.
            public List<string> elementIDs;
            public List<int> elementAmount;
            public List<String> Bound;
        }
        public class arTable
        {
            public String Formula;
            public String Relationship;
            public String Constraint;
        }
        public class generatorData
        {
            public List<compTable> comTable;
            public List<arTable> aTable;
            public String[] Modification;

            public double AdductMassDelta(){
                String adduct = this.Modification[0];
                String replacement = this.Modification[1];
                //Regex for the capital letters
                string regexpattern = @"[A-Z]{1}[a-z]?[a-z]?\d?";
                MatchCollection adducts = Regex.Matches(adduct, regexpattern);
                MatchCollection replacements = Regex.Matches(replacement, regexpattern);
                //Regex for the element;
                string elementregexpattern = @"[A-Z]{1}[a-z]?[a-z]?(\(([^)]*)\))?";
                //Regex for the number of the element.
                string numberregexpattern = @"\d+";
                Double adductmass = 0;
                Double replacementmass = 0;
                periodicTable pTable = new periodicTable();
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
                foreach (Match el in replacements)
                {
                    String element = Convert.ToString(Regex.Match(Convert.ToString(el), elementregexpattern));
                    String snumber = Convert.ToString(Regex.Match(Convert.ToString(el), numberregexpattern));
                    Int32 number = 0;
                    if (snumber == String.Empty)
                    {
                        number = 1;
                    }
                    else
                    {
                        number = Convert.ToInt32(snumber);
                    }
                    replacementmass = replacementmass + number * pTable.getMass(element);
                }

                //Finally, subtract them and obtain delta mass.
                Double dMass = adductmass - replacementmass;
                return dMass;
        }

        }

        //This class stores one row of data in the composition hypothesis result.
        public class comphypo
        {
            public comphypo()
            {
                eqCounts = new List<int>();
                elementIDs = new List<string>();
                elementAmount = new List<int>();
                eqCount = new Dictionary<string,int>();
                MoleNames = new List<string>();
                StartAA = 0;
                EndAA = 0;
            }
            //element ID and amount are used to record the element compositions.
            public List<string> elementIDs;
            public List<int> elementAmount;
            public String compoundCompo;
            public Int32 AdductNum;
            public String AddRep;
            //eqCount are the number of each element
            public Dictionary<String, Int32> eqCount;
            //eqCounts are the number of each molecule
            public List<int> eqCounts;
            public Double MW;
            public List<string> MoleNames;
            //columns for glycopeptides
            public String PepModification;
            public String PepSequence;
            public Int32 MissedCleavages;
            public Int32 numGly;
            public Int32 StartAA;
            public Int32 EndAA;
            //For falseDataset
            public Boolean TrueOrFalse;

            public override string ToString()
            {
                string repr = "CH:ElementIDs[{0}]\nCH:MolNames[{1}]";

                string elementIdString = string.Join(",", elementIDs) + " (" + elementIDs.Count().ToString() + ")";
                string molnamesString = string.Join(",", MoleNames) + " (" + MoleNames.Count().ToString() + ")";

                return string.Format(repr, elementIDs, molnamesString) ;
            }

            /// <summary>
            /// Create a deep copy of the comphypo object.
            /// </summary>
            /// <returns></returns>
            public comphypo Clone()
            {
                comphypo dup = new comphypo();
                dup.compoundCompo = this.compoundCompo;
                dup.AdductNum = this.AdductNum;
                dup.AddRep = this.AddRep;
                dup.PepModification = this.PepModification;
                dup.PepSequence = this.PepSequence;
                dup.MissedCleavages = this.MissedCleavages;
                dup.numGly = this.numGly;
                dup.StartAA = this.StartAA;
                dup.EndAA = this.EndAA;
                dup.MW = this.MW;
                dup.TrueOrFalse = this.TrueOrFalse;

                foreach (string id in this.elementIDs)
                {
                    dup.elementIDs.Add(id);
                }

                foreach (int amount in this.elementAmount)
                {
                    dup.elementAmount.Add(amount);
                }

                foreach (string molName in this.MoleNames)
                {
                    dup.MoleNames.Add(molName);
                }

                foreach (int eq in this.eqCounts)
                {
                    dup.eqCounts.Add(eq);
                }

                foreach(KeyValuePair<String, Int32> kvp in this.eqCount){
                    dup.eqCount.Add(kvp.Key, kvp.Value);
                }

                return dup;
            }

            /// <summary>
            /// Create a deep copy of a list of comphypo objects.
            /// </summary>
            /// <param name="hypothesis"></param>
            /// <returns></returns>
            public static List<comphypo> CloneList(List<comphypo> hypothesis)
            {
                List<comphypo> dup = new List<comphypo>();
                foreach (comphypo row in hypothesis)
                {
                    dup.Add(row.Clone());
                }
                return dup;
            }
        }

        //This class stores the elemental composition from the adduct.
        public class adductcomp
        {
            public adductcomp()
            {
                elementIDs = new List<string>();
                elementAmount = new List<int>();
            }
            public List<string> elementIDs;
            public List<int> elementAmount;
        }

        public class PPMSD : GlycReSoft.CompositionHypothesis.MSDigestPeptide
        {
            //public Boolean selected;
            //public Int32 number;
            //public Double Mass;
            //public Int32 Charge;
            //public String Modifications;
            //public Int32 StartAA;
            //public Int32 EndAA;
            //public Int32 MissedCleavages;
            //public String PreviousAA;
            //public String Sequence;
            //public String NextAA;
            //public Int32 numGly;

            public PPMSD()
                : base()
            {

            }

            public PPMSD(MSDigestPeptide other)
                : base()
            {
                this.selected = other.selected;
                this.number = other.number;
                this.Mass = other.Mass;
                this.Charge = other.Charge;
                this.Modifications = other.Modifications;
                this.StartAA = other.StartAA;
                this.EndAA = other.EndAA;
                this.MissedCleavages = other.MissedCleavages;
                this.PreviousAA = other.PreviousAA;
                this.Sequence = other.Sequence;
                this.NextAA = other.NextAA;
                this.numGly = other.numGly;
            }


            public override string ToString()
            {
                return "PPMSD:" + base.ToString();
            }
        }

        class BackgroundHypothesisFromDBArgument
        {
            public List<comphypo> Hypothesis;
            public generatorData GD;
            public bool HasAdduct;
            public String AdductModFormula;
            public String AdductReplaceFormula;
            public int AdductLowerBound;
            public int AdductUpperBound;


        }

        private void GeneratePrecomputedHypothesisButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Button");
            string database = null;
            if (this.UseAllHumanGlycomeDBRadio.Enabled)
            {
                database = Properties.Resources.Human_GlycomeDB_All;
            }
            else if(this.UseNLinkedHumanGlycomeDBRadio.Enabled)
            {
                database = Properties.Resources.Human_GlycomeDB_NLinked;
            }
            else if (this.UseOLinkedHumanGlycomeDBRadio.Enabled)
            {
                database = Properties.Resources.Human_GlycomeDB_OLinked;
            }
            else if (this.UseAllMammalianGlycomeDBRadio.Enabled)
            {
                database = Properties.Resources.Mammalian_GlycomeDB_All;
            }
            else if (this.UseNLinkedMammalianGlycomeDBRadio.Enabled)
            {
                database = Properties.Resources.Mammalian_GlycomeDB_NLinked;
            }
            else if (this.UseOLinkedMammalianGlycomeDBRadio.Enabled)
            {
                database = Properties.Resources.Mammalian_GlycomeDB_OLinked;
            }
            else
            {
                MessageBox.Show("You must have selected a precomputed Database from the radio option list above.", "Select a Database");
                return;
            }
            List<comphypo> compHypothesis = getCompHypoFromStream(database.ToStream());
            BackgroundWorker backgroundHypothesisFromDBWorker = new BackgroundWorker();
            backgroundHypothesisFromDBWorker.DoWork += backgroundHypothesisFromDBWorker_DoWork;
            backgroundHypothesisFromDBWorker.RunWorkerCompleted += backgroundHypothesisFromDBWorker_RunWorkerCompleted;          

            //foreach (comphypo c in compHypothesis)
            //{
            //    Console.WriteLine(c);
            //}
            Console.WriteLine(compHypothesis.Count());
            
            ///Check if the UI indicates there should be adducts. If so, apply
            ///the adduct Modification transformation over the range given by the bounds.
            ///Otherwise generate the hypothesis from the resource stream unmodified.
            bool hasAdduct = true;
            generatorData GD = new generatorData();
            GD.Modification = new string[] { "", "0" };
            String adductModFormula = this.textBox35.Text;
            String adductReplaceFormula = this.textBox44.Text;
            String adductLowerBoundStr = this.textBox46.Text;
            String adductUpperBoundStr = this.textBox45.Text;
            int adductLowerBoundVal = 0;
            int adductUpperBoundVal = 0;
            try
            {
                bool hasAdductFormula = !((adductModFormula == null) || (adductModFormula == ""));
                bool hasReplaceFormula = !((adductReplaceFormula == null) || (adductReplaceFormula == ""));
                if (hasAdduct && hasReplaceFormula)
                {
                    adductLowerBoundVal = Convert.ToInt32(adductLowerBoundStr);
                    adductUpperBoundVal = Convert.ToInt32(adductUpperBoundStr);
                    if (adductUpperBoundVal == 0) throw new Exception("Adduct Upper Bound Can't Be 0");
                }
                hasAdduct = hasAdduct && hasReplaceFormula;
                
            }
            catch
            {
                Console.WriteLine("No Adducts");
                hasAdduct = false;
            }
            backgroundHypothesisFromDBWorker.RunWorkerAsync(
                new BackgroundHypothesisFromDBArgument{ 
                    Hypothesis = compHypothesis,                      
                    HasAdduct = hasAdduct,
                    GD = GD,
                    AdductModFormula = adductModFormula,
                    AdductReplaceFormula = adductReplaceFormula,
                    AdductLowerBound = adductLowerBoundVal,
                    AdductUpperBound = adductUpperBoundVal
                      
            });
            //if (hasAdduct)
            //{
            //    Console.WriteLine("Doing Adducts");   
            //    GD.Modification = new string[] { adductModFormula, adductReplaceFormula };
            //    string addrepStr = adductModFormula + "/" + adductReplaceFormula;
            //    double adductMassDelta = GD.AdductMassDelta();
            //    List<comphypo> modifiedHypothesis = new List<comphypo>();
            //    for (int c = adductLowerBoundVal; c < adductUpperBoundVal; c++)
            //    {
            //        List<comphypo> unmodifiedCopy = comphypo.CloneList(compHypothesis);
            //        foreach (comphypo row in unmodifiedCopy)
            //        {
            //            row.AddRep = addrepStr;
            //            row.AdductNum = c;
            //            row.MW += adductMassDelta * c;
            //        }
            //        modifiedHypothesis.AddRange(unmodifiedCopy);
            //    }
            //    compHypothesis = modifiedHypothesis;
            //} 
            //

            //Console.WriteLine(compHypothesis.Count());

            //theComhypoOnTab2 = genDT(compHypothesis, GD);
            //dataGridView2.DataSource = theComhypoOnTab2;
            //button6.Enabled = true;

            //tabControl1.SelectedTab = tabPage2;
        }
            
        void backgroundHypothesisFromDBWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            theComhypoOnTab2 = e.Result as DataTable;
            dataGridView2.DataSource = theComhypoOnTab2;
            button6.Enabled = true;

            tabControl1.SelectedTab = tabPage2;
        }

        void backgroundHypothesisFromDBWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            Console.WriteLine(e.Argument);
            var x = e.Argument as BackgroundHypothesisFromDBArgument;
            if (x.HasAdduct)
            {
                x.GD.Modification = new string[] { x.AdductModFormula, x.AdductReplaceFormula };
                string addrepStr = x.AdductModFormula + "/" + x.AdductReplaceFormula;
                double adductMassDelta = x.GD.AdductMassDelta();
                List<comphypo> modifiedHypothesis = new List<comphypo>();
                for (int c = x.AdductLowerBound; c <= x.AdductUpperBound; c++)
                {
                    List<comphypo> unmodifiedCopy = comphypo.CloneList(x.Hypothesis);
                    foreach (comphypo row in unmodifiedCopy)
                    {
                        row.AddRep = addrepStr;
                        row.AdductNum = c;
                        row.MW += adductMassDelta * c;
                    }
                    modifiedHypothesis.AddRange(unmodifiedCopy);
                }
                x.Hypothesis = modifiedHypothesis;
            }
            e.Result = genDT(x.Hypothesis, x.GD); ;
            //theComhypoOnTab2 = 
            //dataGridView2.DataSource = theComhypoOnTab2;
            //button6.Enabled = true;

            //tabControl1.SelectedTab = tabPage2;
        }



    }
}