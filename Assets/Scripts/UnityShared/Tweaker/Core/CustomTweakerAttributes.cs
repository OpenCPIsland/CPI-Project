using System.Reflection;

namespace Tweaker.Core
{
	public static class CustomTweakerAttributes
	{
		public static ICustomTweakerAttribute[] Get(MemberInfo member)
		{
			return member.GetCustomAttributes(typeof(ICustomTweakerAttribute), true) as ICustomTweakerAttribute[];
		}
	}
}
