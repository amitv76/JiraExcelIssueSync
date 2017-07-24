using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using JiraRestApiWrapper.JiraModel;
using JiraRESTClient;

namespace UI
{
    public partial class FrmExcelView : Form
    {
        public FrmExcelView()
        {
            InitializeComponent();
        }

        //List<string> _headers = new List<string>();
        public JiraRestApiWrapper.JiraClient Client { get; set; }
        private Issues _issues = null;

        private FileStream _excelFile = null;

        private void LoadIssueData()
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

        private Issue GetSelectedIssue(string issueKey)
        {
            return _issues.issues.Find(a => a.key == issueKey);
        }

        private void btnExcelData_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                //If filestream has ben disposed, recreate it.
                if ((!_excelFile.CanSeek) && lblExcelPath.Text.Length > 0)
                {
                    _excelFile = new FileStream(lblExcelPath.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }

                if (_excelFile.CanSeek)
                {
                    UpdateJiraFromExcelData();
                }
                else
                {
                    MessageBox.Show("Relaunch the app.", "Sync", MessageBoxButtons.OK);
                }

                MessageBox.Show("Update is complete.", "Sync", MessageBoxButtons.OK);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private string GetCellValue(SpreadsheetDocument doc, Cell cell, uint rowIndex)
        {
            string value = cell?.CellValue?.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var innerText = doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
                return innerText;
            }

            if (value != null && cell.CellReference.ToString().StartsWith("AH") && rowIndex > 3)
            {
                return Math.Round(decimal.Parse(value), 2).ToString();
            }

            return value;
        }

        private void UpdateJiraFromExcelData()
        {
            //string fileName = @"2017 Projects Choiceboard (Muni) FINAL.xlsx";


            using (FileStream fs = _excelFile)
            {
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fs, false))
                {
                    // Retrieve a reference to the workbook part.
                    WorkbookPart workbookPart = doc.WorkbookPart;

                    // Find the sheet with the supplied name, and then use that 
                    // Sheet object to retrieve a reference to the first worksheet.
                    Sheet theSheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == "Client Requests");

                    // Throw an exception if there is no sheet.
                    if (theSheet == null)
                    {
                        throw new ArgumentException("sheetName");
                    }

                    // Retrieve a reference to the worksheet part.
                    Worksheet wSheet = (doc.WorkbookPart.GetPartById(theSheet.Id.Value) as WorksheetPart)?.Worksheet;

                    IEnumerable<Row> rows = wSheet.GetFirstChild<SheetData>().Descendants<Row>();

                    //If there are rows in excel, get JIRA issue data
                    if (rows.Count() > 2)
                    {
                        LoadIssueData();
                    }

                    foreach (Row row in rows)
                    {
                        if (row.RowIndex.Value < 2)
                        {
                            continue;
                        }

                        //Read the 2nd row as header
                        if (row.RowIndex.Value == 2)
                        {
                            var j = 1;
                            //Headers required only for cells B2, AH2, & AI2
                            var headerCells = row.Descendants<Cell>()
                                .Where(a => (a.CellReference == "B2" || a.CellReference == "AH2" ||
                                             a.CellReference == "AI2"));
                            foreach (Cell cell in headerCells)
                            {
                                var colunmName = GetCellValue(doc, cell, row.RowIndex);
                                Console.WriteLine(colunmName);
                                //_headers.Add(colunmName);
                            }
                        }
                        else
                        {
                            //Actual data rows
                            //================

                            //Row data required only for columns B (MCR ID), AH (Total score), & AI (Status)
                            var rowCells = row.Descendants<Cell>()
                                .Where(a => (a.CellReference.ToString().StartsWith("B") ||
                                             a.CellReference.ToString().StartsWith("AH") ||
                                             a.CellReference.ToString().StartsWith("AI")));
                            int i = 0;
                            string issueKey = string.Empty, score = string.Empty, action = string.Empty;
                            Console.WriteLine();
                            Console.Write("Cell contents for Row Index: {0}: ", row.RowIndex);
                            foreach (Cell cell in rowCells)
                            {
                                var cellValue = GetCellValue(doc, cell, row.RowIndex);

                                //If column B (MCR ID) is null, then quit this row
                                if (cellValue == null && cell.CellReference.ToString().StartsWith("B"))
                                {
                                    break;
                                }
                                else
                                {
                                    Console.Write("{0}  ", cellValue);

                                    if (cell.CellReference.ToString().StartsWith("B"))
                                    {
                                        issueKey = cellValue;
                                    }
                                    else if (cell.CellReference.ToString().StartsWith("AH"))
                                    {
                                        score = cellValue;
                                    }
                                    else if (cell.CellReference.ToString().StartsWith("AI"))
                                    {
                                        action = cellValue;
                                    }
                                }

                                i++;
                            }

                            if (issueKey?.Length > 0)
                            {
                                //Get equivalent JIRA Issue 
                                var jiraIssue = GetSelectedIssue(issueKey);

                                //Only try to update this excel issue to Jira if this excel issue exists on Jira
                                if (jiraIssue != null)
                                {
                                    //Does Excel action exist in JIRA action?
                                    var actionId = FrmSyncIssues.ActionList.Find(a => a.Name == action)?.Value;
                                    if (actionId != null)
                                    {
                                        var customfield_13901 = new customfield()
                                        {
                                            value = action,
                                            id = actionId, //This is the action ID of the action status from JIRA
                                            self = jiraIssue.fields.customfield_13901?.self
                                        };

                                        //Update Issue, Score & Status for this MCR ID (Issue Key)
                                        RestClient restClient = new RestClient();
                                        var update = restClient.UpdateJiraIssueAsync(Client, issueKey, score, customfield_13901).Result;
                                    }
                                    else
                                    {
                                        Console.WriteLine(". Excel issue with key: {0} has an action of {1} that does NOT exist in allowed values in JIRA",issueKey, action);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(". Excel issue with key: {0} does not exist on JIRA", issueKey);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FrmExcelView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.Cancel == false && e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        private void FrmExcelView_Load(object sender, EventArgs e)
        {
            ProjectMeta projectMetaData = Client.GetProjectMeta("MUN"); //hackathon proj in JIRA
            txtProject.Text = projectMetaData.name;

            btnExcelData.Enabled = false;
        }

        private void btnBrowseToExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Excel Files|*.xlsx";
            openFileDialog1.Title = "Select a Excel file";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    _excelFile = openFileDialog1.OpenFile() as FileStream;
                    lblExcelPath.Text = _excelFile.Name;
                    btnExcelData.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
    }
}