using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                success = ProcessCreateTableStatement(tokens, ref queryResult);
            }
            else if (type == "insert")
            {
                success = ProcessInsertStatement(tokens, ref queryResult);
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
                ProcessUpdateStatement(tokens, ref queryResult);
            }
            else
            {
                results = null;
            }
        }

        private void ProcessSelectStatement(List<string> tokens, ref QueryResult queryResult)
        {
            // <Select Stm> ::= SELECT <Columns> <From Clause> <Where Clause> <Group Clause> <Having Clause> <Order Clause>\

            IntSchTable tableToSelectFrom = null;
            IntSchTable results = null;

            List<string> colsToSelect = new List<string>();
            List<string> colsToValidate = new List<string>();
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
                else if (i + 3 < tokens.Count &&
                         (tokens[i].ToLower() == "avg" || tokens[i].ToLower() == "max" ||
                          tokens[i].ToLower() == "min" ||
                          tokens[i].ToLower() == "sum"))
                {
                    if (tokens[i + 2] == "*")
                    {
                        string errorMessage = "Invalid arguments for function " + tokens[i] + "()";
                        string errorType = "SQL";
                        queryResult.Error = new InputError(errorType, errorMessage);
                        return;
                    }

                    colsToSelect.Add(tokens[i] + tokens[i + 1] + tokens[i + 2] + tokens[i + 3]);
                    colsToValidate.Add(tokens[i + 2]);
                    i += 3;
                }
                else if (i + 3 < tokens.Count && tokens[i].ToLower() == "count")
                {
                    colsToSelect.Add(tokens[i] + tokens[i + 1] + tokens[i + 2] + tokens[i + 3]);
                    colsToValidate.Add(tokens[i + 2]);
                    i += 3;
                }
                else
                {
                    colsToSelect.Add(tokens[i]);
                    colsToValidate.Add(tokens[i]);
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
                string errorMessage = "Could not find table: " + nameOfTableToSelectFrom;
                string errorType = "SQL";
                queryResult.Error = new InputError(errorType, errorMessage);
                return;
            }

            if (!ValidateColumns(ref queryResult, colsToValidate, tableToSelectFrom, /*allowWildcard=*/ true)) return;

            SQLConditional sqlConditional = ParseWhereClause(tokens);

            // Validate conditional
            List<string> conditionalColumns = sqlConditional.GetColumns();
            if (!ValidateColumns(ref queryResult, conditionalColumns, tableToSelectFrom, /*allowWildCard=*/
                false)) return;

            // Execute Selection
            results = tableToSelectFrom.Select(colsToSelect, sqlConditional);
            queryResult.Results = results;
            return;
        }

        private bool ValidateColumns(ref QueryResult queryResult, List<string> colsToValidate,
            IntSchTable tableToValidate,
            bool allowWildCard)
        {
            List<string> missingColumns = new List<string>();

            foreach (string column in colsToValidate)
            {
                if (column != "*" && !tableToValidate.ContainsColumn(column))
                {
                    missingColumns.Add(column);
                }

                else if (column == "*" && !allowWildCard)
                {
                    queryResult.Error = new InputError("SQL", "Wildcard (*) is not allowed.");
                    return false;
                }
            }

            if (missingColumns.Count != 0)
            {
                string errorMessage = "Could not find columns: " + String.Join(",", missingColumns);
                string errorType = "SQL";
                queryResult.Error = new InputError(errorType, errorMessage);
                return false;
            }

            return true;
        }

        public List<IntSchTable> GetTables()
        {
            return tables;
        }

        private bool ProcessInsertStatement(List<string> tokens, ref QueryResult queryResult)
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
                    } else if (!ValidateColumns(ref queryResult, columnNames, tbl, false))
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

        private bool ProcessCreateTableStatement(List<string> tokens, ref QueryResult queryResult)
        {
            // assuming only the following rule is accepted
            // <Create Stm> ::= CREATE TABLE Id '(' <ID List> ')'  ------ NO SUPPORT for <Constraint Opt>

            string newTableName = tokens[2];

            foreach (IntSchTable tbl in tables)
            {
                if (tbl.Name == newTableName)
                {
                    string errorType = "SQL";
                    string errorMsg = "table " + tbl.Name + " already exists";
                    queryResult.Error = new InputError(errorType, errorMsg);
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

        private void ProcessUpdateStatement(List<string> tokens, ref QueryResult queryResult)
        {
            string nameOfTableToUpdate = tokens[1];

            List<string> columnsToUpdate = new List<string>();
            List<string> updatedValues = new List<string>();

            ParseUpdates(tokens, ref columnsToUpdate, ref updatedValues);

            SQLConditional conditional = ParseWhereClause(tokens);
            IntSchTable tableToUpdate = GetTable(nameOfTableToUpdate);

            // Validate update columns
            if (!ValidateColumns(ref queryResult, columnsToUpdate, tableToUpdate, /*allowWildCard=*/false)) return;

            // Validate conditional columns
            List<string> conditionalColumns = conditional.GetColumns();
            if (!ValidateColumns(ref queryResult, conditionalColumns, tableToUpdate, /*allowWildCard=*/ false)) return;

            tableToUpdate.Update(columnsToUpdate, updatedValues, conditional);
        }

        private void ParseUpdates(List<string> tokens, ref List<string> columnsToUpdate,
            ref List<string> updatedValues)
        {
            int setIndex = tokens.FindIndex(it => it == "SET" || it == "set");
            int whereIndex = tokens.FindIndex(it => it == "WHERE" || it == "where");
            int endIndex = whereIndex;

            if (endIndex == -1)
            {
                endIndex = tokens.Count;
            }

            for (int i = setIndex + 1; i < endIndex; i += 4)
            {
                columnsToUpdate.Add(tokens[i]);
                updatedValues.Add(tokens[i + 2]);
            }
        }

        private SQLConditional ParseWhereClause(List<string> tokens)
        {
            int whereIndex = 0;

            while (whereIndex < tokens.Count && tokens[whereIndex] != "WHERE" && tokens[whereIndex] != "where")
            {
                whereIndex++;
            }

            if (whereIndex < tokens.Count - 1)
            {
                int whereClauseStart = whereIndex + 1;
                int whereClauseEnd = whereClauseStart;

                for (; whereClauseEnd < tokens.Count; ++whereClauseEnd)
                {
                    // Rough stopping point for now, TODO implement more robust parsing
                    if (tokens[whereClauseEnd] == "GROUP" || tokens[whereClauseEnd] == "HAVING")
                    {
                        break;
                    }
                }

                string[] conditionSubArray =
                    tokens.ToList().GetRange(whereClauseStart, whereClauseEnd - whereClauseStart).ToArray();
                return new SQLConditional(conditionSubArray);
            }
            else
            {
                return new SQLConditional(new string[0]);
            }
        }

        private void ProcessDropStatement(List<string> tokens, ref QueryResult queryResult)
        {
            string nameOfTableToDrop = tokens[2];
            IntSchTable tableToDrop = GetTable(nameOfTableToDrop);

            if (tableToDrop == null)
            {
                string errorMessage = "Could not find table: " + nameOfTableToDrop;
                string errorType = "SQL";
                queryResult.Error = new InputError(errorType, errorMessage);
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
            
            SQLConditional sqlConditional = ParseWhereClause(tokens);

            if (!ValidateColumns(ref queryResult, sqlConditional.GetColumns(), tableToDeleteFrom,
                allowWildCard: false)) return;

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