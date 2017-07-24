using System;
using System.Collections.Generic;
using System.Windows.Forms;
using JiraRestApiWrapper.JiraModel;

namespace UI
{
    public partial class FrmSyncIssues : Form
    {
        private Issues _issues = null;
        private static List<ActionItem> _actionList = null;

        private Issue SelectedIssue
        {
            get { return _issues.issues.Find(a => a.id == ((Issue) cmbIssues.SelectedItem).id); }
        }

        public JiraRestApiWrapper.JiraClient Client { get; set; }

        public static List<ActionItem> ActionList
        {
            get
            {
                if (_actionList == null)
                {
                    _actionList =
                            new List<ActionItem>
                            {
                                new ActionItem() {Name = "None", Value = ""},
                                new ActionItem() {Name = "New", Value = "15205"},
                                new ActionItem() {Name = "Investigating", Value = "13902"},
                                new ActionItem() {Name = "Scored", Value = "15600"},
                                new ActionItem() {Name = "Backlog", Value = "13906"},
                                new ActionItem() {Name = "Refined", Value = "13905"},
                                new ActionItem() {Name = "In Progress", Value = "13904"},
                                new ActionItem() {Name = "To Deploy", Value = "15000"},
                                new ActionItem() {Name = "Done", Value = "15001"},
                                new ActionItem() {Name = "On Hold", Value = "13903"}
                            };
                 }

                return _actionList;
            }
        }

        public FrmSyncIssues()
        {
            InitializeComponent();
        }

        private void frmSyncIssues_Load(object sender, EventArgs e)
        {
            RefreshData();
        }
        private void SetSelectedIssue(Issues issues)
        {
            var selectedIssue = issues.issues.Find(a => a.id == ((Issue) cmbIssues.SelectedItem).id);
            //Issue ID
            txtIssueId.Text = selectedIssue.id;

            //Issue ID
            txtSummary.Text = selectedIssue.fields.summary;

            //Score
            txtScore.Text = selectedIssue.fields.customfield_13503.ToString();

            //Action
            if (selectedIssue.fields.customfield_13901 != null)
            {
                cmbAction.SelectedItem = cmbAction.Items[ActionList.FindIndex(a => a.Value == selectedIssue.fields.customfield_13901?.id)];
            }
            else
            {
                cmbAction.SelectedIndex = 0;
            }
            
        }

        private void cmbIssues_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetSelectedIssue(_issues);
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            var updateIssue = new
            {
                //{"errorMessages":[],"errors":{"customfield_13901":"Invalid value 'customfield_13901' passed for customfield 'Action'. 
                //Allowed values are: 15205[New], 13902[Investigating], 15600[Scored], 13906[Backlog], 13905[Refined], 13904[In Progress], 15000[To Deploy], 15001[Done],
                //                      13903[On Hold], -1"}}
                fields = new
                {
                    customfield_13503 = decimal.Parse(txtScore.Text),
                    customfield_13901 = new customfield()
                    {
                        value = (cmbAction.SelectedItem as ActionItem)?.Name,
                        id = (cmbAction.SelectedItem as ActionItem)?.Value,
                        self = SelectedIssue.fields.customfield_13901?.self
                    }
                }
            };

            if (Client.UpdateIssueFields(SelectedIssue.key, updateIssue))
            {
                MessageBox.Show($"Updated JIRA Issue (Key: '{SelectedIssue.key}')");
            }
            else
            {
                MessageBox.Show($"Failed to udpate JIRA Issue Key '{SelectedIssue.key}')");
            }

            RefreshIssueData();
        }

        private void RefreshData()
        {
            ProjectMeta projectMetaData = Client.GetProjectMeta("MUN"); //hackathon proj in JIRA

            //Action
            cmbAction.DataSource = ActionList;
            cmbAction.DisplayMember = "Name";
            cmbAction.ValueMember = "Value";

            //Project Hackathon
            txtProject.Text = projectMetaData.name;

            RefreshIssueData();

            //Issues
            cmbIssues.DataSource = _issues.issues;
            cmbIssues.DisplayMember = "key";
            cmbIssues.ValueMember = "id";
        }

        private void RefreshIssueData()
        {
            var fields =
                new List<string>
                {
                    "key",
                    "summary",
                    "customfield_13503",
                    "customfield_13901"
                }; //custom fields are JIRA fields for Score & Action

            _issues = Client.GetIssuesByProject("MUN", 0, 1000, fields);
        }

        private void FrmSyncIssues_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.Cancel == false && e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }
    }

    public class ActionItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}