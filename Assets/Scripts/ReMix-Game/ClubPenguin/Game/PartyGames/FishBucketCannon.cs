using Disney.Kelowna.Common;
using Fabric;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class FishBucketCannon : MonoBehaviour
	{
		public Action<long, GameObject, int> ShotCompleteAction;

		public Action RotationCompleteAction;

		public float CannonRotationTimeInSeconds;

		public iTween.EaseType CannonRotationEaseType;

		public Transform ShotOrigin;

		public float CannonShotTimeInSeconds;

		public Animator CannonAnimator;

		public Transform RotationTransform;

		public float CannonShotTimeRandomOffsetInSeconds;

		public GameObject CameraPosition;

		public GameObject CameraTarget;

		public GameObject StandaloneCameraPosition;

		public GameObject StandaloneCameraTarget;

		[Space(10f)]
		public string CannonTurnSFXTrigger;

		public string CannonShootFishSFXTrigger;

		public string CannonShootSquidSFXTrigger;

		private Transform cannonTransform;

		private PrefabContentKey cannonProjectileContentKey_Fish = new PrefabContentKey("Prefabs/FishBucket/FishBucketFluffyProjectile");

		private PrefabContentKey cannonProjectileContentKey_Squid = new PrefabContentKey("Prefabs/FishBucket/FishBucketSquidProjectile");

		private readonly int ANIMATOR_HASH_FIRE = Animator.StringToHash("Fire");

		private GameObject currentShotTarget;

		private int currentShotDelta;

		private long currentShotTargetPlayerId;

		private FishBucketCannonProjectile currentShot;

		private void Start()
		{
			cannonTransform = base.transform;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (currentShot != null)
			{
				FishBucketCannonProjectile fishBucketCannonProjectile = currentShot;
				fishBucketCannonProjectile.ShotCompleteAction = (Action)Delegate.Remove(fishBucketCannonProjectile.ShotCompleteAction, new Action(onShotComplete));
			}
		}

		public void ShootFish(GameObject playerGO, int fishDelta)
		{
			currentShotDelta = fishDelta;
			shootCannon();
			string eventName = (fishDelta > 0) ? CannonShootFishSFXTrigger : CannonShootSquidSFXTrigger;
			EventManager.Instance.PostEvent(eventName, EventAction.PlaySound);
		}

		public void PointCannonAtPlayer(long playerId, GameObject playerGO)
		{
			currentShotTargetPlayerId = playerId;
			currentShotTarget = playerGO;
			Vector3 forward = playerGO.transform.position - cannonTransform.position;
			Quaternion quaternion = Quaternion.LookRotation(forward);
			Hashtable args = iTween.Hash("name", "RotateCannonTween", "y", quaternion.eulerAngles.y, "time", CannonRotationTimeInSeconds, "easetype", CannonRotationEaseType, "oncomplete", "OnCannonRotationComplete", "oncompletetarget", base.gameObject);
			iTween.RotateTo(RotationTransform.gameObject, args);
			EventManager.Instance.PostEvent(CannonTurnSFXTrigger, EventAction.PlaySound);
		}

		public void OnCannonRotationComplete()
		{
			if (RotationCompleteAction != null)
			{
				RotationCompleteAction();
			}
		}

		public void OnCannonAnimationShootProjectile()
		{
			CoroutineRunner.Start(createCannonProjectile(currentShotTarget, currentShotDelta), this, "");
		}

		private void onShotComplete()
		{
			FishBucketCannonProjectile fishBucketCannonProjectile = currentShot;
			fishBucketCannonProjectile.ShotCompleteAction = (Action)Delegate.Remove(fishBucketCannonProjectile.ShotCompleteAction, new Action(onShotComplete));
			if (ShotCompleteAction != null)
			{
				ShotCompleteAction(currentShotTargetPlayerId, currentShotTarget, currentShotDelta);
			}
			currentShot = null;
		}

		private void shootCannon()
		{
			CannonAnimator.SetTrigger(ANIMATOR_HASH_FIRE);
		}

		private IEnumerator createCannonProjectile(GameObject playerGO, int fishDelta)
		{
			Vector3 shotTarget = playerGO.GetComponentInChildren<FishBucketCannonTarget>().gameObject.transform.position;
			int projectileCount = 1;
			AssetRequest<GameObject> request;
			if (fishDelta > 0)
			{
				request = Content.LoadAsync(cannonProjectileContentKey_Fish);
				projectileCount = fishDelta;
			}
			else
			{
				request = Content.LoadAsync(cannonProjectileContentKey_Squid);
			}
			yield return request;
			for (int i = 0; i < projectileCount; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(request.Asset, base.transform);
				FishBucketCannonProjectile component = gameObject.GetComponent<FishBucketCannonProjectile>();
				component.SetTrajectory(ShotOrigin.transform.position, shotTarget, CannonShotTimeInSeconds + UnityEngine.Random.Range(0f - CannonShotTimeRandomOffsetInSeconds, CannonShotTimeRandomOffsetInSeconds));
				if (i == 0)
				{
					currentShot = component;
					FishBucketCannonProjectile fishBucketCannonProjectile = currentShot;
					fishBucketCannonProjectile.ShotCompleteAction = (Action)Delegate.Combine(fishBucketCannonProjectile.ShotCompleteAction, new Action(onShotComplete));
				}
			}
		}
	}
}
