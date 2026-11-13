#define UNITY_ASSERTIONS
using ClubPenguin.Core;
using UnityEngine;

public class NotSwitch : Switch
{
	public new void Awake()
	{
		base.Awake();
		int num = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Switch component = base.transform.GetChild(i).GetComponent<Switch>();
			if (component != null)
			{
				num++;
			}
		}
		Debug.Assert(num <= 1, string.Format("NotSwitch: Unary operator for {0} must have a single child switch.", base.gameObject.GetPath()));
	}

	public override object GetSwitchParameters()
	{
		return null;
	}

	public override string GetSwitchType()
	{
		return "childInverse";
	}

	protected override void Change(bool onoff)
	{
		base.Change(!onoff);
	}
}
