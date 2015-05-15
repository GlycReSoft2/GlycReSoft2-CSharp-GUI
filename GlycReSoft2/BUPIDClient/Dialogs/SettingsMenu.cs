using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BUPIDClient.Dialogs
{
    public partial class SettingsMenu : Form
    {
        public SettingsMenu()
        {
            InitializeComponent();
            this.HostURLTextBox.Text = Properties.Settings.Default.BUPIDHost;
            this.FileUploadTimeoutInput.Value = Properties.Settings.Default.UploadTimeout;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.BUPIDHost = this.HostURLTextBox.Text;
            Properties.Settings.Default.UploadTimeout = Convert.ToInt32(this.FileUploadTimeoutInput.Value);
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
