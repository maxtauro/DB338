using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace DB338Core
{
    public class DB338
    {
        private SQLParser sqlParser;
        private DB338TransactionMgr transactionMgr;

        public DB338()
        {
            sqlParser = new SQLParser();
            sqlParser.Setup();
            transactionMgr = new DB338TransactionMgr();
        }

        public QueryResult SubmitQuery(string query)
        {
            TextReader scriptText = new StringReader(query.ToLower());

            List<string> tokens = sqlParser.Parse(scriptText);

            //sqlParser.Parse will always return the tokens of the query if accepted
            //the TYPE of the query is passed into the transaction manager, so you know what to check

            string queryType = tokens[tokens.Count - 4];
            string done = tokens[tokens.Count - 3];
            string accepted = tokens[tokens.Count - 2];
            string error = tokens[tokens.Count - 1];

            tokens.RemoveAt(tokens.Count - 1);
            tokens.RemoveAt(tokens.Count - 1);
            tokens.RemoveAt(tokens.Count - 1);
            tokens.RemoveAt(tokens.Count - 1);

            if (accepted == "True")
            {
                QueryResult queryResult = new QueryResult(queryType, done, accepted, error);
                transactionMgr.Process(tokens, queryType, ref queryResult);
                return queryResult;
            }
            else
            {
                //error
                QueryResult queryResult = new QueryResult(queryType, done, accepted, error);
                queryResult.Results = null;
                return queryResult;
            }
        }

        public List<IntSchTable> GetTables()
        {
            return transactionMgr.GetTables();
        }

        public IntSchTable CreateTableFromImport(string importName, string importContents)
        {
            // TODO validate that the csv is in a valid format

            // Split the imported csv by line
            string[] tableContents = importContents.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.None
            );

            string[] columnNames = tableContents[0].Split(',');

            string tableNameToCreate = GetNewTableName(importName);
            string newTableCreateStatement =
                GetCreateStatementForNewTable(tableNameToCreate, columnNames);
            string[] insertStatements = GenerateInsertStatements(tableNameToCreate, columnNames, tableContents);

            SubmitQuery(newTableCreateStatement);
            foreach (string insertStatement in insertStatements)
            {
                SubmitQuery(insertStatement);
            }

            IntSchTable createdTable = GetTable(tableNameToCreate);

            return createdTable;
        }

        private IntSchTable GetTable(string tableName)
        {
            var tables = GetTables();
            return tables.FirstOrDefault(table => table.Name == tableName);
        }

        private string[] GenerateInsertStatements(string tableName, string[] columnNames, string[] importContents)
        {
            string[] insertStatements = new string[importContents.Length - 1];
            StringBuilder insertStatementBuilder = new StringBuilder();

            for (int i = 1; i < importContents.Length; ++i)
            {
                insertStatementBuilder.Append("insert into ");
                insertStatementBuilder.Append(tableName);
                insertStatementBuilder.Append("(");
                insertStatementBuilder.Append(string.Join(", ", columnNames));
                insertStatementBuilder.Append(") ");
                insertStatementBuilder.Append("values");
                insertStatementBuilder.Append("(");
                insertStatementBuilder.Append(importContents[i]);
                insertStatementBuilder.Append(") ");

                insertStatements[i - 1] = insertStatementBuilder.ToString();

                insertStatementBuilder.Clear();
            }

            return insertStatements;
        }

        private string GetCreateStatementForNewTable(string tableNameToCreate, string[] columnNames)
        {
            StringBuilder createStatmentBuilder = new StringBuilder();

            createStatmentBuilder.Append("create table ");
            createStatmentBuilder.Append(tableNameToCreate);

            createStatmentBuilder.Append("(");

            for (int i = 0; i < columnNames.Length; ++i)
            {
                if (i != columnNames.Length - 1)
                {
                    createStatmentBuilder.Append(columnNames[i] + " whatever, ");
                }
                else
                {
                    createStatmentBuilder.Append(columnNames[i] + " whatever");
                }
            }

            createStatmentBuilder.Append(")");

            return createStatmentBuilder.ToString();
        }

        private string GetNewTableName(string tableName)
        {
            char[] tableNameChar = Array.FindAll<char>(Path.GetFileName(tableName).Split('.')[0].ToCharArray(),
                (c => (char.IsLetterOrDigit(c))));

            string tableNameToCreate = new string(tableNameChar);

            int numDuplicates = 0;
            while (transactionMgr.GetTables().FindIndex(table => table.Name == tableNameToCreate) >= 0)
            {
                if (numDuplicates == 0)
                {
                    tableNameToCreate = tableNameToCreate + ++numDuplicates;
                }
                else
                {
                    tableNameToCreate = new string(tableNameChar) + ++numDuplicates;
                }
            }

            return tableNameToCreate;
        }
    }
}