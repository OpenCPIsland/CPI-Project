using System.Linq;
using System.Text.RegularExpressions;

namespace Tweaker.AssemblyScanner
{
	public class ScanOption<T>
	{
		public T[] ScannableRefs;

		private Regex regex;

		public string NameRegex
		{
			get
			{
				return (regex != null) ? regex.ToString() : string.Empty;
			}
			set
			{
				regex = new Regex(value);
			}
		}

		public bool ScanNonPublic
		{
			get;
			set;
		}

		public bool ScanPublic
		{
			get;
			set;
		}

		public ScanOption()
		{
			ScanNonPublic = true;
			ScanPublic = true;
		}

		public bool CheckRefMatch(T reference)
		{
			if (ScannableRefs != null && ScannableRefs.Length > 0 && !ScannableRefs.Contains(reference))
			{
				return false;
			}
			return true;
		}

		public bool CheckNameMatch(string name)
		{
			if (regex == null)
			{
				return true;
			}
			return regex.Match(name).Success;
		}
	}
}
