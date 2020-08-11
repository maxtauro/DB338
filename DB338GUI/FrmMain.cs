using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using DB338Core;
using EduDBGUI;
using FastColoredTextBoxNS;

namespace DB338GUI
{
    public partial class FrmMain : Form
    {
        public DB338 db;
        private IntSchTable currentTable = null;

        private OpenFileDialog openFileDialog = new OpenFileDialog()
        {
            Filter = "CSVs (*.csv)|*.csv",
            Title = "Import CSV file"
        };

        public FrmMain()
        {
            InitializeComponent();
            TxtQuery.CustomAction += BtnSubmitQuery_Click;
            SetupSyntaxHighlighting();
            db = new DB338(UpdateAutoComplete);
        }

        private void SetupSyntaxHighlighting()
        {
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.TxtQuery.SyntaxHighlighter.KeywordStyle =
                (Style) new TextStyle(Brushes.Coral, (Brush) null, FontStyle.Regular);
            this.TxtQuery.SyntaxHighlighter.TypesStyle =
                (Style) new TextStyle(Brushes.Coral, (Brush) null, FontStyle.Regular);
            this.TxtQuery.SyntaxHighlighter.StatementsStyle =
                (Style) new TextStyle(Brushes.Coral, (Brush) null, FontStyle.Regular);
            this.TxtQuery.SyntaxHighlighter.NumberStyle =
                (Style) new TextStyle(Brushes.LightSkyBlue, (Brush) null, FontStyle.Regular);
            this.TxtQuery.SyntaxHighlighter.StringStyle =
                (Style) new TextStyle(Brushes.DarkSeaGreen, (Brush) null, FontStyle.Regular);
            this.TxtQuery.SyntaxHighlighter.FunctionsStyle =
                (Style) new TextStyle(Brushes.Goldenrod, (Brush) null, FontStyle.Italic);
            this.TxtQuery.Text = resources.GetString("TxtQuery.Text");
        }

        private void UpdateAutoComplete()
        {
            List<string> autoCompleteList = new List<string>()
            {
                "SELECT", "FROM", "WHERE", "VALUES", "UPDATE", "DELETE", "DROP", "INSERT", "MIN", "MAX", "COUNT", "SUM",
                "AVG"
            };

            foreach (IntSchTable table in db.GetTables())
            {
                autoCompleteList.Add(table.Name);
            }

            autocompleteMenu1.Items = autoCompleteList.ToArray();
        }

        private void BtnSubmitQuery_Click(object sender, EventArgs e)
        {
            SubmitQuery();
        }

        private void SubmitQuery()
        {
            string[] trimmedLines = GetTrimmedLines();

            string[] queries = String.Join(" ", trimmedLines).Split(';').Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
            for (int i = 0; i < queries.Length; ++i)
            {
                QueryResult queryResult = db.SubmitQuery(queries[i]);
                IntSchTable queryResults = queryResult.Results;
                if (queryResult.Error.ErrorType != "none")
                {
                    ShowError(queryResult.Error);
                }
                else
                {
                    Output(queryResults);
                }
            }
        }

        private void ShowError(InputError error)
        {
            MessageBox.Show("Input SQL contained a " + error.ErrorType + " error.\n" + error.ErrorMessage);
        }

        private string[] GetTrimmedLines()
        {
            string[] trimmedLines = new string[TxtQuery.Lines.Count];
            for (int i = 0; i < TxtQuery.Lines.Count; ++i)
            {
                int commentIndex = TxtQuery.Lines[i].IndexOf("--");
                trimmedLines[i] = commentIndex >= 0 ? TxtQuery.Lines[i].Substring(0, commentIndex) : TxtQuery.Lines[i];
            }

            return trimmedLines;
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            FrmExportMessageBox frmExportMessageBox = new FrmExportMessageBox(db, currentTable);
            frmExportMessageBox.Show();
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                var sr = new StreamReader(openFileDialog.FileName);
                var createdTable = db.CreateTableFromImport(openFileDialog.FileName, sr.ReadToEnd());

                Output(createdTable);
            }
            catch (SecurityException ex)
            {
                MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                                $"Details:\n\n{ex.StackTrace}");
            }
        }

        public void Output(IntSchTable results)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();

            currentTable = results;

            if (results == null) return;

            List<string> columns = results.columnNames;
            dataGridView1.ColumnCount = columns.Count;

            for (int i = 0; i < columns.Count; i++)
            {
                dataGridView1.Columns[i].Name = columns[i];
            }

            for (int i = 0; i < results.numRows; i++)
            {
                string[] emptyRow = new string[columns.Count];
                dataGridView1.Rows.Add(emptyRow);
            }

            for (int col = 0; col < columns.Count; ++col)
            {
                string columnName = columns[col];
                IntSchColumn columnEntries = results.GetColumn(columnName);
                for (int row = 0; row < results.numRows; row++)
                {
                    dataGridView1.Rows[row].Cells[col].Value = columnEntries.Get(row);
                }
            }
        }
    }
    
}