using DisneyMobile.CoreUnitySystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MinigameFramework
{
	public class MinigameScreen : UIControlBase
	{
		private Button m_btnPause;

		public bool ShouldShowPauseOver
		{
			get;
			protected set;
		}

		protected bool HasPauseButton
		{
			get;
			set;
		}

		public MinigameScreen()
		{
			HasPauseButton = true;
			ShouldShowPauseOver = true;
		}

		public override void LoadUI(Dictionary<string, string> propertyList = null)
		{
			if (HasPauseButton)
			{
				GameObject prefab = Resources.Load("ScreenElements/Common/mg_btn_pause") as GameObject;
				m_btnPause = CreateUIElement(prefab).GetComponent<Button>();
				m_btnPause.onClick.AddListener(OnCloseClicked);
			}
			base.LoadUI(propertyList);
		}

		public override void UnloadUI()
		{
			if (m_btnPause != null)
			{
				m_btnPause.onClick.RemoveListener(OnCloseClicked);
			}
			base.UnloadUI();
		}

		protected virtual void OnCloseClicked()
		{
			MinigameManager.GetActive().PauseGame();
		}

		private void EnableButtons(bool _enabled = true)
		{
			if (m_btnPause != null)
			{
				m_btnPause.gameObject.SetActive(_enabled);
			}
			Button[] componentsInChildren = GetComponentsInChildren<Button>();
			Button[] array = componentsInChildren;
			foreach (Button button in array)
			{
				button.interactable = _enabled;
			}
		}

		public override void OnStacked(string prevTopName)
		{
			base.OnStacked(prevTopName);
			EnableButtons(false);
		}

		public override void OnPoppedToTop(string prevTopName)
		{
			base.OnPoppedToTop(prevTopName);
			EnableButtons();
		}
	}
}
