using DB338Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EduDBGUI
{
    public partial class FrmExportMessageBox : Form
    {
        private DB338 db;
        private IntSchTable currentTable;

        public FrmExportMessageBox()
        {
            InitializeComponent();
        }

        public FrmExportMessageBox(DB338 db, IntSchTable currentTable)
        {
            InitializeComponent();
            this.db = db;
            this.currentTable = currentTable;

            InitializeComboBox(db, currentTable);
            SetUpButtons();
        }

        private void SetUpButtons()
        {
            if (comboBox_ExportTables.Items.Count <= 0)
            {
                BtnExport.Enabled = false;
            } else
            {
                BtnExport.Enabled = true;
            }
        }

        private void InitializeComboBox(DB338 db, IntSchTable currentTable)
        {
            List<IntSchTable> tablesToExport = new List<IntSchTable>();

            if (currentTable != null)
            {
                tablesToExport.Add(currentTable);
            }

            List<IntSchTable> dbTables = db.GetTables();

            foreach (IntSchTable table in dbTables)
            {
                tablesToExport.Add(table);
            }


            comboBox_ExportTables.DataSource = tablesToExport;
            comboBox_ExportTables.DisplayMember = "Name";
            comboBox_ExportTables.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            IntSchTable tableToExport = (IntSchTable) comboBox_ExportTables.SelectedItem;
            SaveFileDialog savefile = new SaveFileDialog();

            savefile.FileName = tableToExport.Name;
            savefile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm;*.csv";
            savefile.DefaultExt = "csv";

            savefile.CheckPathExists = true;

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                StreamWriter streamWriter = new StreamWriter(savefile.FileName);

                streamWriter.WriteLine(String.Join(",", tableToExport.GetColumns()));

                for (int r = 0; r < tableToExport.numRows; r++)
                {
                    List<string> currRow = new List<String>();

                    for (int col = 0; col < tableToExport.GetColumns().Count; ++col)
                    {
                        string columnName = tableToExport.columnNames[col];
                        IntSchColumn columnEntries = tableToExport.GetColumn(columnName);

                        currRow.Add(columnEntries.Get(r));
                    }

                    string currRowString = String.Join(",", currRow);
                    streamWriter.WriteLine(currRowString);
                }

                streamWriter.Close();
                this.Close();

            } else
            {
                MessageBox.Show("Failed to export table " + tableToExport.Name);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
