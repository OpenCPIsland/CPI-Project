using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class LocomotionTracker : MonoBehaviour
	{
		private LocomotionController curController;

		private Dictionary<LocomotionController, float> disallowedControllers = new Dictionary<LocomotionController, float>();

		private void Start()
		{
			curController = findCurrentController();
		}

		public void TEMPHACK_ResetCurController()
		{
			curController = findCurrentController(); //null;
		}

		private LocomotionController findCurrentController()
		{
			LocomotionController locomotionController = null;
			LocomotionController[] components = GetComponents<LocomotionController>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i].enabled)
				{
					if (locomotionController == null)
					{
						locomotionController = components[i];
					}
					else
					{
						components[i].enabled = false;
					}
				}
			}
			return locomotionController;
		}

		public void DisallowController<T>(float delay) where T : LocomotionController
		{
			LocomotionController component = GetComponent<T>();
			if (disallowedControllers.ContainsKey(component))
			{
				disallowedControllers[component] = Time.time + delay;
			}
			else
			{
				disallowedControllers.Add(component, Time.time + delay);
			}
		}

		private bool isControllerAllowed(LocomotionController controller)
		{
			bool result = true;
			if (disallowedControllers.ContainsKey(controller))
			{
				if (disallowedControllers[controller] < Time.time)
				{
					disallowedControllers.Remove(controller);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public LocomotionController GetCurrentController()
		{
			return curController;
		}

		public bool IsCurrentControllerOfType<T>() where T : LocomotionController
		{
			if (curController != null)
			{
				return curController is T;
			}
			return false;
		}

		public bool SetCurrentController<T>() where T : LocomotionController
		{
			T component = GetComponent<T>();
			if ((Object)component != (Object)null)
			{
				return SetCurrentController(component);
			}
			return false;
		}

		public bool SetCurrentController(LocomotionController controller)
		{
			if (curController == controller)
			{
				return true;
			}
			if (!controller.enabled && isControllerAllowed(controller))
			{
				controller.enabled = true;
				if (controller.enabled && curController != null)
				{
					curController.enabled = false;
					curController = controller;
					controller.Broadcaster.BroadcastOnControllerChanged(controller);
				}
			}
			return curController == controller;
		}

		public void DisableAllControllers()
		{
			LocomotionController[] components = GetComponents<LocomotionController>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = false;
			}
			curController = null;
		}
	}
}
