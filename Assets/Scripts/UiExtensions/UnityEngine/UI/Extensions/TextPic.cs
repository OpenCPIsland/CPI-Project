using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/TextPic")]
	[ExecuteInEditMode]
	public class TextPic : Text, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, ISelectHandler, IEventSystemHandler
	{
		[Serializable]
		public struct IconName
		{
			public string name;

			public Sprite sprite;
		}

		[Serializable]
		public class HrefClickEvent : UnityEvent<string>
		{
		}

		private class HrefInfo
		{
			public int startIndex;

			public int endIndex;

			public string name;

			public readonly List<Rect> boxes = new List<Rect>();
		}

		private readonly List<Image> m_ImagesPool = new List<Image>();

		private readonly List<GameObject> culled_ImagesPool = new List<GameObject>();

		private bool clearImages = false;

		private Object thisLock = new Object();

		private readonly List<int> m_ImagesVertexIndex = new List<int>();

		private static readonly Regex s_Regex = new Regex("<quad name=(.+?) size=(\\d*\\.?\\d+%?) width=(\\d*\\.?\\d+%?) />", RegexOptions.Singleline);

		private string fixedString;

		private string m_OutputText;

		public IconName[] inspectorIconList;

		private Dictionary<string, Sprite> iconList = new Dictionary<string, Sprite>();

		public float ImageScalingFactor = 1f;

		public string hyperlinkColor = "blue";

		[SerializeField]
		public Vector2 imageOffset = Vector2.zero;

		private Button button;

		private List<Vector2> positions = new List<Vector2>();

		private string previousText = "";

		public bool isCreating_m_HrefInfos = true;

		private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

		private static readonly StringBuilder s_TextBuilder = new StringBuilder();

		private static readonly Regex s_HrefRegex = new Regex("<a href=([^>\\n\\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

		[SerializeField]
		private HrefClickEvent m_OnHrefClick = new HrefClickEvent();

		public HrefClickEvent onHrefClick
		{
			get
			{
				return m_OnHrefClick;
			}
			set
			{
				m_OnHrefClick = value;
			}
		}

		public override void SetVerticesDirty()
		{
			base.SetVerticesDirty();
			UpdateQuadImage();
		}

		private new void Start()
		{
			button = GetComponent<Button>();
			if (inspectorIconList != null && inspectorIconList.Length > 0)
			{
				IconName[] array = inspectorIconList;
				for (int i = 0; i < array.Length; i++)
				{
					IconName iconName = array[i];
					iconList.Add(iconName.name, iconName.sprite);
				}
			}
			Reset_m_HrefInfos();
		}

		protected void UpdateQuadImage()
		{
			m_OutputText = GetOutputText();
			m_ImagesVertexIndex.Clear();
			IEnumerator enumerator = s_Regex.Matches(m_OutputText).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Match match = (Match)enumerator.Current;
					int index = match.Index;
					int item = index * 4 + 3;
					m_ImagesVertexIndex.Add(item);
					m_ImagesPool.RemoveAll((Image image) => image == null);
					if (m_ImagesPool.Count == 0)
					{
						GetComponentsInChildren(m_ImagesPool);
					}
					if (m_ImagesVertexIndex.Count > m_ImagesPool.Count)
					{
						GameObject gameObject = DefaultControls.CreateImage(default(DefaultControls.Resources));
						gameObject.layer = base.gameObject.layer;
						RectTransform rectTransform = gameObject.transform as RectTransform;
						if ((bool)rectTransform)
						{
							rectTransform.SetParent(base.rectTransform);
							rectTransform.localPosition = Vector3.zero;
							rectTransform.localRotation = Quaternion.identity;
							rectTransform.localScale = Vector3.one;
						}
						m_ImagesPool.Add(gameObject.GetComponent<Image>());
					}
					string value = match.Groups[1].Value;
					Image image2 = m_ImagesPool[m_ImagesVertexIndex.Count - 1];
					if ((image2.sprite == null || image2.sprite.name != value) && inspectorIconList != null && inspectorIconList.Length > 0)
					{
						IconName[] array = inspectorIconList;
						for (int i = 0; i < array.Length; i++)
						{
							IconName iconName = array[i];
							if (iconName.name == value)
							{
								image2.sprite = iconName.sprite;
								break;
							}
						}
					}
					image2.rectTransform.sizeDelta = new Vector2((float)base.fontSize * ImageScalingFactor, (float)base.fontSize * ImageScalingFactor);
					image2.enabled = true;
					if (positions.Count == m_ImagesPool.Count)
					{
						image2.rectTransform.anchoredPosition = positions[m_ImagesVertexIndex.Count - 1];
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			for (int j = m_ImagesVertexIndex.Count; j < m_ImagesPool.Count; j++)
			{
				if ((bool)m_ImagesPool[j])
				{
					m_ImagesPool[j].gameObject.SetActive(false);
					m_ImagesPool[j].gameObject.hideFlags = HideFlags.HideAndDontSave;
					culled_ImagesPool.Add(m_ImagesPool[j].gameObject);
					m_ImagesPool.Remove(m_ImagesPool[j]);
				}
			}
			if (culled_ImagesPool.Count > 1)
			{
				clearImages = true;
			}
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			string text = m_Text;
			m_Text = GetOutputText();
			base.OnPopulateMesh(toFill);
			m_Text = text;
			positions.Clear();
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < m_ImagesVertexIndex.Count; i++)
			{
				int num = m_ImagesVertexIndex[i];
				RectTransform rectTransform = m_ImagesPool[i].rectTransform;
				Vector2 sizeDelta = rectTransform.sizeDelta;
				if (num < toFill.currentVertCount)
				{
					toFill.PopulateUIVertex(ref vertex, num);
					positions.Add(new Vector2(vertex.position.x + sizeDelta.x / 2f, vertex.position.y + sizeDelta.y / 2f) + imageOffset);
					toFill.PopulateUIVertex(ref vertex, num - 3);
					Vector3 position = vertex.position;
					int num2 = num;
					int num3 = num - 3;
					while (num2 > num3)
					{
						toFill.PopulateUIVertex(ref vertex, num);
						vertex.position = position;
						toFill.SetUIVertex(vertex, num2);
						num2--;
					}
				}
			}
			if (m_ImagesVertexIndex.Count != 0)
			{
				m_ImagesVertexIndex.Clear();
			}
			foreach (HrefInfo hrefInfo in m_HrefInfos)
			{
				hrefInfo.boxes.Clear();
				if (hrefInfo.startIndex < toFill.currentVertCount)
				{
					toFill.PopulateUIVertex(ref vertex, hrefInfo.startIndex);
					Vector3 position2 = vertex.position;
					Bounds bounds = new Bounds(position2, Vector3.zero);
					int j = hrefInfo.startIndex;
					for (int endIndex = hrefInfo.endIndex; j < endIndex && j < toFill.currentVertCount; j++)
					{
						toFill.PopulateUIVertex(ref vertex, j);
						position2 = vertex.position;
						float x = position2.x;
						Vector3 min = bounds.min;
						if (x < min.x)
						{
							hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
							bounds = new Bounds(position2, Vector3.zero);
						}
						else
						{
							bounds.Encapsulate(position2);
						}
					}
					hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
				}
			}
			UpdateQuadImage();
		}

		protected string GetOutputText()
		{
			s_TextBuilder.Length = 0;
			int num = 0;
			fixedString = text;
			if (inspectorIconList != null && inspectorIconList.Length > 0)
			{
				IconName[] array = inspectorIconList;
				for (int i = 0; i < array.Length; i++)
				{
					IconName iconName = array[i];
					if (iconName.name != null && iconName.name != "")
					{
						fixedString = fixedString.Replace(iconName.name, "<quad name=" + iconName.name + " size=" + base.fontSize + " width=1 />");
					}
				}
			}
			int num2 = 0;
			IEnumerator enumerator = s_HrefRegex.Matches(fixedString).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Match match = (Match)enumerator.Current;
					s_TextBuilder.Append(fixedString.Substring(num, match.Index - num));
					s_TextBuilder.Append("<color=" + hyperlinkColor + ">");
					Group group = match.Groups[1];
					if (isCreating_m_HrefInfos)
					{
						HrefInfo hrefInfo = new HrefInfo();
						hrefInfo.startIndex = s_TextBuilder.Length * 4;
						hrefInfo.endIndex = (s_TextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3;
						hrefInfo.name = group.Value;
						HrefInfo item = hrefInfo;
						m_HrefInfos.Add(item);
					}
					else if (m_HrefInfos.Count > 0)
					{
						m_HrefInfos[num2].startIndex = s_TextBuilder.Length * 4;
						m_HrefInfos[num2].endIndex = (s_TextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3;
						num2++;
					}
					s_TextBuilder.Append(match.Groups[2].Value);
					s_TextBuilder.Append("</color>");
					num = match.Index + match.Length;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			if (isCreating_m_HrefInfos)
			{
				isCreating_m_HrefInfos = false;
			}
			s_TextBuilder.Append(fixedString.Substring(num, fixedString.Length - num));
			return s_TextBuilder.ToString();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
			foreach (HrefInfo hrefInfo in m_HrefInfos)
			{
				List<Rect> boxes = hrefInfo.boxes;
				for (int i = 0; i < boxes.Count; i++)
				{
					if (boxes[i].Contains(localPoint))
					{
						m_OnHrefClick.Invoke(hrefInfo.name);
						return;
					}
				}
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (m_ImagesPool.Count >= 1)
			{
				foreach (Image item in m_ImagesPool)
				{
					if (button != null && button.isActiveAndEnabled)
					{
						item.color = button.colors.highlightedColor;
					}
				}
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (m_ImagesPool.Count >= 1)
			{
				foreach (Image item in m_ImagesPool)
				{
					if (button != null && button.isActiveAndEnabled)
					{
						item.color = button.colors.normalColor;
					}
					else
					{
						item.color = color;
					}
				}
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (m_ImagesPool.Count >= 1)
			{
				foreach (Image item in m_ImagesPool)
				{
					if (button != null && button.isActiveAndEnabled)
					{
						item.color = button.colors.highlightedColor;
					}
				}
			}
		}

		private void Update()
		{
			lock (thisLock)
			{
				if (clearImages)
				{
					for (int i = 0; i < culled_ImagesPool.Count; i++)
					{
						Object.DestroyImmediate(culled_ImagesPool[i]);
					}
					culled_ImagesPool.Clear();
					clearImages = false;
				}
			}
			if (previousText != text)
			{
				Reset_m_HrefInfos();
			}
		}

		private void Reset_m_HrefInfos()
		{
			previousText = text;
			m_HrefInfos.Clear();
			isCreating_m_HrefInfos = true;
		}
	}
}
