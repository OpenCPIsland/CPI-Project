using System;

namespace Tweaker.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter, AllowMultiple = false)]
	public class PublicTweak : Attribute, ICustomTweakerAttribute
	{
		public DateTime UnlockTime;

		public PublicTweak()
		{
			UnlockTime = DateTime.MinValue;
		}

		public PublicTweak(int year, int month, int day)
		{
			UnlockTime = new DateTime(year, month, day);
		}

		internal static bool IsUnlocked(ITweakerObject t)
		{
			PublicTweak customAttribute = t.GetCustomAttribute<PublicTweak>();
			if (customAttribute == null)
			{
				return false;
			}
			return customAttribute.UnlockTime < DateTime.UtcNow;
		}
	}
}
