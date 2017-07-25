using System;
using System.Configuration;
using System.Windows.Forms;
using JiraRESTClient;

namespace UI
{
    public partial class FrmLogin : Form
    {
        public bool Authenticated { get; set; } = false;

        public FrmLogin()
        {
            InitializeComponent();
        }

        private string ProxyUserName
        {
            get
            {
                var config = GetExecutingAssemblysConfiguration();

                return config.AppSettings.Settings["ProxyUsername"].Value;
            }
        }

        private string ProxyPassword
        {
            get
            {
                var config = GetExecutingAssemblysConfiguration();

                return config.AppSettings.Settings["ProxyPassword"].Value;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {            
            if (txtUsername.Text.Length > 0 && txtPassword.Text.Length > 0)
            {
                if (txtProxyUser.Text.Length > 0 && txtProxyPwd.Text.Length > 0)
                {
                    //Write to App.config
                    WriteProxyCredentialsToConfig(txtProxyUser.Text, txtProxyPwd.Text);
                }
                else
                {
                    MessageBox.Show("Enter you Proxy credentials.", "Jira", MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Enter you Jira credentials.", "Jira", MessageBoxButtons.OK);
                return;
            }

            RestClient client = new RestClient();
            var auth = client.AuthenticateToJiraAsync(ConfigurationManager.AppSettings["JiraUrl"], 
                                                        txtUsername.Text, txtPassword.Text,
                                                        ConfigurationManager.AppSettings["JiraProject"],
                                                        ConfigurationManager.AppSettings["ProxyUrl"],
                                                        ProxyUserName,
                                                        ProtectCredentials.Decrypt(ProxyPassword)
                                                     ).Result;

            if (auth)
            {
                this.Hide();
                Authenticated = true;

                //FrmSyncIssues frm = new FrmSyncIssues();
                //Set the client before bye to login
                //frm.Client = client.JiraClient;

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

        private void WriteProxyCredentialsToConfig(string userName, string password)
        {
            var encrPwd = ProtectCredentials.Encrypt(password);

            var config = GetExecutingAssemblysConfiguration();

            //// Set Values for Application Settings.
            config.AppSettings.Settings["ProxyUsername"].Value = userName;
            config.AppSettings.Settings["ProxyPassword"].Value = encrPwd;

            //// Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);

            //// Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            var config = GetExecutingAssemblysConfiguration();

            // Set Values for Application Settings.
            if (config.AppSettings.Settings["ProxyUsername"].Value.Length > 0 &&
                config.AppSettings.Settings["ProxyPassword"].Value.Length > 0 && chkRememberProxy.Checked)
            {
                //set proxy username password in textboxes
                txtProxyUser.Text = config.AppSettings.Settings["ProxyUsername"].Value;
                txtProxyPwd.Text = ProtectCredentials.Decrypt(config.AppSettings.Settings["ProxyPassword"].Value);
            }
        }

        private Configuration GetExecutingAssemblysConfiguration()
        {
            //Check config for username and password, and if so
            // Open App.Config of executable
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string appPath = System.IO.Path.GetDirectoryName(assembly.Location);
            string configFile = System.IO.Path.Combine(appPath, string.Concat(assembly.ManifestModule.Name, ".config"));
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;

            return ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }
    }
}