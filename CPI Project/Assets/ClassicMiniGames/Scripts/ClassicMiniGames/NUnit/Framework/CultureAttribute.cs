using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;
using System.Globalization;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class CultureAttribute : IncludeExcludeAttribute, IApplyToTest
	{
		private CultureDetector cultureDetector = new CultureDetector();

		private CultureInfo currentCulture = CultureInfo.CurrentCulture;

		public CultureAttribute()
		{
		}

		public CultureAttribute(string cultures)
			: base(cultures)
		{
		}

		public void ApplyToTest(ITest test)
		{
			if (test.RunState != 0 && !IsCultureSupported())
			{
				test.RunState = RunState.Skipped;
				test.Properties.Set(PropertyNames.SkipReason, base.Reason);
			}
		}

		private bool IsCultureSupported()
		{
			if (base.Include != null && !cultureDetector.IsCultureSupported(base.Include))
			{
				base.Reason = string.Format("Only supported under culture {0}", base.Include);
				return false;
			}
			if (base.Exclude != null && cultureDetector.IsCultureSupported(base.Exclude))
			{
				base.Reason = string.Format("Not supported under culture {0}", base.Exclude);
				return false;
			}
			return true;
		}

		public bool IsCultureSupported(string culture)
		{
			culture = culture.Trim();
			if (culture.IndexOf(',') >= 0)
			{
				if (IsCultureSupported(culture.Split(',')))
				{
					return true;
				}
			}
			else if (currentCulture.Name == culture || currentCulture.TwoLetterISOLanguageName == culture)
			{
				return true;
			}
			return false;
		}

		public bool IsCultureSupported(string[] cultures)
		{
			foreach (string culture in cultures)
			{
				if (IsCultureSupported(culture))
				{
					return true;
				}
			}
			return false;
		}
	}
}
