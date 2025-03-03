#define UNITY_ASSERTIONS
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin
{
	public class LookAtLocalPlayerObserver : MonoBehaviour
	{
		[Tooltip("Target game object where its forward vector points at a Target")]
		public Transform TargetLookingAtPlayer;

		[Tooltip("This will be enabled when the local player is within range")]
		public GameObject ObjectToEnableWhenInRange;

		[Tooltip("Min distance between player and item in order for it to react")]
		public float MinDistance = 5f;

		[Tooltip("If disabled the item will always point towards the player. Recommend that this always be true.")]
		public bool UseMinDistance = true;

		[Tooltip("How much the item will turn towards the player. 360 for full rotation")]
		public float MaxAngleDegrees = 90f;

		public bool LockXRotation = false;

		[Tooltip("Locking y means it will never look up or down.")]
		public bool LockYRotation = true;

		public bool LockZRotation = false;

		[Tooltip("How long to catch up via tween to avoid snapping")]
		[Header("Tweening Options")]
		public float TweenTime = 0.25f;

		[Tooltip("How great an angle to catch up via tween to avoid snapping")]
		public float MinDeltaAngleToTween = 10f;

		[Tooltip("Ease Type")]
		public iTween.EaseType EaseType = iTween.EaseType.easeInQuint;

		private Quaternion originalRotation;

		private bool isTweening = false;

		public void Start()
		{
			originalRotation = TargetLookingAtPlayer.rotation;
			Service.Get<EventDispatcher>().AddListener<LocalPlayerPositionEvents.PlayerPositionChangedEvent>(onPlayerPositionChanged);
		}

		public void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<LocalPlayerPositionEvents.PlayerPositionChangedEvent>(onPlayerPositionChanged);
		}

		public void OnValidate()
		{
			Assert.IsNotNull(TargetLookingAtPlayer, "Target cannot be null!");
			Assert.IsTrue(MinDistance >= 0f, "Min distance cannot be negative!");
			Assert.IsTrue(!LockXRotation || !LockYRotation || !LockZRotation, "At least one axis must not be locked!");
		}

		private bool onPlayerPositionChanged(LocalPlayerPositionEvents.PlayerPositionChangedEvent evt)
		{
			if (isTweening)
			{
				return false;
			}
			bool flag = true;
			if (UseMinDistance)
			{
				flag = (Vector3.Distance(base.transform.position, evt.Player.transform.position) <= MinDistance);
			}
			if (flag)
			{
				Vector3 forward = evt.Player.transform.position - TargetLookingAtPlayer.position;
				if (LockYRotation)
				{
					forward.y = 0f;
				}
				if (LockZRotation)
				{
					forward.z = 0f;
				}
				if (LockXRotation)
				{
					forward.x = 0f;
				}
				Quaternion quaternion = Quaternion.LookRotation(forward);
				float num = Quaternion.Angle(quaternion, originalRotation);
				if (num <= MaxAngleDegrees)
				{
					float num2 = Quaternion.Angle(TargetLookingAtPlayer.rotation, quaternion);
					if (num2 >= MinDeltaAngleToTween)
					{
						TweenRotation(quaternion.eulerAngles);
					}
					else
					{
						TargetLookingAtPlayer.rotation = quaternion;
					}
				}
			}
			if (ObjectToEnableWhenInRange != null)
			{
				ObjectToEnableWhenInRange.SetActive(flag);
			}
			return false;
		}

		private void TweenRotation(Vector3 lookDirection)
		{
			Assert.IsTrue(!isTweening, "Should not be asked to tween while already tweening");
			isTweening = true;
			Hashtable args = iTween.Hash("rotation", lookDirection, "time", TweenTime, "easetype", EaseType, "oncomplete", "oniTweenOnComplete", "oncompletetarget", base.gameObject);
			iTween.RotateTo(TargetLookingAtPlayer.gameObject, args);
		}

		public void oniTweenOnComplete()
		{
			isTweening = false;
		}
	}
}
