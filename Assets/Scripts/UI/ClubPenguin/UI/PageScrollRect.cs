using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ScrollRect))]
	public class PageScrollRect : MonoBehaviour
	{
		public int itemsPerPage;

		public Button LeftButton;

		public Button RightButton;

		private ScrollRect scrollRect;

		private RectTransform contentTransform;

		private int currentPage;

		private bool lerpPages;

		private Vector3 pageTarget;

		private List<Vector3> pagePositions;

		private int pages;

		private int pageElements;

		private float canvasScaleFactor;

		private float pageStep;

		private float elementWidth;

		private void Awake()
		{
			pagePositions = new List<Vector3>();
			scrollRect = GetComponent<ScrollRect>();
			canvasScaleFactor = GetComponentInParent<Canvas>().scaleFactor;
			contentTransform = scrollRect.content;
		}

		private void OnEnable()
		{
			LeftButton.onClick.AddListener(previousPage);
			RightButton.onClick.AddListener(nextPage);
		}

		private void OnDisable()
		{
			LeftButton.onClick.RemoveListener(previousPage);
			RightButton.onClick.RemoveListener(nextPage);
		}

		private void Start()
		{
			scrollRect.horizontal = false;
			scrollRect.vertical = false;
			pageStep = scrollRect.GetComponent<RectTransform>().rect.width * canvasScaleFactor;
			pageElements = contentTransform.childCount;
			pages = Mathf.CeilToInt((float)pageElements / (float)itemsPerPage);
			if (pageElements > 0)
			{
				elementWidth = pageStep / (float)itemsPerPage;
				distributePageElements();
				calculatePagePositions();
			}
			scrollRect.horizontalNormalizedPosition = 0f;
			currentPage = 0;
		}

		private void distributePageElements()
		{
			for (int i = 0; i < pageElements; i++)
			{
				RectTransform component = contentTransform.GetChild(i).gameObject.GetComponent<RectTransform>();
				LayoutElement layoutElement = component.GetComponent<LayoutElement>();
				if (layoutElement == null)
				{
					layoutElement = component.gameObject.AddComponent<LayoutElement>();
				}
				layoutElement.preferredWidth = elementWidth;
			}
			float x = elementWidth * (float)pageElements;
			contentTransform.offsetMax = new Vector2(x, 0f);
		}

		private void calculatePagePositions()
		{
			scrollRect.horizontalNormalizedPosition = 0f;
			pagePositions.Add(contentTransform.localPosition);
			for (int i = 0; i < pages; i++)
			{
				contentTransform.localPosition = new Vector3(contentTransform.localPosition.x - pageStep, contentTransform.localPosition.y);
				pagePositions.Add(contentTransform.localPosition);
			}
		}

		private void Update()
		{
			if (lerpPages)
			{
				contentTransform.localPosition = Vector3.Lerp(contentTransform.localPosition, pageTarget, 7.5f * Time.deltaTime);
				if (Vector3.Distance(contentTransform.localPosition, pageTarget) < 0.005f)
				{
					contentTransform.localPosition = pageTarget;
					lerpPages = false;
				}
			}
		}

		private void previousPage()
		{
			if (currentPage > 0)
			{
				currentPage--;
				lerpPages = true;
				pageTarget = pagePositions[currentPage];
			}
		}

		private void nextPage()
		{
			if (currentPage < pages - 1)
			{
				currentPage++;
				lerpPages = true;
				pageTarget = pagePositions[currentPage];
			}
		}
	}
}
