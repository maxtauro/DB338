using System;
using System.Collections.Generic;
using System.Text;

namespace DB338Core
{
    public class IntSchColumn
    {
        public List<string> items;

        private string name;

        public IntSchColumn(string newname, string type)
        {
            name = newname;

            items = new List<string>();
        }
        public string Get(int pos)
        {
            return items[pos];
        }

        public string Name { get => name; set => name = value; }

        public void RemoveRow(int i)
        {
            items.RemoveAt(i);
        }

        public void SetEntryAt(int i, string ValueToSet)
        {
            items[i] = ValueToSet;
        }
    }
}
