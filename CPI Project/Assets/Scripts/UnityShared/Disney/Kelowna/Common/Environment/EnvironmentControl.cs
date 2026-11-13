using System;
using UnityEngine;
using UnityEngine.UI;

namespace Disney.Kelowna.Common.Environment
{
	public class EnvironmentControl : MonoBehaviour
	{
		public GameObject ButtonContainer;

		public GameObject ButtonPrefab;

		public Button CurrentEnvironmentButton;

		public Text CurrentEnvironmentLabel;

		public Action<Environment> OnEnvironmentSet;

		private bool opened;

		public void TogglePanelState()
		{
			GetComponent<Animator>().SetTrigger("ShowHide");
			RectTransform component = GetComponent<RectTransform>();
			if (opened)
			{
				SetPanelAnchoredPosition(component, component.rect.height);
				opened = false;
			}
			else
			{
				SetPanelAnchoredPosition(component, 0f);
				opened = true;
			}
		}

		private void SetPanelAnchoredPosition(RectTransform rt, float y)
		{
			Vector2 anchoredPosition = rt.anchoredPosition;
			anchoredPosition.y = y;
			rt.anchoredPosition = anchoredPosition;
		}

		public void ConfigureCurrentEnvironmentButton(Environment env)
		{
			RectTransform component = GetComponent<RectTransform>();
			SetPanelAnchoredPosition(component, component.rect.height);
			opened = false;
			CurrentEnvironmentButton.onClick.AddListener(TogglePanelState);
			CurrentEnvironmentLabel.text = "Environment: " + env;
		}

		public void ConfigureSelectEnvironmentButons()
		{
			if (!(ButtonContainer == null))
			{
				foreach (Environment value in EnumUtil.GetValues<Environment>())
				{
					AddEnvironmentButton(value);
				}
			}
		}

		private void AddEnvironmentButton(Environment env)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ButtonPrefab);
			gameObject.name = "btn_enviro_" + env;
			gameObject.transform.SetParent(ButtonContainer.transform, false);
			Button component = gameObject.GetComponent<Button>();
			Text componentInChildren = gameObject.GetComponentInChildren<Text>();
			component.onClick.AddListener(delegate
			{
				OnSelectEnvironment(env);
			});
			componentInChildren.text = env.ToString();
		}

		private void OnSelectEnvironment(Environment env)
		{
			TogglePanelState();
			CurrentEnvironmentLabel.text = "Environment: " + env;
			if (OnEnvironmentSet != null)
			{
				OnEnvironmentSet(env);
			}
		}
	}
}
