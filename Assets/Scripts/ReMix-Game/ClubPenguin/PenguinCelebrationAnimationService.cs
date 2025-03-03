using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class PenguinCelebrationAnimationService : MonoBehaviour
	{
		public PrefabContentKey AllAccessCelebrationAnimationKey;

		public PrefabContentKey MembershipUnlockFXKey;

		private AllAccessCelebrationData allAccessCelebrationData;

		private GameObject allAccessCelebrationAnimationPrefab;

		private GameObject membershipUnlockFXPrefab;

		private MembershipData membershipData;

		private GameStateController gameStateController;

		private void Start()
		{
			gameStateController = Service.Get<GameStateController>();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle entityByType = cPDataEntityCollection.GetEntityByType<AllAccessCelebrationData>();
			if (!entityByType.IsNull)
			{
				allAccessCelebrationData = cPDataEntityCollection.GetComponent<AllAccessCelebrationData>(entityByType);
				if (allAccessCelebrationData.ShowAllAccessCelebration)
				{
					showAllAccessCelebration();
				}
			}
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out membershipData))
			{
				membershipData.MembershipDataUpdated += onMembershipDataUpdated;
			}
		}

		private void OnDestroy()
		{
			if (membershipData != null)
			{
				membershipData.MembershipDataUpdated -= onMembershipDataUpdated;
				gameStateController.OnAccountAccountSystemDeacitvated -= onAccountAccountSystemDeacitvated;
			}
		}

		private void showAllAccessCelebration()
		{
			Content.LoadAsync(onAllAccessCelebrationAnimationLoaded, AllAccessCelebrationAnimationKey);
			Content.LoadAsync(onMembershipUnlockFXLoaded, MembershipUnlockFXKey);
		}

		private void onAllAccessCelebrationAnimationLoaded(string path, GameObject prefab)
		{
			allAccessCelebrationAnimationPrefab = prefab;
			if (membershipUnlockFXPrefab != null)
			{
				playAllAccessCelebration();
			}
		}

		private void onMembershipDataUpdated(MembershipData updatedMembershipData)
		{
			if (updatedMembershipData.IsMember)
			{
				if (gameStateController.IsAccountSystenActive)
				{
					gameStateController.OnAccountAccountSystemDeacitvated += onAccountAccountSystemDeacitvated;
				}
				else
				{
					showAllAccessCelebration();
				}
			}
		}

		private void onAccountAccountSystemDeacitvated()
		{
			gameStateController.OnAccountAccountSystemDeacitvated -= onAccountAccountSystemDeacitvated;
			showAllAccessCelebration();
		}

		private void onMembershipUnlockFXLoaded(string path, GameObject prefab)
		{
			membershipUnlockFXPrefab = prefab;
			if (allAccessCelebrationAnimationPrefab != null)
			{
				playAllAccessCelebration();
			}
		}

		private void playAllAccessCelebration()
		{
			GameObject gameObject = Object.Instantiate(allAccessCelebrationAnimationPrefab);
			PenguinCelebrationAnimation component = gameObject.GetComponent<PenguinCelebrationAnimation>();
			component.OnAnimationStarted += onPenguinCelebrationAnimationStarted;
			component.OnAnimationEnded += onPenguinCelebrationAnimationEnded;
		}

		private void onPenguinCelebrationAnimationStarted()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				GameObject gameObject = Object.Instantiate(membershipUnlockFXPrefab);
				gameObject.transform.SetParent(localPlayerGameObject.transform, false);
			}
			else
			{
				Log.LogError(this, "localPlayerGameObject was null");
			}
		}

		private void onPenguinCelebrationAnimationEnded(bool animationComplete)
		{
			if (animationComplete && allAccessCelebrationData != null)
			{
				allAccessCelebrationData.ShowAllAccessCelebration = false;
			}
		}
	}
}
