using UnityEngine;

namespace ClubPenguin
{
	public class DisableGameObjectTrigger : MonoBehaviour
	{
		public enum DisableState
		{
			None,
			Disable,
			Enable
		}

		public GameObject[] GameObjects;

		public DisableState OnEnterState;

		public DisableState OnExitState;

		public DisableState DefaultState;

		private void Start()
		{
			setStates(DefaultState);
		}

		private void OnTriggerEnter(Collider other)
		{
			setStates(OnEnterState);
		}

		private void OnTriggerExit(Collider other)
		{
			setStates(OnExitState);
		}

		private void setStates(DisableState state)
		{
			if (GameObjects == null)
			{
				return;
			}
			for (int i = 0; i < GameObjects.Length; i++)
			{
				switch (state)
				{
				case DisableState.Disable:
					GameObjects[i].SetActive(false);
					break;
				case DisableState.Enable:
					GameObjects[i].SetActive(true);
					break;
				}
			}
		}
	}
}
