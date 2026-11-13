using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class LayerMaskExtensions
	{
		public static bool IsSet(this LayerMask mask, int layer)
		{
			return ((1 << layer) & (int)mask) != 0;
		}

		public static bool IsNotSet(this LayerMask mask, int layer)
		{
			return !mask.IsSet(layer);
		}

		public static bool IsCleared(this LayerMask mask)
		{
			return (int)mask == 0;
		}

		public static LayerMask Union(this LayerMask a, LayerMask b)
		{
			return (int)a | (int)b;
		}

		public static LayerMask Intersection(this LayerMask a, LayerMask b)
		{
			return (int)a & (int)b;
		}

		public static LayerMask Complement(this LayerMask mask)
		{
			return ~(int)mask;
		}

		public static LayerMask Complement(this LayerMask a, LayerMask b)
		{
			return ~(int)a.Union(b);
		}

		public static LayerMask Minus(this LayerMask a, LayerMask b)
		{
			return (int)a & ~(int)b;
		}

		public static LayerMask UnionMinusIntersection(this LayerMask a, LayerMask b)
		{
			return a.Union(b).Minus(a.Intersection(b));
		}
	}
}
