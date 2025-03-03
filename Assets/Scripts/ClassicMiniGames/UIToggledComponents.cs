using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Toggled Components")]
[RequireComponent(typeof(UIToggle))]
[ExecuteInEditMode]
public class UIToggledComponents : MonoBehaviour
{
	public List<MonoBehaviour> activate;

	public List<MonoBehaviour> deactivate;

	[SerializeField]
	[HideInInspector]
	private MonoBehaviour target;

	[HideInInspector]
	[SerializeField]
	private bool inverse = false;

	private void Awake()
	{
		if (target != null)
		{
			if (activate.Count == 0 && deactivate.Count == 0)
			{
				if (inverse)
				{
					deactivate.Add(target);
				}
				else
				{
					activate.Add(target);
				}
			}
			else
			{
				target = null;
			}
		}
		UIToggle component = GetComponent<UIToggle>();
		EventDelegate.Add(component.onChange, Toggle);
	}

	public void Toggle()
	{
		if (base.enabled)
		{
			for (int i = 0; i < activate.Count; i++)
			{
				MonoBehaviour monoBehaviour = activate[i];
				monoBehaviour.enabled = UIToggle.current.value;
			}
			for (int i = 0; i < deactivate.Count; i++)
			{
				MonoBehaviour monoBehaviour = deactivate[i];
				monoBehaviour.enabled = !UIToggle.current.value;
			}
		}
	}
}
