using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tweaker.UI
{
	public class InspectorStepperView : MonoBehaviour, IInspectorContentView
	{
		public Button NextButton;

		public Button PrevButton;

		public event Action NextClicked;

		public event Action PrevClicked;

		public event Action Destroyed;

		public void Awake()
		{
			NextButton.onClick.AddListener(OnNextClicked);
			PrevButton.onClick.AddListener(OnPrevClicked);
		}

		public void DestroySelf()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public void OnDestroy()
		{
			if (this.Destroyed != null)
			{
				this.Destroyed();
				this.Destroyed = null;
			}
			NextButton.onClick.RemoveAllListeners();
			PrevButton.onClick.RemoveAllListeners();
			this.NextClicked = null;
			this.PrevClicked = null;
		}

		private void OnNextClicked()
		{
			if (this.NextClicked != null)
			{
				this.NextClicked();
			}
		}

		private void OnPrevClicked()
		{
			if (this.PrevClicked != null)
			{
				this.PrevClicked();
			}
		}
	}
}
