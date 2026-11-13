using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class CameraController : MonoBehaviour
	{
		[Tooltip("The higher the value, relative to other cameras, the higher the priority. (Default is 0)")]
		public int Priority;

		public string[] HiddenExtraPaths;

		private GameObject[] hiddenExtras;

		private int constraintsInEffect;

		private Dictionary<CameraController, CameraControllerTransition> controllerTransitions;

		public GoalPlanner[] GoalPlanners
		{
			get;
			protected set;
		}

		public Framer Framer
		{
			get;
			protected set;
		}

		internal Glancer[] Glancers
		{
			get;
			set;
		}

		internal Constraint[] Constraints
		{
			get;
			set;
		}

		public bool AreConstraintsDirty
		{
			get;
			set;
		}

		public bool IsScripted
		{
			get;
			set;
		}

		public CameraControllerTransition DefaultControllerTransitionIn
		{
			get;
			private set;
		}

		public CameraControllerTransition DefaultControllerTransitionOut
		{
			get;
			private set;
		}

		public virtual bool IsFinished
		{
			get
			{
				bool flag = true;
				for (int i = 0; i < GoalPlanners.Length; i++)
				{
					flag &= GoalPlanners[i].IsFinished;
				}
				return flag & Framer.IsFinished;
			}
		}

		public void Awake()
		{
			if (HiddenExtraPaths != null)
			{
				hiddenExtras = new GameObject[HiddenExtraPaths.Length];
				for (int i = 0; i < hiddenExtras.Length; i++)
				{
					hiddenExtras[i] = GameObject.Find(HiddenExtraPaths[i]);
				}
			}
			GoalPlanners = GetComponents<GoalPlanner>();
			Constraints = GetComponents<Constraint>();
			Framer = GetComponent<Framer>();
			Glancers = GetComponents<Glancer>();
			deactivateAllCameraComponents();
			AreConstraintsDirty = true;
			List<CameraControllerTransition> list = new List<CameraControllerTransition>(GetComponents<CameraControllerTransition>());
			controllerTransitions = new Dictionary<CameraController, CameraControllerTransition>();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].OtherController != null)
				{
					if (!controllerTransitions.ContainsKey(list[i].OtherController))
					{
						controllerTransitions.Add(list[i].OtherController, list[i]);
					}
					else
					{
						Log.LogError(this, string.Format("'{0}' has a duplicate transition '{1}'", base.gameObject.name, list[i].OtherController.name));
					}
				}
				if (list[i].DefaultTransitionIn)
				{
					DefaultControllerTransitionIn = list[i];
				}
				if (list[i].DefaultTransitionOut)
				{
					DefaultControllerTransitionOut = list[i];
				}
			}
		}

		public void OnDisable()
		{
			CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
			evt.Controller = this;
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			if (eventDispatcher != null)
			{
				eventDispatcher.DispatchEvent(evt);
			}
		}

		public void Activate()
		{
			setHiddenExtrasActive(false);
		}

		public void Update()
		{
			int constraintsInEffectAsBitMask = GetConstraintsInEffectAsBitMask();
			if (constraintsInEffectAsBitMask != constraintsInEffect)
			{
				AreConstraintsDirty = true;
			}
			constraintsInEffect = constraintsInEffectAsBitMask;
		}

		public void Deactivate()
		{
			setHiddenExtrasActive(true);
			deactivateAllCameraComponents();
		}

		private void setHiddenExtrasActive(bool active)
		{
			if (hiddenExtras == null)
			{
				return;
			}
			for (int i = 0; i < hiddenExtras.Length; i++)
			{
				if (hiddenExtras[i] != null)
				{
					hiddenExtras[i].SetActive(active);
				}
			}
		}

		private void deactivateAllCameraComponents()
		{
			if (GoalPlanners != null)
			{
				for (int i = 0; i < GoalPlanners.Length; i++)
				{
					GoalPlanners[i].enabled = false;
				}
			}
			if (Constraints != null)
			{
				for (int i = 0; i < Constraints.Length; i++)
				{
					Constraints[i].enabled = false;
				}
			}
			if (Framer != null)
			{
				Framer.enabled = false;
			}
			if (Glancers != null)
			{
				for (int i = 0; i < Glancers.Length; i++)
				{
					Glancers[i].enabled = false;
				}
			}
		}

		public int GetConstraintsInEffectAsBitMask()
		{
			int num = 0;
			for (int i = 0; i < Constraints.Length; i++)
			{
				num <<= 1;
				if (Constraints[i].Condition == null || Constraints[i].Condition.OnOff)
				{
					num |= 1;
				}
			}
			return num;
		}

		public CameraControllerTransition GetTransitionToOtherController(CameraController destination)
		{
			if (controllerTransitions.ContainsKey(destination))
			{
				return controllerTransitions[destination];
			}
			return null;
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.DrawIcon(base.transform.position, "Cinematography/CameraController.png");
		}
	}
}
