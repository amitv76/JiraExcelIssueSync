﻿namespace UI
{
    partial class FrmExcelView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnExcelData = new System.Windows.Forms.Button();
            this.txtProject = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnBrowseToExcel = new System.Windows.Forms.Button();
            this.lblExcelPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnExcelData
            // 
            this.btnExcelData.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExcelData.Location = new System.Drawing.Point(52, 160);
            this.btnExcelData.Name = "btnExcelData";
            this.btnExcelData.Size = new System.Drawing.Size(769, 37);
            this.btnExcelData.TabIndex = 0;
            this.btnExcelData.Text = "Send Issue updates from Excel to Jira";
            this.btnExcelData.UseVisualStyleBackColor = true;
            this.btnExcelData.Click += new System.EventHandler(this.btnExcelData_Click);
            // 
            // txtProject
            // 
            this.txtProject.Enabled = false;
            this.txtProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProject.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txtProject.Location = new System.Drawing.Point(52, 130);
            this.txtProject.Name = "txtProject";
            this.txtProject.ReadOnly = true;
            this.txtProject.Size = new System.Drawing.Size(206, 22);
            this.txtProject.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Jira Project:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Excel Workbook Path:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnBrowseToExcel
            // 
            this.btnBrowseToExcel.Location = new System.Drawing.Point(165, 28);
            this.btnBrowseToExcel.Name = "btnBrowseToExcel";
            this.btnBrowseToExcel.Size = new System.Drawing.Size(62, 22);
            this.btnBrowseToExcel.TabIndex = 6;
            this.btnBrowseToExcel.Text = "Browse...";
            this.btnBrowseToExcel.UseVisualStyleBackColor = true;
            this.btnBrowseToExcel.Click += new System.EventHandler(this.btnBrowseToExcel_Click);
            // 
            // lblExcelPath
            // 
            this.lblExcelPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExcelPath.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblExcelPath.Location = new System.Drawing.Point(52, 56);
            this.lblExcelPath.Name = "lblExcelPath";
            this.lblExcelPath.ReadOnly = true;
            this.lblExcelPath.Size = new System.Drawing.Size(769, 22);
            this.lblExcelPath.TabIndex = 7;
            // 
            // FrmExcelView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 226);
            this.Controls.Add(this.lblExcelPath);
            this.Controls.Add(this.btnBrowseToExcel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtProject);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExcelData);
            this.Name = "FrmExcelView";
            this.Text = "J-Sync: Sync Excel to Jira";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmExcelView_FormClosing);
            this.Load += new System.EventHandler(this.FrmExcelView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExcelData;
        private System.Windows.Forms.TextBox txtProject;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnBrowseToExcel;
        private System.Windows.Forms.TextBox lblExcelPath;
    }
}