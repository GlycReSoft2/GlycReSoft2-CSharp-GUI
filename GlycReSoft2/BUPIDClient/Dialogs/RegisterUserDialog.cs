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
    public partial class RegisterUserDialog : Form
    {
        public RegisterUserDialog()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            if (PasswordTextBox.Text == PasswordTextBox2.Text)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Your password does not match.");
            }                       
        }

        public UserLoginModel GetCredentials()
        {
            UserLoginModel creds = new UserLoginModel(UserNameBox.Text, PasswordTextBox.Text);
            return creds;
        }

        private void PasswordTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (PasswordTextBox.Text != PasswordTextBox2.Text)
            {
                PasswordTextBox2.BackColor = Color.LightPink;
            }
            else
            {
                PasswordTextBox2.BackColor = Color.LightGreen;
            }
        }
    }


}
