using System.Text.RegularExpressions;

namespace NUnit.Framework.Constraints
{
	public class RegexConstraint : StringConstraint
	{
		public RegexConstraint(string pattern)
			: base(pattern)
		{
		}

		protected override bool Matches(string actual)
		{
			return Regex.IsMatch(actual, expected, caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String matching");
			writer.WriteExpectedValue(expected);
			if (caseInsensitive)
			{
				writer.WriteModifier("ignoring case");
			}
		}
	}
}
