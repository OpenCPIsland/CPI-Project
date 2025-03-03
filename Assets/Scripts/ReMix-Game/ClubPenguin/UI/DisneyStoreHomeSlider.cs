using Disney.Kelowna.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DisneyStoreHomeSlider : MonoBehaviour
	{
		public Transform SliderParent;

		[SerializeField]
		private readonly float SLIDER_TIME = 5f;

		[SerializeField]
		private readonly float SLIDE_INTRO_TIME = 1f;

		private int currentItemIndex;

		private List<GameObject> sliderPrefabs;

		private GameObject currentSliderItem;

		private GameObject nextSliderItem;

		private bool isLoadComplete = false;

		private DisneyStoreController storeController;

		public void SetItems(PrefabContentKey[] sliderPrefabKeys, DisneyStoreController storeController)
		{
			this.storeController = storeController;
			currentItemIndex = 0;
			CoroutineRunner.Start(loadItems(sliderPrefabKeys), this, "");
		}

		private void OnDisable()
		{
			stopSlider();
		}

		private void OnEnable()
		{
			if (isLoadComplete)
			{
				startSlider();
			}
		}

		private void stopSlider()
		{
			CancelInvoke();
			clearSlider();
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			CancelInvoke();
		}

		private IEnumerator loadItems(PrefabContentKey[] sliderPrefabKeys)
		{
			sliderPrefabs = new List<GameObject>();
			List<CoroutineReturn> requests = new List<CoroutineReturn>();
			for (int i = 0; i < sliderPrefabKeys.Length; i++)
			{
				requests.Add(Content.LoadAsync(onItemLoadComplete, sliderPrefabKeys[i]));
			}
			yield return new CompositeCoroutineReturn(requests.ToArray());
			yield return null;
			isLoadComplete = true;
			startSlider();
		}

		private void startSlider()
		{
			clearSlider();
			currentSliderItem = createSlider(sliderPrefabs[currentItemIndex]);
			Object.Destroy(currentSliderItem.GetComponent<Animator>());
			Invoke("showNextItem", SLIDER_TIME);
		}

		private void showNextItem()
		{
			currentItemIndex = ++currentItemIndex % sliderPrefabs.Count;
			nextSliderItem = createSlider(sliderPrefabs[currentItemIndex]);
			Invoke("DestroyCurrentSlider", SLIDE_INTRO_TIME);
			Invoke("showNextItem", SLIDER_TIME);
		}

		private GameObject createSlider(GameObject sliderPrefab)
		{
			GameObject gameObject = Object.Instantiate(sliderPrefab, SliderParent, false);
			DisneyStoreHomeSliderItem component = gameObject.GetComponent<DisneyStoreHomeSliderItem>();
			if (component != null)
			{
				component.InjectStoreController(storeController);
			}
			return gameObject;
		}

		private void clearSlider()
		{
			int childCount = SliderParent.childCount;
			for (int num = childCount - 1; num >= 0; num--)
			{
				Object.Destroy(SliderParent.GetChild(num).gameObject);
			}
		}

		public void DestroyCurrentSlider()
		{
			Object.Destroy(currentSliderItem.gameObject);
			currentSliderItem = nextSliderItem;
		}

		private void onItemLoadComplete(string path, GameObject sliderPrefab)
		{
			sliderPrefabs.Add(sliderPrefab);
		}
	}
}
