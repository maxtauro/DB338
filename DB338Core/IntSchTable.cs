using System;
using System.Collections.Generic;
using System.Text;

namespace DB338Core
{
    class IntSchTable
    {

        private string name;

        private List<IntSchColumn> columns;

        public IntSchTable(string initname)
        {
            name = initname;
            columns = new List<IntSchColumn>();
        }

        public string Name { get => name; set => name = value; }


        public Dictionary<String, List<String>> Select(List<string> columnsToSelect)
        {
            string[,] results = new string[columns[0].items.Count + 1, columnsToSelect.Count];
            Dictionary<String, List<String>> result = new Dictionary<string, List<string>>();

            for (int i = 0; i < columnsToSelect.Count; ++i)
            {
                results[0, i] = columnsToSelect[i];
            }

            foreach (string columnToSelect in columnsToSelect)
            {
                result[columnToSelect] = new List<String>();

                for (int i = 0; i < columns.Count; ++i)
                {
                    if (columnToSelect == columns[i].Name)
                    {
                        for (int z = 0; z < columns[i].items.Count; ++z)
                        {
                            result[columnToSelect].Add(columns[i].items[z]);
                        }
                    }
                }
            }

            return result;
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
        }

        public bool AddColumn(string name, string type)
        {
            foreach (IntSchColumn col in columns)
            {
                if (col.Name == name)
                {
                    return false;
                }
            }

            columns.Add(new IntSchColumn(name, type));
            return true;
        }
    }
}
