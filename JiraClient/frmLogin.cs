using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JiraRESTClient;

namespace JiraClient
{
    public partial class frmLogin : Form
    {
        private bool _authenticated = false;

        public bool Authenticated
        {
            get { return _authenticated; }
            set { _authenticated = value; }
        }

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {            
            RestClient client = new RestClient();
            var auth = client.AuthenticateToJira("https://teamipreo.atlassian.net", txtUsername.Text, txtPassword.Text);

            if (auth)
            {
                this.Hide();
                Authenticated = true;

                frmSyncIssues frm = new frmSyncIssues();
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Invalid Login", "Login to JIRA", MessageBoxButtons.OK);
            }
        }
    }
}
