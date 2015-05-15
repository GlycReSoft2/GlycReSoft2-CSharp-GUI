using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BUPIDClient;

namespace BUPIDClient.Dialogs
{
    public partial class DeconJobConfigDialog : Form
    {

        public String Mode;
        public JobModel Job;

        public DeconJobConfigDialog()
        {
            InitializeComponent();            
        }

        public DeconJobConfigDialog(JobModel model)
        {
            InitializeComponent();
            SetJob(model);
        }

        public void SetJob(JobModel model)
        {
            Job = model;
            JobDescriptionTextBox.Text = Job.JobName;
        }

        private void RunJobButton_Click(object sender, EventArgs e)
        {
            if (DeconBatchRadioButton.Checked)
            {
                Mode = DeconMode.DeconBatch;
            }
            else if(DeconGroupRadioButton.Checked)
            {
                Mode = DeconMode.DeconGroup;
            }
            else
            {
                MessageBox.Show("You must select a deconvolution mode to run.");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }



        


    }
}
