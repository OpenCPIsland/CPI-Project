using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ShowMembershipFlowButton : MonoBehaviour
	{
		private Button button;

		private TapButton tapButton;

		private float reactivateDelay = 1f;

		private bool isActive = true;

		private void Awake()
		{
			button = GetComponent<Button>();
			tapButton = GetComponent<TapButton>();
			if (button == null && tapButton == null)
			{
				Log.LogError(this, "ShowMembershipFlowButton requires a Button component or a TapButton component");
			}
		}

		private void OnEnable()
		{
			if (button != null)
			{
				button.onClick.AddListener(showMembershipFlow);
			}
			else if (tapButton != null)
			{
				tapButton.OnPressed += showMembershipFlow;
			}
		}

		private void OnDisable()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(showMembershipFlow);
			}
			else if (tapButton != null)
			{
				tapButton.OnPressed -= showMembershipFlow;
			}
		}

		private void showMembershipFlow()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			MembershipData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				if (component.MembershipType != MembershipType.Member && isActive)
				{
					LockedItemTag componentInParent = GetComponentInParent<LockedItemTag>();
					string text = (componentInParent != null) ? componentInParent.name : base.name;
					Regex regex = new Regex("\\[\\d\\]Locked(?<trigger>.[^P]*)Prefab \\(Pooled\\)");
					Match match = regex.Match(text);
					string trigger = string.IsNullOrEmpty(match.Groups["trigger"].Value) ? text : match.Groups["trigger"].Value;
					Service.Get<GameStateController>().ShowAccountSystemMembership(trigger);
					isActive = false;
					CoroutineRunner.Start(doReactivateDelay(), this, "MembershipFlowButtonReactivate");
				}
			}
			else
			{
				Log.LogError(this, "Could not find MembershipData on local player");
			}
		}

		private IEnumerator doReactivateDelay()
		{
			yield return new WaitForSeconds(reactivateDelay);
			isActive = true;
		}
	}
}
