using System;
using System.Collections.Generic;
using System.Linq;
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

        public string Name
        {
            get => name;
            set => name = value;
        }

        public void RemoveRow(int i)
        {
            items.RemoveAt(i);
        }

        public void SetEntryAt(int i, string valueToSet)
        {
            items[i] = valueToSet;
        }

        public string GetAverage()
        {
            float sum = 0;
            foreach (string item in items)
            {
                float n = float.Parse(item);
                sum += n;
            }

            float avg = sum / items.Count;
            return avg.ToString();
        }

        public string GetMax()
        {
            return items.Max();
        }

        public string GetMin()
        {
            return items.Min();
        }

        public string GetSum()
        {
            float sum = 0;
            foreach (string item in items)
            {
                float n = float.Parse(item);
                sum += n;
            }

            return sum.ToString();
        }

        public string GetCount()
        {
            return items.Count.ToString();
        }
    }
}