using ClubPenguin.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.UI
{
	public class AllAccessCountdownBarController : MonoBehaviour
	{
		public GameObject Ticker;

		public List<string> ExcludedScenes;

		private void Awake()
		{
			if (isTickerShowable())
			{
				show();
			}
			else
			{
				Hide();
			}
		}

		public void Show()
		{
			if (isTickerShowable())
			{
				show();
			}
		}

		private void show()
		{
			Ticker.SetActive(true);
		}

		public void Hide()
		{
			Ticker.SetActive(false);
		}

		private bool isTickerShowable()
		{
			if (Service.Get<AllAccessService>().IsAllAccessActive() && !ExcludedScenes.Contains(SceneManager.GetActiveScene().name))
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				MembershipData component;
				if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component) && component.MembershipType != MembershipType.Member)
				{
					return true;
				}
			}
			return false;
		}
	}
}
