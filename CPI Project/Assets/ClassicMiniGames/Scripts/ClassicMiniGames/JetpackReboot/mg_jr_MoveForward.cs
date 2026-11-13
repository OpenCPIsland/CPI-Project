using MinigameFramework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_MoveForward : MonoBehaviour
	{
		public float Speed
		{
			get;
			set;
		}

		private void Awake()
		{
			Speed = 3f;
		}

		private void Update()
		{
			if (!MinigameManager.IsPaused)
			{
				base.transform.Translate(base.transform.right * Time.deltaTime * Speed, Space.World);
			}
		}
	}
}
