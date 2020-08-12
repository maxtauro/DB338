using System.Drawing;
using FastColoredTextBoxNS;

namespace DB338GUI
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.TxtQuery = new FastColoredTextBoxNS.FastColoredTextBox();
            this.BtnSubmitQuery = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.BtnExport = new System.Windows.Forms.Button();
            this.BtnImport = new System.Windows.Forms.Button();
            this.autocompleteMenu1 = new AutocompleteMenuNS.AutocompleteMenu();
            ((System.ComponentModel.ISupportInitialize) (this.TxtQuery)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // TxtQuery
            // 
            this.TxtQuery.AutoCompleteBrackets = true;
            this.TxtQuery.AutoCompleteBracketsList = new char[] {'(', ')', '{', '}', '[', ']', '\"', '\"', '\'', '\''};
            this.autocompleteMenu1.SetAutocompleteMenu(this.TxtQuery, this.autocompleteMenu1);
            this.TxtQuery.AutoIndentChars = false;
            this.TxtQuery.AutoIndentCharsPatterns = "";
            this.TxtQuery.AutoScrollMinSize = new System.Drawing.Size(29, 18);
            this.TxtQuery.BackBrush = null;
            this.TxtQuery.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.TxtQuery.CharHeight = 18;
            this.TxtQuery.CharWidth = 9;
            this.TxtQuery.CommentPrefix = "--";
            this.TxtQuery.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TxtQuery.DisabledColor = System.Drawing.Color.FromArgb(((int) (((byte) (100)))), ((int) (((byte) (180)))), ((int) (((byte) (180)))), ((int) (((byte) (180)))));
            this.TxtQuery.FoldingIndicatorColor = System.Drawing.Color.White;
            this.TxtQuery.Font = new System.Drawing.Font("Consolas", 12F);
            this.TxtQuery.ForeColor = System.Drawing.SystemColors.Window;
            this.TxtQuery.Hotkeys = resources.GetString("TxtQuery.Hotkeys");
            this.TxtQuery.IndentBackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.TxtQuery.IsReplaceMode = false;
            this.TxtQuery.Language = FastColoredTextBoxNS.Language.SQL;
            this.TxtQuery.LeftBracket = '(';
            this.TxtQuery.LineNumberColor = System.Drawing.Color.Gray;
            this.TxtQuery.Location = new System.Drawing.Point(12, 12);
            this.TxtQuery.Name = "TxtQuery";
            this.TxtQuery.Paddings = new System.Windows.Forms.Padding(0);
            this.TxtQuery.RightBracket = ')';
            this.TxtQuery.SelectionColor = System.Drawing.Color.FromArgb(((int) (((byte) (60)))), ((int) (((byte) (0)))), ((int) (((byte) (0)))), ((int) (((byte) (255)))));
            this.TxtQuery.ServiceColors = ((FastColoredTextBoxNS.ServiceColors) (resources.GetObject("TxtQuery.ServiceColors")));
            this.TxtQuery.Size = new System.Drawing.Size(672, 151);
            this.TxtQuery.TabIndex = 1;
            this.TxtQuery.Zoom = 100;
            // 
            // BtnSubmitQuery
            // 
            this.BtnSubmitQuery.Location = new System.Drawing.Point(690, 60);
            this.BtnSubmitQuery.Name = "BtnSubmitQuery";
            this.BtnSubmitQuery.Size = new System.Drawing.Size(96, 29);
            this.BtnSubmitQuery.TabIndex = 2;
            this.BtnSubmitQuery.Text = "Run";
            this.BtnSubmitQuery.UseVisualStyleBackColor = true;
            this.BtnSubmitQuery.Click += new System.EventHandler(this.BtnSubmitQuery_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Location = new System.Drawing.Point(12, 169);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.Size = new System.Drawing.Size(672, 213);
            this.dataGridView1.TabIndex = 4;
            // 
            // BtnExport
            // 
            this.BtnExport.Location = new System.Drawing.Point(690, 359);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(96, 23);
            this.BtnExport.TabIndex = 5;
            this.BtnExport.Text = "Export Table";
            this.BtnExport.UseVisualStyleBackColor = true;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // BtnImport
            // 
            this.BtnImport.Location = new System.Drawing.Point(691, 330);
            this.BtnImport.Name = "BtnImport";
            this.BtnImport.Size = new System.Drawing.Size(95, 23);
            this.BtnImport.TabIndex = 6;
            this.BtnImport.Text = "Import Table";
            this.BtnImport.UseVisualStyleBackColor = true;
            this.BtnImport.Click += new System.EventHandler(this.BtnImport_Click);
            // 
            // autocompleteMenu1
            // 
            this.autocompleteMenu1.Colors = ((AutocompleteMenuNS.Colors) (resources.GetObject("autocompleteMenu1.Colors")));
            this.autocompleteMenu1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.autocompleteMenu1.ImageList = null;
            this.autocompleteMenu1.Items = new string[] {"SELECT", "FROM", "WHERE", "VALUES", "UPDATE", "DELETE", "DROP", "INSERT", "MIN", "MAX", "COUNT", "SUM", "AVG"};
            this.autocompleteMenu1.TargetControlWrapper = null;
            // 
            // FrmMain
            // 
            this.AcceptButton = this.BtnSubmitQuery;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(798, 397);
            this.Controls.Add(this.BtnImport);
            this.Controls.Add(this.BtnExport);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.BtnSubmitQuery);
            this.Controls.Add(this.TxtQuery);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmMain";
            this.Text = "DB338";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            ((System.ComponentModel.ISupportInitialize) (this.TxtQuery)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
        }

        private AutocompleteMenuNS.AutocompleteMenu autocompleteMenu1;
        private System.Windows.Forms.Button BtnExport;
        private System.Windows.Forms.Button BtnImport;
        private System.Windows.Forms.Button BtnSubmitQuery;
        private System.Windows.Forms.DataGridView dataGridView1;
        private FastColoredTextBoxNS.FastColoredTextBox TxtQuery;

        #endregion
    }
}

