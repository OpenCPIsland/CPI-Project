using UnityEngine;

namespace ClubPenguin.LOD
{
	public class LODWeightingRefresher : MonoBehaviour
	{
		[HideInInspector]
		[SerializeField]
		private LODSystem lodSystem;

		public float RefreshRateSeconds = 1.5f;

		private float refreshCounter;

		public void Awake()
		{
			lodSystem = GetComponent<LODSystem>();
			refreshCounter = 0f;
		}

		public void Initialize(float refreshRateSeconds)
		{
			RefreshRateSeconds = refreshRateSeconds;
		}

		public void Update()
		{
			refreshCounter += Time.unscaledDeltaTime;
			if (refreshCounter >= RefreshRateSeconds)
			{
				Refresh();
			}
		}

		public void Refresh()
		{
			lodSystem.Refresh();
			refreshCounter = 0f;
		}
	}
}
