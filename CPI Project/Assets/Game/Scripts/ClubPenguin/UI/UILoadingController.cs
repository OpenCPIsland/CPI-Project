using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class UILoadingController : MonoBehaviour
	{
		public bool RegisterSelf = false;

		public bool HideWhileLoading = false;

		public GameObject LoadingIndicatorPrefab;

		public Action LoadCompleteAction;

		private GameObject loadingIndicator;

		private List<GameObject> pendingLoads = new List<GameObject>();

		public static void RegisterLoad(GameObject loadingObject)
		{
			UILoadingController componentInParent = loadingObject.GetComponentInParent<UILoadingController>();
			if (componentInParent != null)
			{
				componentInParent.StartLoad(loadingObject);
			}
		}

		public static void RegisterLoadComplete(GameObject loadingObject)
		{
			UILoadingController componentInParent = loadingObject.GetComponentInParent<UILoadingController>();
			if (componentInParent != null)
			{
				componentInParent.CompleteLoad(loadingObject);
			}
		}

		public void Awake()
		{
			if (LoadingIndicatorPrefab != null)
			{
				showLoadingIndicator();
			}
		}

		public void Start()
		{
			if (RegisterSelf)
			{
				RegisterLoad(base.gameObject);
			}
			if (HideWhileLoading)
			{
				base.gameObject.AddComponent<CanvasGroup>().alpha = 0f;
			}
		}

		public void StartLoad(GameObject gameObject)
		{
			pendingLoads.Add(gameObject);
		}

		public void CompleteLoad(GameObject gameObject)
		{
			pendingLoads.Remove(gameObject);
			checkLoadCompletion();
		}

		private void checkLoadCompletion()
		{
			if (pendingLoads.Count <= 0)
			{
				if (LoadCompleteAction != null)
				{
					LoadCompleteAction();
				}
				if (RegisterSelf)
				{
					RegisterLoadComplete(base.gameObject);
				}
				if (loadingIndicator != null)
				{
					hideLoadingIndicator();
				}
				if (HideWhileLoading)
				{
					UnityEngine.Object.Destroy(base.gameObject.GetComponent<CanvasGroup>());
				}
			}
		}

		private void showLoadingIndicator()
		{
			loadingIndicator = UnityEngine.Object.Instantiate(LoadingIndicatorPrefab);
			loadingIndicator.transform.SetParent(base.transform, false);
		}

		private void hideLoadingIndicator()
		{
			UnityEngine.Object.Destroy(loadingIndicator);
		}
	}
}
