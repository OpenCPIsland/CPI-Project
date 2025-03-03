using System;
using UnityEngine;

namespace ClubPenguin.Props
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Prop))]
	public abstract class PropSpawnLocationValidator : MonoBehaviour
	{
		protected bool isValidPosition = false;

		public bool IsValidPosition
		{
			get
			{
				return isValidPosition;
			}
		}

		public event Action<bool> OnValidPositionChanged;

		protected void setPositionValid(bool valid)
		{
			if (isValidPosition != valid)
			{
				isValidPosition = valid;
				if (this.OnValidPositionChanged != null)
				{
					this.OnValidPositionChanged(isValidPosition);
				}
			}
		}

		private void OnDestroy()
		{
			this.OnValidPositionChanged = null;
			onDestroy();
		}

		protected virtual void onDestroy()
		{
		}
	}
}
