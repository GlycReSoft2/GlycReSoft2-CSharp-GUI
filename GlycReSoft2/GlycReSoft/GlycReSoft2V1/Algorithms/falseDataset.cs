using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathParserTK;
using System.IO;
using System.Windows.Forms;

namespace GlycReSoft
{
    class falseDataset
    {
        static Random rnd = new Random();
        //obtain a random false list for ROC generation, using the input file (comptable file) as base.
        public List<composition.comphypo> genFalse(String path, List<composition.comphypo> trueComhypo, OpenFileDialog oFDPPMSD)
        {
            //table is used to store the data before the calculation
            composition.generatorData DATA = new composition.generatorData();
            List<composition.compTable> GD = new List<composition.compTable>();
            //This function is used to read the cpos file and translate it to generatorData format. Converting all letters in bounds to numbers while its at it.
            DATA = extractFile(path);
            
            //The Final Result will have approximately as many lines as the user input composition hypothesis.
            int usingNumberofFiles = 20;
            List<composition.comphypo> FinalAns = new List<composition.comphypo>();
            for (int i = 0; i < usingNumberofFiles; i++)
            {
                List<composition.comphypo> FalseSet = obtainFalse(DATA, trueComhypo.Count() / usingNumberofFiles, oFDPPMSD);
                FinalAns.AddRange(FalseSet);
            }
            FinalAns.AddRange(trueComhypo);
            return FinalAns;
        }
        //This function is used to read the cpos file and translate it to generatorData format.
        private composition.generatorData extractFile(String path)
        {
            //table is used to store the data before the calculation
            composition.generatorData DATA = new composition.generatorData();
            List<composition.compTable> GD = new List<composition.compTable>();
            try
            {
                FileStream reading = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader readcompo = new StreamReader(reading);
                //Read the first line to throw the column names:
                String Line1 = readcompo.ReadLine();
                String[] headers = Line1.Split(',');
                int sh = 2;
                bool moreElements = true;
                List<string> elementIDs = new List<string>();
                while (moreElements)
                {
                    if (headers[sh] != "Lower Bound")
                    {
                        elementIDs.Add(headers[sh]);
                        sh++;
                    }
                    else
                    {
                        moreElements = false;
                    }

                }
                bool firstRow = true;
                while (readcompo.Peek() >= 0)
                {
                    String Line = readcompo.ReadLine();
                    String[] eachentry = Line.Split(',');
                    if (eachentry[0] == "Modification%^&!@#*()iop")
                        break;
                    composition.compTable CT = new composition.compTable();
                    CT.Letter = eachentry[0];
                    CT.Molecule = eachentry[1];
                    if (firstRow)
                    {
                        CT.elementIDs = elementIDs;
                        firstRow = false;
                    }
                    for (int i = 0; i < elementIDs.Count(); i++)
                    {
                        CT.elementAmount.Add(Convert.ToInt32(eachentry[2 + i]));
                    }
                        //Random the lower bound and upper bound to get random composition hypothesis
                    CT.Bound = new List<String>();
                    CT.Bound.Add(eachentry[2 + elementIDs.Count()]);
                    CT.Bound.Add(eachentry[2 + elementIDs.Count() + 1]);
                    GD.Add(CT);
                }
                String Line2 = readcompo.ReadLine();
                String[] eachentry2 = Line2.Split(',');
                DATA.comTable = GD;
                DATA.Modification = eachentry2;
                readcompo.Close();
                reading.Close();
            }
            catch (Exception compoex)
            {
                MessageBox.Show("Error in loading GlycanCompositions Table (cpos). Error:" + compoex);
            }

            DATA.aTable = new List<composition.arTable>();
            DATA.comTable = GD;
            return DATA;
        }
        private List<composition.comphypo> obtainFalse(composition.generatorData GD, int numofLines, OpenFileDialog oFDPPMSD)
        {
            //obtain a random list with the correct bounds but the elemental composition randomized:
            List<composition.compTable> randomCT = randomList(GD.comTable);
            composition comp = new composition();
            GD.comTable = randomCT;
            List<composition.comphypo> CTAns = comp.genHypo(GD);
            List<int> rangeone = Enumerable.Range(0, CTAns.Count()).ToList();
            rangeone.Shuffle();
            //restricting them to numperlist per list, which is half of the number of lines of the original composition hypothesis divided by numofFile            Int32 numperlist = (numofLines / 2) / numofFiles;
            List<composition.comphypo> finalAns = new List<composition.comphypo>();
            for (int j = 0; j < numofLines; j++)
            {
                finalAns.Add(CTAns[rangeone[j]]);
            }
            Int32 Length = finalAns.Count();
            //Next, mix it with Protein Prospector MS-digest file, if it exists.
            Stream mystream = null;
            try
            {
                if ((mystream = oFDPPMSD.OpenFile()) != null)
                {
                    if (!String.IsNullOrEmpty(oFDPPMSD.FileName))
                    {
                        List<composition.comphypo> FinalAns = getPPhypo(finalAns, finalAns, genFalsePPMSD(oFDPPMSD.FileName));
                        List<int> rangethree = Enumerable.Range(0, FinalAns.Count).ToList();
                        rangethree.Shuffle();
                        finalAns.Clear();
                        for (int i = 0; i < Length; i++)
                        {
                            finalAns.Add(FinalAns[rangethree[i]]);
                        }
                    }
                }
            }
            catch
            {
            }
            for (int i = 0; i < finalAns.Count(); i++)
            {
                finalAns[i].TrueOrFalse = false;
            }

            return finalAns;
        }
        private List<composition.compTable> randomList(List<composition.compTable> oriCT)
        {
            List<composition.compTable> CT = oriCT;
            List<string> correctBounds = new List<string>();
            int numberOfElements = 0;
            for (int i = 0; i < CT.Count; i++)
            {
                Boolean isAllZero = true;
                for (int j = 0; j < CT[i].Bound.Count; j++)
                {
                    if (CT[i].Bound[j] != "0")
                    {
                        isAllZero = false;
                    }
                }
                if (isAllZero)
                    continue;
                numberOfElements++;
            }
            List<int> chosenElements = Enumerable.Range(0, CT.Count).ToList();
            chosenElements.Shuffle();

            List<int> boundrange = Enumerable.Range(0, 6).ToList();
            boundrange.Add(0);
            boundrange.Shuffle();

            for (int i = 0; i < CT.Count; i++)
            {
                CT[i].Bound.Clear();
                CT[i].Bound.Add("0");
                CT[i].Bound.Add("0");
            }
            int l = 0;
            for (int i = 0; i < numberOfElements; i++)
            {
                CT[chosenElements[l]].Bound.Clear();
                boundrange.Shuffle();
                CT[chosenElements[l]].Bound.Add(Convert.ToString(boundrange[0]));
                CT[chosenElements[l]].Bound.Add(Convert.ToString(boundrange[0] + 2));
                l++;
            }

            return CT;
        }
        private List<composition.comphypo> getPPhypo(List<composition.comphypo> CHy, List<composition.comphypo> CH, List<composition.PPMSD> PP)
        {
            List<composition.comphypo> Ans = new List<composition.comphypo>();
            Ans.AddRange(CHy);
            for (int i = 0; i < PP.Count; i++)
            {
                if (PP[i].selected)
                {
                    Int32 Count = Convert.ToInt32(PP[i].numGly);
                    List<composition.comphypo> Temp = new List<composition.comphypo>();
                    composition.comphypo temp = new composition.comphypo();
                    //First line:
                    temp.compoundCompo = "";
                    temp.AdductNum = 0;
                    temp.AddRep = "";
                    temp.eqCount.Add("A", 0);
                    temp.eqCount.Add("B", 0);
                    temp.eqCount.Add("C", 0);
                    temp.eqCount.Add("D", 0);
                    temp.eqCount.Add("E", 0);
                    temp.eqCount.Add("F", 0);
                    temp.eqCount.Add("G", 0);
                    temp.eqCount.Add("H", 0);
                    temp.eqCount.Add("I", 0);
                    temp.eqCount.Add("J", 0);
                    temp.eqCount.Add("K", 0);
                    temp.eqCount.Add("L", 0);
                    temp.eqCount.Add("M", 0);
                    temp.eqCount.Add("N", 0);
                    temp.eqCount.Add("O", 0);
                    temp.eqCount.Add("P", 0);
                    temp.eqCount.Add("Q", 0);
                    temp.MW = PP[i].Mass;
                    //columns for glycopeptides
                    temp.PepModification = PP[i].Modifications;
                    temp.PepSequence = PP[i].Sequence;
                    temp.MissedCleavages = PP[i].MissedCleavages;
                    temp.numGly = 0;
                    Temp.Add(temp);
                    for (int j = 0; j < Count; j++)
                    {
                        List<composition.comphypo> Temp2 = new List<composition.comphypo>();
                        for (int k = 0; k < Temp.Count; k++)
                        {
                            for (int l = 0; l < CH.Count; l++)
                            {
                                composition.comphypo temp2 = new composition.comphypo();
                                temp2 = CH[l];
                                temp2.MW = temp2.MW + Temp[k].MW;
                                temp2.numGly = Count;
                                temp2.PepModification = Temp[k].PepModification;
                                temp2.PepSequence = Temp[k].PepSequence;
                                temp2.MissedCleavages = Temp[k].MissedCleavages;
                                Temp2.Add(temp2);
                            }
                        }
                        Temp.AddRange(Temp2);
                    }
                    Ans.AddRange(Temp);
                }
            }
            return Ans;
        }
        private List<composition.PPMSD> genFalsePPMSD(String path)
        {
            Features fea = new Features();
            List<composition.PPMSD> PP = fea.readtablim(path);
            composition comp = new composition();
            String sequence = comp.getSequence(PP);
            List<int> forRandom = new List<int>
            {
               {0},{1},{2}
            };
            List<int> forlength = new List<int>
            {
               {4},{5},{6},{7},{8},{9},{10},{11}
            };

            List<composition.PPMSD> finalAns = new List<composition.PPMSD>();
            Int32 StartAA = 0;
            Int32 EndAA = 0;
            while (EndAA != sequence.Count())
            {
                forRandom.Shuffle();
                if (forRandom[0] == 1)
                {
                    //add in this fragment
                    composition.PPMSD Ans = new composition.PPMSD();
                    forlength.Shuffle();
                    Int32 length = forlength[0];
                    EndAA = StartAA + length;
                    if (EndAA > sequence.Count())
                        EndAA = sequence.Count();
                    String Fra = "";
                    Ans.StartAA = Convert.ToInt32(StartAA + 1);
                    Ans.EndAA = Convert.ToInt32(EndAA + 1);
                    for (int i = StartAA; i < EndAA; i++)
                    {
                        Fra = Fra + sequence[i];
                    }
                    StartAA = StartAA + length;
                    Ans.selected = true;
                    Ans.number = 1;
                    Ans.Mass = getFragmentMass(Fra);
                    Ans.Charge = 0;
                    Ans.Modifications = "";
                    Ans.MissedCleavages = 0;
                    Ans.PreviousAA = "";
                    Ans.NextAA = "";
                    forRandom.Shuffle();
                    Ans.numGly = Convert.ToInt32(forRandom[0]);
                    finalAns.Add(Ans);
                }
                else
                {
                    //Skip this fragment
                    forlength.Shuffle();
                    Int32 length = forlength[0];
                    EndAA = StartAA + length;
                    if (EndAA > sequence.Count())
                        EndAA = sequence.Count();
                    StartAA = StartAA + length;
                }
                if (EndAA == sequence.Count() && finalAns.Count() == 0)
                {
                    EndAA = 0;
                    StartAA = 0;
                }
            }
            return finalAns;
        }
        private Double getFragmentMass(String Fra)
        {
            String translate = "0";
            aminoAcidMass AAMass = new aminoAcidMass();
            foreach (char i in Fra)
            {
                translate = translate + "+" + Convert.ToString(AAMass.getMass(Convert.ToString(i)));
            }
            MathParser MTK = new MathParser();
            Double mass = MTK.Parse(translate, false);
            return mass;
        }



    }
}
