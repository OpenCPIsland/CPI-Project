using System;
using UnityEngine;

namespace ClubPenguin
{
	public class SceneRaycastHitListener : MonoBehaviour
	{
		public bool DispatchBeganIfFirst = true;

		public bool DispatchEndedIfFirst = false;

		public bool DispatchMovedIfFirst = false;

		public bool DispatchStationaryIfFirst = false;

		public bool BlockDispatchIfUiHit = false;

		[HideInInspector]
		public bool IsTouchDown = false;

		private SceneRaycaster sceneRaycaster;

		public event Action<RaycastHit> TouchBegan;

		public event Action<RaycastHit> TouchEnded;

		public event Action<RaycastHit> TouchMoved;

		public event Action<RaycastHit> TouchStationary;

		private void Awake()
		{
			sceneRaycaster = UnityEngine.Object.FindObjectOfType<SceneRaycaster>();
		}

		private void OnStart()
		{
			if (GetComponent<Collider>() == null)
			{
				throw new MissingMemberException("SceneRaycastHitListener on GameObject with name " + base.name + " requires a collider.");
			}
		}

		private void OnDestroy()
		{
			this.TouchBegan = null;
			this.TouchEnded = null;
			this.TouchMoved = null;
			this.TouchStationary = null;
		}

		private void OnEnable()
		{
			sceneRaycaster.RegisterListener(this);
		}

		private void OnDisable()
		{
			sceneRaycaster.UnRegisterListener(this);
		}

		public void DispatchTouchBegan(RaycastHit hit, int rayHitOrder, bool uiWasHit)
		{
			if (HitConditionsPassed(DispatchBeganIfFirst, rayHitOrder, uiWasHit))
			{
				IsTouchDown = true;
				if (this.TouchBegan != null)
				{
					this.TouchBegan(hit);
				}
			}
		}

		public void DispatchTouchEnded(RaycastHit hit, int rayHitOrder, bool uiWasHit)
		{
			if (HitConditionsPassed(DispatchEndedIfFirst, rayHitOrder, uiWasHit) && this.TouchEnded != null)
			{
				this.TouchEnded(hit);
			}
		}

		public void DispatchMoved(RaycastHit hit, int rayHitOrder, bool uiWasHit)
		{
			if (HitConditionsPassed(DispatchMovedIfFirst, rayHitOrder, uiWasHit) && this.TouchMoved != null)
			{
				this.TouchMoved(hit);
			}
		}

		public void DispatchStationary(RaycastHit hit, int rayHitOrder, bool uiWasHit)
		{
			if (HitConditionsPassed(DispatchStationaryIfFirst, rayHitOrder, uiWasHit) && this.TouchStationary != null)
			{
				this.TouchStationary(hit);
			}
		}

		private bool HitConditionsPassed(bool eventConditional, int rayHitOrder, bool uiWasHit)
		{
			if ((eventConditional && rayHitOrder > 0) || (BlockDispatchIfUiHit && uiWasHit))
			{
				return false;
			}
			return true;
		}
	}
}
