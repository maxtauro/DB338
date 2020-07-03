using System;
using System.Collections.Generic;
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


        public IntSchTable Select(List<string> columnsToSelect)
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
                 
                resultTable.Insert(resultTable.columnNames, rowVals);
            }

            return resultTable;
        }

        public List<string> GetColumns()
        {
            return columnNames;
        }

        public bool Project()
        {
            throw new NotImplementedException();
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
