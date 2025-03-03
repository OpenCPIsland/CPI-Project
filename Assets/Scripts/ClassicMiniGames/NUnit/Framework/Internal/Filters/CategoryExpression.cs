namespace NUnit.Framework.Internal.Filters
{
	public class CategoryExpression
	{
		private static readonly char[] ops = new char[7]
		{
			',',
			';',
			'-',
			'|',
			'+',
			'(',
			')'
		};

		private string text;

		private int next;

		private string token;

		private TestFilter filter;

		public TestFilter Filter
		{
			get
			{
				if (filter == null)
				{
					filter = ((GetToken() == null) ? TestFilter.Empty : GetExpression());
				}
				return filter;
			}
		}

		public CategoryExpression(string text)
		{
			this.text = text;
			next = 0;
		}

		private TestFilter GetExpression()
		{
			TestFilter term = GetTerm();
			if (token != "|")
			{
				return term;
			}
			OrFilter orFilter = new OrFilter(term);
			while (token == "|")
			{
				GetToken();
				orFilter.Add(GetTerm());
			}
			return orFilter;
		}

		private TestFilter GetTerm()
		{
			TestFilter primitive = GetPrimitive();
			if (token != "+" && token != "-")
			{
				return primitive;
			}
			AndFilter andFilter = new AndFilter(primitive);
			while (token == "+" || token == "-")
			{
				string a = token;
				GetToken();
				primitive = GetPrimitive();
				andFilter.Add((a == "-") ? new NotFilter(primitive) : primitive);
			}
			return andFilter;
		}

		private TestFilter GetPrimitive()
		{
			if (token == "-")
			{
				GetToken();
				return new NotFilter(GetPrimitive());
			}
			if (token == "(")
			{
				GetToken();
				TestFilter expression = GetExpression();
				GetToken();
				return expression;
			}
			return GetCategoryFilter();
		}

		private CategoryFilter GetCategoryFilter()
		{
			CategoryFilter categoryFilter = new CategoryFilter(token);
			while (GetToken() == "," || token == ";")
			{
				categoryFilter.AddCategory(GetToken());
			}
			return categoryFilter;
		}

		public string GetToken()
		{
			SkipWhiteSpace();
			if (EndOfText())
			{
				token = null;
			}
			else if (NextIsOperator())
			{
				token = text.Substring(next++, 1);
			}
			else
			{
				int num = text.IndexOfAny(ops, next);
				if (num < 0)
				{
					num = text.Length;
				}
				token = text.Substring(next, num - next).TrimEnd();
				next = num;
			}
			return token;
		}

		private void SkipWhiteSpace()
		{
			while (next < text.Length && char.IsWhiteSpace(text[next]))
			{
				next++;
			}
		}

		private bool EndOfText()
		{
			return next >= text.Length;
		}

		private bool NextIsOperator()
		{
			char[] array = ops;
			foreach (char c in array)
			{
				if (c == text[next])
				{
					return true;
				}
			}
			return false;
		}
	}
}
