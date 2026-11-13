using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public abstract class Constraint : MonoBehaviour
	{
		[Tooltip("An optional switch that can be used to specify whether this constraint should be applied.")]
		public Switch Condition;

		public abstract void Apply(ref Setup setup);
	}
}
