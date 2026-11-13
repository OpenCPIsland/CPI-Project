using UnityEngine;

namespace JetpackReboot
{
	[RequireComponent(typeof(Collider2D))]
	public abstract class mg_jr_Collectable : MonoBehaviour
	{
		public int Quantity
		{
			get;
			private set;
		}

		public void Init(int _quantity)
		{
			Quantity = _quantity;
		}

		public abstract void OnCollection();
	}
}
