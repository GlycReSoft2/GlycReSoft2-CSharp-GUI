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
    public partial class LoginDialog : Form
    {
        BUPIDClientWindow Parent;

        public LoginDialog()
        {
            InitializeComponent();
        }

        public LoginDialog(BUPIDClientWindow parent)
        {
            InitializeComponent();
            Parent = parent;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public UserLoginModel GetCredentials()
        {
            UserLoginModel creds = new UserLoginModel(UserNameBox.Text, PasswordTextBox.Text);
            return creds;
        }

        private void OpenRegisterButton_Click(object sender, EventArgs e)
        {
            RegisterUserDialog register = new RegisterUserDialog();
            this.Hide();
            DialogResult regResult = register.ShowDialog();
            if (DialogResult.OK == regResult)
            {
                UserLoginModel creds = register.GetCredentials();
                Parent.BUPIDRegisterAction(creds.UserName, creds.Password);
            }
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ForgotPasswordButton_Click(object sender, EventArgs e)
        {
            if (this.UserNameBox.Text.Length > 0)
            {
                this.Parent.BUPIDResetPasswordAction(this.UserNameBox.Text);
            }
            else
            {
                MessageBox.Show("Please enter your email address in the marked box to request a reset link");
                UserNameBox.BackColor = Color.LightYellow;
                UserNameBox.Focus();
            }
        }
    }


}
