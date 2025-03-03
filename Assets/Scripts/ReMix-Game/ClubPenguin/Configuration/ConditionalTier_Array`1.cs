using System.Linq;

namespace ClubPenguin.Configuration
{
	public class ConditionalTier_Array<T> : ConditionalTier<T[]>
	{
		public override string ToString()
		{
			return string.Format("[{0}]", string.Join(", ", DynamicValue.Select((T i) => i.ToString()).ToArray()));
		}
	}
}
