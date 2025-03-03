using UnityEngine;

namespace SmoothieSmash
{
	public interface mg_ss_IItemMovement
	{
		void Initialize(mg_ss_ItemObject p_itemObject);

		void UpdatePosition(Transform p_transform, float p_deltaTime, float p_conveyorSpeed);

		void OnCollided();
	}
}
