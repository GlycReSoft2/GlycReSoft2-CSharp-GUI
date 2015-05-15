using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BUPIDClient.Dialogs
{
    public partial class UploadDataDialog : Form
    {

        String CurrentFile;
        DataUploadModel DataModel;

        public UploadDataDialog()
        {
            InitializeComponent();
        }

        private void DoUploadButton_Click(object sender, EventArgs e)
        {
            //Validation
            String description = this.DescriptionTextBox.Text;
            if (description.Replace(" ", "").Length == 0)
            {
                MessageBox.Show("Upload file", "The job's description must not be empty and must be unique with respect to your currently running jobs.");
                return;
            }
            if (CurrentFile == null || !File.Exists(CurrentFile))
            {
                MessageBox.Show("Upload file", "You either did not provide a file to upload, or the file could not be found on disk. Please try again.");
                return;
            }

            DataModel = new DataUploadModel(description, CurrentFile);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ChooseFileUploadButton_Click(object sender, EventArgs e)
        {
            DialogResult diagRes = openDataUploadFileDialog.ShowDialog();
            if (DialogResult.OK == diagRes)
            {
                CurrentFile = openDataUploadFileDialog.FileName;
                SelectedFileLabel.Text = CurrentFile;
            }            
        }

        public DataUploadModel GetDataUploadModel()
        {
            return DataModel;
        }

        
    }
}
