using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
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
            TextReader scriptText = new StringReader(query);

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

        public string CreateTableFromImport(string tableName, string tableImport)
        {
            // create table test(col1 whatever, col2 whatever, col3 whatever)
            string[] tableSplit = tableImport.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            string[] columnNames = tableSplit[0].Split(',');

            char[] tableNameChar = Array.FindAll<char>(Path.GetFileName(tableName).Split('.')[0].ToCharArray(), (c => (char.IsLetterOrDigit(c))));

            string tableNameToCreate = new string(tableNameChar);

            int numDuplicates = 0;
            while (transactionMgr.GetTables().FindIndex(table => table.Name == tableNameToCreate) >= 0)
            {
                tableNameToCreate += ++numDuplicates;
            }

            StringBuilder createStatmentBuilder = new StringBuilder();

            createStatmentBuilder.Append("create table ");
            createStatmentBuilder.Append(tableNameToCreate);

            createStatmentBuilder.Append("(");

            for (int i = 0; i < columnNames.Length; ++i)
            {
                if (i != columnNames.Length - 1)
                {
                    createStatmentBuilder.Append(columnNames[i] + " whatever, ");
                } else
                {
                    createStatmentBuilder.Append(columnNames[i] + " whatever");
                }
            }

            createStatmentBuilder.Append(")");

            SubmitQuery(createStatmentBuilder.ToString());

            StringBuilder insertStatementBuilder = new StringBuilder();

            for (int i = 1; i < tableSplit.Length; ++i)
            {
                insertStatementBuilder.Append("insert into ");
                insertStatementBuilder.Append(tableNameToCreate);
                insertStatementBuilder.Append("(");
                insertStatementBuilder.Append(string.Join(", ", columnNames));
                insertStatementBuilder.Append(") ");
                insertStatementBuilder.Append("values");
                insertStatementBuilder.Append("(");
                insertStatementBuilder.Append(tableSplit[i]);
                insertStatementBuilder.Append(") ");

                SubmitQuery(insertStatementBuilder.ToString());

                insertStatementBuilder.Clear();
            }

            return tableNameToCreate;
        }
    }
}
