using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	[RequireComponent(typeof(GameObjectSelector))]
	public class FishBucketBucket : MonoBehaviour
	{
		public GameObject FishObject;

		public float FishMaxHeight;

		public float FishMaxScale;

		public int FishMaxCount;

		private GameObjectSelector bucketSelector;

		private void Awake()
		{
			bucketSelector = GetComponent<GameObjectSelector>();
		}

		public void SetBucketColor(int index)
		{
			bucketSelector.SelectGameObject(index);
		}

		public void SetFillAmount(int numFish)
		{
			float value = (float)numFish / (float)FishMaxCount;
			value = Mathf.Clamp(value, 0f, 1f);
			FishObject.transform.localPosition = new Vector3(0f, 0f, value * FishMaxHeight);
			float num = 1f + value * FishMaxScale;
			FishObject.transform.localScale = new Vector3(num, num, num);
		}
	}
}
