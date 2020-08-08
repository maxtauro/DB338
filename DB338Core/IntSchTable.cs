using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public string Name
        {
            get => name;
            set => name = value;
        }


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
                string functionName = null;
                string functionParameter = null;

                bool isFunction = IsSelectionFunction(columnName, ref functionName, ref functionParameter);
                
                for (int i = 0; i < columns.Count; ++i)
                {
                    string resultColumnName = columnName;

                    if (isFunction)
                    {
                        resultColumnName = functionParameter;
                    } 
                 
                    if (resultColumnName == columns[i].Name)
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
            
            foreach (IntSchColumn resultTableColumn in resultTable.columns)
            {
                string functionName = null;
                string functionParameter = null;

                bool isFunction = IsSelectionFunction(resultTableColumn.Name, ref functionName, ref functionParameter);

                if (isFunction)
                {
                    string functionResult;
                    
                    if (functionName == "avg")
                    {
                        functionResult = resultTableColumn.GetAverage();
                    }
                    else if (functionName == "max")
                    {
                        functionResult = resultTableColumn.GetMax();
                    }
                    else if (functionName == "min")
                    {
                        functionResult = resultTableColumn.GetMin();
                    }
                    else if (functionName == "sum")
                    {
                        functionResult = resultTableColumn.GetSum();
                    }
                    else
                    {
                        throw new ArgumentException("Invalid function: " + functionName);
                    }
                    
                    resultTableColumn.items[0] = functionResult;
                    resultTable.numRows = 1;
                }
            }

            return resultTable;
        }

        private string GetColumnAverage(string columnName)
        {
            IntSchColumn column = GetColumn(columnName);
            return column.GetAverage();
        }

        private bool IsSelectionFunction(string columnName, ref string functionName, ref string functionParameter)
        {
            if (columnName.Length < 5) return false;

            // Check if the first three chars are: Max, Min, Avg, Sum
            string potentialFunctionName = columnName.Substring(0, 3).ToLower();

            if (potentialFunctionName == "max" || potentialFunctionName == "min" || potentialFunctionName == "avg" ||
                potentialFunctionName == "sum")
            {
                functionName = potentialFunctionName;
                functionParameter = columnName.Substring(4, columnName.Length - 5);
                return true;
            }

            if (columnName.Length < 6)
            {
                return false;
            }

            // Check if function is Last
            potentialFunctionName = columnName.Substring(0, 4).ToLower();

            if (potentialFunctionName == "last")
            {
                functionName = potentialFunctionName;
                functionParameter = columnName.Substring(6, columnName.Length - 7);
                return true;
            }

            if (columnName.Length < 7)
            {
                return false;
            }

            // Check if function is First or Count
            potentialFunctionName = columnName.Substring(0, 5).ToLower();

            if (potentialFunctionName == "first" || potentialFunctionName == "count")
            {
                functionName = potentialFunctionName;
                functionParameter = columnName.Substring(7, columnName.Length - 8);
                return true;
            }

            return false;
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


        public void Delete(SQLConditional sqlConditional)
        {
            List<int> rowsToDelete = new List<int>();

            for (int i = numRows - 1; i >= 0; --i)
            {
                if (sqlConditional.evaluate(this.columnNames.ToArray(), GetRow(i)))
                {
                    rowsToDelete.Add(i);
                }
            }

            foreach (IntSchColumn col in columns)
            {
                foreach (int rowIndex in rowsToDelete)
                {
                    col.RemoveRow(rowIndex);
                }
            }

            numRows -= rowsToDelete.Count;
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
                result.Add(GetRow(i));
            }

            return result;
        }

        public string[] GetRow(int i)
        {
            string[] row = new string[columns.Count];

            for (int col = 0; col < columns.Count; ++col)
            {
                string columnName = columnNames[col];
                IntSchColumn columnEntries = GetColumn(columnName);

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

        public void Update(List<string> columnsToUpdate, List<string> updatedValues, SQLConditional conditional)
        {
            List<string[]> rows = GetRows();
            string[] columnNames = GetColumnNames().ToArray();

            for (int i = 0; i < rows.Count; ++i)
            {
                if (conditional.evaluate(columnNames, rows[i]))
                {
                    foreach (var colNameAndValue in columnsToUpdate.Zip(updatedValues, Tuple.Create))
                    {
                        string columnToUpdate = colNameAndValue.Item1;
                        string valueToSet = colNameAndValue.Item2;
                        SetEntry(columnToUpdate, i, valueToSet);
                    }
                }
            }
        }

        private void SetEntry(string ColumnName, int RowIndex, string ValueToSet)
        {
            IntSchColumn columnToUpdate = GetColumn(ColumnName);
            columnToUpdate.SetEntryAt(RowIndex, ValueToSet);
        }
    }
}