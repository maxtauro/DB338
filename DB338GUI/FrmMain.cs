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
                Dictionary<String, List<String>> queryResults = queryResult.Results;
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

        public void Output(Dictionary<String, List<String>> results)
        {
            if (results == null) return;

            List<String> columns = new List<String>(results.Keys);
            int numRows = results[columns[0]].Count;

            dataGridView1.ColumnCount = columns.Count;

            for (int i = 0; i < columns.Count; i++)
            {
                dataGridView1.Columns[i].Name = columns[i];
            }

            for (int i = 0; i < numRows; i++)
            {
                string[] emptyRow = new string[columns.Count];
                dataGridView1.Rows.Add(emptyRow);
            }

            foreach (string col in columns)
            {
                List<String> columnEntries = results[col];
                for (int row = 0; row < numRows; row++)
                {
                    dataGridView1.Rows[row].Cells[col].Value = columnEntries[row];
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
