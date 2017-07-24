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
    public partial class FrmLogin : Form
    {
        public bool Authenticated { get; set; } = false;

        public FrmLogin()
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

                FrmSyncIssues frm = new FrmSyncIssues();
                //Set the client before bye to login
                frm.Client = client.JiraClient;

                FrmExcelView frmExcel = new FrmExcelView();
                frmExcel.Client = client.JiraClient;

                //Show per issue editing
                //frm.ShowDialog();

                //Update all issues
                frmExcel.ShowDialog();
            }
            else
            {
                MessageBox.Show("Invalid Login", "Login to JIRA", MessageBoxButtons.OK);
            }
        }
    }
}