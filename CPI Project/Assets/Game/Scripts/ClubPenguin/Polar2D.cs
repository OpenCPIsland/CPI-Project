using System;
using UnityEngine;

namespace ClubPenguin
{
	public struct Polar2D
	{
		public static readonly Polar2D Zero = new Polar2D(0f, 0f);

		public float Angle
		{
			get;
			private set;
		}

		public float Distance
		{
			get;
			private set;
		}

		public Polar2D(float angle, float distance)
		{
			this = default(Polar2D);
			Angle = angle;
			Distance = distance;
		}

		public Polar2D(Vector2 v)
		{
			this = default(Polar2D);
			Angle = Mathf.Atan2(v.y, v.x) * 57.29578f;
			Distance = Mathf.Sqrt(v.x * v.x + v.y * v.y);
		}

		public static implicit operator Vector2(Polar2D p)
		{
			return new Vector2(Mathf.Cos(p.Angle * ((float)Math.PI / 180f)) * p.Distance, Mathf.Sin(p.Angle * ((float)Math.PI / 180f)) * p.Distance);
		}

		public static Polar2D MoveTowards(Polar2D current, Polar2D target, float angleMaxDelta, float distanceMaxDelta)
		{
			float angle = Mathf.MoveTowardsAngle(current.Angle, target.Angle, angleMaxDelta);
			float distance = Mathf.MoveTowards(current.Distance, target.Distance, distanceMaxDelta);
			return new Polar2D(angle, distance);
		}

		public static bool operator ==(Polar2D p1, Polar2D p2)
		{
			return p1.Angle == p2.Angle && p1.Distance == p2.Distance;
		}

		public static bool operator !=(Polar2D p1, Polar2D p2)
		{
			return p1.Angle != p2.Angle || p1.Distance != p2.Distance;
		}

		public static Polar2D operator +(Polar2D p1, Polar2D p2)
		{
			return new Polar2D(p1.Angle + p2.Angle, p1.Distance + p2.Distance);
		}

		public static Polar2D operator -(Polar2D p1, Polar2D p2)
		{
			return new Polar2D(p1.Angle - p2.Angle, p1.Distance - p2.Distance);
		}

		public override string ToString()
		{
			return string.Format("({0} deg,{1})", Angle, Distance);
		}

		public override int GetHashCode()
		{
			return (Angle.GetHashCode() * 522133279) ^ Distance.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Polar2D polar2D = (Polar2D)obj;
			return Angle == polar2D.Angle && Distance == polar2D.Distance;
		}
	}
}
