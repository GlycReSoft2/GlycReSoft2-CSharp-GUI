using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using BUPIDClient.Dialogs;

namespace BUPIDClient
{
    public partial class JobDisplayItem : UserControl
    {
        JobModel Job;
        BUPIDClientController Controller;

        public JobDisplayItem()
        {
            InitializeComponent();
        }

        public JobDisplayItem(JobModel job, BUPIDClientController controller)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.Job = job;
            Console.WriteLine(Job.JobName);
            this.JobNameLabel.Text = job.JobName + " (" + job.State.ToString() + ")";
            this.Controller = controller;

            
            if (job.State == JobState.Running)
            {
                RequestProcessingButton.Enabled = false;
            }

            DownloadResultsButton.Visible = false;
            if (job.State == JobState.Complete)
            {
                DownloadResultsButton.Visible = true;
                RequestProcessingButton.Enabled = false;
            }
        }

        private async void RequestProcessing(String mode=null)
        {
            if (mode == null)
            {
                mode = DeconMode.DeconBatch;
            }
            try
            {
                await Controller.RequestProcessing(this.Job, mode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An error occurred");
            }            
        }

        private async void RequestProcessingButton_Click(object sender, EventArgs e)
        {
            DeconJobConfigDialog config = new DeconJobConfigDialog(this.Job);
            if (DialogResult.OK == config.ShowDialog())
            {
                String deconMode = config.Mode;
                Console.WriteLine("Using {0} mode", deconMode);
                RequestProcessing(deconMode);
            }
            Controller.GetUpdatesFromServer();
        }

        private async void DownloadResultsButton_Click(object sender, EventArgs e)
        {
            if(Job.State == JobState.Complete && await Controller.CheckForResults(Job))
            {                
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "yaml";
                saveFileDialog.Filter = "YAML (*.yaml)|*.yaml|All Files (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;
                DialogResult result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Stream content = await Controller.DownloadResults(Job);
                    Stream writer = (saveFileDialog.OpenFile());
                    content.CopyTo(writer);
                    writer.Close();
                    content.Close();
                    await Controller.CheckForResults(Job, true);
                }
                Controller.GetUpdatesFromServer();
            }
            
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            Controller.DeleteJob(this.Job);
            Controller.GetUpdatesFromServer();
        }
    }
}
