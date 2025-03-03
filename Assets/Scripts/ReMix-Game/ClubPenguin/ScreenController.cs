using ClubPenguin.Gui;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class ScreenController : MonoBehaviour
	{
		[Tooltip("height of tray when this screen is shown, as a percentage of total screen height, set to -1 to use global default value specified via TrayController script")]
		public float TrayHeight = -1f;

		private bool isInitialized = false;

		private static List<KeyValuePair<string, WeakReference>> buttonCache = new List<KeyValuePair<string, WeakReference>>();

		private ScreenController[] siblings;

		public ScreenController Previous
		{
			get;
			private set;
		}

		public ScreenController Next
		{
			get;
			private set;
		}

		public bool IsPrimary
		{
			get;
			private set;
		}

		private static void InitializeCache(Transform root)
		{
			foreach (Button item in FindAllObjectsOfTypeInRoot<Button>(root))
			{
				buttonCache.Add(new KeyValuePair<string, WeakReference>(item.name, new WeakReference(item)));
			}
		}

		private static IEnumerable<T> FindAllObjectsOfTypeInRoot<T>(Transform root, bool includeInactive = true)
		{
			try
			{
				Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>(includeInactive);
				foreach (Transform child in componentsInChildren)
				{
					T obj = child.GetComponent<T>();
					if (obj != null)
					{
						yield return obj;
					}
				}
			}
			finally
			{
			}
		}

		private void Awake()
		{
			GameObject gameObject = GetComponentInParent<TrayController>().gameObject;
			if (gameObject != null)
			{
				buttonCache.Clear();
				InitializeCache(gameObject.transform);
			}
			bool flag = false;
			IsPrimary = false;
			Transform parent = base.transform.parent;
			List<ScreenController> list = new List<ScreenController>(parent.childCount);
			for (int i = 0; i < parent.childCount; i++)
			{
				ScreenController component = parent.GetChild(i).GetComponent<ScreenController>();
				if (component == this)
				{
					if (list.Count > 0)
					{
						Previous = list[list.Count - 1];
					}
					else
					{
						IsPrimary = true;
					}
					flag = true;
				}
				else if (component != null)
				{
					list.Add(component);
					if (flag)
					{
						Next = component;
						flag = false;
					}
				}
			}
			siblings = list.ToArray();
			if (IsPrimary)
			{
				SetSiblingsActive(true);
			}
		}

		private void Start()
		{
			foreach (KeyValuePair<string, WeakReference> item in buttonCache)
			{
				if (item.Key.StartsWith(base.name))
				{
					(item.Value.Target as Button).onClick.AddListener(OnClick);
				}
			}
			if (!IsPrimary)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.SetActive(false);
				base.gameObject.SetActive(true);
			}
			Service.Get<EventDispatcher>().AddListener<TrayEvents.SelectTrayScreen>(onSelectTrayScreen);
			isInitialized = true;
		}

		private void OnDestroy()
		{
			foreach (KeyValuePair<string, WeakReference> item in buttonCache)
			{
				if (item.Key.StartsWith(base.name))
				{
					Button button = item.Value.Target as Button;
					if (button != null)
					{
						button.onClick.RemoveListener(OnClick);
					}
				}
			}
			Service.Get<EventDispatcher>().RemoveListener<TrayEvents.SelectTrayScreen>(onSelectTrayScreen);
		}

		private void SetSiblingsActive(bool value)
		{
			for (int i = 0; i < siblings.Length; i++)
			{
				siblings[i].gameObject.SetActive(value);
			}
		}

		private void OnEnable()
		{
			if (isInitialized)
			{
				SetSiblingsActive(false);
			}
			Service.Get<EventDispatcher>().DispatchEvent(new TrayEvents.TrayHeightAdjust(TrayHeight));
		}

		private void OnClick()
		{
			base.gameObject.SetActive(true);
		}

		public void EnableNextScreen()
		{
			ScreenController next = Next;
			if (next != null)
			{
				next.gameObject.SetActive(true);
			}
		}

		public void EnablePreviousScreen()
		{
			ScreenController previous = Previous;
			if (previous != null)
			{
				previous.gameObject.SetActive(true);
			}
		}

		public void GoToScreenWithName(string screenName)
		{
		}

		public void EnableScreenWithController<TController>() where TController : MonoBehaviour
		{
			Transform parent = base.transform.parent;
			TController x = FindAllObjectsOfTypeInRoot<TController>(parent).FirstOrDefault();
			if ((UnityEngine.Object)x != (UnityEngine.Object)null)
			{
				x.gameObject.SetActive(true);
				return;
			}
			throw new Exception(typeof(TController).FullName + " is not a Child of the parent transform.");
		}

		private bool onSelectTrayScreen(TrayEvents.SelectTrayScreen evt)
		{
			if (evt.ScreenName == base.gameObject.name)
			{
				foreach (KeyValuePair<string, WeakReference> item in buttonCache)
				{
					if (item.Key.StartsWith(base.gameObject.name))
					{
						if (evt.JumpToScreen)
						{
							PointerEventData eventData = new PointerEventData(EventSystem.current);
							ExecuteEvents.Execute((item.Value.Target as Button).gameObject, eventData, ExecuteEvents.pointerClickHandler);
						}
						if (!string.IsNullOrEmpty(evt.SubScreenName))
						{
							PlayerPrefs.SetString("SelectTrayScreen_SubScene", evt.SubScreenName);
						}
					}
				}
			}
			return false;
		}
	}
}
