using UnityEngine;

public class SetTriggerInChild : MonoBehaviour
{
	private const int ANIMATOR_INDEX = 1;

	public void FireAnimTriggerInChild(string NameOfTrigger)
	{
		Animator[] componentsInChildren = base.gameObject.GetComponentsInChildren<Animator>();
		if (componentsInChildren != null && componentsInChildren.Length > 1)
		{
			componentsInChildren[1].SetTrigger(NameOfTrigger);
		}
	}
}
