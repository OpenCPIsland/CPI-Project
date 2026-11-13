using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ConveyorObject : MonoBehaviour
	{
		private Animator m_animator;

		public Transform ItemSpawnPoint_Bottom
		{
			get;
			private set;
		}

		public Transform ItemSpawnPoint_Top
		{
			get;
			private set;
		}

		public Transform ConveyorPosition
		{
			get;
			private set;
		}

		protected void Awake()
		{
			ItemSpawnPoint_Bottom = base.transform.Find("mg_ss_item_spawn_bottom");
			ItemSpawnPoint_Top = base.transform.Find("mg_ss_item_spawn_top");
			ConveyorPosition = base.transform.Find("mg_ss_conveyor_pos");
			m_animator = GetComponentInChildren<Animator>();
		}

		public void OnConveyorCollision()
		{
			m_animator.SetTrigger("Collided");
		}
	}
}
