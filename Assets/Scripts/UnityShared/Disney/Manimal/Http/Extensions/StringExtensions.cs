using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Disney.Manimal.Http.Extensions
{
	public static class StringExtensions
	{
		public static string UrlEncode(this string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (input.Length <= 32766)
			{
				return Uri.EscapeDataString(input);
			}
			StringBuilder stringBuilder = new StringBuilder(input.Length * 2);
			string text;
			for (int i = 0; i < input.Length; i += text.Length)
			{
				int length = Math.Min(input.Length - i, 32766);
				text = input.Substring(i, length);
				stringBuilder.Append(Uri.EscapeDataString(text));
			}
			return stringBuilder.ToString();
		}

		public static string RemoveUnderscoresAndDashes(this string input)
		{
			return input.Replace("_", "").Replace("-", "");
		}

		public static DateTime ParseJsonDate(this string input, CultureInfo culture)
		{
			input = input.Replace("\n", "");
			input = input.Replace("\r", "");
			input = input.RemoveSurroundingQuotes();
			long? num = null;
			try
			{
				num = long.Parse(input);
			}
			catch (Exception)
			{
			}
			if (num.HasValue)
			{
				return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(num.Value);
			}
			if (input.Contains("/Date("))
			{
				return ExtractDate(input, "\\\\?/Date\\((-?\\d+)(-|\\+)?([0-9]{4})?\\)\\\\?/", culture);
			}
			if (input.Contains("new Date("))
			{
				input = input.Replace(" ", "");
				return ExtractDate(input, "newDate\\((-?\\d+)*\\)", culture);
			}
			return ParseFormattedDate(input, culture);
		}

		public static string RemoveSurroundingQuotes(this string input)
		{
			if (input.StartsWith("\"", StringComparison.Ordinal) && input.EndsWith("\"", StringComparison.Ordinal))
			{
				input = input.Substring(1, input.Length - 2);
			}
			return input;
		}

		private static DateTime ParseFormattedDate(string input, CultureInfo culture)
		{
			string[] formats = new string[8]
			{
				"u",
				"s",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
				"yyyy-MM-ddTHH:mm:ssZ",
				"yyyy-MM-dd HH:mm:ssZ",
				"yyyy-MM-ddTHH:mm:ss",
				"yyyy-MM-ddTHH:mm:sszzzzzz",
				"M/d/yyyy h:mm:ss tt"
			};
			DateTime result;
			if (DateTime.TryParseExact(input, formats, culture, DateTimeStyles.None, out result))
			{
				return result;
			}
			if (DateTime.TryParse(input, culture, DateTimeStyles.None, out result))
			{
				return result;
			}
			return default(DateTime);
		}

		private static DateTime ExtractDate(string input, string pattern, CultureInfo culture)
		{
			DateTime result = DateTime.MinValue;
			Regex regex = new Regex(pattern);
			if (regex.IsMatch(input))
			{
				MatchCollection matchCollection = regex.Matches(input);
				Match match = matchCollection[0];
				long num = Convert.ToInt64(match.Groups[1].Value);
				result = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(num);
				if (match.Groups.Count > 2 && !string.IsNullOrEmpty(match.Groups[3].Value))
				{
					DateTime dateTime = DateTime.ParseExact(match.Groups[3].Value, "HHmm", culture);
					result = ((!(match.Groups[2].Value == "+")) ? result.Subtract(dateTime.TimeOfDay) : result.Add(dateTime.TimeOfDay));
				}
			}
			return result;
		}

		public static bool Matches(this string input, string pattern)
		{
			return Regex.IsMatch(input, pattern);
		}

		public static string ToPascalCase(this string lowercaseAndUnderscoredWord, CultureInfo culture)
		{
			return lowercaseAndUnderscoredWord.ToPascalCase(true, culture);
		}

		public static string ToPascalCase(this string text, bool removeUnderscores, CultureInfo culture)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			text = text.Replace("_", " ");
			string separator = removeUnderscores ? string.Empty : "_";
			string[] array = text.Split(' ');
			if (array.Length > 1 || array[0].IsUpperCase())
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Length > 0)
					{
						string text2 = array[i];
						string text3 = text2.Substring(1);
						if (text3.IsUpperCase())
						{
							text3 = text3.ToLower(culture);
						}
						char c = char.ToUpper(text2[0], culture);
						array[i] = c + text3;
					}
				}
				return string.Join(separator, array);
			}
			return array[0].Substring(0, 1).ToUpper(culture) + array[0].Substring(1);
		}

		public static string ToCamelCase(this string lowercaseAndUnderscoredWord, CultureInfo culture)
		{
			return lowercaseAndUnderscoredWord.ToPascalCase(culture).MakeInitialLowerCase();
		}

		public static string MakeInitialLowerCase(this string word)
		{
			return word.Substring(0, 1).ToLower() + word.Substring(1);
		}

		public static bool IsUpperCase(this string inputString)
		{
			return Regex.IsMatch(inputString, "^[A-Z]+$");
		}

		public static string AddUnderscores(this string pascalCasedWord)
		{
			return Regex.Replace(Regex.Replace(Regex.Replace(pascalCasedWord, "([A-Z]+)([A-Z][a-z])", "$1_$2"), "([a-z\\d])([A-Z])", "$1_$2"), "[-\\s]", "_");
		}

		public static string AddDashes(this string pascalCasedWord)
		{
			return Regex.Replace(Regex.Replace(Regex.Replace(pascalCasedWord, "([A-Z]+)([A-Z][a-z])", "$1-$2"), "([a-z\\d])([A-Z])", "$1-$2"), "[\\s]", "-");
		}

		public static string AddUnderscorePrefix(this string pascalCasedWord)
		{
			return string.Format("_{0}", pascalCasedWord);
		}

		public static string AddSpaces(this string pascalCasedWord)
		{
			return Regex.Replace(Regex.Replace(Regex.Replace(pascalCasedWord, "([A-Z]+)([A-Z][a-z])", "$1 $2"), "([a-z\\d])([A-Z])", "$1 $2"), "[-\\s]", " ");
		}

		public static IEnumerable<string> GetNameVariants(this string name, CultureInfo culture)
		{
			if (!string.IsNullOrEmpty(name))
			{
				yield return name;
				yield return name.ToCamelCase(culture);
				yield return name.ToLower(culture);
				yield return name.AddUnderscores();
				yield return name.AddUnderscores().ToLower(culture);
				yield return name.AddDashes();
				yield return name.AddDashes().ToLower(culture);
				yield return name.AddUnderscorePrefix();
				yield return name.ToCamelCase(culture).AddUnderscorePrefix();
				yield return name.AddSpaces();
				yield return name.AddSpaces().ToLower(culture);
			}
		}
	}
}
