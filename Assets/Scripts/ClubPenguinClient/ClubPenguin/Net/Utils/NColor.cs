using System;
using UnityEngine;

namespace ClubPenguin.Net.Utils
{
	[Serializable]
	public struct NColor
	{
		public double a;

		public double b;

		public double g;

		public double r;

		public NColor(Color source)
		{
			a = source.a;
			b = source.b;
			g = source.g;
			r = source.r;
		}

		public static implicit operator NColor(Color c)
		{
			return new NColor(c);
		}

		public static implicit operator Color(NColor c)
		{
			return c.toColor();
		}

		public Color toColor()
		{
			return new Color((float)r, (float)g, (float)b, (float)a);
		}

		public string getSignableString()
		{
			return "Colour(a=" + a.ToString("0.0###########") + ", r=" + r.ToString("0.0###########") + ", b=" + b.ToString("0.0###########") + ", g=" + g.ToString("0.0###########") + ")";
		}
	}
}
