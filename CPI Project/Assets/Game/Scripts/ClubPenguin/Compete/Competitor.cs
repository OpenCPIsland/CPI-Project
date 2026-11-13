#define UNITY_ASSERTIONS
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace ClubPenguin.Compete
{
	public class Competitor<T>
	{
		public int Points;

		internal T theObject;

		public T Value
		{
			get
			{
				return theObject;
			}
		}

		public Competitor(T competitor)
		{
			Assert.IsTrue(competitor != null, "A competitor cannot be null!");
			theObject = competitor;
		}

		protected bool Equals(Competitor<T> other)
		{
			return EqualityComparer<T>.Default.Equals(theObject, other.theObject);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((Competitor<T>)obj);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<T>.Default.GetHashCode(theObject);
		}

		public int CompareTo(Competitor<T> other)
		{
			if (other == null)
			{
				return 1;
			}
			if (Points == other.Points)
			{
				return 0;
			}
			return (Points > other.Points) ? 1 : (-1);
		}

		public override string ToString()
		{
			return string.Format("[Competitor value={0}, points={1}]", theObject, Points);
		}
	}
}
