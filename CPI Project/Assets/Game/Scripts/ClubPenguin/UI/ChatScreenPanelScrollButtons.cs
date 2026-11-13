using Disney.Kelowna.Common.SEDFSM;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ChatScreenPanelScrollButtons : MonoBehaviour
	{
		[SerializeField]
		private Button leftButton;

		[SerializeField]
		private Button rightButton;

		[SerializeField]
		private Vector2 scrollVelocity = new Vector2(1000f, 0f);

		[SerializeField]
		private GameObject ChatScreenPanel;

		private bool leftButtonHidden;

		private bool rightButtonHidden;

		private void OnEnable()
		{
			leftButton.onClick.AddListener(onLeftButtonClicked);
			rightButton.onClick.AddListener(onRightButtonClicked);
			leftButton.gameObject.SetActive(false);
			leftButtonHidden = true;
		}

		private void OnDisable()
		{
			leftButton.onClick.RemoveListener(onLeftButtonClicked);
			rightButton.onClick.RemoveListener(onRightButtonClicked);
		}

		private void onLeftButtonClicked()
		{
			ScrollRect componentInChildren = base.transform.parent.GetComponentInChildren<ScrollRect>();
			componentInChildren.StopMovement();
			componentInChildren.horizontalNormalizedPosition -= 0.01f;
			componentInChildren.velocity += scrollVelocity;
		}

		private void onRightButtonClicked()
		{
			ScrollRect componentInChildren = base.transform.parent.GetComponentInChildren<ScrollRect>();
			componentInChildren.StopMovement();
			componentInChildren.horizontalNormalizedPosition += 0.01f;
			componentInChildren.velocity -= scrollVelocity;
		}

		private void Update()
		{
			StateMachine component = ChatScreenPanel.GetComponent<StateMachine>();
			if (component.CurrentState.Name == "Instant")
			{
				leftButton.gameObject.SetActive(false);
				rightButton.gameObject.SetActive(false);
				leftButtonHidden = true;
				rightButtonHidden = true;
			}
			ScrollRect componentInChildren = base.transform.parent.GetComponentInChildren<ScrollRect>();
			if (componentInChildren == null)
			{
				return;
			}
			if ((double)componentInChildren.horizontalNormalizedPosition < 0.05 && !leftButtonHidden)
			{
				leftButton.GetComponent<Animator>().Play("CloseButtonExit");
				leftButtonHidden = true;
			}
			else if ((double)componentInChildren.horizontalNormalizedPosition > 0.05 && leftButtonHidden)
			{
				if (!leftButton.gameObject.activeSelf)
				{
					leftButton.gameObject.SetActive(true);
				}
				leftButton.GetComponent<Animator>().Play("CloseButtonIntro");
				leftButtonHidden = false;
			}
			if ((double)componentInChildren.horizontalNormalizedPosition > 0.95 && !rightButtonHidden)
			{
				rightButton.GetComponent<Animator>().Play("CloseButtonExit");
				rightButtonHidden = true;
			}
			else if ((double)componentInChildren.horizontalNormalizedPosition < 0.95 && rightButtonHidden)
			{
				if (!rightButton.gameObject.activeSelf)
				{
					rightButton.gameObject.SetActive(true);
				}
				rightButton.GetComponent<Animator>().Play("CloseButtonIntro");
				rightButtonHidden = false;
			}
		}
	}
}
