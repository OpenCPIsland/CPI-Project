using Disney.LaunchPadFramework.Utility;
using System;
using System.Collections.Generic;

namespace Disney.Kelowna.Common
{
	public class TableBuilder
	{
		private struct ColumnDefition
		{
			public string HeaderLabel;

			public Type FieldType;

			public Func<object, string> Formatter;

			public int MinPadding;
		}

		private MessageBuilder parent;

		private readonly List<ColumnDefition> columnDefinitions = new List<ColumnDefition>();

		private readonly List<List<object>> rows = new List<List<object>>();

		private int minColumnPadding;

		public TableBuilder()
		{
			if (parent == null)
			{
				parent = new MessageBuilder();
			}
		}

		public TableBuilder(MessageBuilder parent)
			: this()
		{
			this.parent = parent;
		}

		public TableBuilder Column(string headerLabel, Func<object, string> columnFormatter = null, int minPadding = 0)
		{
			return this.Column<object>(headerLabel, columnFormatter, minPadding);
		}

		public TableBuilder Column<TField>(string headerLabel, Func<TField, string> columnFormatter = null, int minPadding = 0)
		{
			minPadding.Clamp(0, 100);
			ColumnDefition item = default(ColumnDefition);
			item.FieldType = typeof(TField);
			item.HeaderLabel = headerLabel;
			item.MinPadding = minColumnPadding + minPadding;
			if (columnFormatter != null)
			{
				item.Formatter = ((object obj) => (obj is TField) ? columnFormatter((TField)obj) : obj.ToString());
			}
			else
			{
				item.Formatter = null;
			}
			columnDefinitions.Add(item);
			return this;
		}

		public TableBuilder Row(params object[] fields)
		{
			rows.Add(new List<object>(fields));
			return this;
		}

		public TableBuilder MinColumnPadding(int count)
		{
			minColumnPadding = count;
			minColumnPadding.Clamp(0, 100);
			return this;
		}

		public MessageBuilder End()
		{
			formatRows();
			int count = columnDefinitions.Count;
			int[] array = new int[count];
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				num += (array[i] = calculateColumnWidth(i));
			}
			num += count + 1;
			parent.AppendLine(getHorizontalDivider(num, '=')).Indent();
			for (int j = 0; j < count; j++)
			{
				string headerLabel = columnDefinitions[j].HeaderLabel;
				parent.Append("|");
				int length = headerLabel.Length;
				int num2 = array[j];
				int num3 = num2 / 2;
				int num4 = num3 - length / 2;
				if (num4 > 0)
				{
					parent.Append("".PadRight(num4));
				}
				parent.Append(headerLabel.PadRight(num2 - num4));
			}
			parent.Append("|\n");
			parent.AppendLine(getHorizontalDivider(num, '='));
			foreach (List<object> row in rows)
			{
				parent.Indent();
				for (int j = 0; j < row.Count; j++)
				{
					object field = row[j];
					parent.Append("|");
					if (minColumnPadding > 0)
					{
						parent.Append(" ".PadRight(minColumnPadding));
					}
					parent.Append(getFieldString(field).PadRight(array[j] - minColumnPadding));
				}
				parent.Append("|\n");
				parent.AppendLine(getHorizontalDivider(num));
			}
			return parent;
		}

		private void formatRows()
		{
			foreach (List<object> row in rows)
			{
				for (int i = 0; i < row.Count; i++)
				{
					if (i < columnDefinitions.Count)
					{
						Func<object, string> formatter = columnDefinitions[i].Formatter;
						if (formatter != null)
						{
							row[i] = formatter(row[i]);
						}
					}
				}
			}
		}

		private string getFieldString(object field)
		{
			string result = "";
			if (field != null)
			{
				result = field.ToString();
			}
			return result;
		}

		private string getHorizontalDivider(int length, char c = '-')
		{
			return "".PadRight(length, c);
		}

		private int calculateColumnWidth(int columnIndex)
		{
			int num = 0;
			if (columnDefinitions.Count > 0)
			{
				num = Math.Max(num, columnDefinitions[columnIndex].HeaderLabel.Length);
			}
			foreach (List<object> row in rows)
			{
				if (columnIndex < row.Count)
				{
					num = Math.Max(num, getFieldString(row[columnIndex]).Length);
				}
			}
			return num + 2 * minColumnPadding;
		}
	}
}
