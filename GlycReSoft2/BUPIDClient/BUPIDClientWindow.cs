using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BUPIDClient.Dialogs;
using System.Net;


namespace BUPIDClient
{
    public partial class BUPIDClientWindow : Form
    {
        private BUPIDClientController Controller;
        private Timer ServerUpdatePollingTimer;

        /// <summary>
        /// Default Host
        /// </summary>
        public BUPIDClientWindow()
            : this("http://bumc-florida.bumc.bu.edu/BUPID_TD/cgi-bin")
        {}

        /// <summary>
        /// Default Update Frequency
        /// </summary>
        /// <param name="hostUrl"></param>
        public BUPIDClientWindow(String hostUrl) : this(hostUrl, null, null, 30)
        {}
        /// <summary>
        /// Complete Constructor
        /// </summary>
        /// <param name="hostUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="updateFrequency"></param>
        public BUPIDClientWindow(String hostUrl, String userName, String password, int updateFrequency)
        {
            InitializeComponent();
            
            Controller = new BUPIDClientController(userName, password, new Uri(hostUrl));
            LoggedInLabel.DataBindings.Add(new Binding("Text", Controller, "UserEmail"));
            UploadFileButton.DataBindings.Add(new Binding("Enabled", Controller, "IsAuthenticated"));
            Controller.JobListUpdated += new PropertyChangedEventHandler(UpdateJobListView);
                                    
            //Configure polling frequency
            ServerUpdatePollingTimer = new Timer();
            //Time in seconds
            ServerUpdatePollingTimer.Interval = 1000 * updateFrequency;
            ServerUpdatePollingTimer.Tick += UpdateTimer_Tick;
            ServerUpdatePollingTimer.Start();
            
            //Detect changes to the user's settings options
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        /// <summary>
        /// Handle changing of host URL and attempt to re-authenticate with new host.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName);
            if (e.PropertyName == "BUPIDHost")
            {

                String email = Controller.UserEmail;
                String password = Controller.Password;                
                Controller.HostUri = new Uri(Properties.Settings.Default.BUPIDHost);
                Controller.InitHttpClient();
                try
                {
                    if (Controller.UserEmail == null) return;
                    await Controller.TryLogin(email, password);
                }
                catch(UserLoginException)
                {
                    MessageBox.Show("Could not authenticate with new host " + Properties.Settings.Default.BUPIDHost);
                    Controller.LogOut();
                }
                catch (WebException ex)
                {
                    MessageBox.Show("Could not connect to new host " + Properties.Settings.Default.BUPIDHost + "\n" + ex.Message);
                    Controller.LogOut();
                }                
            }
        }

        /// <summary>
        /// Event handler for requesting new data from the Controller and the Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
               await Controller.GetUpdatesFromServer();
            }
            catch (UserLoginException userEx)
            {
                //Do Nothing
            }
            catch (WebException ex)
            {
                MessageBox.Show("Could not connect to host " + Properties.Settings.Default.BUPIDHost + "\n" + ex.Message);
            }
        }


        /// <summary>
        /// Updates the listed set of jobs on the main screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateJobListView(object sender, PropertyChangedEventArgs e)
        {
            //Remove all prior jobs from the display, and dispose of them to
            //free results
            JobItemPanel.Controls.Clear();
            foreach (Control control in JobItemPanel.Controls)
            {
                control.Dispose();
            }
            //For each job still on the server, create a JobDisplayItem
            //in the panel.
            foreach (JobModel job in Controller.JobList)
            {
                var item = new JobDisplayItem(job, Controller);
                JobItemPanel.Controls.Add(item);
            }
            //There are no jobs on the server, nothing to show. Instead display
            //a label to that effect.
            if (Controller.JobList.Count == 0)
            {
                Label noDatalabel = new Label();
                noDatalabel.Text = "No job information available.";
                noDatalabel.Font = new Font(noDatalabel.Font, FontStyle.Bold);
                noDatalabel.TextAlign = ContentAlignment.MiddleCenter;
                noDatalabel.Width = Convert.ToInt32(JobItemPanel.Width * .9);
                noDatalabel.Anchor = AnchorStyles.Right;
                JobItemPanel.Controls.Add(noDatalabel);
            }
        }

        /// <summary>
        /// If not currently logged in: 
        ///     Show a dialog to get BUPID authentication from the user, and attempt to log in.
        ///     If log in is successful, get the user's job information from the server. 
        ///     Else catch the ensuing exception and show an error message box. 
        /// Else
        ///     Log out the current user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BUPIDLoginButton_Click(object sender, EventArgs e)
        {            
            if (Controller.IsAuthenticated)
            {
                Controller.LogOut();
                return;
            }

            LoginDialog login = new LoginDialog(this);
            DialogResult loginResult = login.ShowDialog();
            Console.WriteLine(loginResult);
            if (loginResult == DialogResult.OK)
            {
                var creds = login.GetCredentials();
                try
                {
                    await Controller.TryLogin(creds.UserName, creds.Password);
                }
                catch (UserLoginException ex)
                {
                    MessageBox.Show(ex.Message, "Login Error");
                }                
                catch (WebException webEx)
                {
                    MessageBox.Show("Could not connect to host " + Properties.Settings.Default.BUPIDHost + "\n" + webEx.Message);
                }
            }
            // Configure button text
            if (Controller.IsAuthenticated)
            {
                BUPIDLoginButton.Text = "Log Out";
            }
            else
            {
                BUPIDLoginButton.Text = "Log In";
            }
        }


        public async void BUPIDRegisterAction(String userEmail, String userPassword)
        {
            try
            {
                bool result = await Controller.TryRegister(userEmail, userPassword);
                if (result)
                {
                    MessageBox.Show("Check your email for an activation link from BUPID");
                }
                else
                {
                    MessageBox.Show("The email address you specified is already in use.");
                }
            }
            catch(WebException webex)
            {               
                MessageBox.Show("Could not register." + webex.Message);
            }
            
        }

        public async void BUPIDResetPasswordAction(String userEmail)
        {
            try
            {
                bool result = await Controller.ResetPassword(userEmail);
                if (result)
                {
                    MessageBox.Show("Check your email for a password reset link from BUPID");
                }
                else
                {
                    MessageBox.Show(String.Format("There was no account for the email address {0}", userEmail));
                }

            }
            catch (WebException webex)
            {
                MessageBox.Show("Could not reset password." + webex.Message);
            }
        }

        /// <summary>
        /// Handle the showing of the UploadDataDialog for uploading data for a job
        /// to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UploadFileButton_Click(object sender, EventArgs e)
        {
            UploadDataDialog addData = new UploadDataDialog();
            DialogResult addDataResult = addData.ShowDialog();
            if (addDataResult == DialogResult.OK)
            {
                var dataModel = addData.GetDataUploadModel();
                if (Controller.IsAuthenticated)
                {
                    DateTime startTime = DateTime.Now;                    
                    try
                    {
                        await Controller.UploadDataFile(dataModel).ConfigureAwait(false);
                    }
                    catch (DataUploadException uploadEx)
                    {
             
                        MessageBox.Show(uploadEx.Message, "DataUploadException");
                    }
                    catch (WebException webEx)
                    {
                        MessageBox.Show("Could not connect to host " + Properties.Settings.Default.BUPIDHost + "\n" + webEx.Message);
                    }
                    catch (Exception ex)
                    {
                        String message = String.Format("{0}\n", DateTime.Now - startTime);
                        while (true)
                        {
                            message += ex.Message + "\n";
                            if (ex.InnerException != null)
                            {
                                Console.WriteLine("recursing");
                                ex = ex.InnerException;
                            }
                            else
                            {
                                break;
                            }
                        }
                        MessageBox.Show(message);
                    }                   
                }
            }
        }

        /// <summary>
        /// Show the Settings Menu, which will modify the application user Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SettingsMenu settingsMenu = new SettingsMenu();
            settingsMenu.ShowDialog();
        }
    }
}
