using System;
using System.Collections.Generic;

namespace DB338Core
{


	public class SQLConditional
	{
		private List<Conditional> conditions = new List<Conditional>();
		 
		/*
		 * This class will be used to evaluate basic conditionals for our SQL statements
		 * For now, this class will only evaluate conditionals with OR statements and equality
		 * All other statements will evaluate to false at this time.
		 */

		public SQLConditional(string[] conditions)
		{ 
			for (int i = 0; i < conditions.Length;++i)
			{
				this.conditions.Add(new Conditional(conditions[i], conditions[i + 1], conditions[i + 2]));
				while (i < conditions.Length && (conditions[i].ToLower() != "or" && conditions[i].ToLower() != "and"))
                {
					i++;
                }
			}
		}


		/*
		 * A basic conditional evaluation, it evaluates each condition under the assumption that they are linked as OR
		 */
		public Boolean evaluate(string[] cols, string[] columnEntries)
		{
			if (conditions.Count == 0)
            {
				return true;
            }

			foreach (Conditional condition in conditions)
			{
				if (condition.evaluate(cols, columnEntries))
				{
					return true;
				}
			}

			return false;
		}

		/* 
		 *  A Triple consisting of a column name, condidtion type and the condition itself
		 */
		private struct Conditional
		{
			private readonly string columnName;
			private readonly string conditionType;
			private readonly string condition;

			public Conditional(string columnName, string conditionType, string condition)
			{
				this.columnName = columnName;
				this.conditionType = conditionType;
				this.condition = condition;
			}

			public bool evaluate(string[] col, string[] columnEntries)
			{
				string correspondingCol = null;

				for (int i = 0; i < col.Length; ++i)
				{
					if (col[i] == columnName)
					{
						correspondingCol = columnEntries[i];
						break;
					}
				}

				if (conditionType == "=" || conditionType == "IS")
				{
					return condition.Equals(correspondingCol);
				}
				else
				{
					throw new NotImplementedException("Have not implemented conditional for: " + conditionType);
				}
			}
		}

	}
}