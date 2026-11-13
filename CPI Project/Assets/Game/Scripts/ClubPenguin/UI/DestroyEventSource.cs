using ClubPenguin.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DestroyEventSource : MonoBehaviour
	{
		public string Target;
		public string Event;
		public bool AllowMissingStateMachine = false;

		private void OnDestroy()
		{
			string goName = gameObject.name;
			string path = GetHierarchyPath(gameObject);

			if (!string.IsNullOrEmpty(Target))
			{
				StateMachineContext ctx = GetComponentInParent<StateMachineContext>();
				if (ctx == null)
				{
					GameObject trayRoot = GameObject.FindGameObjectWithTag(UIConstants.Tags.UI_Tray_Root);
					if (!trayRoot.IsDestroyed())
					{
						ctx = trayRoot.GetComponent<StateMachineContext>();
					}
				}

				if (ctx != null)
				{
					ctx.SendEvent(new ExternalEvent(Target, Event));
				}
				else if (!AllowMissingStateMachine)
				{
					Log.LogError(this,
						$"[DestroyEventSource] Missing component: StateMachineContext\n" +
						$" Expected: StateMachineContext (Target='{Target}', Event='{Event}')\n" +
						$" GameObject: '{goName}'\n Hierarchy: '{path}'");
				}

				return;
			}

			if (string.IsNullOrEmpty(Event))
			{
				return;
			}

			StateMachine sm = GetComponent<StateMachine>();
			if (sm != null)
			{
				sm.SendEvent(Event);
			}
			else
			{
				Log.LogError(this,
					$"[DestroyEventSource] Missing component: StateMachine\n" +
					$" Expected: StateMachine on this object to receive event '{Event}'\n" +
					$" GameObject: '{goName}'\n Hierarchy: '{path}'");
			}
		}

		private static string GetHierarchyPath(GameObject obj)
		{
			string path = obj.name;
			Transform t = obj.transform;

			while (t.parent != null)
			{
				t = t.parent;
				path = t.name + "/" + path;
			}

			return path;
		}
	}
}
