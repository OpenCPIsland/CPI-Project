public static class ArrayExtensions
{
	public static bool ValueEquals<T>(this T[] lhs, T[] rhs) where T : struct
	{
		if (lhs != null && rhs != null && lhs.Length == rhs.Length)
		{
			for (int i = 0; i < lhs.Length; i++)
			{
				if (!lhs[i].Equals(rhs[i]))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public static int GetValueHashCode<T>(this T[] o) where T : struct
	{
		StructHash sh = default(StructHash);
		sh.Combine(o);
		return sh;
	}
}
