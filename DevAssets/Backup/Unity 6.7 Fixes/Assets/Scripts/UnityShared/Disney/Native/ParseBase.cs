using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Disney.Native
{
	public class ParseBase<T>
	{
		protected IAccessibilityLocalization localization;

		protected Dictionary<EntityId, T> items = new Dictionary<EntityId, T>();

		public ParseBase(IAccessibilityLocalization aLocalization)
		{
			localization = aLocalization;
		}

		public void Clear()
		{
			items.Clear();
		}

		public string GetTokenText(AccessibilitySettings aSettings)
		{
			string text = "";
			if (aSettings == null)
			{
				return text;
			}
			if (!string.IsNullOrEmpty(aSettings.DynamicText))
			{
				return aSettings.DynamicText;
			}
			if (!string.IsNullOrEmpty(aSettings.CustomToken))
			{
				text = localization.GetString(aSettings.CustomToken);
			}
			if (aSettings.ReferenceToken != null)
			{
				text = GetLabelFromReferenceToken(aSettings.ReferenceToken);
			}
			if (aSettings.AdditionalReferenceTokens != null && aSettings.AdditionalReferenceTokens.Length > 0)
			{
				string[] array = new string[aSettings.AdditionalReferenceTokens.Length];
				for (int i = 0; i < aSettings.AdditionalReferenceTokens.Length; i++)
				{
					array[i] = GetLabelFromReferenceToken(aSettings.AdditionalReferenceTokens[i]);
				}
				text = text + " " + string.Join(" ", array);
			}
			if (aSettings is ScrollerAccessibilitySettings)
			{
				string scrollPercent = (aSettings as ScrollerAccessibilitySettings).GetScrollPercent();
				if (!string.IsNullOrEmpty(scrollPercent))
				{
					text = text + ", " + scrollPercent;
				}
			}
			string text2 = "";
			text2 = GetControlDescriptionForLabel(aSettings);
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + " " + text2;
			}
			return Regex.Replace(text, "<.*?>", string.Empty);
		}

		protected string GetLabelFromReferenceToken(GameObject reference_token)
		{
			string result = "";
			Text componentInChildren = reference_token.GetComponentInChildren<Text>();
			if (componentInChildren != null)
			{
				result = componentInChildren.text;
			}
			return result;
		}

		public void Parse(GameObject[] gameObjects)
		{
			List<EntityId> list = new List<EntityId>();
			foreach (GameObject gameObject in gameObjects)
			{
				if (!gameObject.activeSelf)
				{
					continue;
				}
				T[] componentsInChildren = gameObject.GetComponentsInChildren<T>(false);
				T[] array = componentsInChildren;
				foreach (T val in array)
				{
					Object x = val as Object;
					if (x == null)
					{
						continue;
					}
					AccessibilitySettings component = GetGameobject(val).GetComponent<AccessibilitySettings>();
					if (!(component != null) || component.IgnoreText || (val is Image && (GetGameobject(val).GetComponent<Button>() != null || GetGameobject(val).GetComponent<InputFieldEx>() != null)) || (component.VoiceOnly && MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel != NativeAccessibilityLevel.VOICE) || component.DontRender)
					{
						continue;
					}
					if (GetGameobject(val).transform.parent.gameObject.activeSelf && component.VisibleOnlyForSwitchControl)
					{
						GetGameobject(val).SetActive(true);
					}
					if (!GetGameobject(val).activeSelf)
					{
						continue;
					}
						EntityId instanceID = GetGameobject(val).GetEntityId();
					if (MonoSingleton<NativeAccessibilityManager>.Instance.HiddenScrollItemIds.IndexOf(component) <= -1)
					{
						list.Add(instanceID);
						if (items.ContainsKey(instanceID))
						{
							Update(val);
						}
						else if (Add(val, instanceID, component.VoiceOnly))
						{
							items.Add(instanceID, val);
						}
					}
				}
			}
			Dictionary<EntityId, T> dictionary = new Dictionary<EntityId, T>();
			foreach (KeyValuePair<EntityId, T> item in items)
			{
				if (item.Value != null && list.Contains(item.Key))
				{
					dictionary.Add(item.Key, item.Value);
				}
				else
				{
					Remove(item.Key);
				}
			}
			items = dictionary;
		}

		protected void CheckCustomOnClickHandler(EntityId aId)
		{
			if (items != null && items.ContainsKey(aId) && items[aId] != null)
			{
				AccessibilitySettings component = GetGameobject(items[aId]).GetComponent<AccessibilitySettings>();
				if (component != null && component.CustomOnClickEvent != null)
				{
					component.CustomOnClickEvent.Invoke();
				}
			}
		}

		protected virtual GameObject GetGameobject(T aItem)
		{
			return (aItem as Component).gameObject;
		}

		protected virtual bool Add(T aItem, EntityId aId, bool isVoiceOnly)
		{
			AccessibilitySettings component = GetGameobject(aItem).GetComponent<AccessibilitySettings>();
			Rect rectInPhysicalScreenSpace = Util.GetRectInPhysicalScreenSpace((component.ReferenceRect != null) ? component.ReferenceRect : GetGameobject(aItem).GetComponent<RectTransform>());
			string tokenText = GetTokenText(component);
			if (rectInPhysicalScreenSpace.width <= 0f || rectInPhysicalScreenSpace.height <= 0f)
			{
				return false;
			}
			if (isVoiceOnly)
			{
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.RenderText(aId, rectInPhysicalScreenSpace, tokenText);
			}
			else
			{
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.RenderButton(aId, rectInPhysicalScreenSpace, tokenText);
			}
			return true;
		}

		protected virtual void Update(T aItem)
		{
			AccessibilitySettings component = GetGameobject(aItem).GetComponent<AccessibilitySettings>();
			Rect rectInPhysicalScreenSpace = Util.GetRectInPhysicalScreenSpace((component.ReferenceRect != null) ? component.ReferenceRect : GetGameobject(aItem).GetComponent<RectTransform>());
			if (rectInPhysicalScreenSpace.width <= 0f || rectInPhysicalScreenSpace.height <= 0f)
			{
				items.Remove(GetGameobject(aItem).GetEntityId());
			}
			else
			{
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.UpdateView(GetGameobject(aItem).GetEntityId(), rectInPhysicalScreenSpace, GetTokenText(component));
			}
		}

		protected virtual void Remove(EntityId aId)
		{
			MonoSingleton<NativeAccessibilityManager>.Instance.Native.RemoveView(aId);
		}

		protected virtual string GetControlDescriptionForLabel(AccessibilitySettings aSettings)
		{
			return "";
		}
	}
}
