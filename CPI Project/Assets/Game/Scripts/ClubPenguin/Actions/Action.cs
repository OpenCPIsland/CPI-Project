using UnityEngine;

namespace ClubPenguin.Actions
{
	public abstract class Action : MonoBehaviour
	{
		public enum TargetType
		{
			TriggeringAvatar,
			NearestAvatar,
			NearestPet,
			SceneObject
		}

		public string Name;

		[Tooltip("If true, all remaining active warpers will end upon completion of this warper.")]
		public bool EndAllOnExit = false;

		public bool AbortOnUserInput = false;

		public TargetType Target = TargetType.TriggeringAvatar;

		public GameObject CustomTarget;

		[HideInInspector]
		public GameObject Owner;

		[HideInInspector]
		public object IncomingUserData;

		[HideInInspector]
		public int Id = -1;

		[HideInInspector]
		public int ParentId = -1;

		[HideInInspector]
		public int ParentIdOnFalse = -1;

		[HideInInspector]
		public int InterruptedBy = -1;

		public bool Complete
		{
			get;
			protected set;
		}

		protected virtual void Awake()
		{
			base.enabled = false;
		}

		protected virtual void OnEnable()
		{
			if (AbortOnUserInput)
			{
				SceneRefs.ActionSequencer.SetAbortOnUserInput(Owner, true);
			}
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void Update()
		{
		}

		protected virtual void OnAnimatorMove()
		{
		}

		public virtual void OnDestroy()
		{
			Abort();
		}

		public Action AddToGameObject(GameObject targetGO)
		{
			Action action = targetGO.AddComponent(GetType()) as Action;
			CopyTo(action);
			return action;
		}

		protected virtual void CopyTo(Action destWarper)
		{
			destWarper.Name = Name;
			destWarper.Id = Id;
			destWarper.ParentId = ParentId;
			destWarper.ParentIdOnFalse = ParentIdOnFalse;
			destWarper.InterruptedBy = InterruptedBy;
			destWarper.EndAllOnExit = EndAllOnExit;
			destWarper.AbortOnUserInput = AbortOnUserInput;
			destWarper.CustomTarget = CustomTarget;
			destWarper.Target = Target;
		}

		public GameObject GetTarget()
		{
			GameObject result = Owner;
			if (Target != 0)
			{
				result = CustomTarget;
			}
			return result;
		}

		public virtual void Completed(object userData = null, bool conditionBranchValue = true)
		{
			if (Owner == null)
			{
				Abort();
				return;
			}
			Complete = true;
			SceneRefs.ActionSequencer.ActionCompleted(Owner, this, userData, conditionBranchValue);
		}

		protected virtual void Abort()
		{
			if (!Complete && SceneRefs.ActionSequencer != null)
			{
				SceneRefs.ActionSequencer.OnActionAborted(Owner, this);
			}
		}
	}
}
