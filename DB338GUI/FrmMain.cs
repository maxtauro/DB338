using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DB338Core;

namespace DB338GUI
{
    public partial class FrmMain : Form
    {
        public DB338 db;

        public FrmMain()
        {
            InitializeComponent();
            db = new DB338();
        }

        private void BtnSubmitQuery_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < TxtQuery.Lines.Length; ++i)
            {
                QueryResult queryResult = db.SubmitQuery(TxtQuery.Lines[i]);
                IntSchTable queryResults = queryResult.Results;
                if (queryResult.Error != "none")
                {
                    //null means error
                    MessageBox.Show("Input SQL contained a " + queryResult.Error + " error.");
                }
                else {
                    Output(queryResults);
                }
            }
        }

        public void Output(IntSchTable results)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();

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
