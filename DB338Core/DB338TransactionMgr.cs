using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DB338Core
{
    class DB338TransactionMgr
    {
        //the List of Internal Schema Tables holds the actual data for DB338
        //it is implemented using Lists, which could be replaced.
        List<IntSchTable> tables;

        public DB338TransactionMgr()
        {
            tables = new List<IntSchTable>();
        }

        public void Process(List<string> tokens, string type, ref QueryResult queryResult)
        {
            Dictionary<String, List<String>> results = null;
            bool success;

            if (type == "create")
            {
                success = ProcessCreateTableStatement(tokens);
            }
            else if (type == "insert")
            {
                success = ProcessInsertStatement(tokens);
            }
            else if (type == "select")
            {
               ProcessSelectStatement(tokens, ref queryResult);
            }
            else if (type == "alter")
            {
               // results = ProcessAlterStatement(tokens);
            }
            else if (type == "delete")
            {
                ProcessDeleteStatement(tokens, ref queryResult);
            }
            else if (type == "drop")
            {
                 ProcessDropStatement(tokens, ref queryResult);
            }
            else if (type == "update")
            {
                // results = ProcessUpdateStatement(tokens);
            }
            else
            {
                results = null;
            }
            //other parts of SQL to do...
        }

        private void ProcessSelectStatement(List<string> tokens, ref QueryResult queryResult)
        {
            // <Select Stm> ::= SELECT <Columns> <From Clause> <Where Clause> <Group Clause> <Having Clause> <Order Clause>\

            IntSchTable tableToSelectFrom = null;
            IntSchTable results = null;

            List<string> colsToSelect = new List<string>();
            int tableOffset = 0;

            for (int i = 1; i < tokens.Count; ++i)
            {
                if (tokens[i] == "from")
                {
                    tableOffset = i + 1;
                    break;
                }
                else if (tokens[i] == ",")
                {
                    continue;
                }
                else
                {
                    colsToSelect.Add(tokens[i]);
                }
            }

            string nameOfTableToSelectFrom = tokens[tableOffset];

            // Validate Table's existence
            for (int i = 0; i < tables.Count; ++i)
            {
                if (tables[i].Name == nameOfTableToSelectFrom)
                {
                    tableToSelectFrom = tables[i];
                    break;
                }
            }

            if (tableToSelectFrom == null)
            {
                queryResult.Error = "Could not find table: " + nameOfTableToSelectFrom;
                return;
            }


            // Validate Columns
            List<string> missingColumns = new List<string>();           

            foreach (string column in colsToSelect)
            {
            if (column != "*" && !tableToSelectFrom.ContainsColumn(column))
                {
                    missingColumns.Add(column);
                }

            }

            if (missingColumns.Count != 0)
            {
                queryResult.Error = "Could not find columns: " + missingColumns;
                return;
            }

            // Parse WHERE clause
            SQLConditional sqlConditional;

            if (tableOffset + 1 < tokens.Count - 1 && (tokens[tableOffset + 1] == "WHERE" ||  tokens[tableOffset + 1] == "where"))
            {
                int whereClauseStart = tableOffset + 2;
                int whereClauseEnd = whereClauseStart;

                for (; whereClauseEnd < tokens.Count; ++whereClauseEnd)
                {
                    // Rough stopping point for now, TODO implement more robust parsing
                    if (tokens[whereClauseEnd] == "GROUP" || tokens[whereClauseEnd] == "HAVING")
                    {
                        break;
                    }
                }

                string[] conditionSubArray = tokens.ToList().GetRange(whereClauseStart, whereClauseEnd - whereClauseStart).ToArray();
                sqlConditional = new SQLConditional(conditionSubArray);
            } else
            {
                sqlConditional = new SQLConditional(new string[0]);
            }

            // Execute Selection
            results = tableToSelectFrom.Select(colsToSelect, sqlConditional);
            queryResult.Results = results;
            return;
        }

        public List<IntSchTable> GetTables()
        {
            return tables;
        }

        private bool ProcessInsertStatement(List<string> tokens)
        {
            // <Insert Stm> ::= INSERT INTO Id '(' <ID List> ')' VALUES '(' <Expr List> ')'

            string insertTableName = tokens[2];

            foreach (IntSchTable tbl in tables)
            {
                if (tbl.Name == insertTableName)
                {
                    List<string> columnNames = new List<string>();
                    List<string> columnValues = new List<string>();

                    int offset = 0;

                    for (int i = 4; i < tokens.Count; ++i)
                    {
                        if (tokens[i] == ")")
                        {
                            offset = i + 3;
                            break;
                        }
                        else if (tokens[i] == ",")
                        {
                            continue;
                        }
                        else
                        {
                            columnNames.Add(tokens[i]);
                        }
                    }

                    for (int i = offset; i < tokens.Count; ++i)
                    {
                        if (tokens[i] == ")")
                        {
                            break;
                        }
                        else if (tokens[i] == ",")
                        {
                            continue;
                        }
                        else
                        {
                            columnValues.Add(tokens[i]);
                        }
                    }

                    if (columnNames.Count != columnValues.Count)
                    {
                        return false;
                    }
                    else
                    {
                        tbl.Insert(columnNames, columnValues);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ProcessCreateTableStatement(List<string> tokens)
        {
            // assuming only the following rule is accepted
            // <Create Stm> ::= CREATE TABLE Id '(' <ID List> ')'  ------ NO SUPPORT for <Constraint Opt>

            string newTableName = tokens[2];

            foreach (IntSchTable tbl in tables)
            {
                if (tbl.Name == newTableName)
                {
                    //cannot create a new table with the same name
                    return false;
                }
            }

            List<string> columnNames = new List<string>();
            List<string> columnTypes = new List<string>();

            int idCount = 2;
            for (int i = 4; i < tokens.Count; ++i)
            {
                if (tokens[i] == ")")
                {
                    break;
                }
                else if (tokens[i] == ",")
                {
                    continue;
                }
                else
                {
                    if (idCount == 2)
                    {
                        columnNames.Add(tokens[i]);
                        --idCount;
                    }
                    else if (idCount == 1)
                    {
                        columnTypes.Add(tokens[i]);
                        idCount = 2;
                    }
                }
            }

            IntSchTable newTable = new IntSchTable(newTableName);

            for (int i = 0; i < columnNames.Count; ++i)
            {
                newTable.AddColumn(columnNames[i], columnTypes[i]);
            }

            tables.Add(newTable);

            return true;
        }

        private string[,] ProcessUpdateStatement(List<string> tokens)
        {
            throw new NotImplementedException();
        } 

        private void ProcessDropStatement(List<string> tokens, ref QueryResult queryResult)
        {
            string nameOfTableToDrop = tokens[2];
            IntSchTable tableToDrop = GetTable(nameOfTableToDrop);

            //TODO make error message more verbose.
            if (tableToDrop == null)
            {
                queryResult.Error = "Could not find table: " + nameOfTableToDrop;
                return;
            }

            tables.Remove(tableToDrop);
        }

        private void ProcessDeleteStatement(List<string> tokens, ref QueryResult queryResult)
        {
            // <Delete Stm> ::= DELETE <From Clause> TABLE <Where Clause>
            IntSchTable tableToDeleteFrom = null;

            int tableOffset = 0;

            for (int i = 1; i < tokens.Count; ++i)
            {
                if (tokens[i] == "from")
                {
                    tableOffset = i + 1;
                    break;
                }
            }

            tableToDeleteFrom = GetTable(tokens[tableOffset]);


            SQLConditional sqlConditional;

            if (tableOffset + 1 < tokens.Count - 1 && (tokens[tableOffset + 1] == "WHERE" || tokens[tableOffset + 1] == "where"))
            {
                int whereClauseStart = tableOffset + 2;
                int whereClauseEnd = whereClauseStart;

                for (; whereClauseEnd < tokens.Count; ++whereClauseEnd)
                {
                    // Rough stopping point for now, TODO implement more robust parsing
                    if (tokens[whereClauseEnd] == "GROUP" || tokens[whereClauseEnd] == "HAVING")
                    {
                        break;
                    }
                }

                string[] conditionSubArray = tokens.ToList().GetRange(whereClauseStart, whereClauseEnd - whereClauseStart).ToArray();
                sqlConditional = new SQLConditional(conditionSubArray);
            }
            else
            {
                sqlConditional = new SQLConditional(new string[0]);
            }

            tableToDeleteFrom.Delete(sqlConditional);
        }

        private string[,] ProcessAlterStatement(List<string> tokens)
        {
            throw new NotImplementedException();
        }

        private IntSchTable GetTable(string tableName)
        {
            for (int i = 0; i < tables.Count; ++i)
            {
                if (tables[i].Name == tableName)
                {
                   return tables[i];
                }
            }

            return null;
        }
    }
}
