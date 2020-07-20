using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DB338Core
{
    public class IntSchTable
    {
        private string name;
        private readonly bool allowDuplicateCols;

        public List<string> columnNames = new List<string>();
        private List<IntSchColumn> columns;
        public int numRows = 0;

        public IntSchTable(string initname, bool allowDuplicateCols = false)
        {
            this.name = initname;
            this.columns = new List<IntSchColumn>();
            this.allowDuplicateCols = allowDuplicateCols;
        }

        public string Name { get => name; set => name = value; }


        public IntSchTable Select(List<string> columnsToSelect, SQLConditional conditional)
        {
            IntSchTable resultTable = new IntSchTable("Current Table", /*allowDuplicateCols=*/ true);
            Dictionary<String, List<String>> result = new Dictionary<string, List<string>>();

            foreach (string columnToSelect in columnsToSelect)
            {
                if (columnToSelect == "*")
                {
                    foreach (IntSchColumn column in this.columns)
                    {
                        resultTable.AddColumn(column.Name, "Any");
                        result[column.Name] = new List<String>();
                    }
                }
                else
                {
                    resultTable.AddColumn(columnToSelect, "Any");
                    result[columnToSelect] = new List<String>();
                }
            }

            foreach (IntSchColumn columnToSelect in resultTable.columns)
            {
                string columnName = columnToSelect.Name;

                for (int i = 0; i < columns.Count; ++i)
                {
                    if (columnName == columns[i].Name)
                    {
                        for (int z = 0; z < columns[i].items.Count; ++z)
                        {
                            result[columnName].Add(columns[i].items[z]);
                        }
                    }
                }
            }

            for (int row = 0; row < this.numRows; ++row)
            {
                List<string> rowVals = new List<string>();

                foreach (string column in resultTable.columnNames)
                {
                    rowVals.Add(result[column][row]);
                }


                if (conditional.evaluate(this.columnNames.ToArray(), this.GetRow(row)))
                {
                    resultTable.Insert(resultTable.columnNames, rowVals);
                }
            }

            return resultTable;
        }

        public void Insert(List<string> cols, List<string> vals)
        {
            for (int i = 0; i < cols.Count; ++i)
            {
                for (int j = 0; j < columns.Count; ++j)
                {
                    if (columns[j].Name == cols[i])
                    {
                        columns[j].items.Add(vals[i]);
                    }
                }
            }

            numRows++;
        }

        public bool AddColumn(string name, string type)
        {
            if (!allowDuplicateCols)
            {
                foreach (IntSchColumn col in columns)
                {
                    if (col.Name == name)
                    {
                        return false;
                    }
                }
            }
            
            columnNames.Add(name);
            columns.Add(new IntSchColumn(name, type));

            return true;
        }

        public void WriteToStreamWriter(ref StreamWriter streamWriter)
        {
            streamWriter.WriteLine(String.Join(",", this.GetColumnNames()));

            for (int r = 0; r < this.numRows; r++)
            {
                List<string> currRow = new List<String>();

                for (int col = 0; col < this.GetColumnNames().Count; ++col)
                {
                    string columnName = this.columnNames[col];
                    IntSchColumn columnEntries = this.GetColumn(columnName);

                    currRow.Add(columnEntries.Get(r));
                }

                string currRowString = String.Join(",", currRow);
                streamWriter.WriteLine(currRowString);
            }
        }

        public List<string> GetColumnNames()
        {
            return columnNames;
        }

        public int GetColumnCount()
        {
            return columnNames.Count;
        }

        public IntSchColumn GetColumn(string name)
        {
            foreach (IntSchColumn col in columns)
            {
                if (col.Name == name)
                {
                    return col;
                }
            }

            return null;
        }

        public IntSchColumn GetColumn(int columnIndex)
        {
            if (columnIndex >= columns.Count) return null;
            return columns[columnIndex];
        }

        public List<string[]> GetRows()
        {
            List<string[]> result = new List<string[]>();

            for (int i = 0; i < numRows; ++i)
            {
                result[i] = this.GetRow(i);
            }

            return result;
        }

        public string[] GetRow(int i)
        {
            string[] row = new string[columns.Count];

            for (int col = 0; col < columns.Count; ++col)
            {
                string columnName = columnNames[col];
                IntSchColumn columnEntries = this.GetColumn(columnName);

                row[col] = columnEntries.Get(i);
            }

            return row;
        }

        public int GetRowCount()
        {
            return numRows;
        }

        public bool ContainsColumn(string name)
        {
            foreach (IntSchColumn col in columns)
            {
                if (col.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
